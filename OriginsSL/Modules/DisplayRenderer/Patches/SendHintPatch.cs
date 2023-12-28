using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events;
using CursedMod.Features.Wrappers.Player;
using HarmonyLib;
using Hints;
using NorthwoodLib.Pools;
using OriginsSL.Features.Display;
using OriginsSL.Modules.CustomLobby.Patches;

namespace OriginsSL.Modules.DisplayRenderer.Patches;

[HarmonyPatch(typeof(HintDisplay), nameof(HintDisplay.Show))]
public class SendHintPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<CheckPlayerPatch>(27, instructions);

        newInstructions.Clear();
        
        newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_0));
        newInstructions.Add(new CodeInstruction(OpCodes.Ldarg_1));
        newInstructions.Add(new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SendHintPatch), nameof(OverrideSend))));
        newInstructions.Add(new CodeInstruction(OpCodes.Ret));

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    private static void OverrideSend(HintDisplay hintDisplay, Hint hint)
    {
        if (!CursedPlayer.TryGet(hintDisplay.gameObject, out CursedPlayer player))
            return;
        
        if (hint is not TextHint textHint)
            return;
        
        player.SendOriginsHint(textHint.Text, ScreenZone.Environment, textHint.DurationScalar);
    }
}