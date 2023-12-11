using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CursedMod.Events.Handlers;
using CursedMod.Features.Logger;
using CursedMod.Features.Wrappers.Player;
using OriginsSL;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles.Spectating;

namespace CursedSL.Modules.SpectatorCount;

public class SpectatorCountManager : OriginsModule
{
    private static readonly List<CancellationTokenSource> CancellationTokenSources = [];

    public override void OnLoaded()
    {
        CursedRoundEventsHandler.WaitingForPlayers += Start;
        CursedRoundEventsHandler.RestartingRound += Stop;
    }

    private static void Start()
    {
        CancellationTokenSource cancellationTokenSource = new();
        Task.Run(() => Timer(cancellationTokenSource), cancellationTokenSource.Token);
        CancellationTokenSources.Add(cancellationTokenSource);
    }

    private static void Stop()
    {
        foreach (CancellationTokenSource cancellationTokenSource in CancellationTokenSources)
        {
            cancellationTokenSource.Cancel();
        }
        
        CancellationTokenSources.Clear();
    }
    
    private static async Task Timer(CancellationTokenSource cancellationTokenSource)
    {
        CursedLogger.LogInformation("SpectatorCountManager started");
        Dictionary<uint, int> spectatorCount = new ();
        
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            foreach (CursedPlayer player in CursedPlayer.Collection)
            {
                if (player.RoleBase is not SpectatorRole spectatorRole)
                    continue;

                uint id = spectatorRole.SyncedSpectatedNetId;

                if (spectatorCount.ContainsKey(id))
                {
                    spectatorCount[id]++;
                    continue;
                }
                
                spectatorCount.Add(id, 1);
            }

            foreach (CursedPlayer player in CursedPlayer.Collection)
            {
                if (!DisplayRendererModule.TryGetDisplayBuilder(player, out CursedDisplayBuilder displayBuilder))
                    continue;
                
                if (!spectatorCount.ContainsKey(player.NetId))
                {
                    displayBuilder.WithSpectators(0);
                    continue;
                }
                
                displayBuilder.WithSpectators(spectatorCount[player.NetId]);
            }
            
            spectatorCount.Clear();

            await Task.Delay(1500);
        }

        CursedLogger.LogInformation("SpectatorCountManager stopped");
    }
}