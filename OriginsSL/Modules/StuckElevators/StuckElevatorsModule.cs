using System.Collections.Generic;
using CursedMod.Events.Arguments.Facility.Elevators;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.AdminToys;
using CursedMod.Features.Wrappers.Facility.Elevators;
using Interactables.Interobjects;
using Mirror;
using OriginsSL.Loader;
using UnityEngine;

namespace OriginsSL.Modules.StuckElevators;

public class StuckElevatorsModule : OriginsModule
{
    private const float StuckTime = 10f; 
    
    public override void OnLoaded()
    {
        CursedElevatorEventHandler.PlayerInteractingElevator += OnPlayerInteractingElevator;
        CursedElevatorEventHandler.ElevatorMoving += OnElevatorMoving;
        CursedRoundEventsHandler.RestartingRound += OnRestartingRound;
    }

    private record struct StuckElevators(CursedElevatorChamber Chamber, float ExpirationTime, int TargetLevel);

    private static readonly HashSet<StuckElevators> ActiveStuckElevators = [];
    private static readonly Dictionary<CursedElevatorChamber, CursedLightSource> ElevatorLights = new();
    
    private static void OnRestartingRound()
    {
        ActiveStuckElevators.Clear();
        ElevatorLights.Clear();
    }    
        
    private static void OnPlayerInteractingElevator(PlayerInteractingElevatorEventArgs args)
    {
        if (!args.IsAllowed)
            return;
        
        ActiveStuckElevators.Add(new StuckElevators(args.ElevatorChamber, Time.timeSinceLevelLoad + StuckTime, args.TargetLevel));
        ElevatorLights.Add(args.ElevatorChamber, CreateLightSource(args.ElevatorChamber));
    }
    
    private static void OnElevatorMoving(ElevatorMovingEventArgs args)
    {
        if (!TryGetActiveStuckElevator(args.ElevatorChamber, out StuckElevators stuckElevators))
            return;
        
        if (Time.timeSinceLevelLoad > stuckElevators.ExpirationTime)
        {
            ActiveStuckElevators.Remove(stuckElevators);
            ElevatorManager.TrySetDestination(stuckElevators.Chamber.AssignedGroup, stuckElevators.TargetLevel, true);

            if (!ElevatorLights.TryGetValue(stuckElevators.Chamber, out CursedLightSource lightSource)) 
                return;
            
            NetworkServer.Destroy(lightSource.GameObject);
            ElevatorLights.Remove(stuckElevators.Chamber);
            return;
        }
        
        ElevatorManager.TrySetDestination(stuckElevators.Chamber.AssignedGroup, stuckElevators.Chamber.CurrentLevel == 0 ? 1 : 0, true);
    }
    
    private static bool TryGetActiveStuckElevator(CursedElevatorChamber chamber, out StuckElevators stuckElevators)
    {
        stuckElevators = default;
        
        foreach (StuckElevators activeStuckElevators in ActiveStuckElevators)
        {
            if (activeStuckElevators.Chamber != chamber)
                continue;
            
            stuckElevators = activeStuckElevators;
            return true;
        }

        return false;
    }

    private static CursedLightSource CreateLightSource(CursedElevatorChamber elevatorChamber)
    {
        Transform elevatorChamberTransform = elevatorChamber.Base.transform;
        CursedLightSource lightSource = CursedLightSource.Create(elevatorChamberTransform.position + new Vector3(0, 3.25f, -0.25f), lightIntensity: 14.5f, lightShadows: false, lightColor: Color.red, lightRange: 4f, spawn: true);
        lightSource.Transform.SetParent(elevatorChamberTransform, true);
        return lightSource;
    }
}