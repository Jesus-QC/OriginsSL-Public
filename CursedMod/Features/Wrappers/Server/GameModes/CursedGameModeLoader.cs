// -----------------------------------------------------------------------
// <copyright file="CursedGameModeLoader.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CursedMod.Events.Handlers;
using CursedMod.Loader;
using CursedMod.Loader.Modules;
using PluginAPI.Core;

namespace CursedMod.Features.Wrappers.Server.GameModes;

public static class CursedGameModeLoader
{
    public static ICursedGameMode CurrentGameMode;
    
    public static readonly Queue<ICursedGameMode> GameModeQueue = [];
    
    public static readonly HashSet<ICursedGameMode> AvailableGameModes = [];
    
    public static void RegisterGameMode(ICursedGameMode gameMode) => AvailableGameModes.Add(gameMode);
    
    public static void UnRegisterGameMode(ICursedGameMode gameMode) => AvailableGameModes.Remove(gameMode);
    
    public static void AddGameModeToQueue(ICursedGameMode gameMode) => GameModeQueue.Enqueue(gameMode);

    internal static void InitGameModes()
    {
        Log.Info("Loading GameModes...");
        
        LoadGameModes();
        
        CursedRoundEventsHandler.WaitingForPlayers += OnWaitingForPlayers;
        CursedRoundEventsHandler.RestartingRound += OnRestartingRound;
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
    }
    
    private static void LoadGameModes()
    {
        foreach (ICursedModule module in CursedLoader.EnabledModules)
        {
            foreach (Type type in module.ModuleAssembly.GetTypes())
            {
                if (type.IsInterface || !typeof(ICursedGameMode).IsAssignableFrom(type)) 
                    continue;
            
                ICursedGameMode gameMode = (ICursedGameMode)Activator.CreateInstance(type);
                RegisterGameMode(gameMode);
                Log.Warning("Loaded GameMode from plugin: " + module.ModuleName + $" | GameMode Name: {gameMode.GameModeName}");
            }
        }
    }
    
    private static void OnWaitingForPlayers()
    {
        CurrentGameMode = null;
        
        if (GameModeQueue.Count > 0)
            CurrentGameMode = GameModeQueue.Dequeue();

        if (CurrentGameMode is null) 
            return;
        
        CurrentGameMode.PrepareGameMode();
        Log.Info("Enabled GameMode: " + CurrentGameMode.GameModeName + " | GameMode Description: " + CurrentGameMode.GameModeDescription + " For this round");
    }
    
    private static void DisableGameModes()
    {
        AvailableGameModes.Clear();
        GameModeQueue.Clear();
        CurrentGameMode = null;
    }
    
    private static void OnRestartingRound()
    {
        DisableCurrentGameMode();
    }

    private static void OnRoundEnded()
    {
        DisableCurrentGameMode();
    }

    private static void DisableCurrentGameMode()
    {
        if (CurrentGameMode is null)
            return;
        
        Log.Info("Disabled GameMode: " + CurrentGameMode.GameModeName + " Due to a round end");
        CurrentGameMode.StopGameMode();
        CurrentGameMode = null;
    }
}