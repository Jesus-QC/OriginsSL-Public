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
public class GameModeCommand : ICommand, IUsageProvider
{
    public string Command { get; } = "gamemode";
    
    public string[] Aliases { get; } = { "gm" };
    
    public string Description { get; } = "Manage the game modes installed.";
    
    public string[] Usage { get; } = { "%list%", "%enable%", "%disable%", "%queue%" };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, [UnscopedRef] out string response)
    {
        if (arguments.Count < 1)
        {
            response = "Usage: gamemode <list|enable|disable|queue> [gamemode]";
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

                response = CursedGameModeLoader.AvailableGameModes.Aggregate("Available Gamemodes:\n", (current, gm) => current + $"- {gm.GameModeName} ({gm.GameModeDescription})\n");
                return true;
            
            case "enable":
                if (arguments.Count < 2)
                {
                    response = "Usage: gamemode enable <gamemode>";
                    return false;
                }

                ICursedGameMode gameModeToEnable = CursedGameModeLoader.AvailableGameModes.FirstOrDefault(gm => gm.GameModeName.Equals(arguments.At(1), StringComparison.OrdinalIgnoreCase));

                if (gameModeToEnable is null)
                {
                    response = $"Game mode {arguments.At(1)} not found";
                    return false;
                }
                
                CursedGameModeLoader.AddGameModeToQueue(Activator.CreateInstance(gameModeToEnable.GetType()) as ICursedGameMode);
            
                response = "Done";
                return true;
            
            case "queue":
                response = "Game mode Queue:\n";
                response += CursedGameModeLoader.GameModeQueue.Count > 0 ? CursedGameModeLoader.GameModeQueue.Aggregate(" ", (current, gm) => current + $"- {gm.GameModeName} ({gm.GameModeDescription})\n") : "The Gamemode Queue is empty";
                return true;
        }
        
        response = "Usage: gamemode <list|enable|disable|queue> [gamemode]";
        return false;
    }
}