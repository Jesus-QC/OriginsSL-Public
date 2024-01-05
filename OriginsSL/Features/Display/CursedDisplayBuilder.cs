using System.Collections.Generic;
using System.Linq;
using System.Text;
using CursedMod.Features.Wrappers.Player;
using NorthwoodLib.Pools;
using OriginsSL.Modules.CustomItems;
using OriginsSL.Modules.EndScreen;
using OriginsSL.Modules.LevelingSystem;
using OriginsSL.Modules.RespawnTimer;
using OriginsSL.Modules.ScpList;
using OriginsSL.Modules.SpectatorFeed;
using OriginsSL.Modules.Subclasses;

namespace OriginsSL.Features.Display;

public class CursedDisplayBuilder(CursedPlayer player)
{
    private const string Header = "<size=65%><line-height=87%><voffset=12.9em>";
    private const string Footer = "<line-height=0><size=55%><b><color=#E2E0A6>o</color><color=#D8D4AC>r</color><color=#CEC8B2>i</color><color=#C4BCB8>g</color><color=#BAB0BE>i</color><color=#B0A4C4>n</color><color=#A698CA>s</color><align=right><size=40%>";
    private const string Discord = "<lowercase><b><color=#EEEAA1>d</color><color=#E4E2A1>i</color><color=#DADAA1>s</color><color=#D0D2A1>c</color><color=#C6CAA1>o</color><color=#BCC2A1>r</color><color=#B2BAA1>d</color><color=#A8B2A1>.</color><color=#9EAAA1>g</color><color=#94A2A1>g</color><color=#8A9AA1>/</color><color=#8092A1>s</color><color=#768AA1>c</color><color=#6C82A1>p</color><color=#627AA1>o</color><color=#5872A1>r</color><color=#4E6AA1>i</color><color=#4462A1>g</color><color=#3A5AA1>i</color><color=#3052A1>n</color><color=#264AA1>s</color></b></lowercase>";
    private const string DiscordFooter = "<line-height=0><size=50%><b><color=#EEEAA1>d</color><color=#E4E2A1>i</color><color=#DADAA1>s</color><color=#D0D2A1>c</color><color=#C6CAA1>o</color><color=#BCC2A1>r</color><color=#B2BAA1>d</color><color=#A8B2A1>.</color><color=#9EAAA1>g</color><color=#94A2A1>g</color><color=#8A9AA1>/</color><color=#8092A1>s</color><color=#768AA1>c</color><color=#6C82A1>p</color><color=#627AA1>o</color><color=#5872A1>r</color><color=#4E6AA1>i</color><color=#4462A1>g</color><color=#3A5AA1>i</color><color=#3052A1>n</color><color=#264AA1>s</color></b><align=right><size=40%>";

    private readonly Dictionary<ScreenZone, HudNotification> _savedZones = new()
    {
        [ScreenZone.Environment] = new HudNotification(string.Empty),
        [ScreenZone.Important] = new HudNotification(string.Empty),
        [ScreenZone.Center] = new HudNotification(string.Empty),
    };

    private readonly List<HudNotification> _notifications = [];

    private readonly StringBuilder _stringBuilder = StringBuilderPool.Shared.Rent();
    private int _spectators;

