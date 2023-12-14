namespace OriginsSL.Modules.LevelingSystem;

public class LevelingSystemModule : OriginsModule
{
    private static readonly LevelingSystemEventsHandler EventsHandler = new ();
    
    public override void OnLoaded()
    {
        EventsHandler.RegisterServerEvents();
        EventsHandler.RegisterPlayerEvents();
    }
}