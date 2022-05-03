using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;
using UnityEngine.VFX;
using UnityEngine.UI;
using Photon.Pun;
using System.IO;

public class TrailLine
{
    public GameObject LineObject;
    public float tForLerp;
}

public class Gun : MonoBehaviour
{
    [Header("MUST HAVE")]
    public string GunName = "EMPTY GUN NAME";
    public int GunPrice = 100;
    public int GunNumber = 000;
    public int gunLevel = 1;

    [Header("General Data")]
    public Camera playerCamera;
    public GameObject player;
    private Image cross;
    public GameObject headJoint;

    [Header("Gun Shooting Data")]
    public bool isAutomatic = true;

    public bool isBurst = false;
    public float burstRate = 0.05f;
    private bool canBurst = true;

    public bool isShotgun = false;
    public int shellCount = 10;

    [Header("Gun Damage Data")]
    public int damagePerBullet = 20;
    [Range(0f,1f)]
    public float spreadFactor = 0.5f;

    public float heavyDamageMulti = 2.5f;
    public float midDamageMulti = 1.0f;
    public float lowDamageMulti = 0.75f;

    [Header("Gun Magazine Data")]
    public int magazineSize = 8;
    [HideInInspector]
    public int roundsRemaining;
    public bool isReloading;

    [Header("Gun Fire Rate Data")]
    public float fireRate = 0.25f;
    private float fireTimer;

    [Header("Gun Pierce Data")]
    public int pierceHealth = 3;

    [Header("Effect Data")]
    public GameObject MuzzleFlash;
    public GameObject sparksParticle;
    private GunDecals gDecs;
    public Color gDecsColor;

    [Header("UI Icon")]
    public Sprite gunIcon;

    [Header("Animation Data")]
    public Animator anim;

    [Header("Recoil Data")]
    public float recoilStrengthVertical;
    public float recoilStrengthHorizontal;
    private float RecoilUp;
    private float RecoilSide;
    private int NumberOfShotsFullAuto; //counts how many shots go off

    private float ResetLerpT = 0;
    private bool isReseting = false;
    private Quaternion RotXForReset = new Quaternion(1, 1, 1, 1);

    //For Gun Visuals
    //public GameObject bulletTrail;

    [SerializeField] private Transform[] bulletExit;

    private List<TrailLine> SpawnedLines;
    



    void Start()
    {
        SpawnedLines = new List<TrailLine>();

        gDecs = GetComponent<GunDecals>();
        gDecsColor = gDecs.secondaryColor;
        roundsRemaining = magazineSize;
        spreadFactor = spreadFactor * 0.1f;

        cross = player.GetComponent<gunManager>().cross;
    }
    private void Update()
    {
        //Automatic Inputs
        if (Input.GetButton("Fire1") && isAutomatic)
        {
            if (roundsRemaining > 0 && isBurst)
            {
                StartCoroutine(BurstFire());
            }
            else if (roundsRemaining > 0 && isShotgun)
            {
                ShotgunFire();
            }
            else if (roundsRemaining > 0 && !isBurst) {
                Fire();
            }
            else
            {
                DoReload();
            }
        }
        //Semi Auto Inputs
        else if (Input.GetButtonDown("Fire1")) {
            if (roundsRemaining > 0 && isBurst)
            {
                StartCoroutine(BurstFire());
            }
            else if (roundsRemaining > 0 && isShotgun)
            {
                ShotgunFire();
            }
            else if (roundsRemaining > 0 && !isBurst)
            {
                Fire();
            }
            else
            {
                DoReload();
            }
        }


        if (isReseting) //This block handles the recoil reset, needs to be in update since it uses LERP
        {
            ResetLerpT += Time.deltaTime * 7;

            headJoint.transform.localRotation = Quaternion.Lerp(RotXForReset, Quaternion.Euler(-1,0,0), ResetLerpT);

            if (ResetLerpT >= 1)
            {
                isReseting = false;
                ResetLerpT = 0f;
            }

        }

        //Reloading Inputs
        if (Input.GetKeyDown(KeyCode.R)) DoReload();

        if (fireTimer < fireRate) fireTimer += Time.deltaTime;

        //Aiming Sinputs
        AimDownSights();

        //update the line renderers to they look like they are moving
        
        foreach(TrailLine trail in SpawnedLines)
        {
            Debug.Log("Updating Lines");

            trail.LineObject.GetComponent<Renderer>().material.SetVector("DirectionOffset", new Vector4(Mathf.Lerp(1f, -2f, trail.tForLerp), 0));
            trail.tForLerp += Time.deltaTime * 4f;

            if (trail.tForLerp >= 1f)
            {
                GameObject.Destroy(trail.LineObject);
                SpawnedLines.Remove(trail);
                break;
            }

          
        }
        


    }

