using System;
using System.Collections.Generic;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Features.Commands;
using UnityEngine;
using NWAPIPermissionSystem;

namespace OriginsSL.Modules.AdminTools.Fun;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class SizeCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if (!sender.CheckPermission("origins.fun.size") && !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }
        
        if (arguments.Count < 4)
        {
            response = "Not enough arguments!";
            return false;
        }

        List<CursedPlayer> players = CursedCommandUtils.GetPlayers(arguments.At(0));

        if (players.Count is 0)
        {
            response = "Players not found.";
            return false;
        }

        if (!float.TryParse(arguments.At(1), out float x) || !float.TryParse(arguments.At(2), out float y) || !float.TryParse(arguments.At(3), out float z))
        {
            response = "Couldn't parse the X Y Z values.";
            return false;
        }

        Vector3 scale = new (x, y, z);
        foreach (CursedPlayer player in players)
            player.Scale = scale;
        
        /*
         Vector3 scale = new (x, y, z);
        foreach (CursedPlayer player in players)
        {
            player.Transform.localScale = scale;
            foreach (CursedPlayer target in CursedPlayer.Collection)
            {
                if (target == player)
                    continue;
                
                NetworkServer.SendSpawnMessage(player.NetworkIdentity, target.NetworkConnection);
            }

            player.Transform.localScale = Vector3.one;
        }
         */

        response = "Scaled players!";
        return true;
    }

    public string Command { get; } = "size";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Changes the size of players.";

    public string[] Usage { get; } = { "%player%", "Size X", "Size Y", "Size Z" };
}