    public void ClearData()
    {
        StringBuilderPool.Shared.Return(_stringBuilder);
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
        _stringBuilder.Clear();
        _stringBuilder.AppendLine(Header);
        
        string color = $"<color={player.CurrentRole.RoleColor.ToHex()}>";
        _stringBuilder.Append("<size=50%><align=right>");
        _stringBuilder.AppendLine(_spectators == 0 ? "" : $"<b>{color}<alpha=#30>S<lowercase>pectators:</lowercase> {_spectators}<alpha=#ff></color></b>");

        for (int i = 0; i < 6; i++)
            _stringBuilder.AppendLine(GetNotification(i));

        _stringBuilder.AppendLine("</align></size>");
        _stringBuilder.Append("\n\n\n\n\n\n");
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        _stringBuilder.Append("\n\n\n\n\n\n\n");
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        _stringBuilder.Append("\n<size=40>");
        _stringBuilder.AppendLine(player.GetCustomItemName());
        _stringBuilder.Append("</size>");
        _stringBuilder.AppendLine(player.GetCustomItemDescription());
        _stringBuilder.AppendLine();
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append(GetZone(ScreenZone.Important));
        _stringBuilder.AppendLine("</size>\n");
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append(player.GetSubclassName());
        _stringBuilder.AppendLine("</size>");
        _stringBuilder.AppendLine(player.GetSubclassDescription());
        _stringBuilder.Append("\n\n\n\n");
        _stringBuilder.AppendLine(Footer);

        if (player.DoNotTrack)
        {
            _stringBuilder.AppendLine("DNT - Leveling Disabled");
        }
        else
        {
            (int level,int exp, int total) = player.GetLevelingProgress();
            _stringBuilder.Append("<alpha=#50>LEVEL: ");
            _stringBuilder.Append(level);
            _stringBuilder.Append(" - EXP: ");
            _stringBuilder.Append(exp);
            _stringBuilder.Append(" / ");
            _stringBuilder.Append(total);
        }
        
        return _stringBuilder.ToString();
    }

    public string BuildForScp()
    { 
        UpdateZones();
        _stringBuilder.Clear();
        _stringBuilder.AppendLine(Header);
        
        string color = $"<color={player.CurrentRole.RoleColor.ToHex()}>";
        _stringBuilder.Append("<size=50%><align=right>");
        _stringBuilder.AppendLine(_spectators == 0 ? "" : $"<b>{color}<alpha=#30>S<lowercase>pectators:</lowercase> {_spectators}<alpha=#ff></color></b>");

        for (int i = 0; i < 6; i++)
            _stringBuilder.AppendLine(GetNotification(i));
        
        _stringBuilder.AppendLine("<color=#EC2121><b><u>SCP LIST</u></b>");
        _stringBuilder.AppendLine(ScpListModule.GetContent(0));
        _stringBuilder.AppendLine(ScpListModule.GetContent(1));
        _stringBuilder.AppendLine(ScpListModule.GetContent(2));
        _stringBuilder.AppendLine(ScpListModule.GetContent(3));
        _stringBuilder.AppendLine(ScpListModule.GetContent(4));
        _stringBuilder.AppendLine(ScpListModule.GetContent(5));
        _stringBuilder.AppendLine(ScpListModule.GetContent(6));
        _stringBuilder.Append(ScpListModule.GetContent(7));
        _stringBuilder.AppendLine("</align></size></color>");
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        _stringBuilder.Append("\n\n\n\n\n");
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        _stringBuilder.Append("\n\n\n\n");
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append(GetZone(ScreenZone.Important));
        _stringBuilder.AppendLine("</size>\n");
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append(player.GetSubclassName());
        _stringBuilder.AppendLine("</size>");
        _stringBuilder.AppendLine(player.GetSubclassDescription());
        _stringBuilder.Append("\n\n\n\n");
        _stringBuilder.AppendLine(Footer);
        
        if (player.DoNotTrack)
        {
            _stringBuilder.AppendLine("DNT - Leveling Disabled");
        }
        else
        {
            (int level,int exp, int total) = player.GetLevelingProgress();
            _stringBuilder.Append("<alpha=#50>LEVEL: ");
            _stringBuilder.Append(level);
            _stringBuilder.Append(" - EXP: ");
            _stringBuilder.Append(exp);
            _stringBuilder.Append(" / ");
            _stringBuilder.Append(total);
        }
        
        return _stringBuilder.ToString();
    }

