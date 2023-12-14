// -----------------------------------------------------------------------
// <copyright file="CursedDesyncModule.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using CursedMod.Events.Arguments.Player;
using MEC;
using Mirror;
using PlayerRoles;
using PlayerRoles.FirstPersonControl;
using RelativePositioning;
using UnityEngine;

namespace CursedMod.Features.Wrappers.Player;

public static class CursedDesyncModule
{
    public static readonly Dictionary<CursedPlayer, Vector3> FakedScales = new ();
    public static readonly Dictionary<CursedPlayer, RoleTypeId> FakedRoles = new ();
    public static readonly Dictionary<CursedPlayer, RoleTypeId> FakedRolesNoSpectators = new ();

    public static void ClearCache()
    {
        FakedScales.Clear();
        FakedRoles.Clear();
        FakedRolesNoSpectators.Clear();
    }

    public static void HandlePlayerConnected(PlayerConnectedEventArgs args)
    {
        foreach (KeyValuePair<CursedPlayer, Vector3> fakedScale in FakedScales)
        {
            args.Player.SendFakeScaleMessage(fakedScale.Key, fakedScale.Value);
        }
        
        foreach (KeyValuePair<CursedPlayer, RoleTypeId> fakedRole in FakedRoles)
        {
            fakedRole.Key.ChangeAppearance(fakedRole.Value, [args.Player]);
        }
    }

    public static void HandlePlayerChangingRole(PlayerChangingRoleEventArgs args)
    {
        if (FakedRoles.ContainsKey(args.Player))
            FakedRoles.Remove(args.Player);
        if (FakedRolesNoSpectators.ContainsKey(args.Player))
            FakedRolesNoSpectators.Remove(args.Player);

        if (args.NewRole is not(RoleTypeId.Spectator or RoleTypeId.Overwatch or RoleTypeId.None or RoleTypeId.Filmmaker)) 
            return;
        
        foreach (KeyValuePair<CursedPlayer, RoleTypeId> fakedRole in FakedRolesNoSpectators)
        {
            fakedRole.Key.ChangeAppearance(fakedRole.Key.Role, [args.Player]);
        }
    }
    
    public static void HandlePlayerSpawning(PlayerSpawningEventArgs args)
    {
        if (args.RoleType is RoleTypeId.Spectator or RoleTypeId.Overwatch or RoleTypeId.None or RoleTypeId.Filmmaker)
            return;

        foreach (KeyValuePair<CursedPlayer, RoleTypeId> fakedRole in FakedRolesNoSpectators)
        {
            if (fakedRole.Key == args.Player)
                continue;
                
            fakedRole.Key.ChangeAppearance(fakedRole.Value, [args.Player]);
        }
    }

    public static void SendFakeScaleMessage(this CursedPlayer target, CursedPlayer player, Vector3 scale)
    {
        try
        {
            NetworkIdentity identity = player.NetworkIdentity;
            NetworkConnection conn = target.NetworkConnection;
        
            if (identity.serverOnly)
                return;

            using NetworkWriterPooled ownerWriter = NetworkWriterPool.Get();
            using NetworkWriterPooled observersWriter = NetworkWriterPool.Get();
            bool isOwner = identity.connectionToClient == conn;
            ArraySegment<byte> spawnMessagePayload = NetworkServer.CreateSpawnMessagePayload(isOwner, identity, ownerWriter, observersWriter);
            Transform transform = identity.transform;
            
            SpawnMessage message = new ()
            {
                netId = identity.netId,
                isLocalPlayer = conn.identity == identity,
                isOwner = isOwner,
                sceneId = identity.sceneId,
                assetId = identity.assetId,
                position = transform.localPosition,
                rotation = transform.localRotation,
                scale = scale,
                payload = spawnMessagePayload,
            };
            
            conn.Send(message);
        }
        catch
        {
            // Safely Ignore
        }
    }
    
    public static void ChangeAppearance(this CursedPlayer player, RoleTypeId type, IEnumerable<CursedPlayer> playersToAffect, byte unitId = 0)
    {
        if (player.Role is RoleTypeId.Spectator or RoleTypeId.Filmmaker or RoleTypeId.Overwatch)
            throw new InvalidOperationException("You cannot change a spectator into non-spectator via change appearance.");
        
        NetworkWriterPooled writer = NetworkWriterPool.Get();
        writer.WriteUShort(38952);
        writer.WriteUInt(player.NetId);
        writer.WriteRoleType(type);
        
        if (type.GetTeam() == Team.FoundationForces)
            writer.WriteByte(unitId);

        if (type != RoleTypeId.Spectator && player.RoleBase is IFpcRole fpc)
        {
            fpc.FpcModule.MouseLook.GetSyncValues(0, out ushort syncHorizontal, out _);
            writer.WriteRelativePosition(new RelativePosition(player.ReferenceHub.transform.position));
            writer.WriteUShort(syncHorizontal);
        }

        foreach (CursedPlayer target in playersToAffect)
            target.NetworkConnection.Send(writer.ToArraySegment());
        NetworkWriterPool.Return(writer);
    }
}