using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events;
using HarmonyLib;
using NorthwoodLib.Pools;
using PlayerRoles;
using PlayerRoles.RoleAssign;

namespace OriginsSL.Modules.CustomLobby.Patches;

[HarmonyPatch(typeof(ScpPlayerPicker), nameof(ScpPlayerPicker.ChoosePlayers))]
public class ScpPlayerPickerPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
	    List<CodeInstruction> newInstructions = CursedEventManager.CheckEvent<ScpPlayerPickerPatch>(67 , instructions);

	    newInstructions.InsertRange(2, new CodeInstruction[]
	    {
		    new (OpCodes.Ldloc_0),
		    new (OpCodes.Call, AccessTools.Method(typeof(ScpPlayerPickerPatch), nameof(ChoosePlayers))),
	    });
	    
	    newInstructions.InsertRange(newInstructions.Count - 2, new []
	    {
		    new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[newInstructions.Count - 2]),
		    new (OpCodes.Call, AccessTools.Method(typeof(ScpPlayerPickerPatch), nameof(CleanTickets))),
	    });

	    foreach (CodeInstruction instruction in newInstructions)
		    yield return instruction;
	    
	    ListPool<CodeInstruction>.Shared.Return(newInstructions);
    }


    private static List<ReferenceHub> _hubs;

    private static void CleanTickets(ScpTicketsLoader ticketsLoader)
    {
	    foreach (ReferenceHub hub in _hubs)
	    {
		    ticketsLoader.ModifyTickets(hub, ticketsLoader.GetTickets(hub, 10) - 1000);
	    }
	    
	    _hubs.Clear();
    }
    
    private static void ChoosePlayers(ScpTicketsLoader ticketsLoader)
    {
	    _hubs = RoleManager.GetTeam(Team.SCPs);

	    foreach (ReferenceHub ply in _hubs)
	    {
		    ticketsLoader.ModifyTickets(ply, ticketsLoader.GetTickets(ply, 10) + 1000);
	    }
    }
}