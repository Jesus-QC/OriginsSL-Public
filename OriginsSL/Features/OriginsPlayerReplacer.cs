using System.Collections.Generic;
using System.Linq;
using CursedMod.Features.Wrappers.Inventory.Items;
using CursedMod.Features.Wrappers.Inventory.Items.Firearms;
using CursedMod.Features.Wrappers.Player;
using CustomPlayerEffects;
using InventorySystem.Items.Firearms;
using MEC;
using OriginsSL.Modules.Subclasses;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Features;

public static class OriginsPlayerReplacer
{
    public static void ReplacePlayer(CursedPlayer replacer, CursedPlayer oldPlayer, bool checkSubclass = true)
    {
        // If the old player is in pocket dimension, don't replace
        if (oldPlayer.TryGetEffect(out PocketCorroding pc) && pc.IsEnabled)
            return;
        
        replacer.SetData(oldPlayer.ClearItemsWithoutDestroying().ToList(), oldPlayer.Ammo, oldPlayer.Role, oldPlayer.Health, oldPlayer.HumeShield, oldPlayer.Position);
        
        if (!checkSubclass || oldPlayer.TryGetSubclass(out SubclassBase subclass))
            return;
        
        oldPlayer.ForceSavedSubclass(subclass);

        Timing.CallDelayed(0.4f, () =>
        {
            replacer.ForceSubclass(subclass);
        });
    }
    
    private static void SetData(this CursedPlayer target, List<CursedItem> items, Dictionary<ItemType, ushort> ammo, RoleTypeId role, float health, float humeShield, Vector3 position)
    {
        target.SetItemsOwnership(items);
        target.SetAmmo(ammo);
        
        if (role != RoleTypeId.None)
            target.SetRole(role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);

        target.Health = health;
        target.HumeShield = humeShield;
        
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