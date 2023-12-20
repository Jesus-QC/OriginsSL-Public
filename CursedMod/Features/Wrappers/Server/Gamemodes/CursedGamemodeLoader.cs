// -----------------------------------------------------------------------
// <copyright file="CursedGamemodeLoader.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using CursedMod.Events.Handlers;
using CursedMod.Loader;
using PluginAPI.Core;

namespace CursedMod.Features.Wrappers.Server.Gamemodes;

public static class CursedGamemodeLoader
{
    public static CursedGamemode CurrentGamemode = null;
    
    public static HashSet<CursedGamemode> GamemodeQueue = [];
    
    public static HashSet<CursedGamemode> AvailableGamemodes = [];

    internal static void LoadGamemodes()
    {
        Log.Info("Loading gamemodes...");
        
        foreach (var module in CursedLoader.EnabledModules)
        {
            foreach (var type in module.ModuleAssembly.GetTypes().Where(x => typeof(CursedGamemode).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract))
            {
                var ctor = type.GetConstructor(Type.EmptyTypes);
                
                if (ctor is null)
                    continue;
                    
                if (!typeof(CursedGamemode).IsAssignableFrom(type))
                    continue;

                if (ctor.Invoke(null) is not CursedGamemode gamemode)
                    continue;
                
                RegisterGamemode(gamemode);
                Log.Warning("Loaded Gamemode from plugin: " + module.ModuleName + $" | Gamemode Name: {gamemode.GamemodeName}");
            }
        }
        
        CursedRoundEventsHandler.WaitingForPlayers += OnWaitingForPlayers;
        CursedRoundEventsHandler.RestartingRound += OnRestartingRound;
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
    }

    internal static void DisableGamemodes()
    {
        AvailableGamemodes.Clear();
        GamemodeQueue.Clear();
        CurrentGamemode = null;
    }
    
    public static void AddGamemodeToQueue(CursedGamemode gamemode)
    {
        GamemodeQueue.Add(gamemode);
    }
    
    public static void RemoveGamemodeFromQueue(CursedGamemode gamemode)
    {
        GamemodeQueue.Remove(gamemode);
    }
    
    public static void RemoveGamemodeFromQueue(int queuePos)
    {
        var toRemove = GamemodeQueue.ElementAt(queuePos - 1);
        GamemodeQueue.Remove(toRemove);
    }
    
    public static void UnRegisterGamemode(CursedGamemode gamemode)
    {
        AvailableGamemodes.Remove(gamemode);
    }
    
    private static void RegisterGamemode(CursedGamemode gamemode)
    {
        AvailableGamemodes.Add(gamemode);
    }
    
    private static void OnWaitingForPlayers()
    {
        if (GamemodeQueue.Count >= 1)
        {
            var next = GamemodeQueue.FirstOrDefault();
            CurrentGamemode = next;
            GamemodeQueue.Remove(next);
        }

        if (CurrentGamemode is null) 
            return;
        
        CurrentGamemode.PrepareGamemode();
        Log.Info("Enabled Gamemode: " + CurrentGamemode.GamemodeName + " | Gamemode Description: " + CurrentGamemode.GamemodeDescription + " For this round");
    }

    private static void OnRestartingRound()
    {
        if (CurrentGamemode is null) 
            return;
        
        Log.Info("Disabled Gamemode: " + CurrentGamemode.GamemodeName + " Due to a round restart");
        CurrentGamemode.StopGamemode();
        CurrentGamemode = null;
    }

    private static void OnRoundEnded()
    {
        if (CurrentGamemode is null)
            return;
        
        Log.Info("Disabled Gamemode: " + CurrentGamemode.GamemodeName + " Due to a round end");
        CurrentGamemode.StopGamemode();
        CurrentGamemode = null;
    }
}