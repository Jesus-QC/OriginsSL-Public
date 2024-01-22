using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CursedMod.Features.Wrappers.Player;
using NorthwoodLib.Pools;
using OriginsSL.Modules.CustomItems;
using OriginsSL.Modules.EndScreen;
using OriginsSL.Modules.GameModes;
using OriginsSL.Modules.LevelingSystem;
using OriginsSL.Modules.PollManager;
using OriginsSL.Modules.RespawnTimer;
using OriginsSL.Modules.ScpList;
using OriginsSL.Modules.SpectatorFeed;
using OriginsSL.Modules.Subclasses;

namespace OriginsSL.Features.Display;

public class CursedDisplayBuilder(CursedPlayer player)
{
    protected const string Header = "<size=65%><line-height=87%><voffset=12.9em>";
    protected const string Footer = "<line-height=0><size=55%><b><color=#E2E0A6>o</color><color=#D8D4AC>r</color><color=#CEC8B2>i</color><color=#C4BCB8>g</color><color=#BAB0BE>i</color><color=#B0A4C4>n</color><color=#A698CA>s</color><align=right><size=40%>";
    private const string Discord = "<lowercase><b><color=#EEEAA1>d</color><color=#E4E2A1>i</color><color=#DADAA1>s</color><color=#D0D2A1>c</color><color=#C6CAA1>o</color><color=#BCC2A1>r</color><color=#B2BAA1>d</color><color=#A8B2A1>.</color><color=#9EAAA1>g</color><color=#94A2A1>g</color><color=#8A9AA1>/</color><color=#8092A1>s</color><color=#768AA1>c</color><color=#6C82A1>p</color><color=#627AA1>o</color><color=#5872A1>r</color><color=#4E6AA1>i</color><color=#4462A1>g</color><color=#3A5AA1>i</color><color=#3052A1>n</color><color=#264AA1>s</color></b></lowercase>";
    private const string DiscordFooter = "<line-height=0><size=50%><b><color=#EEEAA1>d</color><color=#E4E2A1>i</color><color=#DADAA1>s</color><color=#D0D2A1>c</color><color=#C6CAA1>o</color><color=#BCC2A1>r</color><color=#B2BAA1>d</color><color=#A8B2A1>.</color><color=#9EAAA1>g</color><color=#94A2A1>g</color><color=#8A9AA1>/</color><color=#8092A1>s</color><color=#768AA1>c</color><color=#6C82A1>p</color><color=#627AA1>o</color><color=#5872A1>r</color><color=#4E6AA1>i</color><color=#4462A1>g</color><color=#3A5AA1>i</color><color=#3052A1>n</color><color=#264AA1>s</color></b><align=right><size=40%>";

    private readonly Dictionary<ScreenZone, HudNotification> _savedZones = new()
    {
        [ScreenZone.Environment] = new HudNotification(string.Empty),
        [ScreenZone.Important] = new HudNotification(string.Empty),
        [ScreenZone.Center] = new HudNotification(string.Empty),
        [ScreenZone.EventImportant] = new HudNotification(string.Empty),
        [ScreenZone.EventCenter] = new HudNotification(string.Empty),
    };

    private readonly List<HudNotification> _notifications = [];

    protected readonly StringBuilder StringBuilder = StringBuilderPool.Shared.Rent();
    private int _spectators;

    public void ClearData()
    {
        StringBuilderPool.Shared.Return(StringBuilder);
        _savedZones.Clear();
        _notifications.Clear();
    }
    
    public void WithContent(ScreenZone zone, string content, float duration = 4)
    {
        _savedZones[zone].Message = content;
        _savedZones[zone].Duration = duration;
    }

    public void ClearZone(ScreenZone zone)
    {
        _savedZones[zone].Duration = -1;
        _savedZones[zone].Message = string.Empty;
    }

    public void AddNotification(string content) => _notifications.Add(new HudNotification(content));

    private void UpdateZones()
    {
        UpdateZone(ScreenZone.Environment);
        UpdateZone(ScreenZone.Important);
        UpdateZone(ScreenZone.Center);
        UpdateNotifications();
    }

