// -----------------------------------------------------------------------
// <copyright file="CursedElevatorDoor.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using CursedMod.Features.Enums;
using Interactables.Interobjects;
using UnityEngine;

namespace CursedMod.Features.Wrappers.Facility.Doors;

public class CursedElevatorDoor : CursedDoor
{
    internal CursedElevatorDoor(ElevatorDoor door) 
        : base(door)
    {
        ElevatorDoorBase = door;
        DoorType = DoorType.Elevator;
    }
    
    public ElevatorDoor ElevatorDoorBase { get; }
    
    public Vector3 TargetPosition => ElevatorDoorBase.TargetPosition;
    
    public Vector3 TopPosition => ElevatorDoorBase.TopPosition;
    
    public Vector3 BottomPosition => ElevatorDoorBase.BottomPosition;
    
    public ElevatorManager.ElevatorGroup ElevatorGroup => ElevatorDoorBase.Group;
    
    public ElevatorPanel InteractablePanel => ElevatorDoorBase.TargetPanel;
}