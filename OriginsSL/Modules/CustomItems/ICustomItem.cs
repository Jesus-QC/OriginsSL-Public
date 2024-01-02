using CursedMod.Features.Wrappers.Inventory.Items;

namespace OriginsSL.Modules.CustomItems;

public interface ICustomItem
{
    public string Name { get; }
    
    public string Description { get; }
    
    public float SpawnChance { get; }
    
    public void OnPickedUp(CursedItem item);
    
    public void OnDropped(CursedItem item);
}