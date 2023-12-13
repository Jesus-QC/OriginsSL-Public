using System;
using System.Collections.Generic;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Features.Commands;
using OriginsSL.Modules.AdminTools.Fun.Components;
using NWAPIPermissionSystem;

namespace OriginsSL.Modules.AdminTools.Fun;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class RocketCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if (!sender.CheckPermission("origins.fun.rocket") && !ply.IsHost)
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
            player.GameObject.AddComponent<RocketComponent>().Player = player;
        }

        response = $"Done for {players.Count} players";
        return true;
    }

    public string Command { get; } = "rocket";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Makes players a rocket.";
    public string[] Usage { get; } = {"%player%"};
}