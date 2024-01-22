using System;
using System.Linq;
using System.Threading.Tasks;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using NWAPIPermissionSystem;

namespace OriginsSL.Modules.PollManager;


[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class PollCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer player = CursedPlayer.Get(sender);

        if (!player.IsHost && !sender.CheckPermission("origins.poll.start"))
        {
            response = "You don't have perms to do that!";
            return false;
        }

        string desc = arguments.Aggregate(string.Empty, (current, arg) => current + (arg + ' '));
        
        Task.Run(() =>
        {
#pragma warning disable CS4014
            PollManager.RunPoll(player.RealNickname, desc.TrimEnd(' '));
#pragma warning restore CS4014
        });
        
        response = "Poll published.";
        return true;
    }

    public string Command { get; } = "poll";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Makes a poll.";
}
