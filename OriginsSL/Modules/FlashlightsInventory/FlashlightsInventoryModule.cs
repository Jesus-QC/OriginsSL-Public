using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using OriginsSL.Loader;
using OriginsSL.Modules.CustomItems;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.FlashlightsInventory;

public class FlashlightsInventoryModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Spawning += OnPlayerSpawning;
    }
    
    private static void OnPlayerSpawning(PlayerSpawningEventArgs args)
    {
        if (args.RoleType != RoleTypeId.ClassD)
            return;

        if (Random.value > 0.5f) // 50% chance
        {
            CustomItemManager.RemoveCustomItem(args.Player.AddItem(ItemType.Coin).Serial);
            return;
        }
        
        args.Player.AddItem(ItemType.Flashlight);
    }
}