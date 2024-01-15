using System;

namespace OriginsSL.Modules.GameModes.Misc.GameModeComponents;

public class GameModeMaxTimeComponent(TimeSpan maxDuration) : GameModeComponent
{
    private CursedGameModeBase _gameModeBase;

    public override void OnStarting(CursedGameModeBase gameModeBase)
    {
        _gameModeBase = gameModeBase;
        base.OnStarting(_gameModeBase);
    }

    public override void OnUpdate()
    {
        _gameModeBase.OverrideTimer = maxDuration - new TimeSpan(DateTime.Now.Ticks - _gameModeBase.StartTime);
        
        if (_gameModeBase.OverrideTimer <= TimeSpan.Zero)
            _gameModeBase.StopGameMode();
        
        base.OnUpdate();
    }
}