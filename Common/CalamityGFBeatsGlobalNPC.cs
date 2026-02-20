using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.SunkenSea;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityGFBeats.Common;

public class CalamityGFBeatsGlobalNPC : GlobalNPC
{
    public override void OnKill(NPC npc)
    {
        if(npc.type == ModContent.NPCType<DesertScourgeHead>())
            SongEndingManager.SetBossEndMusic(SongEndingManager.BossesWithEnds.DesertScourge, 168);
        else if(npc.type == ModContent.NPCType<GiantClam>())
            SongEndingManager.SetBossEndMusic(SongEndingManager.BossesWithEnds.GiantClam, 390);
    }   
}

public class SongEndingManager : ModSystem
{
    public enum BossesWithEnds
    {
        None = -1,
        GiantClam,
        DesertScourge
    }

    internal static Dictionary<BossesWithEnds, int> BossEndMusicSlots = [];

    public static BossesWithEnds CurrentBossEnding => BossEnd;
    internal static BossesWithEnds BossEnd = BossesWithEnds.None;
    internal static int BossEndTime = -1;
    internal static int PreviousMusic = -1;

    public override void OnModLoad()
    {
        BossEndMusicSlots.Add(BossesWithEnds.GiantClam, MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, "Assets/Music/GiantClamEnd"));
        BossEndMusicSlots.Add(BossesWithEnds.DesertScourge, MusicLoader.GetMusicSlot(CalamityGFBeats.Instance, "Assets/Music/DesertScourgeEnd"));
    }

    public override void PostUpdateEverything()
    {
        if (BossEndTime >= 0)
        {
            Main.musicFade[Main.curMusic] = 1f;
            if (--BossEndTime == -1)
            {
                BossEnd = BossesWithEnds.None;
                Main.musicFade[Main.curMusic] = 0;
                Main.curMusic = 0;
                Main.newMusic = 0;
                PreviousMusic = -1;
            }
        }
        else
            PreviousMusic = -1;

        if (PreviousMusic != -1)
            Main.musicFade[PreviousMusic] = 0;
        else
            PreviousMusic = -1;
    }

    internal static void SetBossEndMusic(BossesWithEnds bossEnd, int endDuration)
    {
        BossEnd = bossEnd;
        BossEndTime = endDuration;
        ForceSetMusic(BossEndMusicSlots[bossEnd]);
    }

    internal static void ForceSetMusic(int musicSlot)
    {
        SongEndingManager.PreviousMusic = Main.curMusic;
        Main.musicFade[SongEndingManager.PreviousMusic] = 0f;
        Main.newMusic = Main.curMusic = musicSlot;
    }
}
