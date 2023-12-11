// -----------------------------------------------------------------------
// <copyright file="PlayerEscapingPocketDimensionEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CursedMod.Features.Wrappers.Player;

namespace CursedMod.Events.Arguments.Player;

public class PlayerEscapingPocketDimensionEventArgs : EventArgs, ICursedCancellableEvent, ICursedPlayerEvent
{
    public PlayerEscapingPocketDimensionEventArgs(ReferenceHub hub)
    {
        Player = CursedPlayer.Get(hub);
        IsAllowed = true;
    }
    
    public bool IsAllowed { get; set; }
    
    public CursedPlayer Player { get; }
}