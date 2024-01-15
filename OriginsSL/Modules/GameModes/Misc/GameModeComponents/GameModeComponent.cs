namespace OriginsSL.Modules.GameModes.Misc.GameModeComponents;

public abstract class GameModeComponent
{
    public virtual void OnStarting(CursedGameModeBase gameModeBase){}

    public virtual void OnStopping(){}
    
    public virtual void OnUpdate(){}
}