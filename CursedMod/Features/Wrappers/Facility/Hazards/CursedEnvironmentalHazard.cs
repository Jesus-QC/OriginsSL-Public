﻿// -----------------------------------------------------------------------
// <copyright file="CursedEnvironmentalHazard.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using CursedMod.Features.Enums;
using CursedMod.Features.Wrappers.Player;
using Hazards;
using PlayerRoles.PlayableScps.Scp939;
using UnityEngine;

namespace CursedMod.Features.Wrappers.Facility.Hazards;

public class CursedEnvironmentalHazard
{
    public static readonly Dictionary<EnvironmentalHazard, CursedEnvironmentalHazard> Dictionary = new ();
    
    internal CursedEnvironmentalHazard(EnvironmentalHazard hazard)
    {
        Dictionary.Add(hazard, this);
        EnvironmentalHazard = hazard;
        HazardType = EnvironmentalHazardType.Other;
    }
    
    public EnvironmentalHazard EnvironmentalHazard { get; }

    public List<ReferenceHub> AffectedPlayers => EnvironmentalHazard.AffectedPlayers;

    public EnvironmentalHazardType HazardType { get; internal set; }

    public float MaxDistance
    {
        get => EnvironmentalHazard.MaxDistance;
        set => EnvironmentalHazard.MaxDistance = value;
    }

    public float MaxHeightDistance
    {
        get => EnvironmentalHazard.MaxHeightDistance;
        set => EnvironmentalHazard.MaxHeightDistance = value;
    }

    public Vector3 SourceOffset
    {
        get => EnvironmentalHazard.SourceOffset;
        set => EnvironmentalHazard.SourceOffset = value;
    }
    
    public Vector3 SourcePosition
    {
        get => EnvironmentalHazard.SourcePosition;
        set => EnvironmentalHazard.SourcePosition = value;
    }

    public bool IsActive => EnvironmentalHazard.IsActive;
    
    public static CursedEnvironmentalHazard Get(EnvironmentalHazard environmentalHazard)
    {
        if (Dictionary.TryGetValue(environmentalHazard, out CursedEnvironmentalHazard value))
            return value;
        
        return environmentalHazard switch
        {
            SinkholeEnvironmentalHazard sinkholeEnvironmentalHazard => new CursedSinkholeHazard(sinkholeEnvironmentalHazard),
            TantrumEnvironmentalHazard tantrumEnvironmentalHazard => new CursedTantrumHazard(tantrumEnvironmentalHazard),
            Scp939AmnesticCloudInstance amnesticCloudHazard => new CursedAmnesticCloudHazard(amnesticCloudHazard),
            TemporaryHazard temporaryHazard => new CursedTemporaryHazard(temporaryHazard),
            _ => new CursedEnvironmentalHazard(environmentalHazard)
        };
    }
    
    public IEnumerable<CursedPlayer> GetAffectedPlayers()
    {
        foreach (ReferenceHub hub in AffectedPlayers)
        {
            if (!CursedPlayer.TryGet(hub, out CursedPlayer ply))
                continue;
                
            yield return ply;
        }
    }

    public bool IsInArea(Vector3 sourcePosition, Vector3 targetPosition) => EnvironmentalHazard.IsInArea(sourcePosition, targetPosition);
    
    public bool IsInArea(Vector3 targetPosition) => EnvironmentalHazard.IsInArea(SourcePosition, targetPosition);
}