using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JSAM;

public class PlayerAugmentStation : Interactablle
{
    public bool isSpeed;
    public bool isHealth;

        public override string getDescription(PlayerGameData pgd)
        {
        if (!pgd._speedAugment && isSpeed)
            return "Encahnce Your Speed For 5000";
        if (!pgd._healthAugment && isHealth)
            return "Enhance your Health For 5000";
        else return " ";
        }
        public override void Interact(PlayerGameData pgd)
        {
        if (!pgd._speedAugment && isSpeed && pgd.currentPoints >= 5000)
            {
                pgd.currentPoints -= 5000;
                pgd._speedAugment = true;
                pgd.gameObject.GetComponent<Player>().moveSpeed += 1.5f;
                pgd.gameObject.GetComponent<Player>().sprintSpeed += 1.5f;
                pgd.gameObject.GetComponent<Player>().actionSpeed = 1.5f;
            }
        if (!pgd._healthAugment && isHealth && pgd.currentPoints >= 5000)
        {
            pgd.currentPoints -= 5000;
            pgd._healthAugment = true;
            pgd.gameObject.GetComponent<PlayerGameData>().maxHealth = 10;
        }
    }
}
