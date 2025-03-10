using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGoal : MonoBehaviour
{
    [Header("Framework")]
    [Space]
    [Header("Camera Position is where the camera resides at (Red)")]
    [Space]
    [Header("Target is where it's aiming at (Green)")]
    [Space]
    [Header("The green cube is where sonic will be position and rotated as")]
    public Vector3 CameraPosition;
    public Vector3 CameraTarget;
    [HideInInspector] public Vector3 Cam_Pos;
    [HideInInspector] public Vector3 Cam_Tgt;

    private void OnValidate()
    {
        Cam_Pos = transform.position + CameraPosition;
        Cam_Tgt = transform.position + CameraTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Debug.DrawLine(base.transform.position, Cam_Pos);
        Gizmos.DrawWireSphere(Cam_Pos, 1f);
        Gizmos.color = Color.green;
        Debug.DrawLine(Cam_Pos, Cam_Tgt);
        Gizmos.DrawWireSphere(Cam_Tgt, 2f);
    }
}
