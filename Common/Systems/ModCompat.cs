using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.SunkenSea;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityGFBeats.Common.Systems;

public class ModCompat : ModSystem
{
    private static readonly string displayPath = "ModCompat.MusicDisplay.";

    public override void PostAddRecipes()
    {
        if (!ModLoader.TryGetMod("MusicDisplay", out Mod display))
            return;

        LocalizedText modName = Language.GetOrRegister("Mods.CalamityGFBeats." + displayPath + "ModName");

        void AddMusic(string path, string name)
        {
            LocalizedText author = Language.GetOrRegister("Mods.CalamityGFBeats." + displayPath + name + ".Author");
            LocalizedText displayName = Language.GetOrRegister("Mods.CalamityGFBeats." + displayPath + name + ".DisplayName");
            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, path), displayName, author, modName);
        }

        void AddMusicWithCondition(string path, string name, Func<bool> displayCondition)
        {
            LocalizedText author = Language.GetOrRegister("Mods.CalamityGFBeats." + displayPath + name + ".Author");
            LocalizedText displayName = Language.GetOrRegister("Mods.CalamityGFBeats." + displayPath + name + ".DisplayName");
            display.Call("AddMusic", (short)MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, path), displayName, author, modName, displayCondition);
        }

        AddMusicWithCondition("Assets/Music/DesertScourge", "DesertScourge", () => NPC.AnyNPCs(ModContent.NPCType<DesertScourgeHead>()) && SongEndingManager.CurrentBossEnding == SongEndingManager.BossesWithEnds.None);
        AddMusicWithCondition("Assets/Music/GiantClam", "GiantClam", () => NPC.AnyNPCs(ModContent.NPCType<GiantClam>()) && SongEndingManager.CurrentBossEnding == SongEndingManager.BossesWithEnds.None);
    }
}
