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

internal class DesertScourgeEndMusicScene : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, "Assets/Music/DesertScourgeEnd");
    public override bool IsSceneEffectActive(Player player)
    {
        if (!CalamityGFBeatsConfig.Instance.OverrideDesertScourge || BossRushEvent.BossRushActive)
            return false;
        return SongEndingManager.CurrentBossEnding == SongEndingManager.BossesWithEnds.DesertScourge;
    }
    public override SceneEffectPriority Priority => SceneEffectPriority.BossHigh;
}