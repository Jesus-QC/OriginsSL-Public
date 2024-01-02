using CursedMod.Features.Wrappers.Inventory.Items;

namespace OriginsSL.Modules.CustomItems;

public abstract class CustomItemBase : ICustomItem
{ 
    public virtual string Name => string.Empty;
    
    public virtual string Description => string.Empty;

    public virtual float SpawnChance => 0f;
    
    public virtual void OnPickedUp(CursedItem item) { }

    public void OnDropped(CursedItem item) { }
}