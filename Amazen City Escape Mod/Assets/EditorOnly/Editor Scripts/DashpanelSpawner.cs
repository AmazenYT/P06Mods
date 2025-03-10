using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashpanelSpawner : MonoBehaviour
{
    [Space]
    [Header("Dashpanel Properties")]
    [Space]
    [Tooltip("The speed at which the player launches off on the ground")]
    public float Speed;
    [Space]
    [Tooltip("The amount of time player control is locked")]
    public float Timer;

    void OnValidate()
    {
        name = "DashpanelSpawner" + "/" + Speed.ToString() + "/" + Timer.ToString();
        name = name.Replace(",", ".");
    }

    void OnDrawGizmosSelected()
    {
        if (Speed <= 20)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0f,0f,3f));
        }
        if (Speed > 20)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, transform.position + new Vector3(0f, 0f, 6f));
        }
    }
}
