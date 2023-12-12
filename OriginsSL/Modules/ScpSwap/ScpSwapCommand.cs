using System;
using System.Collections.Generic;
using System.Linq;
using CommandSystem;
using CursedMod.Events.Arguments.Player;
using CursedMod.Features.Wrappers.Player;
using GameCore;
using PlayerRoles;

namespace OriginsSL.Modules.ScpSwap;

[CommandHandler(typeof(ClientCommandHandler))]
public class ScpSwapCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!CursedPlayer.TryGet(sender, out CursedPlayer player))
        {
            response = "Player not found.";
            return false;
        }

        if (player.CurrentRole.Team != Team.SCPs)
        {
            response = "You have to be a scp to execute the command!";
            return false;
        }

        if (RoundStart.RoundLength.TotalSeconds > 80)
        {
            response = "Swap time is over.";
            return false;
        }

        if (arguments.Count == 0)
        {
            response = "Incorrect usage. Example: .scpswap dog";
            return false;
        }

        string req = arguments.At(0).ToLower();

        if (req is "accept")
        {
            RoleTypeId role = player.Role;
            
            if (Requests.ContainsKey(role))
            {
                if (arguments.Count > 1)
                {
                    string sRole = arguments.At(1).ToLower();
                    
                    if (!ScpAliases.ContainsKey(sRole))
                    {
                        response = "Scp " + req + " was not found. Possible SCPs:";
                        response = ScpAliases.Keys.Aggregate(response, (current, alias) => current + "\n- " + alias);
                        return false;
                    }

                    if (Requests[role].Any(x => x.Role == ScpAliases[sRole]))
                    {
                        CursedPlayer swapped = Requests[role].First(x => x.Role == ScpAliases[sRole]);
                        player.Role = swapped.Role;
                        swapped.Role = role;
                        response = "Swapped roles!";
                        return true;
                    }

                    response = "Role not found in requests.";
                    return false;
                }
                {
                    CursedPlayer swapped = Requests[role].First();
                    player.Role = swapped.Role;
                    swapped.Role = role;
                    response = "Swapped roles!";
                    return true;
                }
            }

            response = "You don't have swap requests.";
            return false;
        }

        if (!ScpAliases.ContainsKey(req))
        {
            response = "Scp " + req + " was not found. Possible SCPs:";
            foreach (string alias in ScpAliases.Keys)
                response += "\n- " + alias;
            return false;
        }

        RoleTypeId desiredRole = ScpAliases[req];

        foreach (CursedPlayer ply in CursedPlayer.Collection)
        {
            if (ply.Role != desiredRole)
                continue;
            
            if (!Requests.ContainsKey(desiredRole))
                Requests.Add(desiredRole, new List<CursedPlayer>());

            if (Requests[desiredRole].Contains(player))
            {
                response = "You already sent a request!";
                return true;
            }
            
            Requests[desiredRole].Add(player);
            ply.ShowBroadcast($"<b>You have received a Swap Request by <color=red>{player.Role}</color></b>\nType <u><color=orange>.scpswap accept {ScpAliases.First(x => x.Value == player.Role).Key}</color></u> to accept it!", 10);
            response = "Request sent";
            return true;
        }

        // patron PERKS
        // if (sender.CheckPermission("cursed.donator.scpswap"))
        // {
        //     if (desiredRole is RoleTypeId.Scp079 or RoleTypeId.Scp096 && CursedPlayer.Collection.Any(x => x.Role is RoleTypeId.Scp096 or RoleTypeId.Scp079))
        //     {
        //         response = "SCP079 or SCP096 are already in the game. You can't use donator perks to get them.";
        //         return false;
        //     }
        //     
        //     player.SetRole(desiredRole);
        //     response = "Used donator perks for getting the desired role.";
        //     return true;
        // }

        response = "Couldn't find any player with that role.";
        return false;
    }

    public static void ClearCache()
    {
        Requests.Clear();
    }
    
    private static readonly Dictionary<RoleTypeId, List<CursedPlayer>> Requests = new();
    
    private static readonly Dictionary<string, RoleTypeId> ScpAliases = new()
    {
        {"173", RoleTypeId.Scp173},
        {"peanut", RoleTypeId.Scp173},
        {"939", RoleTypeId.Scp939},
        {"dog", RoleTypeId.Scp939},
        {"079", RoleTypeId.Scp079},
        {"computer", RoleTypeId.Scp079},
        {"106", RoleTypeId.Scp106},
        {"larry", RoleTypeId.Scp106},
        {"096", RoleTypeId.Scp096},
        {"shyguy", RoleTypeId.Scp096},
        {"049", RoleTypeId.Scp049},
        {"doctor", RoleTypeId.Scp049},
    };

    public string Command { get; } = "scpswap";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Lets you swap roles with other teammates";

    public static void HandleMessage(PlayerChangingRoleEventArgs args)
    {
        if (args.ChangeReason is not RoleChangeReason.RoundStart)
            return;
        
        if (args.NewRole.GetTeam() is not Team.SCPs)
            return;
        
        args.Player.ShowBroadcast("<b><i>Don't you like your SCP?</i>\nUse <u><color=red>.scpswap</color></u> to swap it!</b>", 10);
    }
}