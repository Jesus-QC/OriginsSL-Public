using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace OriginsSL.Modules.ChaosTargets.Patches;

[HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.ChaosTargetCount), MethodType.Setter)]
public class ChaosTargetPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<ChaosTargetPatch>(9, instructions);

        newInstructions.InsertRange(0, new []
        {
            new CodeInstruction(OpCodes.Ldc_I4_0),
            new CodeInstruction(OpCodes.Starg_S, 1),
        });

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
	    
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}