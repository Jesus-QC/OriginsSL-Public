using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CursedMod.Features.Wrappers.Player;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using MapGeneration.Distributors;
using NorthwoodLib.Pools;

namespace OriginsSL.Modules.RemoteKeyCard;

[HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
public class GeneratorCheckPermissionsPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        int index = newInstructions.FindIndex(x
            => x.operand is MethodInfo m 
               && m == AccessTools.Method(typeof(DoorPermissionUtils), nameof(DoorPermissionUtils.HasFlagFast))) - 17;
        
        newInstructions.InsertRange(index, new CodeInstruction[]
        {
            new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
            new (OpCodes.Ldfld, AccessTools.Field(typeof(Scp079Generator), nameof(Scp079Generator._requiredPermission))),
            new (OpCodes.Ldarg_1),
            new (OpCodes.Call, AccessTools.Method(typeof(GeneratorCheckPermissionsPatch), nameof(CheckPerms))),
            new (OpCodes.Br, newInstructions[index + 18].operand)
        });
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    private static bool CheckPerms(KeycardPermissions permissions, ReferenceHub ply)
    {
        foreach (ItemBase item in CursedPlayer.Get(ply).Items.Values)
        {
            if (item is not KeycardItem keyCardItem)
                continue;

            if (keyCardItem.Permissions.HasFlagFast(permissions))
                return true;
        }
        
        return false;
    }
}