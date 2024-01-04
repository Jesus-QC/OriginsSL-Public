using System.Collections.Generic;
using CursedMod.Events.Arguments.Items;
using CursedMod.Events.Handlers;
using OriginsSL.Loader;

namespace OriginsSL.Modules.CustomItems;

public class CustomItemManager : OriginsModule
{
    private static readonly Dictionary<ushort, ICustomItem> CustomItems = new();

    private static readonly Dictionary<ItemType, ICustomItem[]> NaturallySpawnedItems = new()
    {
        
    };
    
    private static readonly HashSet<ushort> AlreadyRegisteredSerials = [];
    
    public static bool TryGetCustomItem(ushort itemId, out ICustomItem customItem) => CustomItems.TryGetValue(itemId, out customItem);
    
    public static void RegisterCustomItem(ushort itemId, ICustomItem customItem) => CustomItems.Add(itemId, customItem);
    
    public override void OnLoaded()
    {
        CursedItemsEventsHandler.CreatedPickup += OnPickupCreated;
    }

    private static void OnPickupCreated(CreatedPickupEventArgs args)
    {
        // We only create custom items on newly spawned pickups.
        if (!AlreadyRegisteredSerials.Add(args.Serial))
            return;
        
        // Check chances and spawn custom items.
    }
}