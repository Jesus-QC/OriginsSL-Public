using CursedMod.Features.Wrappers.Player;
using UnityEngine;

namespace OriginsSL.Modules.CustomItems;

public interface ICustomItem
{
    public string Name { get; }
    
    public string Description { get; }
    
    public float SpawnChance { get; }
    
    public Vector3 SpawnLocation { get; }
    
    public ushort ItemSerial { get; }
    
    public void OnPickedUp(CursedPlayer player);
    
    public void OnChangedItem(CursedPlayer player);
    
    public void OnDropped(CursedPlayer player);
    
    public void OnUsed(CursedPlayer player);
    
    public void OnShoot(CursedPlayer player);
    
    public void OnReload(CursedPlayer player);
}