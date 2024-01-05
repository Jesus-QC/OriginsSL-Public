// -----------------------------------------------------------------------
// <copyright file="CreatedNewItemPatch.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events.Arguments.Items;
using CursedMod.Events.Handlers;
using HarmonyLib;
using InventorySystem;
using NorthwoodLib.Pools;

namespace CursedMod.Events.Patches.Items.Pickups;

[DynamicEventPatch(typeof(CursedItemsEventsHandler), nameof(CursedItemsEventsHandler.CreatedItem))]
[DynamicEventPatch(typeof(CursedItemsEventsHandler), nameof(CursedItemsEventsHandler.SpawnedItem))]
[HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
public class CreatedNewItemPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<CreatedPickupEventArgs>(81, instructions);

        int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Starg_S);
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new (OpCodes.Dup),
            new (OpCodes.Ldarg_1),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(CreatedItemEventArgs))[0]),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedItemsEventsHandler), nameof(CursedItemsEventsHandler.OnCreatedItem))),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}