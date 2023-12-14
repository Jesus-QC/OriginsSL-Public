namespace OriginsSL.Modules.LevelingSystem;

public class LevelingSystemModule : OriginsModule
{
    public override void OnLoaded()
    {
        LevelingSystemEventsHandler.RegisterServerEvents();
        LevelingSystemEventsHandler.RegisterPlayerEvents();
    }
}