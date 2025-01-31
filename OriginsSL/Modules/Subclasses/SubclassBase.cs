using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses;

public abstract class SubclassBase
{
    public static readonly HashSet<CoroutineHandle> ActiveCoroutines = [];
    
    public abstract string CodeName { get; }
    public virtual bool Spoofed => false;
    public abstract string Name { get; }
    public abstract string Description  { get; }
    public virtual float SpawnChance => 0;
    public virtual RoleTypeId SpawnRole => RoleTypeId.None;
    public virtual RoleTypeId SpawnLocation => RoleTypeId.None;
    public virtual Vector3 PlayerSize { get; } = Vector3.zero;
    public virtual Vector3 FakeSize { get; } = Vector3.zero;
    public virtual float Health => -1;
    public virtual float MaxHealth => -1;
    public virtual float ArtificialHealth => -1;
    public virtual float HumeShield => -1;
    public virtual List<ItemType> OverrideInventory => null;
    public virtual List<ItemType> AdditiveInventory => null;
    public bool AllowCustomItems => true;
    public virtual Dictionary<ItemType, ushort> OverrideAmmo => null;
    public virtual Dictionary<ItemType, ushort> AdditiveAmmo => null;
    public virtual bool KeepAfterEscaping => false;
    
    public virtual bool IsLocked { get; set; } = false;

    public virtual bool SkipSpawning { get; set; } = false;

    public virtual bool FilterSubclass(CursedPlayer player) => true;

    public virtual void OnSpawn(CursedPlayer player) { }

    public virtual void OnDeath(CursedPlayer player) { }
    public virtual void OnDestroy(CursedPlayer player) { }
    
    public CoroutineHandle RunCoroutine(IEnumerator<float> coroutine, CursedPlayer player)
    {
        CoroutineHandle coroutineHandle = Timing.RunCoroutine(coroutine.CancelWith(player.GameObject));
        ActiveCoroutines.Add(coroutineHandle);
        return coroutineHandle;
    }

    public void KillCoroutine(CoroutineHandle coroutineHandle)
    {
        ActiveCoroutines.Remove(coroutineHandle);
        Timing.KillCoroutines(coroutineHandle);
    }
}