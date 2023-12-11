using System.Collections.Generic;
using System.Linq;
using CursedMod.Events.Arguments.Facility.Tesla;
using CursedMod.Events.Handlers;
using InventorySystem.Items;

namespace OriginsSL.Modules.TeslaConditions;

public class TeslaConditionsModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedTeslaEventHandler.PlayerTriggerTesla += OnTriggerTesla;
    }

    private static readonly HashSet<ItemType> KeyCards = new()
    {
        ItemType.KeycardO5,
        ItemType.KeycardMTFCaptain,
        ItemType.KeycardMTFOperative,
        ItemType.KeycardMTFPrivate,
        ItemType.KeycardGuard,
    };

    private static void OnTriggerTesla(PlayerTriggerTeslaEventArgs args)
    {
        Dictionary<ushort, ItemBase> items = args.Player.Items;

        if (!items.Any(item => KeyCards.Contains(item.Value.ItemTypeId))) 
            return;
        
        args.IsTriggerable = false;
        args.IsInIdleRange = false;
    }
}