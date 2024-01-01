using CursedMod.Features.Wrappers.Player;
using UnityEngine;

namespace OriginsSL.Modules.CustomItems;

public abstract class CustomItemBase : ICustomItem
{
    public virtual ushort ItemSerial { get; }
    
    public virtual string Name { get; }
    
    public virtual string Description { get; }
    
    public virtual float SpawnChance { get; }
    
    public virtual Vector3 SpawnLocation { get; }
    
    public virtual void OnPickedUp(CursedPlayer player) { }

    public virtual void OnChangedItem(CursedPlayer player) { }

    public virtual void OnDropped(CursedPlayer player) { }

    public virtual void OnUsed(CursedPlayer player) { }

    public virtual void OnShoot(CursedPlayer player) { }

    public virtual void OnReload(CursedPlayer player) { }
}