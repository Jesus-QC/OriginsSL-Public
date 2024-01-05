// -----------------------------------------------------------------------
// <copyright file="CreatedItemEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;

namespace CursedMod.Events.Arguments.Items;

public class CreatedItemEventArgs(ushort serial, ItemType id)
    : EventArgs
{
    public ItemType ItemType { get; } = id;
    
    public ushort Serial { get; } = serial;
}