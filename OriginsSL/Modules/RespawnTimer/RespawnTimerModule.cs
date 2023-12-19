using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Facility;
using CursedMod.Features.Wrappers.Player;
using PluginAPI.Core;
using Respawning;
using UnityEngine;

namespace OriginsSL.Modules.RespawnTimer;

public class RespawnTimerModule : OriginsModule
{
    public override void OnLoaded()
    {
        CursedRoundEventsHandler.WaitingForPlayers += Start;
        CursedRoundEventsHandler.RestartingRound += Stop;
    }
    
    private static readonly List<CancellationTokenSource> CancellationTokenSources = [];

    private static void Start()
    {
        CancellationTokenSource cancellationTokenSource = new();
        Task.Run(() => RunTimer(cancellationTokenSource), cancellationTokenSource.Token);
        CancellationTokenSources.Add(cancellationTokenSource);
    }

    private static void Stop()
    {
        foreach (CancellationTokenSource cancellationTokenSource in CancellationTokenSources)
        {
            cancellationTokenSource.Cancel();
        }
        
        CancellationTokenSources.Clear();
        
        Info = "<b>Restarting Round...</b>\n\n\n\n";
    }

    private static async Task RunTimer(CancellationTokenSource cancellationTokenSource)
    {
        int tipCount = 0;
        string tip = string.Empty;
        AvailableTips.Add($"D<lowercase>id you know there are in total {AvailableTips.Count + 1} tips like this one?</lowercase>");
        
        while (!cancellationTokenSource.IsCancellationRequested)
        {
            await Task.Delay(1000, cancellationTokenSource.Token);
            
            tipCount++;
            if (tipCount == 9)
            {
                tipCount = 0;
                tip = AvailableTips.RandomItem();
            }

            TimeSpan timeUntilSpawn = CursedRespawnManager.RemainingSequenceTime;
            Timer = $"{timeUntilSpawn.Minutes.ToString().PadLeft(2, '0')}:{timeUntilSpawn.Seconds.ToString().PadLeft(2, '0')}";
            
            if (CursedRespawnManager.NextKnownTeam == SpawnableTeamType.None)
                Info = "<color=#717ac9>❓</color> " + tip;
            else
                Info = Respawn.NextKnownTeam == SpawnableTeamType.ChaosInsurgency ? "<b><color=#58e85f>C<lowercase>haos wave spawning</lowercase></color></b>" : "<b><color=#479ade>M<lowercase>tf wave spawning</lowercase></color></b>";

            ChaosChance = $"{Mathf.Round(Respawn.ChaosTickets * 100)}%";
            MtfChance = $"{Mathf.Round(Respawn.NtfTickets * 100)}%";
            SpectatorCount = CursedPlayer.Collection.Count(x => x.IsDead).ToString();
        }
    }

    public static string Timer = string.Empty;
    public static string Info = string.Empty;
    public static string ChaosChance = string.Empty;
    public static string MtfChance = string.Empty;
    public static string SpectatorCount = string.Empty;

