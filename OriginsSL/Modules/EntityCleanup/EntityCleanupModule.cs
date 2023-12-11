using System.Collections.Generic;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Facility;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Ragdolls;
using MEC;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.Ragdolls;

namespace OriginsSL.Modules.EntityCleanup;

public class EntityCleanupModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedRoundEventsHandler.RoundStarted += OnRoundStarted;
        CursedRoundEventsHandler.RoundEnded += OnRoundEnded;
    }
    
    private static void OnRoundStarted() => Timing.RunCoroutine(AutoCleanup(), nameof(EntityCleanupModule));
    
    private static void OnRoundEnded() => Timing.KillCoroutines(nameof(EntityCleanupModule));

    private static IEnumerator<float> AutoCleanup()
    {
        // ReSharper disable once IteratorNeverReturns
        while (true)
        {
            yield return Timing.WaitForSeconds(180); // 3 Minutes Placeholder (Sujeto a cambios)

            HashSet<BasicRagdoll> stolenRagdolls = new();
            
            foreach (CursedPlayer player in CursedPlayer.Collection)
            {
                if (player.CurrentRole.RoleBase is not Scp3114Role scp3114)
                    continue;
                
                if (!scp3114.Disguised || scp3114.CurIdentity.Ragdoll == null)
                    continue;

                stolenRagdolls.Add(scp3114.CurIdentity.Ragdoll);
            }

            foreach (CursedRagdoll ragdoll in CursedRagdoll.List)
            {
                if (stolenRagdolls.Contains(ragdoll.Base))
                    continue;
                
                ragdoll.Destroy();
            }
        }
    }
}