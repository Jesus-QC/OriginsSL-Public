using System.Collections.Generic;
using System.Linq;
using System.Text;
using CursedMod.Features.Wrappers.Player;
using NorthwoodLib.Pools;

namespace OriginsSL.Features.Display;

public class CursedDisplayBuilder(CursedPlayer player)
{
    private const string Header = "<size=65%><line-height=87%><voffset=12.9em>";
    private const string Footer = "<size=55%><b><color=#E2E0A6>o</color><color=#D8D4AC>r</color><color=#CEC8B2>i</color><color=#C4BCB8>g</color><color=#BAB0BE>i</color><color=#B0A4C4>n</color><color=#A698CA>s</color>";

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
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append(GetZone(ScreenZone.Important));
        _stringBuilder.AppendLine("</size>\n");
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append("S<lowercase>ubclass</lowercase>");
        _stringBuilder.AppendLine("</size>");
        _stringBuilder.AppendLine("Subclass Description");
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine(Footer);
        
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
        
        _stringBuilder.AppendLine("SCP LIST");
        _stringBuilder.AppendLine("1");
        _stringBuilder.AppendLine("2");
        _stringBuilder.AppendLine("3");
        _stringBuilder.AppendLine("4");
        _stringBuilder.AppendLine("5");
        _stringBuilder.AppendLine("6");
        _stringBuilder.AppendLine("7");
        _stringBuilder.AppendLine("8</align></size>");
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Environment));
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine(RenderZone(ScreenZone.Center));
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append(GetZone(ScreenZone.Important));
        _stringBuilder.AppendLine("</size>\n");
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append("Subclass");
        _stringBuilder.AppendLine("</size>");
        _stringBuilder.AppendLine("Subclass Description");
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine(Footer);
        
        return _stringBuilder.ToString();
    }

    public string BuildForLobby()
    {
        UpdateZones();
        _stringBuilder.Clear();
        
        return _stringBuilder.ToString();
    }

    public string BuildForSpectator()
    {
        UpdateZones();
        _stringBuilder.Clear();
        _stringBuilder.AppendLine(Header);

        _stringBuilder.Append("<size=50%>spectators - ");
        _stringBuilder.Append(10);
        _stringBuilder.AppendLine("</size>");
        _stringBuilder.AppendLine();
        
        _stringBuilder.Append("<size=50><b>");
        _stringBuilder.Append("00:00");
        _stringBuilder.AppendLine("</b></size>");
        
        _stringBuilder.AppendLine("tip");

        for (byte i = 0; i < 29; i++)
            _stringBuilder.AppendLine();
        
        _stringBuilder.AppendLine("jonh killed wadaw");
        _stringBuilder.AppendLine("<alpha=#a0>jonh killed wadaw");
        _stringBuilder.AppendLine("<alpha=#80>jonh killed wadaw");
        _stringBuilder.AppendLine("<alpha=#50>jonh killed wadaw");
        _stringBuilder.AppendLine("<alpha=#10>jonh killed wadaw");
        _stringBuilder.AppendLine("<alpha=#ff>");
        _stringBuilder.Append("<size=40>");
        _stringBuilder.Append("S<lowercase>ubclass</lowercase>");
        _stringBuilder.AppendLine("</size>");
        _stringBuilder.AppendLine("Subclass Description");
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
        _stringBuilder.AppendLine();
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