using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Facility.Rooms;
using CursedMod.Features.Wrappers.Player;
using Hazards;
using MapGeneration;
using MEC;
using OriginsSL.Loader;
using OriginsSL.Modules.Scp1162;
using UnityEngine;

namespace OriginsSL.Modules.BetterSinkholes;

public class BetterSinkholesModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedMapGenerationEventsHandler.MapGenerated += OnMapGenerated;
    }
    
    private static void OnMapGenerated()
    {
        CursedRoom room = CursedRoom.Get(RoomName.Lcz173);
        Vector3 lastPosition = room.GetLocalPoint(new Vector3(17.25f, 11, 8f));
        
        Timing.CallDelayed(1, () =>
        {
            bool spawnedScp1162Sinkhole = false;
            foreach (SinkholeEnvironmentalHazard sinkhole in Object.FindObjectsOfType<SinkholeEnvironmentalHazard>())
            {
                sinkhole.MaxDistance = 3;

                if (spawnedScp1162Sinkhole)
                    continue;
                
                Transform transform = sinkhole.transform;
                sinkhole.MaxDistance = 0;
                transform.position = lastPosition + new Vector3(0,0.6f,0);
                transform.rotation = room.Rotation * Quaternion.Euler(0, 100, 0);
                CursedPlayer.SendSpawnMessageToAll(sinkhole.netIdentity);
                spawnedScp1162Sinkhole = true;
            }
        });   
    }
}