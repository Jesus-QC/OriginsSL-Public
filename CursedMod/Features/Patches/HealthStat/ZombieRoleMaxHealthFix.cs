// -----------------------------------------------------------------------
// <copyright file="ZombieRoleMaxHealthFix.cs" company="CursedMod">
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
using PlayerRoles.PlayableScps.Scp049.Zombies;

namespace CursedMod.Features.Patches.HealthStat;

[HarmonyPatch(typeof(ZombieRole), nameof(ZombieRole.MaxHealth), MethodType.Getter)]
public class ZombieRoleMaxHealthFix
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
            new (OpCodes.Ldfld, AccessTools.Field(typeof(ZombieRole), nameof(ZombieRole._lastOwner))),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedPlayer), nameof(CursedPlayer.Get), new[] { typeof(ReferenceHub) })),
            new (OpCodes.Dup),
            new (OpCodes.Stloc_S, player.LocalIndex),
            new (OpCodes.Brfalse_S, end),
            new (OpCodes.Ldloc_S, player),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(CursedPlayer), nameof(CursedPlayer.OverrideMaxHealth))),
            new (OpCodes.Dup),
            new (OpCodes.Stloc_S, value.LocalIndex),
            new (OpCodes.Ldc_R4, 0.0f),
            new (OpCodes.Beq_S, end),
            new (OpCodes.Ldloc_S, value.LocalIndex),
            new (OpCodes.Ret),

            new CodeInstruction(OpCodes.Nop).WithLabels(end),
        });

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}