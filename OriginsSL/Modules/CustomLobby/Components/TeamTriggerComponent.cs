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
        collider.size = new Vector3(1, 25f, 1);
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
        player.SendOriginsHint("<size=40><b><u><color=#ff0000>R</color><lowercase><color=#e87d44>a</color><color=#cad473>n</color><color=#9ce27a>d</color><color=#84e0a3>o</color><color=#76d8dd>m</color></lowercase></b></u></size></size>", ScreenZone.Environment);
    }

    private static string GetTeamName(Team team)
    {
        return team switch
        {
            Team.SCPs => "<color=#ee2835>SCPs</color>",
            Team.FoundationForces => "<color=#4a48d7>F</color><lowercase><color=#4d64dc>o</color><color=#4f7fe1>u</color><color=#529be6>n</color><color=#54b6eb>d</color><color=#58bce8>a</color><color=#5cc2e6>t</color><color=#5fc8e3>i</color><color=#63cee0>o</color><color=#5fc8e3>n</color></lowercase> <color=#58bce8>F</color><lowercase><color=#54b6eb>o</color><color=#529be6>r</color><color=#4f7fe1>c</color><color=#4d64dc>e</color><color=#4a48d7>s</color></lowercase>",
            Team.Scientists => "<color=#eeb663>S</color><lowercase><color=#e5ba65>c</color><color=#ddbe66>i</color><color=#d4c268>e</color><color=#cbc669>n</color><color=#c3c96b>t</color><color=#bacd6c>i</color><color=#b1d16e>s</color><color=#a9d56f>t</color><color=#a0d971>s</color></lowercase>",
            Team.ClassD => "<color=#e66236>C</color><lowercase><color=#e36c37>l</color><color=#df7638>a</color><color=#dc8139>s</color><color=#d88b3a>s</color></lowercase><color=#d5953b>D</color>",
            _ => string.Empty
        };
    }
}