// -----------------------------------------------------------------------
// <copyright file="PlayerPickedUpItemEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using CursedMod.Features.Wrappers.Inventory.Items;
using CursedMod.Features.Wrappers.Player;
using InventorySystem;
using InventorySystem.Items;

namespace CursedMod.Events.Arguments.Items;

public class PlayerPickedUpItemEventArgs : EventArgs, ICursedPlayerEvent, ICursedItemEvent 
{
    public PlayerPickedUpItemEventArgs(Inventory inventory, ItemBase itemBase)
    {
        Player = CursedPlayer.Get(inventory._hub);
        Item = CursedItem.Get(itemBase);
    }

    public CursedPlayer Player { get; }

    public CursedItem Item { get; }
}