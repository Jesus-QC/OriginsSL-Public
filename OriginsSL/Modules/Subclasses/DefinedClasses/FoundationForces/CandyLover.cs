using System.Collections.Generic;
using UnityEngine;

namespace OriginsSL.Modules.Subclasses.DefinedClasses.FoundationForces;

public class CandyLover : SubclassBase
{
    public override string CodeName => "candylover";
    public override string Name => "<color=#d51143>C<lowercase>andy</lowercase> L<lowercase>over</lowercase></color>";
    public override string Description => "loves candy, probably saved some in their pockets";
    public override float SpawnChance => 0.5f;
    public override bool KeepAfterEscaping => true;
    public override Vector3 FakeSize => new (1.7f, 1, 1.7f);
    public override List<ItemType> AdditiveInventory { get; } = [ItemType.SCP330, ItemType.SCP330, ItemType.SCP330];
}