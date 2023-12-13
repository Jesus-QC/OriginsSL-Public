using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events;
using CursedMod.Features.Wrappers.Player.Dummies;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.RoleAssign;

namespace OriginsSL.Modules.CustomLobby.Patches;

[HarmonyPatch(typeof(RoleAssigner), nameof(RoleAssigner.CheckPlayer))]
public class CheckPlayerPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<CheckPlayerPatch>(27, instructions);

        Label skip = generator.DefineLabel();
        Label skip2 = generator.DefineLabel();
        
        newInstructions[0].labels.Add(skip);
        
        newInstructions.InsertRange(0, new List<CodeInstruction>()
        {
            new (OpCodes.Ldarg_0),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),
            new (OpCodes.Call, AccessTools.Method(typeof(CursedDummy), nameof(CursedDummy.IsDummy))),
            new (OpCodes.Brfalse_S, skip2),
            new (OpCodes.Ldc_I4_0),
            new (OpCodes.Ret),
            new CodeInstruction(OpCodes.Ldarg_0).WithLabels(skip2),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(ReferenceHub), nameof(ReferenceHub.roleManager))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleManager), nameof(PlayerRoleManager.CurrentRole))),
            new (OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(PlayerRoleBase), nameof(PlayerRoleBase.RoleTypeId))),
            new (OpCodes.Ldc_I4, 14),
            new (OpCodes.Ceq),
            new (OpCodes.Brfalse_S, skip),
            new (OpCodes.Ldc_I4_1),
            new (OpCodes.Ret)
        });

        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }
}