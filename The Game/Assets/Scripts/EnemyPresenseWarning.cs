using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyPresenseWarning : MonoBehaviour
{
    public Image IndicatorArrow;
    public BoxCollider BehindDetectionTrigger;

    private List<GameObject> InTrigger;

    private void Start()
    {
        InTrigger = new List<GameObject>();
    }

    private void Update()
    {
        if (InTrigger.Count >= 1)
        {
            IndicatorArrow.gameObject.SetActive(true);
        }
        else if (InTrigger.Count == 0)
        {
            IndicatorArrow.gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            InTrigger.Add(other.gameObject);
            Debug.Log("Enemy Behind");
        }
    }

   
        private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
            InTrigger.Remove(other.gameObject);
    }



}
