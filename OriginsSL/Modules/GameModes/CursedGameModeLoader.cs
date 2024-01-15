// -----------------------------------------------------------------------
// <copyright file="CursedGameModeLoader.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using OriginsSL.Modules.GameModes.GameModes.RainbowRun;

namespace OriginsSL.Modules.GameModes;

public static class CursedGameModeLoader
{
    private static CursedGameModeBase _currentGameMode;
    
    public static readonly CursedGameModeBase[] AvailableGameModes = 
    [
        new RainbowRunGameMode(),
    ];
    
    public static void RunGameMode(CursedGameModeBase gameMode)
    {
        _currentGameMode = gameMode;
        gameMode.StartGameMode();
    }

    public static void StopGameMode()
    {
        _currentGameMode.StopGameMode();
        _currentGameMode = null;
    }
    
    public static string GetEventName() => _currentGameMode == null ? string.Empty : _currentGameMode.Name;

    public static string GetEventDescription() => _currentGameMode == null ? string.Empty : _currentGameMode.Description;
    
    public static TimeSpan GetEventTimer()
    {
        if (_currentGameMode == null)
            return new TimeSpan(0);
        
        return _currentGameMode.OverrideTimer == TimeSpan.Zero 
            ? new TimeSpan(DateTime.Now.Ticks - _currentGameMode.StartTime) 
            : _currentGameMode.OverrideTimer;
    }
    
    public static bool EventRunning => _currentGameMode != null;
}