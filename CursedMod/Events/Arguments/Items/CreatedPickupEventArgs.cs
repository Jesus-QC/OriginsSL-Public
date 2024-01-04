// -----------------------------------------------------------------------
// <copyright file="CreatedPickupEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace CursedMod.Events.Arguments.Items;

public class CreatedPickupEventArgs(ItemType id, float weight, ushort serial)
    : EventArgs
{
    public ItemType ItemType { get; } = id;
    
    public float Weight { get; } = weight;
    
    public ushort Serial { get; } = serial;
}