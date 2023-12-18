using System;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using NWAPIPermissionSystem;
using OriginsSL.Modules.Emote;

namespace OriginsSL.Modules.AdminTools.Fun;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class EmoteCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if (!sender.CheckPermission("origins.fun.emote") && !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }
        
        ply.Dance();
        response = "Dancing!";
        return true;
    }

    public string Command { get; } = "emote";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Dances";
}