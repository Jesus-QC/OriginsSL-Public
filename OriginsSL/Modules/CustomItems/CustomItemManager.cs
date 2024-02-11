using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CursedMod.Events.Arguments.Items;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Loader;
using OriginsSL.Modules.CustomItems.Items.Coins;

namespace OriginsSL.Modules.CustomItems;

public class CustomItemManager : OriginsModule
{
    public static readonly HashSet<ushort> AlreadyRegisteredSerials = [];
    
    private static readonly Dictionary<ushort, CustomItemBase> CustomItems = new();

    public static readonly Dictionary<ItemType, CustomItemBase[]> NaturallySpawnedItems = new()
    {
        [ItemType.Coin] = [new SpecialCoin()]
    };
    
    public static bool TryGetCustomItem(ushort itemId, out CustomItemBase customItem) => CustomItems.TryGetValue(itemId, out customItem);

    public static bool TryGetCurrentCustomItem(CursedPlayer player, out CustomItemBase item) => TryGetCustomItem(player.HoldingItem.SerialNumber, out item);

    public static void RegisterCustomItem(ushort itemId, CustomItemBase customItem) => CustomItems.Add(itemId, customItem);
    
    public static void ForceCustomItem(ushort itemId, CustomItemBase customItem) => CustomItems[itemId] = customItem;
    
    public static bool RemoveCustomItem(ushort itemId) => CustomItems.Remove(itemId);

    public override void OnLoaded()
    {
        LoadEventsHandlers();
        CursedItemsEventsHandler.SpawnedItem += OnSpawnedItem;
    }
    
    private static void OnSpawnedItem(SpawnedItemEventArgs args)
    {
        // We only create custom items on newly spawned pickups.
        if (!AlreadyRegisteredSerials.Add(args.Serial))
            return;
        
        // We create a random custom item for the pickup.
        CustomItemBase customItem = GetRandomCustomItem(args.ItemType);
        
        // If the custom item is null, we don't do anything as no custom item was selected.
        if (customItem == null)
            return;
        
        // Otherwise, we register the custom item.
        RegisterCustomItem(args.Serial, customItem);
    }
    
    private static CustomItemBase GetRandomCustomItem(ItemType itemType)
    {
        if (!NaturallySpawnedItems.TryGetValue(itemType, value: out CustomItemBase[] customItems) || customItems.Length == 0)
            return null;
        
        float totalChance = customItems.Sum(subclass => subclass.FilterItem() ? subclass.SpawnChance : 0);

        totalChance++; // We add 1 to the total chance to allow for no custom items to be spawned.
        
        float finalChance = UnityEngine.Random.Range(0f, totalChance);
        
        foreach (CustomItemBase customItem in customItems)
        {
            finalChance -= customItem.FilterItem() ? customItem.SpawnChance : 0;
            
            if (finalChance <= 0f)
                return Activator.CreateInstance(customItem.GetType()) as CustomItemBase;
        }
        
        return null;
    }
    
    private static void LoadEventsHandlers()
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsInterface || !typeof(ICustomItemEventsHandler).IsAssignableFrom(type)) 
                continue;
            
            if (Activator.CreateInstance(type) is not ICustomItemEventsHandler eventsHandler)
                continue;
            
            eventsHandler.OnLoaded();
        }
    }
}