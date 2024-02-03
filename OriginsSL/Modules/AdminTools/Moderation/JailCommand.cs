using System;
using System.Collections.Generic;
using CommandSystem;
using CursedMod.Features.Wrappers.Inventory.Items;
using CursedMod.Features.Wrappers.Player;
using CursedMod.Features.Wrappers.Round;
using OriginsSL.Features.Commands;
using PlayerRoles;
using UnityEngine;
using NWAPIPermissionSystem;
using OriginsSL.Modules.Subclasses;

namespace OriginsSL.Modules.AdminTools.Moderation;

[CommandHandler(typeof(RemoteAdminCommandHandler))]
public class JailCommand : ICommand, IUsageProvider
{
    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        CursedPlayer ply = CursedPlayer.Get(sender);
        
        if (!sender.CheckPermission("origins.moderation.jail") && !ply.IsHost)
        {
            response = "Not enough perms.";
            return false;
        }
        
        if (CursedRound.IsInLobby)
        {
            response = "Not in lobby";
            return false;
        }
        
        if (arguments.Count == 0)
        {
            if (JailedPlayers.TryGetValue(ply, out JailInfo jailInfo))
            {
                jailInfo.UnJail(ply);
                response = "You have been released from jail.";
                return true;
            }
   
            JailInfo.Jail(ply);

            response = "Jailed yourself.";
            return true;
        }
        
        List<CursedPlayer> players = CursedCommandUtils.GetPlayers(arguments.At(0));

        if (players.Count is 0)
        {
            response = "Players not found.";
            return false;
        }

        foreach (CursedPlayer player in players)
        {
            if (JailedPlayers.TryGetValue(player, out JailInfo jailInfo))
            {
                jailInfo.UnJail(player);
                continue;
            }
   
            JailInfo.Jail(player);
        }

        response = $"Jailed/Released {players.Count} players";
        return true;
    }

    public struct JailInfo
    {
        public static void Jail(CursedPlayer player)
        {
            JailedPlayers.Add(player, new JailInfo(player));
        }
        
        private JailInfo(CursedPlayer player)
        {
            _items = player.ClearItemsWithoutDestroying();
            _health = player.HealthStat.CurValue;
            _humeShield = player.HumeShieldStat.CurValue;
            _position = player.Position;
            _role = player.Role;
            _ammo = new Dictionary<ItemType, ushort>(player.Ammo);
            
            if (player.TryGetSubclass(out _subclass))
                _subclass.IsLocked = true;
            
            player.SetRole(RoleTypeId.Tutorial);
            player.ForceSavedSubclass(null);
            player.ShowBroadcast("<b>You have been detained by an administrator, please follow their instructions.</b>");
        }

        public void UnJail(CursedPlayer player)
        {
            player.SetItems(_items);
            player.SetAmmo(_ammo);
            player.Position = _position;
            player.SetRole(_role, RoleChangeReason.RemoteAdmin, RoleSpawnFlags.None);
            player.HealthStat.CurValue = _health;
            player.HumeShieldStat.CurValue = _humeShield;
            JailedPlayers.Remove(player);
            
            if (_subclass is null)
                return;

            player.ForceSavedSubclass(_subclass);
            _subclass.IsLocked = false;
        }

        private readonly float _health;
        private readonly float _humeShield;
        private readonly Vector3 _position;
        private readonly RoleTypeId _role;
        private readonly IEnumerable<CursedItem> _items;
        private readonly Dictionary<ItemType, ushort> _ammo;
        private readonly SubclassBase _subclass;
    }

    private static readonly Dictionary<CursedPlayer, JailInfo> JailedPlayers = new ();

    public string Command { get; } = "ogjail";
    public string[] Aliases { get; } = Array.Empty<string>();
    public string Description { get; } = "Manages the jail for players.";
    public string[] Usage { get; } = {"%player%"};
}