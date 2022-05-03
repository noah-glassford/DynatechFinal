using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunDecals : MonoBehaviour
{
    public bool randomColor;
    public Color primaryColor;
    public Color secondaryColor;
    private Color[] colors;

    public LineRenderer trail;

    [Header("Body")]
    public GameObject[] body;

    [Header("Gun Attachments")]
    public GameObject[] attachments;

    [Header("Particles")]
    public ParticleSystem[] particles;

    [Header("Gun Ligthing")]
    public Light[] lights;

    private void Awake()
    {
        if (randomColor) 
        {
            colors = new Color[4];

            colors[0] = new Color(255 / 255f ,246 / 255f, 0 / 255f);
            colors[1] = new Color(255 / 255f, 0 / 255f, 102 / 255f);
            colors[2] = new Color(255 / 255f, 127 / 255f, 51 / 255f);
            colors[3] = new Color(184 / 255f, 60 / 255f, 130 / 255f);

            secondaryColor = colors[Random.Range(0, colors.Length)];

            colors = new Color[2];
            colors[0] = Color.white;
            colors[1] = Color.black;

            primaryColor = colors[Random.Range(0, colors.Length)];
        }

        primaryColor.a = 255;
        secondaryColor.a = 255f;
        //Apply main color
        foreach (GameObject t in body)
        {
            MeshRenderer temp = t.GetComponent<MeshRenderer>();
            temp.material.color = primaryColor;

            Material temp2 = t.GetComponent<Renderer>().material;
            temp2.EnableKeyword("_EMISSION");
            temp2.SetColor("_EmissionColor", primaryColor);
            temp2.SetColor("_BaseMap", primaryColor);
        }

        //Apply Color to Attachments
        foreach (GameObject t in attachments) 
        {
            MeshRenderer temp = t.GetComponent<MeshRenderer>();
            temp.material.color = secondaryColor;

            Material temp2 = t.GetComponent<Renderer>().material;
            temp2.EnableKeyword("_EMISSION");
            temp2.SetColor("_EmissionColor", secondaryColor);
            temp2.SetColor("_BaseMap", secondaryColor);
        }
        //Apply Color to Effects
        foreach (ParticleSystem t in particles)
        {
            var main = t.main;
            main.startColor = secondaryColor;
        }
        //Apply Color To Lights
        foreach (Light t in lights)
        {
            t.color = secondaryColor;
        }

        //Apply Color To Trail
        if (trail != null)
        {
            var rend = trail.GetComponent<Renderer>().sharedMaterial;
            rend.SetColor("MainC", secondaryColor);
        }
    }
}
