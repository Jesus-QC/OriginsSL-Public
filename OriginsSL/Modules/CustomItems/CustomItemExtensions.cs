using CursedMod.Features.Wrappers.Player;

namespace OriginsSL.Modules.CustomItems;

public static class CustomItemExtensions
{
    public static bool TryGetCurrentCustomItem(this CursedPlayer player, out CustomItemBase item) =>
        CustomItemManager.TryGetCurrentCustomItem(player, out item);
    
    public static string GetCustomItemName(this CursedPlayer player) 
        => TryGetCurrentCustomItem(player, out CustomItemBase customItem) ? customItem.Name : string.Empty;
    
    public static string GetCustomItemDescription(this CursedPlayer player) 
        => TryGetCurrentCustomItem(player, out CustomItemBase customItem) ? customItem.Description : string.Empty;
}