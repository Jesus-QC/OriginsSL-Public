using System;
using System.Collections.Generic;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using NWAPIPermissionSystem;
using OriginsSL.Features.Commands;
using OriginsSL.Modules.AdminTools.Fun.Components;

namespace OriginsSL.Modules.AdminTools.Fun;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class AliveGrenadeCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if (!sender.CheckPermission("origins.fun.alivegrenade") && !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }
        
        if (arguments.Count == 0)
        {
            response = "Not enough arguments";
            return false;
        }
        
        List<CursedPlayer> players = CursedCommandUtils.GetPlayers(arguments.At(0));

        if (players.Count is 0)
        {
            response = "Players not found.";
            return false;
        }

        foreach (CursedPlayer player in players)
        {
            AliveGrenadeComponent al = player.GameObject.AddComponent<AliveGrenadeComponent>();
            al.Player = player;
            if (arguments.Count > 1)
                al.ItemType = ItemType.GrenadeFlash;
        }

        response = $"Done for {players.Count} players";
        return true;
    }

    public string Command { get; } = "alivegrenade";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Makes players a grenade.";
    public string[] Usage { get; } = {"%player%"};
}