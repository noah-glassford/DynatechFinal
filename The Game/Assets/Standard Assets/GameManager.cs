using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    //Players Data
    [Header("Player Data")]
    public GameObject player;
    public List<PlayerSpawnPoint> playerSpawners;
    [Range(1,4)]public int playerCount = 1;

    private GameObject[] players;
    private PlayerGameData[] pDatas;

    [Header("Gun Data")]
    public List<GameObject> guns;

    //Enemies Data
    [Header("Enemy Data")]
    public GameObject humanEnemy, dogEnemy;
    public List<EnemySpawnPoint> enemySpawners = new List<EnemySpawnPoint>();

    private float minSpeed = 3.5f;
    private float maxSpeed = 5f;

    public int maxEnemiesSpawnedAtOnce = 28;

    private float timeBetweenSpawns = 2.5f;
    private float timeBetweenRounds = 10f;

    [Header("Enemy Wave Debug Data")]
    public int enemiesToSpawn;
    public int enemiesAlive;
    public int enemiesHealth;

    //Rounds Data
    [Header("Round Data")]
    [Range(1,10)]public int startingRound = 1;

    public int currentRound;

    //Games Data
    [Header("Game Data")]
    public bool customDifficulty;
    [Range(1, 4)] public int customDifficultyLevel;

    private int dificulty;

     public static GameManager _instance;

    /*
    
    Game Manager Tasks

    -  
    - Get information to and from all players in the game
    - Get infomation to and from all enemies in the game
    - Get information on the current state of the game
    - 

     */

    private void Awake()
    {
        //singleton
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        InitializeGame();
        StartGameSetup();
    }

    private void Start()
    {
        playerCount = PhotonNetwork.PlayerList.Length;
        BeginRound();
    }

    public void Update()
    {
        timeBetweenSpawns -= Time.deltaTime;
        if (enemiesToSpawn == 0 && enemiesAlive == 0)
            BeginRound();
        else if (timeBetweenSpawns <= 0)
        {
            float t = Random.Range(1.0f, 2.5f);
            timeBetweenSpawns = t;
            Debug.Log("Time Between Spawns: " + t);
            SpawnEnemiesIntoRound();
        }
        if (enemiesAlive < 0) {
            enemiesAlive = 0;
        }
    }

    public void enemyDeath() {
        enemiesAlive--;
    }

    private void InitializeGame() {

        //Locks cursor
        Cursor.lockState = CursorLockMode.Locked;


        //find the player count of the game -- default to 1 until fixed

        int playersToSpawn = playerCount;

        int playerNumber = 1;

        while (playersToSpawn > 0)
        {
            int r = Random.Range(0, playerSpawners.Count);
            playerSpawners[r].spawnPlayer(player, GetComponent<GameManager>(), playerCount, playerNumber);
            playerSpawners.Remove(playerSpawners[r]);
            playersToSpawn--;
            playerNumber++;
        }
        

        foreach (PlayerSpawnPoint p in playerSpawners) {
            p.Destroy();
        }
        //Enemy Spawners
        enemySpawners.Clear();
        foreach (GameObject eSpawner in GameObject.FindGameObjectsWithTag("ESP"))
        {

            enemySpawners.Add(eSpawner.GetComponent<EnemySpawnPoint>());
        }

    }

    private void StartGameSetup() {

        //set game dificulty
        if (customDifficulty)
            dificulty = customDifficultyLevel;
        else
            dificulty = playerCount;

        //set starting round
        currentRound = startingRound - 1;

        SetGunsToWorld();
    }
    private void SetGunsToWorld()
    {
        GameObject[] gunBuys = GameObject.FindGameObjectsWithTag("GunBuy");
        foreach (GameObject gb in gunBuys) {
            int rand = Random.Range(0, guns.Count);
            gb.GetComponent<GunBuy>().gun = guns[rand];
            guns.Remove(guns[rand]);
        }
        
    }

    private void BeginRound() {

        currentRound++;

        minSpeed = 2 + currentRound / 6f;
        maxSpeed = 2 + currentRound / 6f;

        if (currentRound > 10) {
            minSpeed = 3.5f;
            maxSpeed = 4.8f;
        }

        timeBetweenRounds = 10f;
        timeBetweenSpawns = timeBetweenRounds;
        Debug.Log("Time Between Rounds: " + timeBetweenRounds);
        SetEnemyData(currentRound);
        enemiesAlive = 0;
    }
    private void SetEnemyData(int r) {

        //set the enemy health
        if (r <= 10)
            enemiesHealth = (int)(100 * r);
        else
            enemiesHealth = (int)(enemiesHealth * 1.1);

        //set enemies to spawn
        if (dificulty == 1)
            enemiesToSpawn = (int)
                (0.000058 * Mathf.Pow(r, 3) +
                0.07 * Mathf.Pow(r, 2) +
                1.5 * r +
                9.7);
        else if (dificulty == 2)
            enemiesToSpawn = (int)
                (0.000054 * Mathf.Pow(r, 3) +
                0.16 * Mathf.Pow(r, 2) +
                1.7 * r +
                10.9);
        else if (dificulty == 3)
            enemiesToSpawn = (int)
                (0.00017 * Mathf.Pow(r, 3) +
                0.24 * Mathf.Pow(r, 2) +
                2.3 * r +
                16.3);
        else if (dificulty == 4)
            enemiesToSpawn = (int)
                (0.00024 * Mathf.Pow(r, 3) +
                0.32 * Mathf.Pow(r, 2) +
                2.8 * r +
                22.6);
    }

    void SpawnEnemiesIntoRound() {

            int enemiesToSpawnTemp = Random.Range(1, (currentRound / 2) + 1);

            //Make sure you wont go into negative enemies to spawn
            if (enemiesToSpawn - enemiesToSpawnTemp < 0)
                enemiesToSpawnTemp = enemiesToSpawn;
            //Make sure you don't spawn more than the max amount of enemies allowd at one time
            if (enemiesAlive + enemiesToSpawnTemp > maxEnemiesSpawnedAtOnce)
                enemiesToSpawnTemp = maxEnemiesSpawnedAtOnce - enemiesAlive;
            
            //Subtract the remaining enemies to spawn
                enemiesToSpawn -= enemiesToSpawnTemp;

            //Add current living enemies
            enemiesAlive += enemiesToSpawnTemp;

        //Spawn the enemies into the game
        bool hasSpawned = false;
        while (!hasSpawned)
        {
            EnemySpawnPoint temp = enemySpawners[Random.Range(0, enemySpawners.Count)];
            if (temp.isActive) {
                if (Random.Range(0.0f, 1.0f) < 0.9f)
                {
                    temp.SpawnEnemies(humanEnemy, enemiesHealth, Random.Range(minSpeed, maxSpeed), enemiesToSpawnTemp);
                    hasSpawned = true;
                }
                else {
                    temp.SpawnEnemies(dogEnemy, enemiesHealth, Random.Range(minSpeed, maxSpeed), enemiesToSpawnTemp);
                    hasSpawned = true;
                }
            }
        }

    }
}
