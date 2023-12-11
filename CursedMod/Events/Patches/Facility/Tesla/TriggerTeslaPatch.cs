// -----------------------------------------------------------------------
// <copyright file="TriggerTeslaPatch.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using CursedMod.Events.Arguments.Facility.Tesla;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace CursedMod.Events.Patches.Facility.Tesla;

[DynamicEventPatch(typeof(CursedTeslaEventHandler), nameof(CursedTeslaEventHandler.PlayerTriggerTesla))]
[HarmonyPatch(typeof(TeslaGateController), nameof(TeslaGateController.FixedUpdate))]
public class TriggerTeslaPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<TriggerTeslaPatch>(101, instructions);
        
        int index = newInstructions.FindIndex(i => i.Calls(AccessTools.PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.AllHubs))));

        newInstructions.RemoveRange(index, newInstructions.FindIndex(i => i.opcode == OpCodes.Endfinally) + 1 - index);

        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new (OpCodes.Ldloc_1),
            new (OpCodes.Ldloca_S, 2),
            new (OpCodes.Ldloca_S, 3),
            new (OpCodes.Call, AccessTools.Method(typeof(TriggerTeslaPatch), nameof(ProcessEventBehavior), new[] { typeof(TeslaGate), typeof(bool).MakeByRefType(), typeof(bool).MakeByRefType() })),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    private static void ProcessEventBehavior(TeslaGate teslaGate, ref bool inIdleRange, ref bool isTriggerable)
    {
        foreach (CursedPlayer player in CursedPlayer.List.Where(x => teslaGate.IsInIdleRange(x.ReferenceHub)))
        {
            PlayerTriggerTeslaEventArgs args = new (player.ReferenceHub, teslaGate);
            CursedTeslaEventHandler.OnPlayerTriggerTesla(args);

            if (!args.IsAllowed)
            {
                isTriggerable = false;
                inIdleRange = false;
                continue;
            }

            isTriggerable = args.IsTriggerable;
            inIdleRange = args.IsInIdleRange;
        }
    }
}