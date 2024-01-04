using System;
using CursedMod.Events.Arguments.Items;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Facility;
using CursedMod.Features.Wrappers.Player.Roles;
using CustomPlayerEffects;
using InventorySystem.Items.Usables.Scp330;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles;
using Utils;
using Random = UnityEngine.Random;

namespace OriginsSL.Modules.CustomItems.Items.Coins;

public class SpecialCoin : CustomItemBase
{
    public override string CodeName => "specialcoin";
    
    public override string Name => "<color=#edce85>S<lowercase>pecial</lowercase> C<lowercase>oin</lowercase></color>";
    
    public override string Description => "its power is unlimited, you should try to flip it";

    public override float SpawnChance => 0.5f;

    public class SpecialCoinHandler : ICustomItemEventsHandler
    {
        public void OnLoaded()
        {
            CursedItemsEventsHandler.PlayerFlippingCoin += OnPlayerFlippingCoin;
        }
        
        private static void OnPlayerFlippingCoin(PlayerFlippingCoinEventArgs args)
        {
            if (!args.Player.TryGetCurrentCustomItem(out ICustomItem item) || item is not SpecialCoin)
                return;

            args.Player.RemoveItem(args.Item);
            
            switch (Random.value)
            {
                case < 0.1f:
                    ExplosionUtils.ServerExplode(args.Player.ReferenceHub);
                    break;
                case < 0.25f:
                    args.Player.SendOriginsHint("Y<lowercase>ou used the special coin and got a random effect!</lowercase>", ScreenZone.Environment);
                    args.Player.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
                    break;
                case < 0.5f:
                    args.Player.SendOriginsHint("Y<lowercase>ou used the special coin and got a random candy!</lowercase>", ScreenZone.Environment);
                    args.Player.AddCandy(Scp330Candies.GetRandom());
                    break;
                default:
                    args.Player.SendOriginsHint("Y<lowercase>ou used the special coin!</lowercase>", ScreenZone.Environment);
                    args.Player.Position = CursedRoleManager.GetRoleSpawnPosition(CursedWarhead.Detonated ? RoleTypeId.ChaosConscript : CursedDecontamination.IsDecontaminating ? RoleTypeId.FacilityGuard : RoleTypeId.Scientist);
                    break;
            }
        }
    }
}