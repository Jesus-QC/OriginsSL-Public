// -----------------------------------------------------------------------
// <copyright file="PlayerFlippingCoinEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CursedMod.Features.Wrappers.Inventory.Items;
using CursedMod.Features.Wrappers.Player;
using InventorySystem.Items.Coin;

namespace CursedMod.Events.Arguments.Items;

public class PlayerFlippingCoinEventArgs(Coin coin, bool isTails)
    : EventArgs, ICursedPlayerEvent, ICursedItemEvent, ICursedCancellableEvent
{
    public bool IsAllowed { get; set; } = true;

    public CursedPlayer Player { get; } = CursedPlayer.Get(coin.Owner);

    public CursedItem Item { get; } = CursedItem.Get(coin);

    public bool IsTails { get; set; } = isTails;
}