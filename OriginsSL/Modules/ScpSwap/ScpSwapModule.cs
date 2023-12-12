using CursedMod.Events.Handlers;

namespace OriginsSL.Modules.ScpSwap;

public class ScpSwapModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.ChangingRole += ScpSwapCommand.HandleMessage;
        CursedRoundEventsHandler.RestartingRound += ScpSwapCommand.ClearCache;
    }
}