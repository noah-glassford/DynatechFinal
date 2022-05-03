using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using JSAM;

public class gunManager : MonoBehaviour
{
    [Header("Gun Data")]
    public GameObject[] guns;
    public GameObject startingGun;

    [Header("Logical Data")]

    public Camera playerCamera;
    public GameObject player;
    public GameObject headJoint;
    public Transform gunPosition;


    [Header("UI Data")]
    public Image cross;

    public Text ammoDisplay;
    public Image gunIC;
    public Text gunLvl;
    public Gun currentGun;

    [Header("Debug Data")]
    public int index = 0;

    bool isSwitching;

    private void Start()
    {
        InitializeWeapons();
    }

    private void InitializeWeapons() 
    {
        addNewGun(startingGun, 0);
        addNewGun(startingGun, 1);
        switchWeapons(0);
    }

    private void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0 && !isSwitching) {
            index++;
            if (index > guns.Length-1) index = 0;
            StartCoroutine(SwitchAfterDelay(index));
        }
        ammoDisplay.text = currentGun.roundsRemaining + "/" + currentGun.magazineSize;
    }

    public void addNewGun(GameObject gunHolder, int indexPos) {

        if (guns[indexPos] != null) {
            Destroy(currentGun.transform.parent.gameObject);
            guns[indexPos] = null;
        }
        GameObject g = Instantiate(gunHolder, gunPosition.position, gunPosition.rotation);
        currentGun = g.GetComponentInChildren<Gun>();
        g.transform.parent = gunPosition;
        Gun t = g.GetComponentInChildren<Gun>();
        t.playerCamera = playerCamera;
        t.player = player;
        t.headJoint = headJoint;

        guns[indexPos] = g;
        switchWeapons(indexPos);
        
    }

    private IEnumerator SwitchAfterDelay(int indexpos) {
        if (!currentGun.isReloading)
        {

            isSwitching = true;

            //Modify later to pause on anim reset
            yield return new WaitForSeconds(0.0f);
            switchWeapons(indexpos);

            isSwitching = false;
        }
    }

    private void switchWeapons(int indexpos) 
    {
        for (int i = 0; i < guns.Length; i++)
        {
            if (guns[i] != null)
                guns[i].SetActive(false);
        }
        guns[indexpos].SetActive(true);
        currentGun = guns[indexpos].GetComponentInChildren<Gun>();

        //UI Data
        ammoDisplay.text = currentGun.roundsRemaining + "/" + currentGun.magazineSize;
        gunIC.sprite = currentGun.gunIcon;
        gunLvl.text = currentGun.gunLevel.ToString();
    }
}
