using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;
using UnityEngine.AI;
using Photon.Pun;

public class Door : Interactablle
{

    public bool isClosed = true;
    public int priceToOpen = 2500;

    public EnemySpawnPoint[] enemySpawnPointsToEnable;
    public Door[] doorsToOpen;
    public Animator doorAnim;
    public Collider col;
    PhotonView PV;

    private void Start()
    {
        PV = GetComponent<PhotonView>();
    }

    public override string getDescription(PlayerGameData pgd)
    {
        if (isClosed) return "Press [E] To Open Door For " + priceToOpen.ToString() + " Gears";
        else return " ";
    }
    public override void Interact(PlayerGameData pgd)
    {
        if (pgd.currentPoints >= priceToOpen)
        {   
            pgd.currentPoints -= priceToOpen;
            PV.RPC("RPC_Open", RpcTarget.All);
        }
    }

    [PunRPC]
    public void RPC_Open() {

        if (isClosed)
        {
            isClosed = false;
            doorAnim.SetBool("DoorOpen", true);
            Destroy(col);
            Destroy(gameObject.GetComponent<NavMeshObstacle>());
            //JSAM.AudioManager.PlaySound(Sounds.DOOR);


            foreach (EnemySpawnPoint e in enemySpawnPointsToEnable)
            {
                e.isActive = true;
            }
            foreach (Door d in doorsToOpen)
            {
                d.PV.RPC("RPC_Open", RpcTarget.All);
            }
        }
    }
}
