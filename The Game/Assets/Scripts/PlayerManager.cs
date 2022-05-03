using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    PhotonView PV;
    //List<PlayerSpawnPoint> playerSpawns;
    PlayerSpawnPoint[] spawns;
    int chooseRand;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
    }
    // Start is called before the first frame update
    void Start()
    {
        spawns = FindObjectsOfType<PlayerSpawnPoint>();
        foreach (PlayerSpawnPoint spawn in spawns)
        {
            spawn.gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        if (PV.IsMine)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        chooseRand = Random.Range(0, spawns.Length - 1);
        GameObject bobject = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"), spawns[chooseRand].gameObject.transform.position, spawns[chooseRand].gameObject.transform.rotation);
        PV.RPC("RPC_DestroySpawn", RpcTarget.All);
        bobject.GetComponent<PlayerGameData>().setData(GameManager._instance, PhotonNetwork.CurrentRoom.PlayerCount, 1);
    }

    [PunRPC]
    public void RPC_DestroySpawn()
    {
        Destroy(spawns[chooseRand].gameObject);
    }
}
