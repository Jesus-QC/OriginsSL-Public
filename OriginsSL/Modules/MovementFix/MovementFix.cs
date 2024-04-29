using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles.FirstPersonControl;

namespace OriginsSL.Modules.MovementFix;

[HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.DesiredMove), MethodType.Getter)]
internal class MovementFix
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<MovementFix>(165, instructions);

        int offset = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Newobj) + 1;

        newInstructions.InsertRange(offset, new[]
        {
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Ldloc_3),
            new CodeInstruction(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(FpcMotor), nameof(FpcMotor.Position))),
        });

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;

        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}