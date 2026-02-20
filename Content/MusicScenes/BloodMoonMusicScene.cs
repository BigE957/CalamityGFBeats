using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityGFBeats.Content.MusicScenes;

public class BloodMoonMusicScene : ModSceneEffect
{
    public override int Music => MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, "Assets/Music/TenebreRossoSangueCalm");

    public override SceneEffectPriority Priority => SceneEffectPriority.Event;

    public override bool IsSceneEffectActive(Player player) => Main.bloodMoon;
}
