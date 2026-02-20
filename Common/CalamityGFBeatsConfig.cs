using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader.Config;

namespace CalamityGFBeats.Common;

public class CalamityGFBeatsConfig : ModConfig
{
    public static CalamityGFBeatsConfig Instance;
    public override void OnLoaded()
    {
        Instance = this;
    }
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [Header("SongOverrides")]

    [BackgroundColor(120, 94, 47, 192)]
    [DefaultValue(true)]
    public bool OverrideDesertScourge { get; set; }

    [Header("SongAdditions")]

    [BackgroundColor(58, 83, 145, 192)]
    [DefaultValue(true)]
    public bool AddGiantClam { get; set; }
}