    private void FixedUpdate()
    {
        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        isReloading = info.IsName("Reload");
    }

    private void AimDownSights()
    {
        if (Input.GetButton("Fire2") && !isReloading && playerCamera.fieldOfView > 57)
        {
            playerCamera.fieldOfView -= 5f;
            if (playerCamera.fieldOfView < 57) playerCamera.fieldOfView = 57;
        }
        if (!Input.GetButton("Fire2") && playerCamera.fieldOfView < 77 || isReloading && playerCamera.fieldOfView < 90)
        {
            playerCamera.fieldOfView += 5f;
            if (playerCamera.fieldOfView > 77) playerCamera.fieldOfView = 77;
        }
    }

    IEnumerator BurstFire() {
        if (canBurst && fireTimer >= fireRate && !isReloading)
        {
            canBurst = false;
            for (int i = 0; i < 3; i++)
            {
                Fire();
                yield return new WaitForSeconds(burstRate); // wait till the next round
            }
            fireTimer = 0.0f;
            canBurst = true;
        }
    }

    IEnumerator DestroyMuzzleFlash(GameObject flashObj)
    {
        yield return new WaitForSeconds(0.3f);
        PhotonNetwork.Destroy(flashObj.GetComponent<PhotonView>());
    }

    private void ShotgunFire() {
        if (fireTimer >= fireRate && !isReloading)
        {

            for (int i = 0; i < shellCount; i++)
            {
                Fire();
            }
            //Logistics
            roundsRemaining--;
            anim.CrossFadeInFixedTime("Fire", 0.1f);
            fireTimer = 0.0f;

            //Muzzle
            foreach (Transform p in bulletExit)
            {
              //  GameObject flashObj = Instantiate(MuzzleFlash, p.position, Quaternion.identity);
                GameObject flashObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", MuzzleFlash.name), p.position, Quaternion.identity);
                flashObj.transform.parent = p;
                flashObj.transform.localRotation = Quaternion.Euler(0, -90, 0);
                Destroy(flashObj, 0.15f);
                StartCoroutine(DestroyMuzzleFlash(flashObj));
            }
        }
    }

