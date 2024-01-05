// -----------------------------------------------------------------------
// <copyright file="SpawnedItemEventArgs.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

namespace CursedMod.Events.Arguments.Items;

public class SpawnedItemEventArgs(ItemType id, ushort serial) : CreatedItemEventArgs(serial, id);