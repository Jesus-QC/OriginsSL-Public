// -----------------------------------------------------------------------
// <copyright file="CursedGameModeLoader.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CursedMod.Features.Wrappers.Player.Dummies;
using OriginsSL.Modules.AudioPlayer;
using OriginsSL.Modules.GameModes.Misc;
using PlayerRoles;
using PluginAPI.Core;

namespace OriginsSL.Modules.GameModes;

public static class CursedGameModeLoader
{
    public static ICursedGameMode CurrentGameMode;
    
    public static readonly HashSet<ICursedGameMode> AvailableGameModes = [];
    
    public static void RegisterGameMode(ICursedGameMode gameMode) => AvailableGameModes.Add(gameMode);
    
    public static void UnRegisterGameMode(ICursedGameMode gameMode) => AvailableGameModes.Remove(gameMode);

    internal static void InitGameModes()
    {
        Log.Info("Loading GameModes...");
        LoadGameModes();
    }
    
    private static void LoadGameModes()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsInterface || !typeof(ICursedGameMode).IsAssignableFrom(type)) 
                continue;
            
            ICursedGameMode gameMode = (ICursedGameMode)Activator.CreateInstance(type);
            RegisterGameMode(gameMode);
            Log.Warning($"Loaded GameMode Name: {gameMode.Name}");
        }
    }

    public static void RunGameMode(ICursedGameMode gameMode)
    {
        CurrentGameMode = gameMode;
        gameMode.PrepareGameMode();
    }

    public static void StopGameMode()
    {
        CurrentGameMode.StopGameMode();
        CurrentGameMode = null;
    }
}