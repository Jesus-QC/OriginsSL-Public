using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using CursedMod.Events;
using HarmonyLib;
using PlayerRoles;
using PlayerRoles.RoleAssign;
using PluginAPI.Core;

namespace OriginsSL.Modules.CustomLobby.Patches;

[HarmonyPatch(typeof(HumanSpawner), nameof(HumanSpawner.SpawnHumans))]
public class SpawnHumansPatch
{
    private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        CursedEventManager.CheckEvent<SpawnHumansPatch>(46, instructions);
        yield return new CodeInstruction(OpCodes.Ldarg_0);
        yield return new CodeInstruction(OpCodes.Ldarg_1);
        yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(SpawnHumansPatch), nameof(SpawnHumans)));
        yield return new CodeInstruction(OpCodes.Ret);
    }

    private static void SpawnHumans(Team[] queue, int queueLength)
    {
        HumanSpawner._humanQueue = queue;
        HumanSpawner._queueClock = 0;
        HumanSpawner._queueLength = queueLength;
        
        List<ReferenceHub> classD = RoleManager.GetTeam(Team.ClassD);
        List<ReferenceHub> scientist = RoleManager.GetTeam(Team.Scientists);
        List<ReferenceHub> guard = RoleManager.GetTeam(Team.FoundationForces);
        List<ReferenceHub> random = [];
        
        foreach (ReferenceHub hub in ReferenceHub.AllHubs)
        {
            if (classD.Contains(hub))
            {
                if (hub.gameObject == null || !RoleAssigner.CheckPlayer(hub))
                    classD.Remove(hub);
                
                continue;
            }
            
            if (scientist.Contains(hub))
            {
                if (hub.gameObject == null || !RoleAssigner.CheckPlayer(hub))
                    scientist.Remove(hub);
                
                continue;
            }
            
            if (guard.Contains(hub))
            {
                if (hub.gameObject == null || !RoleAssigner.CheckPlayer(hub))
                    guard.Remove(hub);
                
                continue;
            }
            
            if (!RoleAssigner.CheckPlayer(hub))
                continue;
            
            random.Add(hub);
        }
        
        while (classD.Count != 0 || scientist.Count != 0 || guard.Count != 0 || random.Count != 0)
        {
            Team key = HumanSpawner._humanQueue[HumanSpawner._queueClock++ % HumanSpawner._queueLength];

            switch (key)
            {
                case Team.ClassD:
                {
                    if (classD.Count != 0)
                    {
                        SetRole(classD.PullRandomItem().roleManager, RoleTypeId.ClassD);
                        break;
                    }

                    if (random.Count != 0)
                    {
                        SetRole(random.PullRandomItem().roleManager, RoleTypeId.ClassD);
                        break;
                    }

                    if (scientist.Count != 0)
                    {
                        SetRole(scientist.PullRandomItem().roleManager, RoleTypeId.ClassD);
                        break;
                    }

                    SetRole(guard.PullRandomItem().roleManager, RoleTypeId.ClassD);
                    break;
                }

                case Team.Scientists:
                {
                    if (scientist.Count != 0)
                    {
                        SetRole(scientist.PullRandomItem().roleManager, RoleTypeId.Scientist);
                        break;
                    }

                    if (random.Count != 0)
                    {
                        SetRole(random.PullRandomItem().roleManager, RoleTypeId.Scientist);
                        break;
                    }

                    if (guard.Count != 0)
                    {
                        SetRole(guard.PullRandomItem().roleManager, RoleTypeId.Scientist);
                        break;
                    }

                    SetRole(classD.PullRandomItem().roleManager, RoleTypeId.Scientist);
                    break;
                }

                case Team.SCPs:
                case Team.FoundationForces:
                case Team.ChaosInsurgency:
                case Team.Dead:
                case Team.OtherAlive:
                default:
                {
                    if (guard.Count != 0)
                    {
                        SetRole(guard.PullRandomItem().roleManager, RoleTypeId.FacilityGuard);
                        break;
                    }

                    if (random.Count != 0)
                    {
                        SetRole(random.PullRandomItem().roleManager, RoleTypeId.FacilityGuard);
                        break;
                    }

                    if (scientist.Count != 0)
                    {
                        SetRole(scientist.PullRandomItem().roleManager, RoleTypeId.FacilityGuard);
                        break;
                    }

                    SetRole(classD.PullRandomItem().roleManager, RoleTypeId.FacilityGuard);
                    break;
                }
            }
        }

        RoleManager.Clear();
    }

    private static void SetRole(PlayerRoleManager roleManager, RoleTypeId roleTypeId)
    {
        try
        {
            roleManager.ServerSetRole(roleTypeId, RoleChangeReason.RoundStart);
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
}
