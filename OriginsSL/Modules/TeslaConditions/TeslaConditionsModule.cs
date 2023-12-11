using System.Collections.Generic;
using System.Linq;
using CursedMod.Events.Arguments.Facility.Tesla;
using CursedMod.Events.Handlers;

namespace OriginsSL.Modules.TeslaConditions;

public class TeslaConditionsModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedTeslaEventHandler.PlayerTriggerTesla += OnTriggerTesla;
    }

    private static readonly HashSet<ItemType> KeyCards = new() // Sujeto a Cambios
    {
        ItemType.KeycardO5,
        ItemType.KeycardMTFCaptain,
        ItemType.KeycardMTFOperative,
        ItemType.KeycardMTFPrivate,
        ItemType.KeycardChaosInsurgency,
    };

    private static void OnTriggerTesla(PlayerTriggerTeslaEventArgs args)
    {
        var items = args.Player.Items;

        if (!items.Any(item => KeyCards.Contains(item.Value.ItemTypeId))) return;
        
        args.IsTriggerable = false;
        args.IsInIdleRange = false;
    }
}