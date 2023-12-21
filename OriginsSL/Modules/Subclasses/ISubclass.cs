using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses;

public interface ISubclass
{
    public string CodeName { get; }
    
    public bool Spoofed { get; }
    
    public string Name { get; }
    
    public string Description { get; }
    
    public float SpawnChance { get; }
    
    public RoleTypeId SpawnRole { get; }
    
    public RoleTypeId SpawnLocation { get; }
    
    public Vector3 PlayerSize { get; }
    public Vector3 FakeSize { get; }
    
    public float Health { get; }
    
    public float ArtificialHealth { get; }
    
    public float HumeShield { get; }
    
    public List<ItemType> OverrideInventory { get; }
    
    public List<ItemType> AdditiveInventory { get; }
    
    public Dictionary<ItemType, ushort> OverrideAmmo { get; }
    
    public Dictionary<ItemType, ushort> AdditiveAmmo { get; }
    
    public bool KeepAfterEscaping { get; }
    
    public bool IsLocked { get; set; }

    public bool FilterSubclass(CursedPlayer player);
    
    public void OnSpawn(CursedPlayer player);
    
    public void OnDeath(CursedPlayer player);

    public void OnDestroy(CursedPlayer player);
}