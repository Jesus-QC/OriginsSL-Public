using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses;

public interface ISubclass
{
    public string CodeName { get; }
    
    public string Name { get; }
    
    public string Description { get; }
    
    public float SpawnChance { get; }
    
    public RoleTypeId SpawnRole { get; }
    
    public RoleTypeId SpawnLocation { get; }
    
    public Vector3 PlayerSize { get; }
    
    public float Health { get; }
    
    public float Ahp { get; }
    
    public List<ItemType> Inventory { get; }
    
    public Dictionary<ItemType, ushort> Ammo { get; }
    
    public void OnSpawn(CursedPlayer player);
    
    public void OnDeath(CursedPlayer player);
}