    private void Fire()
    {
        //Kickback if the gun is unable to shoot
        if (fireTimer < fireRate || roundsRemaining <= 0 || isReloading) return;
        else if (!isShotgun)
        {
            //spawn muzzle flash
            foreach (Transform p in bulletExit)
            {
                //GameObject flashObj = Instantiate(MuzzleFlash, p.position, Quaternion.identity);
                GameObject flashObj = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", MuzzleFlash.name), p.position, Quaternion.identity);
                flashObj.transform.parent = p;
                flashObj.transform.localRotation = Quaternion.Euler(0, -90, 0);
                StartCoroutine(DestroyMuzzleFlash(flashObj));
              
            }

            if (!isBurst) fireTimer = 0.0f;
        }

        //Bullet Spread
        Vector3 shootDir = playerCamera.transform.forward;
        shootDir.x += Random.Range(-spreadFactor, spreadFactor);
        shootDir.y += Random.Range(-spreadFactor, spreadFactor);

        //Pierce Bullet Code

        RaycastHit[] hit = Physics.RaycastAll(playerCamera.transform.position, shootDir, LayerMask.GetMask("Enemy"));

        if (SpawnedLines.Count <= 25)
        {
            //spawn bullet trail
            foreach (Transform p in bulletExit)
            {
                GameObject bulletTrailFX = Instantiate(gDecs.trail.gameObject, p.position, p.rotation);
                bulletTrailFX.transform.parent = p.transform;
                LineRenderer lineR = bulletTrailFX.GetComponent<LineRenderer>();
                lineR.SetPosition(0, p.position);
                lineR.SetPosition(1, p.position + (-p.transform.right * 15f));

                TrailLine temp = new TrailLine();
                temp.LineObject = lineR.gameObject;
                temp.tForLerp = 0f;

                SpawnedLines.Add(temp);

                //            Destroy(bulletTrailFX, 0.25f);

                if (hit.Length > 0)
                    lineR.SetPosition(1, hit[0].point);
            }
        }

        cross.rectTransform.sizeDelta = new Vector2(40, 40);

        int tPierce = pierceHealth;
        foreach (RaycastHit r in hit)
        {
            if (r.transform.gameObject.tag == "EnemyHeavy" && tPierce > 0)
            {
                GameObject newParticle = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", sparksParticle.name), r.point, r.transform.rotation);
                GameObject.Destroy(newParticle, 0.3f);

                float damage = damagePerBullet * Random.Range(heavyDamageMulti - 0.1f, heavyDamageMulti + 0.1f);
                r.transform.gameObject.GetComponentInParent<Enemy>().TakeDamage((int)damage, player, gDecsColor);
                tPierce--;
            }
            else if (r.transform.gameObject.tag == "EnemyAverage" || r.transform.gameObject.tag == "Enemy" && tPierce > 0)
            {
                GameObject newParticle = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", sparksParticle.name), r.point, r.transform.rotation);
                GameObject.Destroy(newParticle, 0.3f);

                float damage = damagePerBullet * Random.Range(midDamageMulti - 0.1f, midDamageMulti + 0.1f);
                r.transform.gameObject.GetComponentInParent<Enemy>().TakeDamage((int)damage, player, gDecsColor);
                tPierce--;
            }
            else if (r.transform.gameObject.tag == "EnemyLow" && tPierce > 0)
            {
                GameObject newParticle = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", sparksParticle.name), r.point, r.transform.rotation);
                GameObject.Destroy(newParticle, 1f);

                float damage = damagePerBullet * Random.Range(lowDamageMulti - 0.1f, lowDamageMulti + 0.1f);
                r.transform.gameObject.GetComponentInParent<Enemy>().TakeDamage((int)damage, player, gDecsColor);
                tPierce--;
            }
            else if (r.transform.gameObject.tag != "GunIgnore")
            {
                GameObject newParticle = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", sparksParticle.name), r.point, r.transform.rotation);
                GameObject.Destroy(newParticle, 0.3f);
            }

            //Provide a force to the rigidbody of the enemies
            Collider[] colliders = Physics.OverlapSphere(r.transform.position, 1f);

            foreach (Collider closeObj in colliders) {
                Rigidbody rb = closeObj.GetComponent<Rigidbody>();

                if (rb != null) {
                    rb.AddExplosionForce(100f, r.transform.position, 1f);
                }
            }

        }

        if (!isShotgun)
        {
            //Gun Logic Triggers
            roundsRemaining--;

            //Animation Triggers
            anim.CrossFadeInFixedTime("Fire", 0.1f);
        }
    }

    private void DoReload()
    {
        fireTimer = 0.0f;

        AnimatorStateInfo info = anim.GetCurrentAnimatorStateInfo(0);

        if (isReloading) return;
        if (roundsRemaining == magazineSize) return;
        anim.CrossFadeInFixedTime("Reload", 0.01f);

        Reload();
    }

    private void Reload()
    {
        roundsRemaining = magazineSize;
    }


}
