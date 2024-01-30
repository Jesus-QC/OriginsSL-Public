// -----------------------------------------------------------------------
// <copyright file="CursedGameModeLoader.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using OriginsSL.Modules.GameModes.GameModes.RainbowRun;
using OriginsSL.Modules.GameModes.Misc.GameModeComponents;

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
            return TimeSpan.Zero;

        if (_currentGameMode.TryGetComponent(out GameModeMaxTimeComponent maxTimeComponent))
            return maxTimeComponent.OverrideTimer;
        
        return new TimeSpan(DateTime.Now.Ticks - _currentGameMode.StartTime);
    }
    
    public static bool EventRunning => _currentGameMode != null;
}