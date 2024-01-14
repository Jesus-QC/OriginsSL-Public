using System.Collections.Generic;
using CursedMod.Features.Wrappers.Facility;
using MEC;

namespace OriginsSL.Modules.GameModes;

public abstract class CursedGameModeBase : ICursedGameMode
{
    public bool IsPreparing { get; set; } = true;
    
    public virtual string CodeName => string.Empty;
    
    public virtual string Name => string.Empty;
    
    public virtual string Description => string.Empty;
    
    public float StartDuration => 15f;

    public virtual void PrepareGameMode()
    {
        IsPreparing = true;
        StartGameMode();
    }

    public virtual void StartGameMode()
    {
        IsPreparing = false;
    }

    public virtual void StopGameMode() { }
}