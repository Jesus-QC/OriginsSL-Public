﻿// -----------------------------------------------------------------------
// <copyright file="CursedRagdoll.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using CursedMod.Features.Wrappers.Server;
using Mirror;
using PlayerRoles;
using PlayerRoles.Ragdolls;
using PlayerStatsSystem;
using UnityEngine;

namespace CursedMod.Features.Wrappers.Player.Ragdolls;

public class CursedRagdoll
{
    public static readonly Dictionary<BasicRagdoll, CursedRagdoll> Dictionary = new ();

    private CursedRagdoll(BasicRagdoll ragdoll)
    {
        Base = ragdoll;
        Dictionary.Add(ragdoll, this);
    }

    public static IReadOnlyCollection<CursedRagdoll> Collection => Dictionary.Values;
    
    public static List<CursedRagdoll> List => Collection.ToList();

    public BasicRagdoll Base { get; }

    public RoleTypeId Role
    {
        get => Base.Info.RoleType;
        set => Base.NetworkInfo = new RagdollData(Base.Info.OwnerHub, Base.Info.Handler, value, Base.Info.StartPosition, Base.Info.StartRotation, Base.Info.Nickname, Base.Info.CreationTime);
    }

    public CursedPlayer Owner => CursedPlayer.Get(Base.Info.OwnerHub);

    public string Nickname
    {
        get => Base.NetworkInfo.Nickname;
        set => Base.NetworkInfo = new RagdollData(Base.Info.OwnerHub, Base.Info.Handler, Base.Info.RoleType, Base.Info.StartPosition, Base.Info.StartRotation, value, Base.Info.CreationTime);
    }
    
    public string DamageText
    {
        get => (DamageHandler as CustomReasonDamageHandler)?._deathReason ?? DamageHandler.GetType().Name;
        set => DamageHandler = new CustomReasonDamageHandler(value);
    }
    
    public DamageHandlerBase DamageHandler
    {
        get => Base.Info.Handler;
        set => Base.NetworkInfo = new RagdollData(Base.Info.OwnerHub, value, Base.Info.RoleType, Base.Info.StartPosition, Base.Info.StartRotation, Base.Info.Nickname, Base.Info.CreationTime);
    }

    public Vector3 Position => Base.gameObject.transform.position;

    public RagdollData Data => Base.Info;
    
    public bool IsFrozen => Base._frozen;

    public int FreezeTime
    {
        get => RagdollManager.FreezeTime;
        set => RagdollManager.FreezeTime = value;
    }

    public float ExistenceTime => Base.Info.ExistenceTime;

    public bool AllowCleanUp => ExistenceTime < FreezeTime;

    public static CursedRagdoll Create(RoleTypeId role, string reason, Vector3 position, Vector3 rotation, bool spawn = true) => Create(role, new CustomReasonDamageHandler(reason), position, rotation, spawn);

    public static CursedRagdoll Create(RoleTypeId role, DamageHandlerBase damageHandler, Vector3 position, Vector3 rotation, bool spawn = true, string nickname = "")
    {
        if (!PlayerRoleLoader.TryGetRoleTemplate(role, out PlayerRoleBase roleBase))
            return null;
        
        if (roleBase is not IRagdollRole ragdollRole)
            return null;
        
        GameObject gameObject = Object.Instantiate(ragdollRole.Ragdoll.gameObject);

        if (gameObject.TryGetComponent(out BasicRagdoll basicRagdoll))
        {
            basicRagdoll.NetworkInfo = new RagdollData(CursedServer.LocalPlayer.ReferenceHub, damageHandler, role, position, Quaternion.Euler(rotation), nickname, NetworkTime.time);
        }
        else
        {
            basicRagdoll = null;
        }
        
        if (spawn)
            NetworkServer.Spawn(gameObject);
        
        return Get(basicRagdoll);
    }

    public static CursedRagdoll Get(BasicRagdoll basicRagdoll) => Dictionary.ContainsKey(basicRagdoll) ? Dictionary[basicRagdoll] : new CursedRagdoll(basicRagdoll);

    public static IEnumerable<CursedRagdoll> Get(CursedPlayer player) => Collection.Where(ragdoll => player == ragdoll.Owner);

    public void Spawn() => NetworkServer.Spawn(Base.gameObject);
    
    public void Destroy() => NetworkServer.Destroy(Base.gameObject);

    public void FreezeRagdoll() => Base.FreezeRagdoll();
}