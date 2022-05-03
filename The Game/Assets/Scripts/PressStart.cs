using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

public class PressStart : MonoBehaviour
{
    [SerializeField] Animator startText;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButton("Submit") && startText.GetCurrentAnimatorStateInfo(0).IsName("PressStart"))
        {
            startPress();
        }
    }

    void startPress()
    {
        MenuManager.Instance.OpenMenu("mainmenu");
        JSAM.AudioManager.PlaySound(Sounds.UICONFIRM);
    }    
}
