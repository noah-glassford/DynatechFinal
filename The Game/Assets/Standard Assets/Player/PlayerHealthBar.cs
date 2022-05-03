using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHealthBar : MonoBehaviour
{
    public PlayerGameData pgd;

    public Image healthBar;

    private float lerpSpeed;

    private void Update()
    {
        lerpSpeed = 2f * Time.deltaTime;
        healthBar.fillAmount = Mathf.Lerp(healthBar.fillAmount, (float)(pgd.health / (float)pgd.maxHealth), lerpSpeed);

        Color healthColor = Color.Lerp(Color.red, Color.white, ((float)(pgd.health / (float)pgd.maxHealth)));
        healthBar.color = healthColor;
    }
}
