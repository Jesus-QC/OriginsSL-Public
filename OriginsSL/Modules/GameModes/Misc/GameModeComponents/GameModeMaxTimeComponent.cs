using System;

namespace OriginsSL.Modules.GameModes.Misc.GameModeComponents;

public class GameModeMaxTimeComponent(TimeSpan maxDuration) : GameModeComponent
{
    private CursedGameModeBase _gameModeBase;
    
    public TimeSpan OverrideTimer = TimeSpan.Zero;

    public override void OnStarting(CursedGameModeBase gameModeBase)
    {
        _gameModeBase = gameModeBase;
        base.OnStarting(_gameModeBase);
    }

    public override void OnUpdate()
    {
        OverrideTimer = maxDuration - new TimeSpan(DateTime.Now.Ticks - _gameModeBase.StartTime);
        
        if (OverrideTimer <= TimeSpan.Zero)
            _gameModeBase.StopGameMode();
        
        base.OnUpdate();
    }
}