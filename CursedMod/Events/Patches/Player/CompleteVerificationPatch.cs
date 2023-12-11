// -----------------------------------------------------------------------
// <copyright file="CompleteVerificationPatch.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;
using CentralAuth;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using HarmonyLib;
using NorthwoodLib.Pools;

namespace CursedMod.Events.Patches.Player;

[HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.FinalizeAuthentication))]
public class CompleteVerificationPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<CompleteVerificationPatch>(142, instructions);

        Label ret = generator.DefineLabel();
        
        newInstructions[newInstructions.Count - 1].labels.Add(ret);

        int offset = newInstructions.FindIndex(i =>
            i.Calls(AccessTools.Method(typeof(ServerRoles), nameof(ServerRoles.RefreshPermissions), new[] { typeof(bool) }))) + 1;
        
        newInstructions.InsertRange(offset, new List<CodeInstruction>()
        {
            new (OpCodes.Ldarg_0),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager._hub))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.isLocalPlayer))),
            new (OpCodes.Brtrue_S, ret),
            new (OpCodes.Ldarg_0),
            new (OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(PlayerConnectedEventArgs))[0]),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedPlayerEventsHandler), nameof(CursedPlayerEventsHandler.OnPlayerConnected))),
        });

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}