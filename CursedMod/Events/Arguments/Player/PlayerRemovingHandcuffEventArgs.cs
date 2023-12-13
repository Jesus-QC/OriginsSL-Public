// -----------------------------------------------------------------------
// <copyright file="PlayerRemovingHandcuffEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CursedMod.Features.Wrappers.Player;

namespace CursedMod.Events.Arguments.Player;

public class PlayerRemovingHandcuffEventArgs : EventArgs, ICursedPlayerEvent, ICursedCancellableEvent
{
    public PlayerRemovingHandcuffEventArgs(ReferenceHub player, ReferenceHub target)
    {
        Player = CursedPlayer.Get(player);
        Target = CursedPlayer.Get(target);
        IsAllowed = true;
    }
    
    public CursedPlayer Player { get; }
    
    public CursedPlayer Target { get; }

    public bool IsAllowed { get; set; }
}