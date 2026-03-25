using CalamityGFBeats.Content.MusicScenes;
using CalamityMod.NPCs.Crabulon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityGFBeats.Common.Systems.DynamicMusic.Tracks;

public class CrabulonDynamicMusic : LayeredDynamicMusic
{
    private PlaybackSpeedEffect _playbackSpeed;

    protected override bool CanUpdate() => ModContent.GetInstance<CrabulonMusicScene>().IsSceneEffectActive(Main.LocalPlayer);

    protected override int GetDesiredLayerIndex()
    {
        NPC crabulon = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<Crabulon>())];
        float lifeRatio = crabulon.life / (float)crabulon.lifeMax;
        if (lifeRatio <= 0.666f)
            return 1;
        return 0; 
    }

    protected override string[] GetLayerPaths() =>
    [
        "Assets/Music/CrabulonDisco.ogg",
        "Assets/Music/CrabulonFutureFunk.ogg"
    ];

    protected override void PostMusicLoad()
    {
        _playbackSpeed = new PlaybackSpeedEffect(LayeredTrack) { Speed = 1f };
        LayeredTrack.AddEffect(_playbackSpeed);
    }

    protected override void PostUpdate()
    {
        NPC crabulon = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<Crabulon>())];
        float lifeRatio = crabulon.life / (float)crabulon.lifeMax;
        if (lifeRatio <= 0.333f)
            _playbackSpeed.Speed = 1.06f;
        else
            _playbackSpeed.Speed = 1f;
    }
}