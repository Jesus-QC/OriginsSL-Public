using CursedMod.Features.Wrappers.Player;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.CustomLobby.Components;

public class TeamTriggerComponent : MonoBehaviour
{
    private Team _team;

    public GameObject Init(Team team)
    {
        _team = team;
        return gameObject;
    }
    
    private void Start()
    {
        BoxCollider collider = gameObject.AddComponent<BoxCollider>();
        collider.isTrigger = true;
        collider.center = new Vector3(0, 12.5f, 0);
        collider.size = new Vector3(.8f, 25f, .8f);
    }

    private void OnTriggerEnter(Collider other)
    {
        CursedPlayer player = CursedPlayer.Get(other.gameObject);
        
        if (player is null || player.IsDummy)
            return;

        RoleManager.AddToQueue(player.ReferenceHub, _team);
        player.SendOriginsHint("SELECTED", ScreenZone.Important);
        player.SendOriginsHint($"<size=40><b><u>{GetTeamName(_team)}</u></b></size></size>", ScreenZone.Environment);
    }

    private void OnTriggerExit(Collider other)
    {
        CursedPlayer player = CursedPlayer.Get(other.gameObject);
        
        if (player is null || player.IsDummy)
            return;

        RoleManager.RemoveFromQueue(player.ReferenceHub, _team);
        player.SendOriginsHint("SELECTED", ScreenZone.Important);
        player.SendOriginsHint("<size=40><b><u><color=#B7A2D7>R</color><lowercase><color=#BFACD0>a</color><color=#C7B6C9>n</color><color=#CFC0C2>d</color><color=#D7CABB>o</color><color=#DFD4B4>m</color></lowercase></b></u></size></size>", ScreenZone.Environment);
    }

    private static string GetTeamName(Team team)
    {
        return team switch
        {
            Team.SCPs => "<color=#FB178E>S</color><color=#F3136D>C</color><color=#EB0F4C>P</color><lowercase><color=#E30B2B>s</color></lowercase>",
            Team.FoundationForces => "<color=#37DDEC>F</color><lowercase><color=#3ED2EC>o</color><color=#45C7EC>u</color><color=#4CBCEC>n</color><color=#53B1EC>d</color><color=#5AA6EC>a</color><color=#619BEC>t</color><color=#6890EC>i</color><color=#6F85EC>o</color><color=#767AEC>n</color></lowercase> <color=#8464EC>F</color><lowercase><color=#8B59EC>o</color><color=#924EEC>r</color><color=#9943EC>c</color><color=#A038EC>e</color><color=#A72DEC>s</color></lowercase>",
            Team.Scientists => "<color=#F4E06D>S</color><lowercase><color=#F2DB6C>c</color><color=#F0D66B>i</color><color=#EED16A>e</color><color=#ECCC69>n</color><color=#EAC768>t</color><color=#E8C267>i</color><color=#E6BD66>s</color><color=#E4B865>t</color><color=#E2B364>s</color></lowercase>",
            Team.ClassD => "<color=#FF8E00>C</color><lowercase><color=#FB7B09>l</color><color=#F76812>a</color><color=#F3551B>s</color><color=#EF4224>s</color></lowercase><color=#EB2F2D>D</color>",
            _ => string.Empty
        };
    }
}