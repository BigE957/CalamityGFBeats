using CalamityGFBeats.Common;
using CalamityMod.Events;
using CalamityMod.NPCs.DesertScourge;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityGFBeats.Content.MusicScenes;

public class DesertScourgeMusicScene : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, "Assets/Music/DesertScourge");
    public override bool IsSceneEffectActive(Player player)
    {
        if (!CalamityGFBeatsConfig.Instance.OverrideDesertScourge || BossRushEvent.BossRushActive)
            return false;
        return NPC.AnyNPCs(ModContent.NPCType<DesertScourgeHead>()) || NPC.AnyNPCs(ModContent.NPCType<DesertScourgeBody>()) || NPC.AnyNPCs(ModContent.NPCType<DesertScourgeTail>());
    }
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
}