    private static readonly List<string> AvailableTips =
    [
        "E<lowercase>xplore the map thoroughly to find valuable items and escape routes.</lowercase>",
        "P<lowercase>ress [TAB] to open your inventory.</lowercase>",
        "U<lowercase>se the radio to communicate with other players and plan your strategy.</lowercase>",
        "C<lowercase>hoose your battles wisely; not every confrontation is worth risking your life.</lowercase>",
        "S<lowercase>tay vigilant in the dark; SCPs are more dangerous in low-light conditions.</lowercase>",
        "A<lowercase>void splitting; it only weakens the overall chances of survival.</lowercase>",
        "L<lowercase>earn the layout of the facility to navigate quickly and efficiently.</lowercase>",
        "U<lowercase>se medkits and SCP items strategically to maximize their benefits.</lowercase>",
        "R<lowercase>emember, cooperation is key to surviving and escaping the facility.</lowercase>",
        "C<lowercase>ommunicate effectively with your team to coordinate actions and plans.</lowercase>",
        "I<lowercase>f you find a keycard, share it with your team to increase everyone's chances of escape.</lowercase>",
        "T<lowercase>ry different playstyles to find what suits you best in various situations.</lowercase>",
        "H<lowercase>elp new players; a knowledgeable and cohesive team has a better chance of survival.</lowercase>",
        "O<lowercase>ptimize your settings for better visibility and performance.</lowercase>",
        "H<lowercase>aving trouble? Ask experienced players for advice and tips.</lowercase>",
        "E<lowercase>xploit the weaknesses of SCPs; each one has a unique set of vulnerabilities.</lowercase>",
        "M<lowercase>emorize the locations of important rooms like armories and SCP chambers.</lowercase>",
        "O<lowercase>bserve your surroundings for signs of SCP activity or other players.</lowercase>",
        "R<lowercase>emember that SCPs have their own objectives; understanding them can help you anticipate their moves.</lowercase>",
        "E<lowercase>levators are the way to escape; locate them as soon as possible.</lowercase>",
        "T<lowercase>ake note of SCP-914's location for possible item upgrades.</lowercase>",
        "I<lowercase>f you're an SCP, coordinate with fellow SCPs to increase your chances of containment.</lowercase>",
        "P<lowercase>ay attention to discord announcements for important updates and events.</lowercase>",
        "S<lowercase>tay calm under pressure; panicking can lead to poor decision-making.</lowercase>",
        "W<lowercase>hen in doubt, communicate with your team for guidance and support.</lowercase>",
        "B<lowercase>e good with your teammates!</lowercase>",
        "E",
        "<color=#FFFF7C>SCP<lowercase>-914</color> is your best ally!</lowercase>",
        "T<lowercase>he cake is a lie</lowercase>",
        "T<lowercase>here is no sanity in this anomaly-filled world.</lowercase>",
        "<color=red>SCP<lowercase>s</color> can toggle proximity chat with the <color=#FFFF7C>[ALT] key</color>!</lowercase>",
        "M<lowercase>ultiple people can talk at the same time in the intercom!</lowercase>",
        "D<lowercase>id you know that in the SCP-173 Containment Chamber, there is a button that can be pressed to open the door</lowercase>?",
        "F<lowercase>ind creative ways to use SCP items; experimentation can lead to unique advantages.</lowercase>",
        "A<lowercase>lways be aware of your surroundings; unexpected events can occur at any moment.</lowercase>",
        "L<lowercase>ook out for alternative routes for strategic movement.</lowercase>",
        "T<lowercase>ake note of SCP-914's different settings; it can be a game-changer for item upgrades.</lowercase>",
        "E<lowercase>xperiment with different SCP roles to understand their strengths and weaknesses.</lowercase>",
        "G<lowercase>uard important areas with your team to control access points and resources.</lowercase>",
        "A<lowercase>ttack strategically; rushing into combat without a plan can lead to disaster.</lowercase>",
        "S<lowercase>tudy the behavior of other players and adapt your tactics accordingly.</lowercase>",
        "P<lowercase>rotect key personnel, such as scientists or guards, for a stronger team.</lowercase>",
        "R<lowercase>emember that teamwork and communication can overcome even the toughest challenges.</lowercase>",
        "U<lowercase>se SCP items to your advantage; they can be powerful tools when handled correctly.</lowercase>",
        "S<lowercase>ave your ammunition for crucial moments; running out at the wrong time can be fatal.</lowercase>",
        "E<lowercase>ncourage a positive and cooperative atmosphere within your team for better coordination.</lowercase>",
        "T<lowercase>ake breaks to avoid burnout; staying focused and alert is crucial for success.</lowercase>",
        "H<lowercase>elp injured teammates by sharing medkits and supporting their recovery.</lowercase>",
        "I<lowercase>f in doubt, consult the discord or ask experienced players for guidance.</lowercase>",
        "C<lowercase>onserve resources; using them wisely can make a significant difference in critical situations.</lowercase>",
        "A<lowercase>dapt to changing circumstances; flexibility is key to survival in unpredictable situations.</lowercase>",
        "L<lowercase>earn from your mistakes; each game provides an opportunity for improvement.</lowercase>",
        "H<lowercase>ave fun and enjoy the unique experiences that each round brings!</lowercase>",
        "T<lowercase>he <color=#FFFF7C>SCP-500</color> can remove all your effects!</lowercase>",
        "J<lowercase>oin our discord server to be alerted about events and announcements.</lowercase>",
        "D<lowercase>on't stand near grenades, they may hurt you</lowercase>",
        "D<lowercase>id you know that inside <color=#EC2121>SCP-173s</color> room you can find <color=#FFFF7C>SCP-1162</color>?</lowercase>"
    ];
}