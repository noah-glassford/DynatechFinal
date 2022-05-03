using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;
using UnityEngine.AI;

public class KlockScript : Interactablle
{

    public bool isActivated = false;

    public override string getDescription(PlayerGameData pgd)
    {
        if (isActivated) return " ";
        else return " ";
    }
    public override void Interact(PlayerGameData pgd)
    {
        isActivated = true;
        JSAM.AudioManager.PlaySound(Sounds.UICONFIRM);
        JSAM.AudioManager.PlaySound(Sounds.KLOCKMUSIC);
        Destroy(this);
    }
}
