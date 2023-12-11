using System.Collections.Generic;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player.Ragdolls;
using MEC;

namespace OriginsSL.Modules.EntityCleanup;

public class EntityCleanupModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedRoundEventsHandler.RoundStarted += OnRoundStarted;
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
    }
    
    private static void OnRoundStarted() => Timing.RunCoroutine(AutoCleanup(), "AutoCleanup");
    
    private static void OnRoundEnded() => Timing.KillCoroutines("AutoCleanup");

    private static IEnumerator<float> AutoCleanup()
    {
        // ReSharper disable IteratorNeverReturns
        
        while (true)
        {
            yield return Timing.WaitForSeconds(180); // 3 Minutes Placeholder (Sujeto a cambios)
            
            foreach (CursedRagdoll ragdoll in CursedRagdoll.List)
                ragdoll.Destroy();
        }
    }
}