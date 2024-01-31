using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Loader;
using OriginsSL.Modules.CustomItems;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.BetterStartingInventories;

public class BetterStartingInventoriesModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.Spawning += OnPlayerSpawning;
    }
    
    private static void OnPlayerSpawning(PlayerSpawningEventArgs args)
    {
        if (args.RoleType is RoleTypeId.ClassD)
            HandleClassD(args.Player);

        if (args.RoleType is not RoleTypeId.Tutorial)
            return;
        
        CustomItemManager.RemoveCustomItem(args.Player.AddItem(ItemType.Flashlight).Serial);
        CustomItemManager.RemoveCustomItem(args.Player.AddItem(ItemType.Coin).Serial);
    }
    
    private static void HandleClassD(CursedPlayer player)
    {
        if (Random.value > 0.5f) // 50% chance
        {
            player.AddItem(ItemType.Coin);
            return;
        }
        
        player.AddItem(ItemType.Flashlight);
    }
}