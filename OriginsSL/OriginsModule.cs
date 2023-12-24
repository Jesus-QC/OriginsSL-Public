namespace OriginsSL;

public abstract class OriginsModule
{
    public virtual byte Priority { get; set; } = 150;
    
    public abstract void OnLoaded();
}