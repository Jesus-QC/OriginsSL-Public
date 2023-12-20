// -----------------------------------------------------------------------
// <copyright file="GamemodeCommand.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using CommandSystem;
using CursedMod.Features.Wrappers.Server.Gamemodes;

namespace CursedMod.Loader.Commands.IntegratedCommands;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class GamemodeCommand : ICommand, IUsageProvider
{
    public string Command { get; } = "gamemode";
    
    public string[] Aliases { get; } = { "gm" };
    
    public string Description { get; } = "Manage the Gamemodes installed.";
    
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
                if (CursedGamemodeLoader.AvailableGamemodes.IsEmpty())
                {
                    response = "There are no Gamemodes available";
                    return false;
                }

                response = "Available Gamemodes:\n";
                
                foreach (var gamemode in CursedGamemodeLoader.AvailableGamemodes)
                {
                    response += $"- {gamemode.GamemodeName} ({gamemode.GamemodeDescription})\n";
                }

                return true;
            
            case "enable":
                if (arguments.Count < 2)
                {
                    response = "Usage: gamemode enable <gamemode>";
                    return false;
                }

                CursedGamemode gamemodeToEnable = CursedGamemodeLoader.AvailableGamemodes.FirstOrDefault(gamemode => gamemode.GamemodeName.Equals(arguments.At(1), StringComparison.OrdinalIgnoreCase));

                if (gamemodeToEnable is null)
                {
                    response = $"Gamemode {arguments.At(1)} not found";
                    return false;
                }
                
                if (CursedGamemodeLoader.GamemodeQueue.Any(ev => ev.GamemodeName == gamemodeToEnable.GamemodeName))
                {
                    response = "The gamemode is already in the queue.";
                    return false;
                }
                
                CursedGamemodeLoader.AddGamemodeToQueue(gamemodeToEnable);
            
                response = "Done";
                return true;
            
            case "disable":
                if (arguments.Count < 2)
                {
                    response = "Usage: gamemode disable <queuepos>";
                    return false;
                }
                
                int queuepos = Convert.ToInt16(arguments.At(1));
                
                if (queuepos >= 1 && CursedGamemodeLoader.GamemodeQueue.Count <= 0)
                {
                    if (CursedGamemodeLoader.GamemodeQueue == null)
                    {
                        response = "The Gamemode Queue is empty";
                        return false;
                    }
                }
                else
                {
                    if (CursedGamemodeLoader.CurrentGamemode == null)
                    {
                        response = "There is no Gamemode running";
                        return false;
                    }
                }
                
                if (queuepos >= 1)
                {
                    CursedGamemodeLoader.RemoveGamemodeFromQueue(queuepos);
                    
                    response = "Done";
                    return true;
                }
                
                response = "Usage: gamemode disable <queuepos>";
                return false;
            
            case "queue":
                response = "Gamemode Queue:\n";
                response += CursedGamemodeLoader.GamemodeQueue.Count > 0 ? CursedGamemodeLoader.GamemodeQueue.Aggregate(" ", (current, gamemode) => current + $"- {gamemode.GamemodeName} ({gamemode.GamemodeDescription})\n") : "The Gamemode Queue is empty";
                return true;
        }
        
        response = "Usage: gamemode <list|enable|disable|queue> [gamemode]";
        return false;
    }
}