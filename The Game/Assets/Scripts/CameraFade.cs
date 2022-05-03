using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class CameraFade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FadeOut();
    }
    private void Update()
    {
        if(this.GetComponent<CanvasRenderer>().GetAlpha() == 0)
        {
            Debug.Log("Done!");
            Destroy(this.gameObject);
        }
    }
    void FadeOut()
    {
        this.GetComponent<Image>().CrossFadeAlpha(0, 3.0f, false);
    }
    void FadeIn()
    {
        this.GetComponent<Image>().CrossFadeAlpha(1, 10.0f, false);
    }
}
