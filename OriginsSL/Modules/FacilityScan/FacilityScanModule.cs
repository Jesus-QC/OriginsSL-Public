using System.Collections.Generic;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Facility;
using CursedMod.Features.Wrappers.Player;
using MEC;
using OriginsSL.Loader;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.FacilityScan;

public class FacilityScanModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedRoundEventsHandler.RoundStarted += OnRoundStarted;
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
    }
    
    private static void OnRoundStarted() => Timing.RunCoroutine(ScanFacility(), nameof(FacilityScanModule));
    
    private static void OnRoundEnded() => Timing.KillCoroutines(nameof(FacilityScanModule));

    // ReSharper disable once IteratorNeverReturns
    private static IEnumerator<float> ScanFacility()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(Random.Range(200, 300));

            int classDCount = 0, scientistCount = 0, mtfCount = 0, chaosCount = 0, scpsCount = 0;
            
            foreach (CursedPlayer player in CursedPlayer.Collection)
            {
                if (player.CurrentRole.Team == Team.ClassD)
                    classDCount++;
                else if (player.CurrentRole.Team == Team.Scientists)
                    scientistCount++;
                else if (player.CurrentRole.Team == Team.FoundationForces)
                    mtfCount++;
                else if (player.CurrentRole.Team == Team.ChaosInsurgency)
                    chaosCount++;
                else if (player.CurrentRole.Team == Team.SCPs)
                    scpsCount++;
            }
            
            if (classDCount == 0 && scientistCount == 0 && mtfCount == 0 && chaosCount == 0 && scpsCount == 0)
                continue;

            string message = ".G6 Facility Scan Completed .G2 .G3 Found ";
            
            if (classDCount > 0)
                message += $"{classDCount} Class D ";
            if (scientistCount > 0)
                message += $"{scientistCount} Scientists ";
            if (mtfCount > 0)
                message += $"{mtfCount} Foundation Forces ";
            if (chaosCount > 0)
                message += $"{chaosCount} Chaos ";
            if (scpsCount > 0)
                message += $"{scpsCount} SCPs ";
            
            CursedCassie.PlayGlitchyPhrase(message);
        }
    }
}