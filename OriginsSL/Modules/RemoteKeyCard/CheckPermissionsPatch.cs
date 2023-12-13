using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using CursedMod.Features.Wrappers.Player;
using HarmonyLib;
using Interactables.Interobjects.DoorUtils;
using InventorySystem.Items;
using InventorySystem.Items.Keycards;
using NorthwoodLib.Pools;
using PlayerRoles;

namespace OriginsSL.Modules.RemoteKeyCard;

[HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract))]
public class CheckPermissionsPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        CodeInstruction c = newInstructions[newInstructions.FindIndex(x => x.operand is MethodInfo m && m == AccessTools.Method(typeof(DoorPermissions), nameof(DoorPermissions.CheckPermissions)))];
        c.opcode = OpCodes.Call;
        c.operand = AccessTools.Method(typeof(CheckPermissionsPatch), nameof(CheckPerms));
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    private static bool CheckPerms(DoorPermissions permissions, ItemBase _, ReferenceHub ply)
    {
        if (permissions.RequiredPermissions == KeycardPermissions.None)
        {
            return true;
        }
        if (ply is not null)
        {
            if (ply.serverRoles.BypassMode)
            {
                return true;
            }
        }

        foreach (ItemBase item in CursedPlayer.Get(ply).Items.Values)
        {
            if(item is not KeycardItem keyCardItem)
                continue;

            if ((!permissions.RequireAll && (keyCardItem.Permissions & permissions.RequiredPermissions) > KeycardPermissions.None) || (keyCardItem.Permissions & permissions.RequiredPermissions) == permissions.RequiredPermissions)
                return true;
        }

        return ply.IsSCP() && permissions.RequiredPermissions.HasFlagFast(KeycardPermissions.ScpOverride);
    }
}