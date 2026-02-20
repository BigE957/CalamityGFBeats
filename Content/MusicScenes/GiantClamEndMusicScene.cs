using CalamityGFBeats.Common;
using CalamityMod.Events;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.SunkenSea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace CalamityGFBeats.Content.MusicScenes;

public class GiantClamEndMusicScene : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, "Assets/Music/GiantClamEnd");
    public override bool IsSceneEffectActive(Player player)
    {
        if (!CalamityGFBeatsConfig.Instance.AddGiantClam || BossRushEvent.BossRushActive)
            return false;
        return SongEndingManager.CurrentBossEnding == SongEndingManager.BossesWithEnds.GiantClam;

    }
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;
}
