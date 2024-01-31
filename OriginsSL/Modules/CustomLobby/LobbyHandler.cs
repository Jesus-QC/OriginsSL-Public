using System.Collections.Generic;
using CursedMod.Events.Arguments;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.AdminToys;
using CursedMod.Features.Wrappers.Facility.Rooms;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Roles;
using CursedMod.Features.Wrappers.Round;
using CursedMod.Features.Wrappers.Server;
using InventorySystem;
using MapGeneration;
using Mirror;
using OriginsSL.Features.Display;
using OriginsSL.Loader;
using OriginsSL.Modules.CustomLobby.Components;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.CustomLobby;

public class LobbyHandler : OriginsModule
{
    private static readonly HashSet<GameObject> Map = [];
    private static Vector3 _spawnPos = Vector3.zero;

    public override void OnLoaded()
    {
        CursedMapGenerationEventsHandler.MapGenerated += OnMapGenerated;
        CursedRoundEventsHandler.RoundStarted += HandleStart;
        CursedPlayerEventsHandler.Connected += HandleConnection;
        CursedPlayerEventsHandler.Disconnecting += HandleDisconnection;
        CursedItemsEventsHandler.PlayerDroppingItem += OnInteractingWithItem;
        CursedItemsEventsHandler.PlayerPickingUpItem += OnInteractingWithItem;
    }

    private static void OnInteractingWithItem(ICursedCancellableEvent args)
    {
        if (!CursedRound.IsInLobby)
            return;

        args.IsAllowed = false;
    }
    
    private static void OnMapGenerated()
    {
        CursedServer.DropPlayerItemsOnDisconnect = false;
        
        CursedRoom room = CursedRoom.Get(RoomName.Hcz049);
        
        Map.Add(CursedPrimitiveObject.Create(PrimitiveType.Cube, room.GetLocalPoint(new Vector3(1.9f, 196.83f, 7.15f)), new Vector3(1.7f, 0.1f, 1.7f), color: new Color(1, 0.58f, 0.2f))
            .Spawn().AddComponent<TeamTriggerComponent>().Init(Team.ClassD));
        Map.Add(CursedPrimitiveObject.Create(PrimitiveType.Cube, room.GetLocalPoint(new Vector3(1.9f, 196.83f, 9.15f)), new Vector3(1.7f, 0.1f, 1.7f), color: new Color(0.96f, 0.88f, 0.43f))
            .Spawn().AddComponent<TeamTriggerComponent>().Init(Team.Scientists));
        Map.Add(CursedPrimitiveObject.Create(PrimitiveType.Cube, room.GetLocalPoint(new Vector3(1.9f, 196.83f, 11.15f)), new Vector3(1.7f, 0.1f, 1.7f), color: new Color(0.36f, 0.39f, 0.44f))
            .Spawn().AddComponent<TeamTriggerComponent>().Init(Team.FoundationForces));
        Map.Add(CursedPrimitiveObject.Create(PrimitiveType.Cube, room.GetLocalPoint(new Vector3(1.9f, 196.83f, 13.15f)), new Vector3(1.7f, 0.1f, 1.7f), color: new Color(0.93f, 0.13f, 0.13f))
            .Spawn().AddComponent<TeamTriggerComponent>().Init(Team.SCPs));

        Vector3 eulerAngles = room.Transform.eulerAngles;
        Map.Add(CursedPrimitiveObject.Create(PrimitiveType.Cube, room.GetLocalPoint(new Vector3(7, 194.74f, 0.6f)),
                new Vector3(1, 6, 7), color: new Color(0.72f, 0.54f, 0.33f, 0.12f), rotation: eulerAngles)
            .Spawn());
        Map.Add(CursedPrimitiveObject.Create(PrimitiveType.Cube, room.GetLocalPoint(new Vector3(-0.96f, 194.74f, 19.93f)),
                new Vector3(1, 6, 7), color: new Color(0.72f, 0.54f, 0.33f, 0.12f), rotation: eulerAngles)
            .Spawn());
        
        _spawnPos = room.GetLocalPoint(new Vector3(6.5f, 198f, 10.02f));
    }
    
    private static void HandleStart()
    {
        CursedServer.DropPlayerItemsOnDisconnect = true;
        
        foreach (GameObject go in Map)
            NetworkServer.Destroy(go);
        
        Map.Clear();

        foreach (CursedPlayer player in CursedPlayer.Collection)
        {
            player.ClearOriginsHintZone(ScreenZone.Important);
            player.ClearOriginsHintZone(ScreenZone.Environment);
        }
    }
    
    private static void HandleDisconnection(PlayerDisconnectingEventArgs args)
    {
        if (!CursedRound.IsInLobby)
            return;
        
        RoleManager.RemoveFromQueue(args.Player.ReferenceHub, Team.SCPs);
        RoleManager.RemoveFromQueue(args.Player.ReferenceHub, Team.ClassD);
        RoleManager.RemoveFromQueue(args.Player.ReferenceHub, Team.Scientists);
        RoleManager.RemoveFromQueue(args.Player.ReferenceHub, Team.FoundationForces);
    }

    private static void HandleConnection(PlayerConnectedEventArgs args)
    {
        if (!CursedRound.IsInLobby)
            return;
        
        args.Player.Role = RoleTypeId.Tutorial;
        args.Player.Position = _spawnPos;
    }
}