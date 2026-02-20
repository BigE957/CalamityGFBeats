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
    protected override bool CanUpdate() => Main.bloodMoon;

    protected override int GetDesiredLayerIndex() => NPC.AnyNPCs(NPCID.BloodNautilus) ? 1 : 0;

    protected override string[] GetLayerPaths() =>
    [
        "Assets/Music/TenebreRossoSangueCalm.ogg",
        "Assets/Music/TenebreRossoSangueCombat.ogg"
    ];
}