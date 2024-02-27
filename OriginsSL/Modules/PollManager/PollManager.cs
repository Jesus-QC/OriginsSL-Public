using System.Collections.Generic;
using System.Threading.Tasks;
using CursedMod.Features.Wrappers.Facility;
using CursedMod.Features.Wrappers.Player;

namespace OriginsSL.Modules.PollManager;

public static class PollManager
{
    public static bool InUse;
    public static string Author = string.Empty;
    public static string Description = string.Empty;
    public static byte AffirmativeVotes;
    public static byte NegativeVotes;
    public static byte TimeLeft;

    private static readonly HashSet<CursedPlayer> Votes = [];

    public static bool AddVote(CursedPlayer player, bool pos)
    {
        if (!Votes.Add(player))
            return false;

        if (pos)
            AffirmativeVotes++;
        else
            NegativeVotes++;

        return true;
    }

    public static async Task<bool> RunPoll(string author, string description)
    {
        Author = author;
        Description = description;
        InUse = true;
        TimeLeft = 45;
        AffirmativeVotes = 0;
        NegativeVotes = 0;
        Votes.Clear();
        
        while (TimeLeft > 0 && Description == description)
        {
            await Task.Delay(1000);
            TimeLeft--;
        }
        
        if (Description != description)
            return false;

        InUse = false;
        CursedFacility.ShowBroadcast($"<size=50%>Poll Ended!</size>\n<color=#61ff69>\u2705 {AffirmativeVotes}</color> - <color=#ff61a0>{NegativeVotes} \u274c</color>", 10);
        return AffirmativeVotes > NegativeVotes;
    }
}