namespace OriginsSL.Loader;

public abstract class OriginsModule
{
    public virtual byte Priority { get; set; } = 150;

    public virtual bool Disabled { get; set; } = false;

    public virtual bool DisruptedOnly { get; set; } = false;
    
    public abstract void OnLoaded();
}