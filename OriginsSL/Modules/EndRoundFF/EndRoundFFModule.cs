using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Server;

namespace OriginsSL.Modules.EndRoundFF;

// ReSharper disable InconsistentNaming

public class EndRoundFFModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
        CursedRoundEventsHandler.RestartingRound += OnRestartingRound;
    }
    
    private static void OnRoundEnded() => CursedServer.IsFriendlyFireEnabled = true;
    
    private static void OnRestartingRound() => CursedServer.IsFriendlyFireEnabled = false;
}