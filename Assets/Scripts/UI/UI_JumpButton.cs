using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_JumpButton : MonoBehaviour, IPointerDownHandler
{
    private Player player;

    public void OnPointerDown(PointerEventData eventData)
    {
        player.JumpButton();
    }

    public void UpdatePlayersRef(Player newPlayer) => player = newPlayer;
}
