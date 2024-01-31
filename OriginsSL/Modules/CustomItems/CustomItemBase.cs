namespace OriginsSL.Modules.CustomItems;

public abstract class CustomItemBase : ICustomItem
{ 
    public virtual string CodeName => "";
    
    public virtual string Name => "";
    
    public virtual string Description => string.Empty;

    public virtual float SpawnChance => 0f;

    public virtual bool FilterItem() => true;
}