using CursedMod.Events.Arguments.Items;
using CursedMod.Events.Handlers;
using CursedMod.Features.Wrappers.Facility;
using CursedMod.Features.Wrappers.Player.Roles;
using CursedMod.Features.Wrappers.Round;
using CustomPlayerEffects;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
using PlayerRoles;
using UnityEngine;

namespace OriginsSL.Modules.CustomItems.Items.Painkillers;

public class SpecialPills : CustomItemBase
{
    public override string CodeName => "specialpills";
    
    public override string Name => "<color=#edce85>S<lowercase>pecial</lowercase> P<lowercase>ills</lowercase></color>";
    
    public override string Description => "they are pretty much special";

    public override float SpawnChance => 0.25f;

    public class SpecialPillsHandler : ICustomItemEventsHandler
    {
        public void OnLoaded()
        {
            CursedItemsEventsHandler.PlayerUsedItem += OnPlayerUsedItem;
        }
        
        private static void OnPlayerUsedItem(PlayerUsedItemEventArgs args)
        {
            if (!CursedRound.HasStarted)
                return;
                
            if (!CustomItemManager.TryGetCustomItem(args.Item.Serial, out ICustomItem item) || item is not SpecialPills)
                return;
            
            switch (Random.value)
            {
                case < 0.1f:
                    args.Player.SendOriginsHint("Y<lowercase>ou got a random effect!</lowercase>", ScreenZone.Environment);
                    args.Player.ReferenceHub.playerEffectsController.EnableEffect<Ghostly>(20);
                    break;
                case < 0.25f:
                    args.Player.SendOriginsHint("Y<lowercase>ou got a random effect!</lowercase>", ScreenZone.Environment);
                    args.Player.ReferenceHub.playerEffectsController.EnableEffect<Scp207>();
                    break;
                case < 0.5f:
                    args.Player.SendOriginsHint("Y<lowercase>ou got a random effect!</lowercase>", ScreenZone.Environment);
                    args.Player.ReferenceHub.playerEffectsController.EnableEffect<Invisible>(30);
                    break;
                default:
                    args.Player.Position = CursedRoleManager.GetRoleSpawnPosition(CursedWarhead.Detonated ? RoleTypeId.ChaosConscript : CursedDecontamination.IsDecontaminating ? RoleTypeId.FacilityGuard : RoleTypeId.Scientist);
                    break;
            }
        }
    }
}