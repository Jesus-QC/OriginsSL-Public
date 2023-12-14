using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses;

public abstract class SubclassBase : ISubclass
{
    public virtual string CodeName => string.Empty;
    public virtual bool Spoofed => false;
    public virtual string Name => string.Empty;
    public virtual string Description => string.Empty;
    public virtual float SpawnChance => 0;
    public virtual RoleTypeId SpawnRole => RoleTypeId.None;
    public virtual RoleTypeId SpawnLocation => RoleTypeId.None;
    public virtual Vector3 PlayerSize { get; } = Vector3.zero;
    public virtual Vector3 FakeSize { get; } = Vector3.zero;
    public virtual float Health => -1;
    public virtual float ArtificialHealth => -1;
    public virtual float HumeShield => -1;
    public virtual List<ItemType> Inventory => null;
    public virtual Dictionary<ItemType, ushort> Ammo => null;

    public virtual bool FilterSubclass(CursedPlayer player) => true;

    public virtual void OnSpawn(CursedPlayer player) { }

    public virtual void OnDeath(CursedPlayer player) { }
}