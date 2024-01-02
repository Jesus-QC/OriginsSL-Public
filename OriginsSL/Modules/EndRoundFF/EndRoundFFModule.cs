using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Server;
using OriginsSL.Loader;

namespace OriginsSL.Modules.EndRoundFF;

// ReSharper disable once InconsistentNaming
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