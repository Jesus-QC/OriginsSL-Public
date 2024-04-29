using System;
using System.Collections.Generic;
using OriginsSL.Modules.GameModes.Misc.GameModeComponents;

namespace OriginsSL.Modules.GameModes;

public abstract class CursedGameModeBase
{
    public bool IsPreparing { get; set; } = true;
    
    public virtual string CodeName => string.Empty;
    
    public virtual string Name => string.Empty;
    
    public virtual string Description => string.Empty;
    
    public long StartTime { get; set; }

    protected virtual IEnumerable<GameModeComponent> Components { get; } = [];
    
    public virtual void StartGameMode()
    {
        StartTime = DateTime.Now.Ticks;
        
        foreach (GameModeComponent component in Components)
        {
            component.OnStarting(this);
        }
        
        StaticUnityMethods.OnUpdate += OnUpdate;
    }

    public virtual void StopGameMode()
    {
        StaticUnityMethods.OnUpdate -= OnUpdate;
        
        foreach (GameModeComponent component in Components)
        {
            component.OnStopping();
        }
    }

    protected virtual void OnUpdate()
    {
        foreach (GameModeComponent component in Components)
        {
            component.OnUpdate();
        }
    }
    
    public bool TryGetComponent<T>(out T comp) where T : GameModeComponent
    {
        foreach (GameModeComponent component in Components)
        {
            if (component is not T t)
                continue;
            
            comp = t;
            return true;
        }

        comp = default;
        return false;
    }
}