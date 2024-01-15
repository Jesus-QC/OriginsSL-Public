using System;
using System.Collections.Generic;
using OriginsSL.Modules.GameModes.Misc.GameModeComponents;
using PluginAPI.Core;

namespace OriginsSL.Modules.GameModes;

public abstract class CursedGameModeBase
{
    public bool IsPreparing { get; set; } = true;
    
    public virtual string CodeName => string.Empty;
    
    public virtual string Name => string.Empty;
    
    public virtual string Description => string.Empty;
    
    public long StartTime { get; set; }

    protected virtual GameModeComponent[] Components { get; } = [];

    public TimeSpan OverrideTimer { get; set; } = TimeSpan.Zero;
    
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

    public virtual void OnUpdate()
    {
        foreach (GameModeComponent component in Components)
        {
            component.OnUpdate();
        }
    }
}