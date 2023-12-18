// -----------------------------------------------------------------------
// <copyright file="CursedScp3114Role.cs" company="CursedMod">
// Copyright (c) CursedMod. All rights reserved.
// Licensed under the GPLv3 license.
// See LICENSE file in the project root for full license information.
// </copyright>
// -----------------------------------------------------------------------

using CursedMod.Features.Wrappers.Player.Ragdolls;
using PlayerRoles.PlayableScps.HumeShield;
using PlayerRoles.PlayableScps.Scp049.Zombies;
using PlayerRoles.PlayableScps.Scp3114;
using PlayerRoles.Ragdolls;
using PlayerRoles.Subroutines;
using PlayerStatsSystem;
using RelativePositioning;

namespace CursedMod.Features.Wrappers.Player.Roles.SCPs;

public class CursedScp3114Role : CursedFpcRole
{
    internal CursedScp3114Role(Scp3114Role roleBase)
        : base(roleBase)
    {
        ScpRoleBase = roleBase;

        if (SubroutineModule.TryGetSubroutine(out Scp3114Dance dance))
            Dance = dance;
        if (SubroutineModule.TryGetSubroutine(out Scp3114FakeModelManager fakeModelManager))
            FakeModelManager = fakeModelManager;
        if (SubroutineModule.TryGetSubroutine(out Scp3114History history))
            History = history;
        if (SubroutineModule.TryGetSubroutine(out Scp3114Identity identity))
            Identity = identity;
        if (SubroutineModule.TryGetSubroutine(out Scp3114Indicators indicators))
            Indicators = indicators;
        if (SubroutineModule.TryGetSubroutine(out Scp3114Reveal reveal))
            Reveal = reveal;
        if (SubroutineModule.TryGetSubroutine(out Scp3114Slap slap))
            Slap = slap;
        if (SubroutineModule.TryGetSubroutine(out Scp3114Strangle strangle))
            Strangle = strangle;
        if (SubroutineModule.TryGetSubroutine(out Scp3114StrangleAudio strangleAudio))
            StrangleAudio = strangleAudio;
        if (SubroutineModule.TryGetSubroutine(out Scp3114VoiceLines voiceLines))
            VoiceLines = voiceLines;
        if (SubroutineModule.TryGetSubroutine(out Scp3114Disguise disguise))
            Disguise = disguise;
        if (SubroutineModule.TryGetSubroutine(out Scp3114RagdollToBonesConverter ragdollToBonesConverter))
            RagdollToBonesConverter = ragdollToBonesConverter;
    }

    public Scp3114Role ScpRoleBase { get; }
    
    public Scp3114Dance Dance { get; }
    
    public Scp3114FakeModelManager FakeModelManager { get; }
    
    public Scp3114History History { get; }
    
    public Scp3114Identity Identity { get; }
    
    public Scp3114Indicators Indicators { get; }
    
    public Scp3114Reveal Reveal { get; }
    
    public Scp3114Slap Slap { get; }
    
    public Scp3114Strangle Strangle { get; }
    
    public Scp3114StrangleAudio StrangleAudio { get; }
    
    public Scp3114VoiceLines VoiceLines { get; }
    
    public Scp3114Disguise Disguise { get; }
    
    public Scp3114RagdollToBonesConverter RagdollToBonesConverter { get; }

    public HumeShieldModuleBase HumeShieldModule
    {
        get => ScpRoleBase.HumeShieldModule;
        set => ScpRoleBase.HumeShieldModule = value;
    }

    public SubroutineManagerModule SubroutineModule
    {
        get => ScpRoleBase.SubroutineModule;
        set => ScpRoleBase.SubroutineModule = value;
    }
    
    public void TriggerDance()
    {
        Dance.IsDancing = true;
        Dance._serverStartPos = new RelativePosition(Dance.CastRole.FpcModule.Position);
        Dance.ServerSendRpc(true);
    }

    public void DisguiseAsRagdoll(CursedRagdoll ragdoll)
    {
        Disguise.CurRagdoll = ragdoll.Base;
        Disguise.OnProgressSet();
        Disguise._completionTime = 10;
        Disguise.OnProgressSet();
        Disguise.ServerComplete();
    }
}