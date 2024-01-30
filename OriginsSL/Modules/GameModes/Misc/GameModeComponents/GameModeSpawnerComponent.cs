using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Modules.Subclasses;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.GameModes.Misc.GameModeComponents;

public class GameModeSpawnerComponent(RoleTypeId role, IReadOnlyList<Vector3> positions, Vector3? offset = null, bool allowSubclasses = false) : GameModeComponent
{
    public override void OnStarting(CursedGameModeBase gameModeBase)
    {
        foreach (CursedPlayer player in CursedPlayer.Collection)
        {
            player.SetRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
            player.ClearInventory();
            
            if (!allowSubclasses)
                player.SetSubclass(null);

            Vector3 pos = positions[Random.Range(0, positions.Count)];
            player.Position = offset.HasValue ? pos + offset.Value : pos;
        }
        
        base.OnStarting(gameModeBase);
    }
}