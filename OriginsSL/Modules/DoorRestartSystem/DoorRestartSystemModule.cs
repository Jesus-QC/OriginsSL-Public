using System.Collections.Generic;
using CursedMod.Events.Arguments.Facility.Doors;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Facility.Doors;
using MEC;
using OriginsSL.Features.Display;
using OriginsSL.Loader;
using OriginsSL.Modules.DisplayRenderer;
using UnityEngine;

namespace OriginsSL.Modules.DoorRestartSystem;

public class DoorRestartSystemModule : OriginsModule
{ 
    public override void OnLoaded()
    {
        CursedDoorsEventsHandler.PlayerInteractingDoor += OnPlayerInteractingDoor;
    }

    private static void OnPlayerInteractingDoor(PlayerInteractingDoorEventArgs args)
    {
        if (args.Door.IsOpened || !args.Door.IsGate)
            return;
     
        if (Random.value > 0.05f)
            return;
        
        args.Player.SendOriginsHint("T<lowercase>his door has been restarted by a system error</lowercase>", ScreenZone.Center, 5f);
        Timing.RunCoroutine(LockDoor(args.Door).CancelWith(args.Door.GameObject));
    }

    private static IEnumerator<float> LockDoor(CursedDoor door)
    {
        door.Lock();
        yield return Timing.WaitForSeconds(5f);
        door.Unlock();
    }
}