    public string BuildForLobby()
    {
        UpdateZones();
        _stringBuilder.Clear();
        _stringBuilder.AppendLine(Header);
        _stringBuilder.Append("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        _stringBuilder.AppendLine(GetZone(ScreenZone.Important));
        _stringBuilder.Append("\n\n");
        _stringBuilder.AppendLine(GetZone(ScreenZone.Environment));
        _stringBuilder.Append("\n\n\n\n\n\n\n");
        _stringBuilder.AppendLine("<size=20>join our discord");
        _stringBuilder.AppendLine(Discord);
        
        return _stringBuilder.ToString();
    }

    public string BuildForSpectator(CursedPlayer spectatedPlayer)
    {
        UpdateZones();
        _stringBuilder.Clear();
        _stringBuilder.AppendLine(Header);
        _stringBuilder.Append("<size=50%>Spectators: ");
        _stringBuilder.AppendLine(RespawnTimerModule.SpectatorCount);
        _stringBuilder.Append("<align=right>MTF Domination: ");
        _stringBuilder.AppendLine(RespawnTimerModule.MtfChance);
        _stringBuilder.Append("Chaos Domination: ");
        _stringBuilder.Append(RespawnTimerModule.ChaosChance);
        _stringBuilder.AppendLine("</align>");
        
        _stringBuilder.Append("<b><size=70>");
        _stringBuilder.Append(RespawnTimerModule.Timer);
        _stringBuilder.AppendLine("</size></b>");
        _stringBuilder.Append(RespawnTimerModule.Info);
        _stringBuilder.AppendLine("</size>");

        _stringBuilder.Append("\n\n\n\n\n");
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        _stringBuilder.Append("\n\n\n\n\n\n\n\n\n<size=40>");
        _stringBuilder.Append(GetZone(ScreenZone.Important));
        _stringBuilder.AppendLine("</size>\n");
        
        _stringBuilder.AppendLine(SpectatorFeedModule.GetContent(0));
        _stringBuilder.AppendLine(SpectatorFeedModule.GetContentWithAlpha(1, "<alpha=#80>"));
        _stringBuilder.AppendLine(SpectatorFeedModule.GetContentWithAlpha(2, "<alpha=#50>"));
        _stringBuilder.AppendLine(SpectatorFeedModule.GetContentWithAlpha(3, "<alpha=#30>"));
        _stringBuilder.AppendLine(SpectatorFeedModule.GetContentWithAlpha(4, "<alpha=#10>"));

        _stringBuilder.AppendLine("<alpha=#ff>");
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append(spectatedPlayer.GetSubclassName());
        _stringBuilder.AppendLine("</size>");
        _stringBuilder.AppendLine(spectatedPlayer.GetSubclassDescription());
        
        _stringBuilder.Append("\n<size=35>");
        _stringBuilder.AppendLine(spectatedPlayer.GetCustomItemName());
        _stringBuilder.Append("</size>");
        _stringBuilder.AppendLine(spectatedPlayer.GetCustomItemDescription());
        _stringBuilder.AppendLine();
        
        
        _stringBuilder.AppendLine(DiscordFooter);
        
        if (player.DoNotTrack)
        {
            _stringBuilder.AppendLine("<alpha=#40>DNT - Leveling Disabled");
        }
        else if (!spectatedPlayer.IsHost)
        {
            (int level,int exp, int total) = spectatedPlayer.GetLevelingProgress();
            _stringBuilder.Append("<alpha=#50>");
            _stringBuilder.Append(spectatedPlayer.DisplayNickname);
            _stringBuilder.Append(" - LEVEL: ");
            _stringBuilder.Append(level);
            _stringBuilder.Append(" - EXP: ");
            _stringBuilder.Append(exp);
            _stringBuilder.Append(" / ");
            _stringBuilder.Append(total);
        }
        
        return _stringBuilder.ToString();
    }

    public string BuildEndScreen()
    {
        UpdateZones();
        _stringBuilder.Clear();
        _stringBuilder.AppendLine(Header);
        _stringBuilder.AppendLine(Discord);
        _stringBuilder.Append("<size=35><b>\n");
        _stringBuilder.AppendLine(EndScreenModule.GetContent(0));
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine(EndScreenModule.GetContent(1));
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine(EndScreenModule.GetContent(2));
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine(EndScreenModule.GetContent(3));

        _stringBuilder.Append("\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n");
        
        _stringBuilder.AppendLine(Footer);
        
        return _stringBuilder.ToString();
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