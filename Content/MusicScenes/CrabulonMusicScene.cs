using CalamityGFBeats.Common;
using CalamityMod.Events;
using CalamityMod.NPCs.Crabulon;
using Terraria;
using Terraria.ModLoader;

namespace CalamityGFBeats.Content.MusicScenes;

public class CrabulonMusicScene : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, "Assets/Music/CrabulonDisco");
    public override bool IsSceneEffectActive(Player player)
    {
        if (!CalamityGFBeatsConfig.Instance.OverrideCrabulon || BossRushEvent.BossRushActive)
            return false;
        return NPC.AnyNPCs(ModContent.NPCType<Crabulon>());
    }
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
}
