using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameOver : MonoBehaviour
{
    public PhotonView PV;
    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Invoke("Restart", 5f);
    }

    void Restart()
    {
        PV.RPC("RPC_Restart", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Restart()
    {
        PhotonNetwork.LoadLevel(1);
    }
}
