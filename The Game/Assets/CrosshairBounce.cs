using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class CrosshairBounce : MonoBehaviour
{
    Image cross;

    private void Start()
    {
        cross = this.GetComponent<Image>();
    }
    private void Update()
    {
        if (cross.rectTransform.sizeDelta.x > 20) {
            cross.rectTransform.sizeDelta = new Vector2(cross.rectTransform.sizeDelta.x - 100f * Time.deltaTime
                , cross.rectTransform.sizeDelta.y - 100f * Time.deltaTime);
        }
        if (cross.rectTransform.sizeDelta.x < 20)
        {
            cross.rectTransform.sizeDelta = new Vector2(20,20);
        }
    }
}
