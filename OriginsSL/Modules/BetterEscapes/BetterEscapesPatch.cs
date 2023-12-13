using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles;

namespace OriginsSL.Modules.BetterEscapes;

[HarmonyPatch(typeof(Escape), nameof(Escape.ServerGetScenario))]
public class BetterEscapesPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<BetterEscapesPatch>(60, instructions);

        int offset = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_3);

        LocalBuilder team = generator.DeclareLocal(typeof(Team));

        Label chaos = generator.DefineLabel();
        Label skip = generator.DefineLabel();

        newInstructions[offset].labels.Add(skip);

        newInstructions.InsertRange(offset, new List<CodeInstruction>()
        {
            new (OpCodes.Ldloc_3),
            new (OpCodes.Call, AccessTools.DeclaredMethod(typeof(PlayerRolesUtils), nameof(PlayerRolesUtils.GetTeam), new []{typeof(RoleTypeId)})),
            new (OpCodes.Stloc_S, team.LocalIndex),

            new (OpCodes.Ldloc_S, team.LocalIndex),
            new (OpCodes.Ldc_I4_1),
            new (OpCodes.Ceq),
            new (OpCodes.Brfalse_S, chaos),

            new (OpCodes.Ldloc_1),
            new (OpCodes.Brfalse_S, skip),
            new (OpCodes.Ldc_I4_4),
            new (OpCodes.Ret),

            new CodeInstruction(OpCodes.Ldloc_S, team.LocalIndex).WithLabels(chaos),
            new (OpCodes.Ldc_I4_2),
            new (OpCodes.Ceq),
            new (OpCodes.Brfalse_S, skip),

            new (OpCodes.Ldloc_1),
            new (OpCodes.Brfalse_S, skip),
            new (OpCodes.Ldc_I4_2),
            new (OpCodes.Ret),
        });

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}