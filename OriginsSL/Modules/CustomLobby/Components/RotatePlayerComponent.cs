using CursedMod.Features.Wrappers.Player;
using UnityEngine;

namespace OriginsSL.Modules.CustomLobby.Components;

public class RotatePlayerComponent : MonoBehaviour
{
    private CursedPlayer _dummy;

    public void Init(CursedPlayer player) => _dummy = player;

    private void Start()
    {
        _dummy.VerticalRotation = -30;
    }

    private void Update()
    {
        _dummy.HorizontalRotation += 20 * Time.deltaTime;
    }
}