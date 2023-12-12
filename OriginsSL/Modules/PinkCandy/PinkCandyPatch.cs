using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;
using InventorySystem.Items.Usables.Scp330;

namespace OriginsSL.Modules.PinkCandy;

[HarmonyPatch(typeof(CandyPink), nameof(CandyPink.SpawnChanceWeight), MethodType.Getter)]
public class PinkCandyPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        yield return new CodeInstruction(OpCodes.Ldc_R4, 0.25f);
        yield return new CodeInstruction(OpCodes.Ret);
    }
}