    private void UpdateNotifications()
    {
        foreach (HudNotification notification in _notifications.ToHashSet())
        {
            if (notification.Duration <= 0)
                _notifications.Remove(notification);
            
            notification.Duration -= 0.5f;
        }
    }
    
    private void UpdateZone(ScreenZone zone)
    {
        HudNotification notification = _savedZones[zone];

        if (notification.Duration <= 0)
        {
            notification.Message = string.Empty;
            return;
        }
        
        notification.Duration -= 0.5f;
    }
    
    public void WithSpectators(int count) => _spectators = count;
    
    public string BuildForHuman()
    {
        UpdateZones();
        StringBuilder.Clear();
        StringBuilder.AppendLine(Header);
        
        string color = $"<color={player.CurrentRole.RoleColor.ToHex()}>";
        StringBuilder.Append("<size=50%><align=right>");
        StringBuilder.AppendLine(_spectators == 0 ? "" : $"<b>{color}<alpha=#30>S<lowercase>pectators:</lowercase> {_spectators}<alpha=#ff></color></b>");

        for (int i = 0; i < 6; i++)
            StringBuilder.AppendLine(GetNotification(i));

        StringBuilder.AppendLine("</align></size>");
        
        RenderPolls();
        
        StringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        StringBuilder.Append("\n\n\n\n\n\n\n");
        StringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        StringBuilder.Append("\n<size=40>");
        StringBuilder.AppendLine(player.GetCustomItemName());
        StringBuilder.Append("</size>");
        StringBuilder.AppendLine(player.GetCustomItemDescription());
        StringBuilder.AppendLine();
        StringBuilder.Append("<size=40>");
        StringBuilder.Append(GetZone(ScreenZone.Important));
        StringBuilder.AppendLine("</size>\n");
        StringBuilder.Append("<size=40>");
        StringBuilder.Append(player.GetSubclassName());
        StringBuilder.AppendLine("</size>");
        StringBuilder.AppendLine(player.GetSubclassDescription());
        StringBuilder.Append("\n\n\n\n");
        StringBuilder.AppendLine(Footer);

        if (player.DoNotTrack)
        {
            StringBuilder.AppendLine("DNT - Leveling Disabled");
        }
        else
        {
            (int level,int exp, int total) = player.GetLevelingProgress();
            StringBuilder.Append("<alpha=#50>LEVEL: ");
            StringBuilder.Append(level);
            StringBuilder.Append(" - EXP: ");
            StringBuilder.Append(exp);
            StringBuilder.Append(" / ");
            StringBuilder.Append(total);
        }
        
        return StringBuilder.ToString();
    }

    public string BuildForScp()
    { 
        UpdateZones();
        StringBuilder.Clear();
        StringBuilder.AppendLine(Header);
        
        string color = $"<color={player.CurrentRole.RoleColor.ToHex()}>";
        StringBuilder.Append("<size=50%><align=right>");
        StringBuilder.AppendLine(_spectators == 0 ? "" : $"<b>{color}<alpha=#30>S<lowercase>pectators:</lowercase> {_spectators}<alpha=#ff></color></b>");

        for (int i = 0; i < 6; i++)
            StringBuilder.AppendLine(GetNotification(i));
        
        StringBuilder.AppendLine("<color=#EC2121><b><u>SCP LIST</u></b>");
        StringBuilder.AppendLine(ScpListModule.GetContent(0));
        StringBuilder.AppendLine(ScpListModule.GetContent(1));
        StringBuilder.AppendLine(ScpListModule.GetContent(2));
        StringBuilder.AppendLine(ScpListModule.GetContent(3));
        StringBuilder.AppendLine(ScpListModule.GetContent(4));
        StringBuilder.AppendLine(ScpListModule.GetContent(5));
        StringBuilder.AppendLine(ScpListModule.GetContent(6));
        StringBuilder.Append(ScpListModule.GetContent(7));
        StringBuilder.AppendLine("</align></size></color>");
        StringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        StringBuilder.Append("\n\n\n");
        StringBuilder.AppendLine(RenderZone(ScreenZone.Center));

        RenderPolls();
        
        StringBuilder.Append("<size=40>");
        StringBuilder.Append(GetZone(ScreenZone.Important));
        StringBuilder.AppendLine("</size>\n");
        StringBuilder.Append("<size=40>");
        StringBuilder.Append(player.GetSubclassName());
        StringBuilder.AppendLine("</size>");
        StringBuilder.AppendLine(player.GetSubclassDescription());
        StringBuilder.Append("\n\n\n\n");
        StringBuilder.AppendLine(Footer);
        
        if (player.DoNotTrack)
        {
            StringBuilder.AppendLine("DNT - Leveling Disabled");
        }
        else
        {
            (int level,int exp, int total) = player.GetLevelingProgress();
            StringBuilder.Append("<alpha=#50>LEVEL: ");
            StringBuilder.Append(level);
            StringBuilder.Append(" - EXP: ");
            StringBuilder.Append(exp);
            StringBuilder.Append(" / ");
            StringBuilder.Append(total);
        }
        
        return StringBuilder.ToString();
    }

