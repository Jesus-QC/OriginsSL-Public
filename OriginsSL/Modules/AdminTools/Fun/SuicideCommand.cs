using System;
using CommandSystem;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Modules.AdminTools.Fun.Components;

namespace OriginsSL.Modules.AdminTools.Fun;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class SuicideCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer player = CursedPlayer.Get(sender);
        player.GameObject.AddComponent<RocketComponent>().Player = player;
        
        response = "Boom";
        return true;
    }

    public string Command { get; } = "suicide";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Lets you suicide.";
}