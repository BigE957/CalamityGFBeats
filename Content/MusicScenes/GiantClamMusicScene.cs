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

public class GiantClamMusicScene : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, "Assets/Music/GiantClam");
    public override bool IsSceneEffectActive(Player player)
    {
        if (!CalamityGFBeatsConfig.Instance.AddGiantClam || BossRushEvent.BossRushActive)
            return false;
        if (!Main.npc.Any(n => n.active && n.type == ModContent.NPCType<GiantClam>() && n.life != n.lifeMax))
            return false;
        return true;
    }
    public override SceneEffectPriority Priority => SceneEffectPriority.Event;
}
