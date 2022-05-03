using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using JSAM;
using TMPro;
using Photon.Pun;

public class Enemy : MonoBehaviour
{

    //wee wee kaka
    private bool canDie = true;

    [Header("General Enemy data")]
    public float attackRange = 5f;
    private bool canAttack = true;

    public float minSeek = 5f;
    public float maxSeek = 10f;

    public int minDamage;
    public int maxDamage; //idk another script was getting mad at me lol

    private float timeTillNavUpdate = 0f;

    [Header("Backend Data")]
    public GameObject floatingText;
    public Animator enemyAnims;

    [Header("Points Distribution Data")]
    public int pointsPerHit;
    public int pointsPerKill;

    [Header("Other Cool Stuff")]
    //public GameObject enemyHead;
    [SerializeField] GameObject enemydiesound;

    [Header("Debug Information")]
    public int Health;
    public GameObject[] players;
    public GameManager gm;
    public NavMeshAgent nav;
    PhotonView PV;

    bool canSendDeath = true;


    //Enemy OnDamageData
    int d;
    GameObject p;
    Color c;

    [Header("VisualEffects")]

    public GameObject ExplosionVisual;
    private float t = 0;
    private bool hasDied = false;
    private Renderer[] renderers;
    public Material DissolveMaterial;
    public Material DefaultMaterial;


    private void Start()
    {
        PV = GetComponent<PhotonView>();
        players = GameObject.FindGameObjectsWithTag("Player");
        nav = GetComponent<NavMeshAgent>();
        gm = GameManager._instance;
        setRidigState(true);
        setColidState(true);

        renderers = GetComponentsInChildren<Renderer>();

    }

    private void Update()
    {
        if(timeTillNavUpdate <= 0 && canDie)
            UpdateNavMesh(); //Will auto optimize the nav mesh agent and hunt the closest player
        timeTillNavUpdate -= Time.deltaTime;

        if (d > 0)
            onDamageUpdate();

        if (hasDied && canSendDeath == true)
        {
            canSendDeath = false;
            PV.RPC("RPC_Die", RpcTarget.Others);
        }
        else if(hasDied)
        {
            t += Time.deltaTime;
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetFloat("Dissolve", t);

                if (t >= 1)
                {
                    t = 0;
                    //PV.RPC("RPC_Die", RpcTarget.All);
                    gameObject.SetActive(false);
                    renderers[i].material = DefaultMaterial;

                    break;
                }

            }
        }
    }

    [PunRPC]
    private void RPC_Die()
    {
        canDie = false;
        canAttack = false;
        enemyAnims.enabled = false;
        gm.enemyDeath();

        nav.enabled = false;
        hasDied = true;

        if (enemydiesound != null)
        {
            GameObject die = Instantiate(enemydiesound, this.transform);
            Destroy(die, 6f);
        }

        Instantiate(ExplosionVisual, transform.position, Quaternion.identity);

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = DissolveMaterial;
        }

        canAttack = false;

        //gameObject.SetActive(false);

        //Die(p);
    }

    private void UpdateNavMesh() {
        PlayerGameData playerGameDataTemp = null;
        float dis = Mathf.Infinity;
        foreach (GameObject p in players) 
        {
            float t = Vector3.Distance(transform.position, p.transform.position);
            if (t < dis) {
                dis = t;
                nav.SetDestination(p.transform.position);
                //enemyHead.transform.LookAt(p.transform.position);
                playerGameDataTemp = p.gameObject.GetComponent<PlayerGameData>();
            }
        }
        if (dis < attackRange && canAttack) StartCoroutine(AttackPlayer(playerGameDataTemp));
        else if (dis > maxSeek) timeTillNavUpdate = 2.5f;
        else if (dis > minSeek) timeTillNavUpdate = 1f;
        else timeTillNavUpdate = 0.25f;
    }

    public void TakeDamage(int _d, GameObject _p, Color _c) {
        d += _d;
        p = _p;
        c = _c;
    }
    private void onDamageUpdate() {
        if (canDie)
        {
            GameObject tempTxt = Instantiate(floatingText, transform.position, Quaternion.identity);
           
            tempTxt.GetComponent<TextMeshPro>().text = d.ToString();
            //c += new Color(0.25f, 0.25f, 0.25f);
            //tempTxt.GetComponent<TextMeshPro>().color = c;
            tempTxt.transform.LookAt(p.transform.position);
            Health -= d;
            d = 0;
            p.GetComponent<PlayerGameData>().currentPoints += pointsPerHit;
            JSAM.AudioManager.PlaySound(Sounds.HITMARKER, this.transform);
            if (Health <= 0) Die(p);
        }
    }

    private void Die(GameObject p) {
        canDie = false;
        canAttack = false;
        enemyAnims.enabled = false;
        p.GetComponent<PlayerGameData>().currentPoints += pointsPerKill;
        gm.enemyDeath();

        nav.enabled = false;

        //JSAM.AudioManager.PlaySound(Sounds.ENEMYDIE);


        hasDied = true;

        if (enemydiesound != null)
        {
            GameObject die = Instantiate(enemydiesound, this.transform);
            Destroy(die, 6f);
        }

        Instantiate(ExplosionVisual, transform.position, Quaternion.identity);
        
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material = DissolveMaterial;
        }

        canAttack = false;

    }

    void setRidigState(bool state) {
        Rigidbody[] rigibodies = GetComponentsInChildren<Rigidbody>();

        foreach (Rigidbody rig in rigibodies) {
            rig.isKinematic = state;
        }
    }
    void setColidState(bool state)
    {
        Collider[] col = GetComponentsInChildren<Collider>();

        foreach (Collider c in col)
        {
            c.enabled = state;
        }
    }

    private void OnCollisionEnter(Collision collision) //does damage to player
    {

        PlayerGameData pgd;
        if (collision.gameObject.TryGetComponent<PlayerGameData>(out pgd))
        {
            if (canAttack == true)
            {
                Debug.Log(pgd.health);
                pgd.TakeDamage(2);
            }
        }
    }
    IEnumerator AttackPlayer(PlayerGameData _pgd) {
        canAttack = false;
        _pgd.TakeDamage(Random.Range(minDamage,maxDamage));
        float holdSpeed = nav.speed;
        nav.speed = 0;
        yield return new WaitForSeconds(1.5f);
        nav.speed = holdSpeed;
        canAttack = true;
    }
    
}
