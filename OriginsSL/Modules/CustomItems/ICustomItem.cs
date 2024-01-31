using CursedMod.Features.Wrappers.Inventory.Items;
using CursedMod.Features.Wrappers.Inventory.Pickups;

namespace OriginsSL.Modules.CustomItems;

public interface ICustomItem
{
    public string CodeName { get; }
    
    public string Name { get; }
    
    public string Description { get; }
    
    public float SpawnChance { get; }

    public bool FilterItem();
}