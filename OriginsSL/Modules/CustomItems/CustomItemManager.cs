using System.Collections.Generic;
using OriginsSL.Loader;

namespace OriginsSL.Modules.CustomItems;

public class CustomItemManager : OriginsModule
{
    private static readonly Dictionary<ushort, ICustomItem> CustomItems = new();
    
    public static bool TryGetCustomItem(ushort itemId, out ICustomItem customItem) => CustomItems.TryGetValue(itemId, out customItem);
    
    public static void RegisterCustomItem(ushort itemId, ICustomItem customItem) => CustomItems.Add(itemId, customItem);

    public override void OnLoaded()
    {
        
    }
}