using System.Collections.Generic;
using System.Linq;
using CursedMod.Features.Wrappers.Inventory.Items;
using CursedMod.Features.Wrappers.Inventory.Items.Firearms;
using CursedMod.Features.Wrappers.Player;
using PlayerRoles;

namespace OriginsSL.Features;


public static class OriginsPlayerReplacer
{
    public static void ReplacePlayer(CursedPlayer target, CursedPlayer other)
    {
        List<CursedItem> items = other.ClearItemsWithoutDestroying().ToList();
        List<ItemType> firearms = [];

        foreach (CursedItem item in items.ToArray())
        {
            if (item is CursedFirearmItem firearmItem)
            {
                firearms.Add(firearmItem.ItemType);
                items.Remove(item);
            }
            
            item.Base.Owner = target.ReferenceHub;
        }
        
        target.SetItems(items);

        foreach (ItemType itemType in firearms)
        {
            target.AddItem(itemType);
        }

        target.SetAmmo(other.Ammo);
        target.SetRole(other.Role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
        target.HealthStat.CurValue = other.HealthStat.CurValue;
        target.HumeShieldStat.CurValue = other.HumeShieldStat.CurValue;
        target.Position = other.Position;
    }
    
    public static bool TryGetRandomSpectator(out CursedPlayer player)
    {
        List<CursedPlayer> players = [];
        
        foreach (CursedPlayer possibleTarget in CursedPlayer.Collection)
        {
            if (possibleTarget.Role != RoleTypeId.Spectator)
                continue;
            
            players.Add(possibleTarget);
        }

        if (players.Count == 0)
        {
            player = null;
            return false;
        }
        
        player = players.RandomItem();
        return true;
    }
}