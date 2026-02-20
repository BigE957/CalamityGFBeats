using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityGFBeats.Common.Systems.DynamicMusic;

public class BloodMoonDynamicMusic : LayeredDynamicMusic
{
    private PlaybackSpeedEffect _playbackSpeed;

    protected override bool CanUpdate() => Main.bloodMoon;

    protected override int GetDesiredLayerIndex() => NPC.AnyNPCs(NPCID.BloodNautilus) ? 1 : 0;

    protected override string[] GetLayerPaths() =>
    [
        "Assets/Music/TenebreRossoSangueCalm.ogg",
        "Assets/Music/TenebreRossoSangueCombat.ogg"
    ];

    protected override void PostMusicLoad()
    {
        _playbackSpeed = new PlaybackSpeedEffect(LayeredTrack) { Speed = 1f };
        LayeredTrack.AddEffect(_playbackSpeed);
    }

    protected override void PostUpdate()
    {
        var player = Main.LocalPlayer;
        if (player == null) return;

        // Example: oscillate speed between 0.5 and 2.0
        float desiredSpeed = MathF.Sin(Main.GlobalTimeWrappedHourly * 0.5f);
        _playbackSpeed.Speed = desiredSpeed;

        // Or test reverse:
        // _playbackSpeed.Speed = -1f; // reverse at normal speed

        Main.NewText($"Speed: {_playbackSpeed.Speed:F2}");
    }
}