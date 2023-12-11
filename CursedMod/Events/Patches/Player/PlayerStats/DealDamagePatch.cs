// -----------------------------------------------------------------------
// <copyright file="DealDamagePatch.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using Footprinting;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerStatsSystem;

namespace CursedMod.Events.Patches.Player.PlayerStats;

[DynamicEventPatch(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.ReceivingDamage))]
[HarmonyPatch(typeof(PlayerStatsSystem.PlayerStats), nameof(PlayerStatsSystem.PlayerStats.DealDamage))]
public class DealDamagePatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<DealDamagePatch>(142, instructions);

        Label ret = generator.DefineLabel();
        
        newInstructions[newInstructions.Count - 1].labels.Add(ret);
        
        int offset = newInstructions.FindIndex(x => x.Calls(AccessTools.PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker)))) - 1;

        newInstructions.InsertRange(offset, new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[offset]),
            new (OpCodes.Ldarg_1),
            new (OpCodes.Ldloc_2),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(AttackerDamageHandler), nameof(AttackerDamageHandler.Attacker))),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(Footprint), nameof(Footprint.Hub))),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerReceivingDamageEventArgs))[0]),
            new (OpCodes.Dup),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.OnPlayerReceivingDamage))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerReceivingDamageEventArgs), nameof(PlayerReceivingDamageEventArgs.IsAllowed))),
            new (OpCodes.Brfalse_S, ret),
        });

        offset = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldnull);
        
        newInstructions.InsertRange(offset, new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[offset]),
            new (OpCodes.Ldarg_1),
            new (OpCodes.Ldnull),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerReceivingDamageEventArgs))[0]),
            new (OpCodes.Dup),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.OnPlayerReceivingDamage))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerReceivingDamageEventArgs), nameof(PlayerReceivingDamageEventArgs.IsAllowed))),
            new (OpCodes.Brfalse_S, ret),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}