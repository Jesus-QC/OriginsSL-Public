// -----------------------------------------------------------------------
// <copyright file="CursedTeslaEventHandler.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using CursedMod.Events.Arguments.Facility.Tesla;

namespace CursedMod.Events.Handlers;

public static class CursedTeslaEventHandler
{
    public static event CursedEventManager.CursedEventHandler<PlayerTriggerTeslaEventArgs> PlayerTriggerTesla;
    
    internal static void OnPlayerTriggerTesla(PlayerTriggerTeslaEventArgs args)
    {
        if (!args.Player.CheckPlayer())
            return;
        
        PlayerTriggerTesla.InvokeEvent(args);
    }
}