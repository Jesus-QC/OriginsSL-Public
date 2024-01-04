// -----------------------------------------------------------------------
// <copyright file="PlayerFlippingCoinPatch.cs" company="CursedMod">
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
using InventorySystem.Items.Coin;
using NorthwoodLib.Pools;
using PluginAPI.Core;

namespace CursedMod.Events.Patches.Items;

[DynamicEventPatch(typeof(CursedItemsEventsHandler), nameof(CursedItemsEventsHandler.PlayerFlippingCoin))]
[HarmonyPatch(typeof(Coin), nameof(Coin.ServerProcessCmd))]
public class PlayerFlippingCoinPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<PlayerFlippingCoinPatch>(73, instructions);

        Label retLabel = generator.DefineLabel();
        LocalBuilder args = generator.DeclareLocal(typeof(PlayerFlippingCoinEventArgs));

        int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldarg_0);
        
        newInstructions[newInstructions.Count - 1].labels.Add(retLabel);
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
            new (OpCodes.Ldloc_1),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerFlippingCoinEventArgs))[0]),
            new (OpCodes.Stloc_S, args.LocalIndex),
            
            new (OpCodes.Ldloc_S, args.LocalIndex),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedItemsEventsHandler), nameof(CursedItemsEventsHandler.OnPlayerFlippingCoin))),
           
            new (OpCodes.Ldloc_S, args.LocalIndex),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerFlippingCoinEventArgs), nameof(PlayerFlippingCoinEventArgs.IsAllowed))),
            new (OpCodes.Brfalse_S, retLabel),
            
            new (OpCodes.Ldloc_S, args.LocalIndex),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerFlippingCoinEventArgs), nameof(PlayerFlippingCoinEventArgs.IsTails))),
            new (OpCodes.Stloc_1),
        });

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}