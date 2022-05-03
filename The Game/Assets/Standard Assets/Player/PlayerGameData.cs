using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using JSAM;
using Photon.Pun;


public class PlayerGameData : MonoBehaviour
{
    public Text roundDisplay;
    public Text[] pointDisplay;

    public GameManager gm;
    public gunManager gunM;

    public int playerCount;

    public int playerNumber = 0;
    public int currentRound;
    public int currentPoints;
    public int pointsToDisplay = 0; 

    public DamageFlicker df;
    public int health = 6;
    public int maxHealth = 6;
    private float timeSinceLastDamage = 0.0f;

    public bool _speedAugment = false;
    public bool _healthAugment = false;


    private void Start()
    {
        pointsToDisplay = 0;

    }

    private void Update()
    {
        timeSinceLastDamage += Time.deltaTime;
        if (timeSinceLastDamage > 5f && health < maxHealth)
        {
            health = maxHealth;
            JSAM.AudioManager.StopSoundLoop(Sounds.LOWHEALTH, this.transform);
            JSAM.AudioManager.PlaySound(Sounds.HEALTHRESTORE, this.transform);
        }

        if (Input.GetKeyDown(KeyCode.P)) currentPoints += 10000;

        //Points to Display Updates
        var diff = Mathf.Abs(pointsToDisplay - currentPoints);

        if (diff > 5000)
        {
            if (pointsToDisplay < currentPoints)
                pointsToDisplay += 100;
            if (pointsToDisplay > currentPoints)
                pointsToDisplay -= 100;
        }
        if (diff > 1000)
        {
            if (pointsToDisplay < currentPoints)
                pointsToDisplay += 20;
            if (pointsToDisplay > currentPoints)
                pointsToDisplay -= 20;
        }
        if (diff > 200)
        {
            if (pointsToDisplay < currentPoints)
                pointsToDisplay += 5;
            if (pointsToDisplay > currentPoints)
                pointsToDisplay -= 5;
        }
            if (pointsToDisplay < currentPoints)
                pointsToDisplay += 1;
            if (pointsToDisplay > currentPoints)
                pointsToDisplay -= 1;
    }

    public void setData(GameManager _gm, int _playerCount, int _playerNumber) {
        SetGM(_gm);
        SetPlayerCount(_playerCount);
        playerNumber = _playerNumber;
        gunM = gameObject.GetComponent<gunManager>();
    }

    private void SetGM(GameManager g) {

        gm = g;

    }
    private void SetPlayerCount(int p) {
        playerCount = p;
        for (int i = 0; i < playerCount; i++) {
            pointDisplay[i].text = "500";
        }
    }

    public void TakeDamage(int damage)
    {
        if(timeSinceLastDamage > 0.15f) health -= damage;
        timeSinceLastDamage = 0f;

        JSAM.AudioManager.PlaySound(Sounds.PLAYERHIT, this.transform);
        df.FadeDamage();
        if (health <= 3 && JSAM.AudioManager.IsSoundLooping(Sounds.LOWHEALTH) == false)
        {
            JSAM.AudioManager.PlaySoundLoop(Sounds.LOWHEALTH, this.transform);
        }
        if (health <= 0) {
            PlayerDie();
            }

    }
    public void SpeedBuff(int ammount) { 
        
    }

    private void PlayerDie() {
        //VERY TEMP || DON'T FORGET TO REMOVE THIS WHEN NETWORKING COMES UP

        gameObject.GetComponent<PhotonView>().RPC("RPC_SyncDeath", RpcTarget.All);
    }

    [PunRPC]
    public void RPC_SyncDeath()
    {
        JSAM.AudioManager.StopAllSounds();
        PhotonNetwork.LoadLevel(2);
    }
    
}
