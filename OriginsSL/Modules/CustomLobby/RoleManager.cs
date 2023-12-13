using System.Collections.Generic;
using PlayerRoles;

namespace OriginsSL.Modules.CustomLobby;

public static class RoleManager
{
    private static readonly Dictionary<Team, List<ReferenceHub>> SpawnDecisions = new()
    {
        [Team.SCPs] = [],
        [Team.ClassD] = [],
        [Team.Scientists] = [],
        [Team.FoundationForces] = [],
    };

    public static void Clear()
    {
        SpawnDecisions[Team.SCPs].Clear();
        SpawnDecisions[Team.ClassD].Clear();
        SpawnDecisions[Team.Scientists].Clear();
        SpawnDecisions[Team.FoundationForces].Clear();
    }
    
    public static List<ReferenceHub> GetTeam(Team team)
    {
        return SpawnDecisions[team];
    }

    public static void AddToQueue(ReferenceHub hub, Team role)
    {
        if(SpawnDecisions[role].Contains(hub))
            return;

        SpawnDecisions[role].Add(hub);
    }

    public static void RemoveFromQueue(ReferenceHub hub, Team role)
    {
        if(!SpawnDecisions[role].Contains(hub))
            return;

        SpawnDecisions[role].Remove(hub);
    }
}