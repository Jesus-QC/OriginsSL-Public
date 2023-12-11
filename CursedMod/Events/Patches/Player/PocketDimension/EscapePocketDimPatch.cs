// -----------------------------------------------------------------------
// <copyright file="EscapePocketDimPatch.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles.PlayableScps.Scp106;

namespace CursedMod.Events.Patches.Player.PocketDimension;

[DynamicEventPatch(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.EscapingPocketDimension))]
[HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
public class EscapePocketDimPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<EscapePocketDimPatch>(102, instructions);
        
        Label retLabel = generator.DefineLabel();

        int offset = newInstructions.FindIndex(i => i.Calls(AccessTools.Method(typeof(Scp106PocketExitFinder), nameof(Scp106PocketExitFinder.GetBestExitPosition)))) - 3;
        
        newInstructions.InsertRange(offset, new[]
        {
            new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[offset]),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerEscapingPocketDimensionEventArgs))[0]),
            new (OpCodes.Dup),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.OnPlayerEscapingPocketDimension))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerEscapingPocketDimensionEventArgs), nameof(PlayerEscapingPocketDimensionEventArgs.IsAllowed))),
            new (OpCodes.Brfalse_S, retLabel),
        });
        
        newInstructions[newInstructions.Count - 1].labels.Add(retLabel);
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}