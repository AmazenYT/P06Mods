using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RainbowRingSpawner : MonoBehaviour
{
    [Space]
    [Title("Static title")]
    public const string MyTitle = "Properties";
    private string MySubtitle = "";
    [Space]
    [Title("$MyTitle", "$MySubtitle", TitleAlignments.Centered)]
    [Space]
    [Tooltip("The speed at which the player launches off")]
    [Range(1, 100)]
    public float Speed;
    [Space]
    [Tooltip("The amount of time player control is locked")]
    [Range(1, 100)]
    public float Timer;
    [Space]
    [Tooltip("Offsets the trajectory up and down of the launch")]
    [Range(-180, 180)]
    public float Inclination;
    [Header("Assign Object to snap to it's position")]
    public Transform TargetPosition;

   
    void OnValidate()
    {
        name = "RainbowDashSpawner" + "/" + Speed.ToString() + "/" + Timer.ToString() + "/" + Inclination;
        name = name.Replace(",", ".");
       if (TargetPosition != null)
        {
            transform.position = TargetPosition.transform.position;
            TargetPosition = null;
        }
    }
    void OnDrawGizmos()
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
        eulerAngles.x = -this.Inclination * 0.75f;
        return Quaternion.Euler(eulerAngles) * Vector3.forward;
    }
}
