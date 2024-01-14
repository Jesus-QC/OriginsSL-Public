// -----------------------------------------------------------------------
// <copyright file="RoundSummaryPatch.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CursedMod.Events.Handlers;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace CursedMod.Events.Patches.Round;

[DynamicEventPatch(typeof(CursedRoundEventsHandler), nameof(CursedRoundEventsHandler.RoundEnded))]
[HarmonyPatch]
public class RoundSummaryPatch
{
    private static MethodInfo TargetMethod() => AccessTools.Method(typeof(RoundSummary).GetNestedTypes(AccessTools.all)[5], "MoveNext");

    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<RoundSummaryPatch>(600, instructions);

        int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Call && x.operand is MethodInfo mi && mi == AccessTools.Method(typeof(GameCore.Console), nameof(GameCore.Console.AddLog))) + 1;
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new (OpCodes.Call, AccessTools.Method(typeof(CursedRoundEventsHandler), nameof(CursedRoundEventsHandler.OnRoundEnded))),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}
