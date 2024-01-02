using System;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Server;
using NWAPIPermissionSystem;

namespace OriginsSL.Modules.Emote.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(ClientCommandHandler))]
public class EmoteCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        if ((CursedServer.Port == 7778 && !sender.CheckPermission("origins.fun.emote")) || !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }

        if (ply.IsDead)
        {
            response = "You can't dance while dead.";
            return false;
        }
        
        EmoteHandler.Dance(ply);
        response = "Dancing!";
        return true;
    }

    public string Command { get; } = "emote";
    public string[] Aliases { get; } = ["dance"];
    public string Description { get; } = "Dances";
}