using System.Collections.Generic;
using CursedMod.Features.Wrappers.Player;
using MEC;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.ClassD;

public class MsSweetieSubclass : SubclassBase
{
    public override string CodeName => "mssweetie";
    public override string Name => "<color=#ffabff>M<lowercase>s</lowercase> S<lowercase>weetie</lowercase></color>";
    public override string Description => "you cook candy each minute";
    public override float SpawnChance => 0.4f;
    public override bool KeepAfterEscaping => true;
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.SCP330, ItemType.SCP330];

    private CoroutineHandle _candyCoroutine;
    
    public override void OnSpawn(CursedPlayer player)
    {
        _candyCoroutine = RunCoroutine(CandyCoroutine(player), player);
        base.OnSpawn(player);
    }

    public override void OnDestroy(CursedPlayer player)
    {
        KillCoroutine(_candyCoroutine);
        base.OnDestroy(player);
    }

    private static IEnumerator<float> CandyCoroutine(CursedPlayer player)
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(55);
            player.SendOriginsHint("Y<lowercase>ou have received candy!</lowercase>", ScreenZone.Environment);
            player.AddItem(ItemType.SCP330);
        }
    }
}
