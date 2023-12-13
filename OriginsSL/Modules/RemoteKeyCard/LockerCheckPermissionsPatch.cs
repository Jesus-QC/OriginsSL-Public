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

[HarmonyPatch(typeof(Locker), nameof(Locker.ServerInteract))]
public class LockerCheckPermissionsPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

        CodeInstruction c = newInstructions[newInstructions.FindIndex(x => x.operand is MethodInfo m && m == AccessTools.Method(typeof(Locker), nameof(Locker.CheckPerms)))];
        c.opcode = OpCodes.Call;
        c.operand = AccessTools.Method(typeof(LockerCheckPermissionsPatch), nameof(CheckPerms));
        
        foreach (CodeInstruction instruction in newInstructions)
            yield return instruction;
        
        ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }

    private static bool CheckPerms(Locker _, KeycardPermissions permissions, ReferenceHub ply)
    {
        if (permissions <= KeycardPermissions.None) 
            return true;
        
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