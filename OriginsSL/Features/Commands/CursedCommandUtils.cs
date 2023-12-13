using System.Collections.Generic;
using System.Linq;
using CursedMod.Features.Wrappers.Player;
using PlayerRoles;

namespace OriginsSL.Features.Commands;

public static class CursedCommandUtils
{
    public static List<CursedPlayer> GetPlayers(string players)
    {
        switch (players.ToLower())
        {
            case "*":
                return CursedPlayer.List;
            case "scp" or "scps":
                return CursedPlayer.Collection.Where(player => player.CurrentRole.Team == Team.SCPs).ToList();
            case "human" or "humans":
                return CursedPlayer.Collection.Where(player => player.IsHuman).ToList();
            case "alive":
                return CursedPlayer.Collection.Where(player => player.IsAlive).ToList();
            case "rip" or "dead" or "spectator" or "spectators":
                return CursedPlayer.Collection.Where(player => player.IsDead).ToList();
        }

        List<CursedPlayer> ret = new();

        foreach (string id in players.Split(' '))
        {
            if (CursedPlayer.TryGet(id, out CursedPlayer player))
                ret.Add(player);
        }

        return ret;
    }
}