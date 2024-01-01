// -----------------------------------------------------------------------
// <copyright file="PlayerReceivingDamageEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CursedMod.Features.Wrappers.Player;
using PlayerStatsSystem;

namespace CursedMod.Events.Arguments.Player;

public class PlayerReceivingDamageEventArgs : EventArgs, ICursedCancellableEvent, ICursedPlayerEvent
{
    public PlayerReceivingDamageEventArgs(PlayerStats playerStats, DamageHandlerBase damageHandlerBase, ReferenceHub attacker)
    {
        IsAllowed = true;
        Player = CursedPlayer.Get(playerStats._hub);
        DamageHandlerBase = damageHandlerBase;
        Attacker = CursedPlayer.Get(attacker);
    }
    
    public bool IsAllowed { get; set; }
    
    public CursedPlayer Player { get; }

    public DamageHandlerBase DamageHandlerBase { get; }

    public float DamageAmount
    {
        get => (DamageHandlerBase as StandardDamageHandler)?.Damage ?? 0;
        set
        {
            if (DamageHandlerBase is StandardDamageHandler standardDamageHandler)
            {
                standardDamageHandler.Damage = value;
            }
        }
    }
    
    public CursedPlayer Attacker { get; }
}