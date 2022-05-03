using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;
using Photon.Pun;

public class Feddy : Interactablle
{
    public bool isActivated = false;
    public GameObject feddysound, feddylight;
    PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public override string getDescription(PlayerGameData pgd)
    {
        if (isActivated) return " ";
        else return " ";
    }
    public override void Interact(PlayerGameData pgd)
    {
        PV.RPC("RPC_Interact", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_Interact()
    {
        isActivated = true;
        feddysound.SetActive(true);
        feddylight.SetActive(true);
    }

}
