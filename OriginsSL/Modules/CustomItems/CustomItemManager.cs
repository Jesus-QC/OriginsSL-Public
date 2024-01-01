using System.Collections.Generic;

namespace OriginsSL.Modules.CustomItems;

public class CustomItemManager : OriginsModule
{
    public static readonly Dictionary<ushort, ICustomItem> CustomItems = new();

    public static readonly Dictionary<ItemType, ICustomItem[]> AvailableCustomItems = new();
    
    public override void OnLoaded()
    {
        
    }
}