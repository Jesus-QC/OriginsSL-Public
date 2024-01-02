using System.Collections.Generic;
using CursedMod.Events.Arguments.Player;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Player.Dummies;
using CursedMod.Features.Wrappers.Player.Ragdolls;
using CursedMod.Features.Wrappers.Player.Roles.SCPs;
using MEC;
using OriginsSL.Loader;
using OriginsSL.Modules.Emote.Components;
using PlayerRoles;
using PlayerRoles.PlayableScps.Scp3114;
using UnityEngine;

namespace OriginsSL.Modules.Emote;

public class EmoteHandler : OriginsModule
{
    public override void OnLoaded()
    {
        CursedPlayerEventsHandler.DummyReceivingDamage += OnDummyReceivingDamage;
    }

    private static void OnDummyReceivingDamage(PlayerReceivingDamageEventArgs args)
    {
        foreach (EmoteDummyOwner emoteDummyOwner in EmoteDummyOwner.PlayersEmoting.Values)
        {
            if (emoteDummyOwner.Dummy != args.Player)
                continue;
            
            emoteDummyOwner.StopEmoting();
            emoteDummyOwner.Owner.Damage(args.DamageHandlerBase);
        }
    }
    
    public static void Dance(CursedPlayer player)
    {
        if (EmoteDummyOwner.PlayersEmoting.ContainsKey(player))
            return;
        
        Timing.RunCoroutine(SkeletonDance(player));
    }

    private static IEnumerator<float> SkeletonDance(CursedPlayer player)
    {
        CursedPlayer dummy = CursedDummy.Create("Dance (" + player.DisplayNickname + ")");
        dummy.Role = RoleTypeId.Scp3114;
        dummy.Position = player.Position;
        
        CursedRagdoll ragdoll = CursedRagdoll.Create(player.Role, "Dancing", Vector3.zero, Vector3.zero);
        
        player.AddComponent<EmoteDummyOwner>().Init(player, dummy, ragdoll);
        
        yield return Timing.WaitForOneFrame;
        yield return Timing.WaitForOneFrame;
        
        if (dummy.CurrentRole is not CursedScp3114Role scp3114Role)
            yield break;
        
        scp3114Role.DisguiseAsRagdoll(ragdoll);
        
        scp3114Role.Identity.CurIdentity.Status = Scp3114Identity.DisguiseStatus.Equipping;
        scp3114Role.Identity.ServerResendIdentity();
        
        yield return Timing.WaitForOneFrame;
        
        scp3114Role.TriggerDance();
    }
}