// -----------------------------------------------------------------------
// <copyright file="Scp106AttackingEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CursedMod.Features.Wrappers.Player;
using PlayerRoles.PlayableScps.Scp106;

namespace CursedMod.Events.Arguments.SCPs.Scp106;

public class Scp106AttackingEventArgs : EventArgs, ICursedPlayerEvent, ICursedCancellableEvent
{
    public Scp106AttackingEventArgs(Scp106Attack attack)
    {
        Player = CursedPlayer.Get(attack.Owner);
        Target = CursedPlayer.Get(attack._targetHub);
        IsAllowed = true;
    }
    
    public CursedPlayer Player { get; }
    
    public CursedPlayer Target { get; }

    public bool IsAllowed { get; set; }
}