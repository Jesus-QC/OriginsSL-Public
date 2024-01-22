using System;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using NWAPIPermissionSystem;

namespace OriginsSL.Modules.PollManager;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class EndPollCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer player = CursedPlayer.Get(sender);

        if (!player.IsHost && !sender.CheckPermission("origins.poll.end"))
        {
            response = "You don't have perms to do that!";
            return false;
        }

        PollManager.TimeLeft = 0;
        
        response = "Poll Ended.";
        return true;
    }

    public string Command { get; } = "endpoll";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Ends a poll.";
}