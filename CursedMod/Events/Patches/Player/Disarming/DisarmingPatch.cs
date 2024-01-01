// -----------------------------------------------------------------------
// <copyright file="DisarmingPatch.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using HarmonyLib;
using InventorySystem.Disarming;
using NorthwoodLib.Pools;

namespace CursedMod.Events.Patches.Player.Disarming;

[DynamicEventPatch(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.Disarming))]
[DynamicEventPatch(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.RemovingHandcuff))]
[HarmonyPatch(typeof(DisarmingHandlers), nameof(DisarmingHandlers.ServerProcessDisarmMessage))]
public class DisarmingPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<DisarmingPatch>(159, instructions);

        int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Newobj) - 3;
        
        Label retLabel = generator.DefineLabel();
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new (OpCodes.Ldloc_0),
            new (OpCodes.Ldarg_1),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerRemovingHandcuffEventArgs))[0]),
            new (OpCodes.Dup),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.OnPlayerRemovingHandcuff))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRemovingHandcuffEventArgs), nameof(PlayerRemovingHandcuffEventArgs.IsAllowed))),
            new (OpCodes.Brfalse_S, retLabel),
        });
        
        index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Newobj) - 3;
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
            new (OpCodes.Ldarg_1),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(DisarmMessage), nameof(DisarmMessage.PlayerToDisarm))),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerDisarmingEventArgs))[0]),
            new (OpCodes.Dup),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.OnPlayerDisarming))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerDisarmingEventArgs), nameof(PlayerDisarmingEventArgs.IsAllowed))),
            new (OpCodes.Brfalse_S, retLabel),
        });
        
        newInstructions[newInstructions.Count - 1].labels.Add(retLabel);
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}