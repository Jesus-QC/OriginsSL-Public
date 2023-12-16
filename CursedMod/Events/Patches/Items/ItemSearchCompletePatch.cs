// -----------------------------------------------------------------------
// <copyright file="ItemSearchCompletePatch.cs" company="CursedMod">
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
using InventorySystem.Items.Pickups;
using InventorySystem.Searching;
using NorthwoodLib.Pools;

namespace CursedMod.Events.Patches.Items;

[DynamicEventPatch(typeof(CursedItemsEventsHandler), nameof(CursedItemsEventsHandler.PlayerPickingUpItem))]
[HarmonyPatch(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.Complete))]
public class ItemSearchCompletePatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<ItemSearchCompletePatch>(29, instructions);

        Label skip = generator.DefineLabel();
        Label ret = generator.DefineLabel();

        newInstructions[0].labels.Add(skip);
        newInstructions[newInstructions.Count - 1].labels.Add(ret);
        
        newInstructions.InsertRange(0, new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new (OpCodes.Ldarg_0),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerPickingUpItemEventArgs))[0]),
            new (OpCodes.Dup),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedItemsEventsHandler), nameof(CursedItemsEventsHandler.OnPlayerPickingUpItem))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerPickingUpItemEventArgs), nameof(PlayerPickingUpItemEventArgs.IsAllowed))),
            new (OpCodes.Brtrue_S, skip),
            new (OpCodes.Ldarg_0),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
            new (OpCodes.Ldarg_0),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(ItemSearchCompletor), nameof(ItemSearchCompletor.TargetPickup))),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(ItemPickupBase), nameof(ItemPickupBase.Info))),
            new (OpCodes.Call, AccessTools.Method(typeof(ItemSearchCompletePatch), nameof(GetCancelInfo))),
            new (OpCodes.Callvirt, AccessTools.PropertySetter(typeof(ItemPickupBase), nameof(ItemPickupBase.NetworkInfo))),
            new (OpCodes.Br_S, ret),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    private static PickupSyncInfo GetCancelInfo(PickupSyncInfo oldInfo) => oldInfo with { InUse = false };
}