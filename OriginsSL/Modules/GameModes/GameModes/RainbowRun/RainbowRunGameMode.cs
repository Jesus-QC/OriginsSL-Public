using System;
using System.Collections.Generic;
using CursedMod.Features.Wrappers.AdminToys;
using OriginsSL.Modules.GameModes.Misc.GameModeComponents;
using PlayerRoles;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OriginsSL.Modules.GameModes.GameModes.RainbowRun;

public class RainbowRunGameMode : CursedGameModeBase
{
    public override string CodeName => "rainbowrun";
    public override string Name => "R<lowercase>ainbow</lowercase> R<lowercase>un</lowercase>";
    public override string Description => "Run through the colors and pass the finish line!";

    protected override GameModeComponent[] Components { get; } =
    [
        new GameModeMusicComponent("Fun.ogg"),
        new GameModeMaxTimeComponent(TimeSpan.FromMinutes(3)),
        new GameModeSpawnerComponent(RoleTypeId.ClassD, 
        [
            new Vector3(-5, 1027f, -50),
            new Vector3(-5, 1027f, -45),
            new Vector3(-5, 1027f, -40),
            new Vector3(-5, 1027f, -35),
            new Vector3(-5, 1027f, -30),
            new Vector3(-5, 1027f, -25),
            new Vector3(-5, 1027f, -23),
        ])
    ];

    private void SpawnMap()
    {
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(-5, 1024.25f, -35), new Vector3(6, 5, 32), spawn: true);
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(45, 1024.25f, -35), new Vector3(6, 5, 32), spawn: true);
        
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(20, 1023, -35), new Vector3(57, 0.05f, 32), color: Color.red, spawn: true);
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(20, 1033, -35), new Vector3(57, 0.05f, 32), spawn: true);
        
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(20, 1028, -51), new Vector3(57, 10, 0.01f), spawn: true);
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(20, 1028, -19), new Vector3(57, 10, 0.01f), spawn: true);
        
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(-8, 1028, -35), new Vector3(0.1f, 10, 32), spawn: true);
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(48, 1028, -35), new Vector3(0.1f, 10, 32), spawn: true);
        
        for (int i = 0; i < 3; i++)
        {
            CursedLightSource.Create(new Vector3(42,1029,-45 + 10 * i), lightIntensity: 600, lightRange: 50, spawn: true);
            CursedLightSource.Create(new Vector3(17,1029,-45 + 10 * i), lightIntensity: 600, lightRange: 50, spawn: true);
            CursedLightSource.Create(new Vector3(-3,1029,-45 + 10 * i), lightIntensity: 600, lightRange: 50, spawn: true);
        }
        
        for (float i = -21; i <= -49; i += 3.5f)
        for (float j = 0; j <= 38.5f; j += 3.5f)
            CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(j, 1026, i), new Vector3(3, 0.2f, 3), color: Random.ColorHSV(), spawn: true);
    }

    public override void StartGameMode()
    {
        SpawnMap();
        base.StartGameMode();
    }
}