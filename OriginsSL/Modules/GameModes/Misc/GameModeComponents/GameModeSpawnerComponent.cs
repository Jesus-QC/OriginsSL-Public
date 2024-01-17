using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Modules.Subclasses;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.GameModes.Misc.GameModeComponents;

public class GameModeSpawnerComponent(RoleTypeId role, IReadOnlyList<Vector3> positions, bool allowSubclasses = false) : GameModeComponent
{
    public override void OnStarting(CursedGameModeBase gameModeBase)
    {
        foreach (CursedPlayer player in CursedPlayer.Collection)
        {
            player.SetRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
            player.ClearInventory();
            player.SetSubclass(null);
            player.Position = positions[Random.Range(0, positions.Count)];
        }
        
        base.OnStarting(gameModeBase);
    }
}