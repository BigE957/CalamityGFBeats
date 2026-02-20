using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NVorbis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace CalamityGFBeats.Common.Systems.DynamicMusic;

/// <summary>
/// An IAudioTrack that mixes multiple OGG layers together.
/// Each layer has its own volume and can be enabled/disabled.
/// Perfect for dynamic music where you want to blend between different versions.
/// </summary>
public class LayeredOGGTrack : ASoundEffectBasedAudioTrack
{
    private class Layer
    {
        public VorbisReader Reader;
        public float Volume = 1f;
        public bool IsEnabled = true;
        public byte[] AudioBytes; // stored for reuse
    }

    private readonly List<Layer> _layers = new List<Layer>();
    public int LayerCount => _layers.Count;
    private readonly object _lock = new object();
    private bool _isDisposed;
    private float[] _mixBuffer;
    private bool _skipNextReuse = false;

    public LayeredOGGTrack(params byte[][] layerBytes)
    {
        if (layerBytes.Length == 0)
            throw new ArgumentException("At least one layer required.");

        // Determine sample rate and channels from first layer
        using (var testReader = new VorbisReader(new MemoryStream(layerBytes[0]), false))
        {
            int sampleRate = testReader.SampleRate;
            AudioChannels channels = testReader.Channels == 1 ? AudioChannels.Mono : AudioChannels.Stereo;
            CreateSoundEffect(sampleRate, channels);
        }

        _mixBuffer = new float[_temporaryBuffer.Length];

        foreach (var bytes in layerBytes)
        {
            var reader = new VorbisReader(new MemoryStream(bytes), false);
            _layers.Add(new Layer
            {
                Reader = reader,
                AudioBytes = bytes
            });
        }

        TimeSpan firstDuration = _layers[0].Reader.TotalTime;
        for (int i = 1; i < _layers.Count; i++)
            if (Math.Abs((_layers[i].Reader.TotalTime - firstDuration).TotalSeconds) > 0.1)
                throw new ArgumentException($"Layer {i} duration mismatch: {_layers[i].Reader.TotalTime} vs {firstDuration}");
    }

    /// <summary>
    /// Sets the volume of a specific layer (0 = silent, 1 = full).
    /// </summary>
    public void SetLayerVolume(int layerIndex, float volume)
    {
        if (layerIndex >= 0 && layerIndex < _layers.Count)
            lock (_lock)
            {
                _layers[layerIndex].Volume = MathHelper.Clamp(volume, 0f, 1f);
            }
    }

    /// <summary>
    /// Enables or disables a layer.
    /// </summary>
    public void SetLayerEnabled(int layerIndex, bool enabled)
    {
        if (layerIndex >= 0 && layerIndex < _layers.Count)
            lock (_lock)
            {
                _layers[layerIndex].IsEnabled = enabled;
            }
    }

    /// <summary>
    /// Seeks all layers to the same time position.
    /// </summary>
    public void SeekAll(TimeSpan position)
    {
        lock (_lock)
        {
            foreach (var layer in _layers)
            {
                try
                {
                    layer.Reader.TimePosition = position;
                }
                catch (Exception ex)
                {
                    CalamityGFBeats.Instance?.Logger.Warn($"Layer seek error at {position.TotalSeconds:F2}s: {ex.Message}");

                    // Attempt to recreate reader
                    var newReader = new VorbisReader(new MemoryStream(layer.AudioBytes), false);
                    try
                    {
                        newReader.TimePosition = position;
                        layer.Reader.Dispose();
                        layer.Reader = newReader;
                    }
                    catch
                    {
                        try
                        {
                            newReader.Dispose();
                        }
                        catch { }

                        try
                        {
                            layer.Reader.TimePosition = TimeSpan.Zero;
                        }
                        catch (Exception ex2)
                        {
                            CalamityGFBeats.Instance?.Logger.Error($"Failed to reset layer position: {ex2}");
                        }
                    }

                }
            }
        }
    }

    public void SkipNextReuse()
    {
        lock (_lock)
        {
            _skipNextReuse = true;
        }
    }

    public override void Reuse()
    {
        bool shouldSkip;
        lock (_lock)
        {
            shouldSkip = _skipNextReuse;
            _skipNextReuse = false;
        }

        if (shouldSkip)
            return;

        Stop(AudioStopOptions.Immediate);
        lock (_lock)
        {
            foreach (var layer in _layers)
            {
                layer.Reader?.Dispose();
                layer.Reader = new VorbisReader(new MemoryStream(layer.AudioBytes), false);
            }
        }
    }

    public new void Play()
    {
        // Pre‑buffer several chunks to avoid startup underrun
        for (int i = 0; i < 8; i++)
            ReadAheadPutAChunkIntoTheBuffer();
        base.Play();
    }

    protected override void ReadAheadPutAChunkIntoTheBuffer()
    {
        lock (_lock)
        {
            if (_layers.Count == 0 || _soundEffectInstance?.IsDisposed != false)
                return;

            int samplesToRead = _temporaryBuffer.Length;

            Array.Clear(_mixBuffer, 0, samplesToRead);

            int maxSamplesRead = 0;
            bool anyLayerLooped = false;

            // Read from ALL layers to keep them synchronized
            foreach (var layer in _layers)
            {
                int samplesRead = layer.Reader.ReadSamples(_temporaryBuffer, 0, samplesToRead);
                if (samplesRead < samplesToRead)
                    anyLayerLooped = true;

                maxSamplesRead = Math.Max(maxSamplesRead, samplesRead);

                // Mix with volume (even if volume is zero, we still read to advance position)
                for (int i = 0; i < samplesRead; i++)
                    _mixBuffer[i] += _temporaryBuffer[i] * layer.Volume;
            }

            // If any layer reached end, loop all together
            if (anyLayerLooped)
            {
                // Seek all layers to start
                foreach (var layer in _layers)
                    layer.Reader.SeekTo(0, 0);

                // Read remaining samples to fill the buffer
                int remaining = samplesToRead - maxSamplesRead;
                if (remaining > 0)
                {
                    foreach (var layer in _layers)
                    {
                        int samplesRead = layer.Reader.ReadSamples(_temporaryBuffer, 0, remaining);
                        for (int i = 0; i < samplesRead; i++)
                            _mixBuffer[maxSamplesRead + i] += _temporaryBuffer[i] * layer.Volume;
                    }
                    maxSamplesRead = samplesToRead;
                }
            }

            if (maxSamplesRead == 0)
                return;

            // Clamp to prevent clipping
            for (int i = 0; i < maxSamplesRead; i++)
                _mixBuffer[i] = Math.Clamp(_mixBuffer[i], -1f, 1f);

            // Convert to 16-bit PCM and submit
            for (int i = 0; i < maxSamplesRead; i++)
            {
                short sample = (short)(_mixBuffer[i] * short.MaxValue);
                _bufferToSubmit[i * 2] = (byte)(sample & 0xFF);
                _bufferToSubmit[i * 2 + 1] = (byte)((sample >> 8) & 0xFF);
            }

            _soundEffectInstance.SubmitBuffer(_bufferToSubmit, 0, maxSamplesRead * 2);
        }
    }

    public override void Dispose()
    {
        if (!_isDisposed)
        {
            lock (_lock)
            {
                foreach (var layer in _layers)
                    layer.Reader?.Dispose();
                _layers.Clear();
            }
            _isDisposed = true;
        }
    }
}