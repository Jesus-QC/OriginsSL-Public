using System;
using System.Collections.Generic;
using System.Linq;
using CursedMod.Features.Wrappers.AdminToys;
using CursedMod.Features.Wrappers.Player;
using OriginsSL.Features.Display;
using OriginsSL.Modules.DisplayRenderer;
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
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(95, 1024.25f, -35), new Vector3(6, 5, 32), spawn: true);
        
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(67, 1023, -35), new Vector3(150, 0.05f, 32), color: Color.red, spawn: true);
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(67, 1033, -35), new Vector3(150, 0.05f, 32), spawn: true);
        
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(57, 1028, -51), new Vector3(150, 10, 0.01f), spawn: true);
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(57, 1028, -19), new Vector3(150, 10, 0.01f), spawn: true);
        
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(-8, 1028, -35), new Vector3(0.1f, 10, 32), spawn: true);
        CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(95, 1028, -35), new Vector3(0.1f, 10, 32), spawn: true);
        
        for (int i = 0; i < 3; i++)
        {
            CursedLightSource.Create(new Vector3(92,1029,-45 + 10 * i), lightShadows: false, lightIntensity: 600, lightRange: 50, spawn: true);
            CursedLightSource.Create(new Vector3(67,1029,-45 + 10 * i), lightShadows: false, lightIntensity: 600, lightRange: 50, spawn: true);
            CursedLightSource.Create(new Vector3(42,1029,-45 + 10 * i), lightShadows: false, lightIntensity: 600, lightRange: 50, spawn: true);
            CursedLightSource.Create(new Vector3(17,1029,-45 + 10 * i), lightShadows: false, lightIntensity: 600, lightRange: 50, spawn: true);
            CursedLightSource.Create(new Vector3(-3,1029,-45 + 10 * i), lightShadows: false, lightIntensity: 600, lightRange: 50, spawn: true);
        }
        
        // Stage 1
        for (float x = 0; x <= 38.5f; x += 3.5f)
        for (float z = -21; z >= -49; z -= 3.5f)
        {
            Color color = GetRandomColor();
            CursedPrimitiveObject primitiveObject = CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(x, 1026, z), new Vector3(3, 0.2f, 3), spawn: true);
            
            primitiveObject.GameObject.AddComponent<RainbowRunTile>().Init(primitiveObject, color);
        }
        
        // Stage 2
        for (float x = 50; x <= 88.5f; x += 3.5f)
        for (float z = -21; z >= -49; z -= 3.5f)
        {
            if (Random.value < 0.33f)
                continue;
            
            Color color = GetRandomColor();
            CursedPrimitiveObject primitiveObject = CursedPrimitiveObject.Create(PrimitiveType.Cube, new Vector3(x, 1026, z), new Vector3(3, 0.2f, 3), spawn: true);
            
            primitiveObject.GameObject.AddComponent<RainbowRunTile>().Init(primitiveObject, color);
        }
    }

    public override void StartGameMode()
    {
        SpawnMap();
        base.StartGameMode();
    }

    public override void StopGameMode()
    {
        RainbowRunTile.Collection.Clear();
        base.StopGameMode();
    }
    
    private bool _waiting = true;
    private float _counter = 7;
    private Color _color;
    
    public override void OnUpdate()
    {
        base.OnUpdate();
        
        _counter -= Time.deltaTime;

        foreach (CursedPlayer player in CursedPlayer.Collection)
        {
            if (_waiting)
            {
                player.SendOriginsHint($"<size=60><b><color={_color.ToHex()}>{_definedColors[_color]}</color> - {_counter}s</b>", ScreenZone.Important);
            }
            else
            {
                player.SendOriginsHint(
                    $"<size=60>{_counter}s</size>", ScreenZone.Important);
            }
        }
            
        
        if (_counter > 0)
            return;
        
        if (_waiting)
        {
            _color = GetRandomColor();
            _counter = 3;
            _waiting = false;
            
            foreach (RainbowRunTile tile in RainbowRunTile.Collection) 
                tile.SendUp();
            return;
        }
        
        foreach (RainbowRunTile tile in RainbowRunTile.Collection) 
            tile.SendDown(_color);
        
        _counter = 5;
        _waiting = true;
    }

    private Color GetRandomColor() => _definedColors.Keys.ToList().RandomItem();

    private readonly Dictionary<Color, string> _definedColors = new ()
    {
        [Color.red] = "red",
        [Color.green] = "green",
        [Color.yellow] = "yellow",
        [Color.cyan] = "cyan",
        [Color.magenta] = "magenta",
        [Color.black] = "black",
        [Color.white] = "white",
        [Color.blue] = "blue",
        [Color.gray] = "gray",
    };
}