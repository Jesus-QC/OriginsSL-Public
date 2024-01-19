using System.Collections.Generic;
using System.Threading.Tasks;
using CursedMod.Features.Wrappers.Player;

namespace OriginsSL.Features.Display;

public static class CursedPollManager
{
    public static bool InUse;
    public static string Author = string.Empty;
    public static string Description = string.Empty;
    public static byte AffirmativeVotes;
    public static byte NegativeVotes;
    public static byte TimeLeft;

    private static readonly HashSet<CursedPlayer> Votes = new ();

    public static bool AddVote(CursedPlayer player, bool pos)
    {
        if (Votes.Contains(player))
            return false;

        Votes.Add(player);
        
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
        return AffirmativeVotes > NegativeVotes;
    }
}