using System.Collections.Generic;
using CursedMod.Events.Arguments.Facility.Hazards;
using CursedMod.Events.Handlers;
using CursedMod.Features.Enums;
using Mirror;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class JanitorSubclass : SubclassBase
{
    public override string CodeName => "janitor";
    public override string Name => "<color=#c0b2e6>J<lowercase>anitor</lowercase></color>";
    public override string Description => "cleans up tantrums by standing on them";
    
    public override float SpawnChance => 0.7f;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.KeycardJanitor];

    public override bool KeepAfterEscaping { get; } = true;

    // Counter for the tantrum cleaning
    private float _counter;
    
    public class JanitorSubclassHandler : ISubclassEventsHandler
    {
        public void OnLoaded()
        {
            CursedHazardsEventHandler.EnteringHazard += OnPlayerEnteringHazard;
            CursedHazardsEventHandler.StayingOnHazard += OnPlayerStayingOnHazard;
            CursedHazardsEventHandler.ExitingHazard += OnPlayerExitingHazard;
        }

        private static void OnPlayerEnteringHazard(PlayerEnteringHazardEventArgs args)
        {
            if (args.Hazard.HazardType != EnvironmentalHazardType.Sinkhole)
                return;

            args.IsAllowed = false;
        }
        
        private static void OnPlayerStayingOnHazard(PlayerStayingOnHazardEventArgs args)
        {
            if (args.Hazard.HazardType != EnvironmentalHazardType.Tantrum)
                return;
            
            if (!args.Player.TryGetSubclass(out ISubclass subclass) || subclass is not JanitorSubclass janitorSubclass)
                return;
            
            janitorSubclass._counter += Time.deltaTime;
            
            if (janitorSubclass._counter < 3)
                return;
            
            janitorSubclass._counter = 0;
            NetworkServer.Destroy(args.Hazard.EnvironmentalHazard.gameObject);
        }

        private static void OnPlayerExitingHazard(PlayerExitingHazardEventArgs args)
        {
            if (args.Hazard.HazardType != EnvironmentalHazardType.Tantrum)
                return;
            
            if (!args.Player.TryGetSubclass(out ISubclass subclass) || subclass is not JanitorSubclass janitorSubclass)
                return;
            
            janitorSubclass._counter = 0;
        }
    }
}