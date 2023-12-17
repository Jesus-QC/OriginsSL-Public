using System.Collections.Generic;
using System.Linq;
using CursedMod.Features.Wrappers.Inventory.Items;
using CursedMod.Features.Wrappers.Inventory.Items.Firearms;
using CursedMod.Features.Wrappers.Player;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Firearms.Attachments.Components;
using MEC;
using OriginsSL.Modules.Subclasses;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Features;

public static class OriginsPlayerReplacer
{
    public static void ReplacePlayer(CursedPlayer target, CursedPlayer other, bool checkSubclass = true)
    {
        if (checkSubclass && other.TryGetSubclass(out ISubclass subclass))
        {
            List<CursedItem> items = other.ClearItemsWithoutDestroying().ToList();
            Dictionary<ItemType, ushort> ammo = other.Ammo.ToDictionary(x => x.Key, x => x.Value);
            float health = other.Health;
            float humeShield = other.HumeShield;
         
            other.SetSubclass(null);
            
            subclass.IsLocked = true;
            target.ForceSubclass(subclass);
            
            target.Position = other.Position;
            target.SetRole(other.Role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
            
            Timing.CallDelayed(0.8f, () =>
            {
                subclass.IsLocked = false;
                target.SetData(items, ammo, RoleTypeId.None, health, humeShield, Vector3.zero);
            });
        }
        
        target.SetData(other.ClearItemsWithoutDestroying().ToList(), other.Ammo, other.Role, other.Health, other.HumeShield, other.Position);
    }
    
    private static void SetData(this CursedPlayer target, List<CursedItem> items, Dictionary<ItemType, ushort> ammo, RoleTypeId role, float health, float humeShield, Vector3 position)
    {
        target.SetItemsOwnership(items);
        target.SetAmmo(ammo);
        
        if (role != RoleTypeId.None)
            target.SetRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
        
        target.HealthStat.CurValue = health;
        target.HumeShieldStat.CurValue = humeShield;
        
        if (position != Vector3.zero)
            target.Position = position;
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

    private static void SetItemsOwnership(this CursedPlayer target, List<CursedItem> items)
    {
        HashSet<(ItemType, byte, uint)> firearms = [];
        
        foreach (CursedItem item in items.ToArray())
        {
            if (item is CursedFirearmItem firearmItem)
            {
                firearms.Add((firearmItem.ItemType, firearmItem.Ammo, firearmItem.AttachmentsCode));
                items.Remove(item);
            }
            
            item.Base.Owner = target.ReferenceHub;
        }
        
        target.SetItems(items);
        
        foreach ((ItemType firearm, byte ammo, uint attachments) in firearms)
        {
            if (target.AddItem(firearm) is not CursedFirearmItem firearmItem)
                continue;

            firearmItem.Status = new FirearmStatus(ammo, firearmItem.Status.Flags, attachments);
        }
    }
}