    public string BuildForLobby()
    {
        UpdateZones();
        StringBuilder.Clear();
        StringBuilder.AppendLine(Header);
        
        StringBuilder.Append("\n\n");
        StringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        StringBuilder.Append("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        
        RenderPolls();
        
        StringBuilder.AppendLine();
        StringBuilder.AppendLine(GetZone(ScreenZone.Important));
        StringBuilder.AppendLine();
        StringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        StringBuilder.AppendLine("<size=20>join our discord");
        StringBuilder.AppendLine(Discord);
        StringBuilder.AppendLine();
        StringBuilder.AppendLine(Footer);
        
        return StringBuilder.ToString();
    }

    public string BuildForSpectator(CursedPlayer spectatedPlayer)
    {
        UpdateZones();
        StringBuilder.Clear();
        StringBuilder.AppendLine(Header);
        StringBuilder.Append("<size=50%>Spectators: ");
        StringBuilder.AppendLine(RespawnTimerModule.SpectatorCount);
        StringBuilder.Append("<align=right>MTF Domination: ");
        StringBuilder.AppendLine(RespawnTimerModule.MtfChance);
        StringBuilder.Append("Chaos Domination: ");
        StringBuilder.Append(RespawnTimerModule.ChaosChance);
        StringBuilder.AppendLine("</align>");
        
        StringBuilder.Append("<b><size=70>");
        StringBuilder.Append(RespawnTimerModule.Timer);
        StringBuilder.AppendLine("</size></b>");
        StringBuilder.Append(RespawnTimerModule.Info);
        StringBuilder.AppendLine("</size>");

        StringBuilder.Append("\n\n\n\n\n");
        StringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        StringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        StringBuilder.Append("\n\n\n");
        
        RenderPolls();

        StringBuilder.Append("<size=40>");
        StringBuilder.Append(GetZone(ScreenZone.Important));
        StringBuilder.AppendLine("</size>\n");
        
        StringBuilder.AppendLine(SpectatorFeedModule.GetContent(0));
        StringBuilder.AppendLine(SpectatorFeedModule.GetContentWithAlpha(1, "<alpha=#80>"));
        StringBuilder.AppendLine(SpectatorFeedModule.GetContentWithAlpha(2, "<alpha=#50>"));
        StringBuilder.AppendLine(SpectatorFeedModule.GetContentWithAlpha(3, "<alpha=#30>"));
        StringBuilder.AppendLine(SpectatorFeedModule.GetContentWithAlpha(4, "<alpha=#10>"));

        StringBuilder.AppendLine("<alpha=#ff>");
        StringBuilder.Append("<size=40>");
        StringBuilder.Append(spectatedPlayer.GetSubclassName());
        StringBuilder.AppendLine("</size>");
        StringBuilder.AppendLine(spectatedPlayer.GetSubclassDescription());
        
        StringBuilder.Append("\n<size=35>");
        StringBuilder.AppendLine(spectatedPlayer.GetCustomItemName());
        StringBuilder.Append("</size>");
        StringBuilder.AppendLine(spectatedPlayer.GetCustomItemDescription());
        StringBuilder.AppendLine();
        
        
        StringBuilder.AppendLine(DiscordFooter);
        
        if (player.DoNotTrack)
        {
            StringBuilder.AppendLine("<alpha=#40>DNT - Leveling Disabled");
        }
        else if (!spectatedPlayer.IsHost)
        {
            (int level,int exp, int total) = spectatedPlayer.GetLevelingProgress();
            StringBuilder.Append("<alpha=#50>");
            StringBuilder.Append(spectatedPlayer.DisplayNickname);
            StringBuilder.Append(" - LEVEL: ");
            StringBuilder.Append(level);
            StringBuilder.Append(" - EXP: ");
            StringBuilder.Append(exp);
            StringBuilder.Append(" / ");
            StringBuilder.Append(total);
        }
        
        return StringBuilder.ToString();
    }

    public string BuildEndScreen()
    {
        UpdateZones();
        StringBuilder.Clear();
        StringBuilder.AppendLine(Header);
        StringBuilder.AppendLine(Discord);
        StringBuilder.Append("<size=35><b>\n");
        StringBuilder.AppendLine(EndScreenModule.GetContent(0));
        StringBuilder.AppendLine();
        StringBuilder.AppendLine(EndScreenModule.GetContent(1));
        StringBuilder.AppendLine();
        StringBuilder.AppendLine(EndScreenModule.GetContent(2));
        StringBuilder.AppendLine();
        StringBuilder.AppendLine(EndScreenModule.GetContent(3));

        StringBuilder.Append("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        
        StringBuilder.AppendLine(Footer);
        
        return StringBuilder.ToString();
    }

    public string BuildForEvent()
    {
        UpdateZone(ScreenZone.EventCenter);
        UpdateZone(ScreenZone.EventImportant);
        UpdateZones();
        
        StringBuilder.Clear();
        StringBuilder.AppendLine(Header);
        StringBuilder.AppendLine();
        StringBuilder.AppendLine();
        TimeSpan timeLeft = CursedGameModeLoader.GetEventTimer();
        StringBuilder.AppendLine($"<size=80><b>{timeLeft.Minutes.ToString().PadLeft(2, '0')}:{timeLeft.Seconds.ToString().PadLeft(2, '0')}</b></size>");
        StringBuilder.AppendLine();
        StringBuilder.AppendLine(CursedGameModeLoader.GetEventName());
        StringBuilder.AppendLine(CursedGameModeLoader.GetEventDescription());
        StringBuilder.Append("\n\n<size=40>");
        StringBuilder.Append(GetZone(ScreenZone.Important));
        StringBuilder.Append("\n\n</size>");
        StringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        StringBuilder.Append("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        StringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        StringBuilder.Append("\n\n<size=40>");
        StringBuilder.Append(GetZone(ScreenZone.EventImportant));
        StringBuilder.AppendLine("</size>");
        StringBuilder.AppendLine(GetZone(ScreenZone.EventCenter));
        StringBuilder.AppendLine();
        StringBuilder.AppendLine();
        StringBuilder.AppendLine();
        StringBuilder.AppendLine(Footer);
        return StringBuilder.ToString();
    }

    private void RenderPolls()
    {
        if (PollManager.InUse)
        {
            StringBuilder.AppendLine($"\n<b><size=25><color=#ffd875>poll by {PollManager.Author}</color></size>");
            StringBuilder.AppendLine(PollManager.Description);
            StringBuilder.AppendLine($"<size=50%><color=#ffd875>{PollManager.TimeLeft} seconds left</color></size>");
            StringBuilder.AppendLine("to vote open the console and write <color=#61ff69>.vote yes</color> or <color=#ff61a0> .vote no</color>");
            StringBuilder.AppendLine($"<color=#61ff69>\u2705 {PollManager.AffirmativeVotes}</color> - <color=#ff61a0>{PollManager.NegativeVotes} \u274c</color></b>");
            return;
        }

        StringBuilder.Append("\n\n\n\n\n\n");
    }

    private string GetZone(ScreenZone zone) => _savedZones[zone].Message;

    private string RenderZone(ScreenZone zone) => FormatStringForHud(GetZone(zone));
    
    private string GetNotification(int index)
    {
        return _notifications.Count > index ? _notifications[index].Message : string.Empty;
    }
    
    private static string FormatStringForHud(string text, int linesNeeded = 5)
    {
        int textLines = text.Count(x => x == '\n');

        for (int i = 0; i < linesNeeded - textLines; i++)
            text += '\n';

        return text;
    }
}