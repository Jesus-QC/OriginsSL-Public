using System;
using System.Globalization;
using CommandSystem;
using CursedMod.Features.Wrappers.Server;

namespace OriginsSL.Modules.AdminTools.Fun;

[CommandHandler(typeof(ClientCommandHandler))]
[CommandHandler(typeof(GameConsoleCommandHandler))]
public class TpsCommand : ICommand
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        response = CursedServer.TicksPerSecond.ToString(CultureInfo.InvariantCulture);
        return true;
    }

    public string Command { get; } = "tps";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Shows you the actual tps.";
}