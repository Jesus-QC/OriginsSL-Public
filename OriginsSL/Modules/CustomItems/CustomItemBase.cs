namespace OriginsSL.Modules.CustomItems;

public abstract class CustomItemBase
{ 
    public abstract string CodeName { get; }
    
    public abstract string Name { get; }
    
    public abstract string Description { get; }

    public virtual float SpawnChance => 0f;

    public virtual bool FilterItem() => true;
}