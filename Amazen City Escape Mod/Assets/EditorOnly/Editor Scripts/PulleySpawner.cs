using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulleySpawner : MonoBehaviour
{
    [BackgroundColor(0.9f, 1f, 0.9f, 1f)]
    [Space]
    [Header("Framework")]
    [Tooltip("The position at which the pulley rests before grabbing it")]
    [Range(-25, -1)]
    public float BottomHeight;
    [Tooltip("The maximum Pulley height after grabbing it")]
    [Range(-3, -0.5f)]
    public float TopHeight;
    [Tooltip("Time from Bottom to Top position")]
    [Range(0, 5f)]
    public float Time;

    public GameObject HandlePoint;
    void OnValidate()
    {
        name = "UpReelSpawner" + "/" + BottomHeight.ToString() + "/" + TopHeight.ToString() + "/" + Time.ToString();
       
        name = name.Replace(",", ".");

        HandlePoint.transform.position = base.transform.position + base.transform.up * BottomHeight;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(base.transform.position + base.transform.up * TopHeight, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(base.transform.position + base.transform.up * BottomHeight, 0.5f);
        Gizmos.DrawLine(base.transform.position + base.transform.up * TopHeight, base.transform.position + base.transform.up * BottomHeight);
    }
}
