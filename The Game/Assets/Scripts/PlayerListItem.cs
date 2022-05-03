using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Text text;
    [SerializeField] Image circle;
    Photon.Realtime.Player player;

    public void SetUp(Photon.Realtime.Player _player)
    {
        player = _player;
        text.text = _player.NickName;
        circle.color = Random.ColorHSV();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (player == otherPlayer)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
