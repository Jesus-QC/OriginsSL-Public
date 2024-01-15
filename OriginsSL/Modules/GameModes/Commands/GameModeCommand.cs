// -----------------------------------------------------------------------
// <copyright file="GameModeCommand.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;

namespace OriginsSL.Modules.GameModes.Commands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class GameModeCommand : ICommand
{
    public string Command { get; } = "gamemode";
    
    public string[] Aliases { get; } = ["gm"];
    
    public string Description { get; } = "Manage the game modes installed.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count < 1)
        {
            response = "Usage: gamemode <list|run|stop> [gamemode]";
            return false;
        }

        switch (arguments.At(0))
        {
            case "list":
                if (CursedGameModeLoader.AvailableGameModes.IsEmpty())
                {
                    response = "There are no game modes available";
                    return false;
                }

                response = CursedGameModeLoader.AvailableGameModes.Aggregate("Available Gamemodes:\n", (current, gm) => current + $"- {gm.Name} ({gm.Description})\n");
                return true;
            
            case "run":
                if (arguments.Count < 2)
                {
                    response = "Usage: gamemode run <gamemode>";
                    return false;
                }

                CursedGameModeBase gameModeToEnable = CursedGameModeLoader.AvailableGameModes.FirstOrDefault(gm => gm.CodeName.Equals(arguments.At(1), StringComparison.OrdinalIgnoreCase));

                if (gameModeToEnable is null)
                {
                    response = $"Game mode {arguments.At(1)} not found";
                    return false;
                }
                
                CursedGameModeLoader.RunGameMode(Activator.CreateInstance(gameModeToEnable.GetType()) as CursedGameModeBase);
                response = "Done";
                return true;
            
            case "stop":
                CursedGameModeLoader.StopGameMode();
                response = "Done";
                return true;
        }
        
        response = "Usage: gamemode <list|run|stop> [gamemode]";
        return false;
    }
}