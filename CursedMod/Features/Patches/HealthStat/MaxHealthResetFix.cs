// -----------------------------------------------------------------------
// <copyright file="MaxHealthResetFix.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Features.Wrappers.Player;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace CursedMod.Features.Patches.HealthStat;

[HarmonyPatch(typeof(PlayerStatsSystem.HealthStat), nameof(PlayerStatsSystem.HealthStat.ClassChanged))]
public class MaxHealthResetFix
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        LocalBuilder player = generator.DeclareLocal(typeof(CursedPlayer));
        LocalBuilder value = generator.DeclareLocal(typeof(float));

        Label end = generator.DefineLabel();

        newInstructions.InsertRange(0, new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerStatsSystem.HealthStat), nameof(PlayerStatsSystem.HealthStat.Hub))),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedPlayer), nameof(CursedPlayer.Get), new[] { typeof(ReferenceHub) })),
            new (OpCodes.Dup),
            new (OpCodes.Stloc_S, player.LocalIndex),
            new (OpCodes.Brfalse_S, end),
            new (OpCodes.Ldloc_S, player),
            new (OpCodes.Ldc_R4, 0.0f),
            new (OpCodes.Stfld, AccessTools.Field(typeof(CursedPlayer), nameof(CursedPlayer.OverrideMaxHealth))),
            new CodeInstruction(OpCodes.Nop).WithLabels(end),
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}