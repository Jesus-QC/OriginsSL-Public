// -----------------------------------------------------------------------
// <copyright file="PlayerTriggerTeslaEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CursedMod.Features.Wrappers.Facility.Props;
using CursedMod.Features.Wrappers.Player;

namespace CursedMod.Events.Arguments.Facility.Tesla;

public class PlayerTriggerTeslaEventArgs : EventArgs, ICursedPlayerEvent, ICursedTeslaEvent, ICursedCancellableEvent
{
    public PlayerTriggerTeslaEventArgs(ReferenceHub hub, TeslaGate teslaGate)
    {
        Player = CursedPlayer.Get(hub);
        Tesla = CursedTeslaGate.Get(teslaGate);
        IsAllowed = true;
        IsInIdleRange = true;
        IsTriggerable = teslaGate.PlayerInRange(Player.ReferenceHub);
    }
    
    public CursedPlayer Player { get; }

    public CursedTeslaGate Tesla { get; }
    
    public bool IsInIdleRange { get; set; }
    
    public bool IsTriggerable { get; set; }

    public bool IsAllowed { get; set; }
}