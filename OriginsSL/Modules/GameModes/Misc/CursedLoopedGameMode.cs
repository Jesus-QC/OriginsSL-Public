namespace OriginsSL.Modules.GameModes.Misc;

public class CursedLoopedGameMode : CursedGameModeBase
{
    public override void StartGameMode()
    {
        StaticUnityMethods.OnUpdate += OnUpdate; 
        base.StartGameMode();
    }
    
    public override void StopGameMode()
    {
        StaticUnityMethods.OnUpdate -= OnUpdate;
        base.StopGameMode();
    }

    public virtual void OnUpdate()
    {
        
    }
}