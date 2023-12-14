namespace OriginsSL.Modules.LevelingSystem;

// TODO: mariadb integrated db in dedi
// PD: Nexus, do not use sqlite
public class LevelingSystemModule : OriginsModule
{
    private static readonly LevelingSystemEventsHandler EventsHandler = new ();
    
    public override void OnLoaded()
    {
        EventsHandler.RegisterServerEvents();
        EventsHandler.RegisterPlayerEvents();
    }
}
