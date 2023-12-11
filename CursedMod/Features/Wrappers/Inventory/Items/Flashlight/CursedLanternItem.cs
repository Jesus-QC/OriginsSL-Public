// -----------------------------------------------------------------------
// <copyright file="CursedLanternItem.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using InventorySystem.Items.ToggleableLights;
using InventorySystem.Items.ToggleableLights.Lantern;
using Utils.Networking;

namespace CursedMod.Features.Wrappers.Inventory.Items.Flashlight;

public class CursedLanternItem : CursedItem
{
    internal CursedLanternItem(LanternItem itemBase)
        : base(itemBase)
    {
        LanternBase = itemBase;
    }
    
    public LanternItem LanternBase { get; }
    
    public bool IsEmittingLight
    {
        get => LanternBase.IsEmittingLight;
        set
        {
            LanternBase.IsEmittingLight = value;
            new FlashlightNetworkHandler.FlashlightMessage(Base.ItemSerial, value).SendToAuthenticated();
        }
    }
}