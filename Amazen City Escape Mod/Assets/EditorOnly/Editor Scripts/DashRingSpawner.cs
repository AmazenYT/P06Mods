using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashRingSpawner : MonoBehaviour
{
    [Space]
    [Header("Rainbow Ring Properties")]
    [Space]
    [Tooltip("The speed at which the player launches off")]
    [Range(1, 100)]
    public float Speed;
    [Space]
    [Tooltip("The amount of time player control is locked")]
    [Range(1, 100)]
    public float Timer;
    [Space]
    [Header("Target Position")]
    public Transform TargetPosition;
    public bool TeleportToTarget;


    void OnValidate()
    {
        name = "Common_DashHoop" + "/" + Speed.ToString() + "/" + Timer.ToString();
        name = name.Replace(",", ".");
        if (TargetPosition != null && TeleportToTarget)
        {
            transform.position = TargetPosition.position;
            TeleportToTarget = false;
        }
    }
    void OnDrawGizmosSelected()
    {
        Vector3 EndPos = transform.position + (MeshDirection() * Speed * Mathf.Min(1f, Timer));
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, EndPos);
        int Steps = 240;
        Vector3 PredVelocity = MeshDirection() * Speed;
        Gizmos.color = Color.green;
        Vector3 PredPos = EndPos;
        Vector3 LastPredPos = EndPos;
        float LastTimer = Mathf.Min(1f, Timer);
        for (int i = 0; i < Steps; i++)
        {
            LastTimer += Time.fixedDeltaTime;
            if (LastTimer > (Timer))
            {
                Gizmos.color = Color.red;
            }
            PredVelocity.y -= ((LastTimer > (Timer) ? 25f : 9.81f) * Time.fixedDeltaTime);
            PredPos += PredVelocity * Time.fixedDeltaTime;
            Gizmos.DrawLine(LastPredPos, PredPos);
            LastPredPos = PredPos;
        }
    }
    private Vector3 MeshDirection()
    {
        Vector3 eulerAngles = base.transform.eulerAngles;
        return Quaternion.Euler(eulerAngles) * Vector3.forward;
    }
}
