using System.Collections.Generic;
using CursedMod.Events.Arguments.Items;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Facility;
using CursedMod.Features.Wrappers.Facility.Rooms;
using MapGeneration;
using OriginsSL.Features.Display;
using OriginsSL.Loader;
using OriginsSL.Modules.DisplayRenderer;
using UnityEngine;

namespace OriginsSL.Modules.Scp1162;

public class Scp1162Module : OriginsModule
{
    public override void OnLoaded()
    {
        CursedItemsEventsHandler.PlayerDroppingItem += OnDroppingItem;
        CursedMapGenerationEventsHandler.MapGenerated += OnMapGenerated;
    }
    
    private static Vector3 _lastPosition;

    private static void OnMapGenerated()
    {
        CursedRoom room = CursedRoom.Get(RoomName.Lcz173);
        _lastPosition = room.GetLocalPoint(new Vector3(17.25f, 11, 8f));
    }

    private static void OnDroppingItem(PlayerDroppingItemEventArgs args)
    {
        if (CursedDecontamination.IsDecontaminating)
            return;
        
        if (Vector3.Distance(args.Player.Position, _lastPosition) > 6f || args.Item.ItemType is ItemType.SCP330)
            return;
        
        args.IsAllowed = false;
        
        ItemType randomItem = RandomAmmo.Contains(args.Item.ItemType) ? RandomAmmo.RandomItem() : AvailableItems.RandomItem();
        
        args.Player.RemoveItem(args.Item);

        if (randomItem != ItemType.None)
        {
            args.Player.AddItem(randomItem).Drop();
        }
        
        args.Player.SendOriginsHint("<b>You dropped an item inside <i><color=yellow>SCP-1162</color></i>...</b>", ScreenZone.Environment);
    }
    
    private static readonly ItemType[] AvailableItems =
    [
        ItemType.Adrenaline,
        ItemType.Lantern,
        ItemType.Coin,
        ItemType.Flashlight,
        ItemType.GrenadeFlash,
        ItemType.GrenadeHE,
        ItemType.GunRevolver,
        ItemType.GunCOM15,
        ItemType.ArmorCombat,
        ItemType.ArmorHeavy,
        ItemType.ArmorLight,
        ItemType.KeycardChaosInsurgency,
        ItemType.KeycardScientist,
        ItemType.KeycardJanitor,
        ItemType.KeycardGuard,
        ItemType.Medkit,
        ItemType.Painkillers,
        ItemType.SCP018,
        ItemType.SCP500,
        ItemType.SCP1576,
        ItemType.SCP244a,
        ItemType.SCP244b,
        ItemType.SCP2176,
        ItemType.Radio,
        ItemType.KeycardZoneManager,
        ItemType.AntiSCP207,
        ItemType.GunCOM18,
        ItemType.GunE11SR,
        ItemType.KeycardMTFOperative,
        ItemType.None
    ];

    private static readonly List<ItemType> RandomAmmo =
    [
        ItemType.Ammo9x19,
        ItemType.Ammo12gauge,
        ItemType.Ammo44cal,
        ItemType.Ammo556x45,
        ItemType.Ammo762x39
    ];
}