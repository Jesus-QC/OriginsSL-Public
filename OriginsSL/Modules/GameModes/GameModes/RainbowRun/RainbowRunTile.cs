using System.Collections.Generic;
using CursedMod.Features.Wrappers.AdminToys;
using PluginAPI.Core;
using UnityEngine;

namespace OriginsSL.Modules.GameModes.GameModes.RainbowRun;

public class RainbowRunTile : MonoBehaviour
{
    public static readonly HashSet<RainbowRunTile> Collection = [];
    
    private CursedPrimitiveObject _primitive;
    private Vector3 _originalPosition;
    
    public Color TileColor;
    
    public void Init(CursedPrimitiveObject primitiveObject, Color color)
    {
        _primitive = primitiveObject;
        _originalPosition = _primitive.Position;
        _primitive.Color = TileColor = color;
        _primitive.MovementSmoothing = 60;
        Collection.Add(this);
        SendDown(Color.clear);
    }

    public void SendDown(Color color)
    {
        if (color == TileColor)
            return;
        
        _primitive.Color = Color.black;
        _primitive.Position = Vector3.zero;
    }
    
    public void SendUp()
    {
        _primitive.Color = TileColor;
        _primitive.Position = _originalPosition;
    }
}