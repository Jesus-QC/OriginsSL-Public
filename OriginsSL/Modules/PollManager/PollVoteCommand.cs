using System;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;

namespace OriginsSL.Modules.PollManager;

[CommandHandler(typeof(ClientCommandHandler))]
public class PollVoteCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer player = CursedPlayer.Get(sender);

        if (arguments.Count == 0)
        {
            response = "To vote use .vote yes or .vote no";
            return false;
        }

        string arg = arguments.At(0).ToLower();
        
        if (arg is not ("yes" or "no"))
        {
            response = "To vote use .vote yes or .vote no";
            return false;
        }

        if (PollManager.AddVote(player, arg is "yes"))
        {
            response = "Voted";
            return true;
        }
        
        response = "You have already voted!";
        return false;
    }

    public string Command { get; } = "vote";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Command for voting on polls.";
}