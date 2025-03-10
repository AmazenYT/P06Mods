using UnityEditor;
using UnityEngine;

public class WideSpringSpawner : MonoBehaviour
{
    [BackgroundColor(0.9f, 1f, 0.9f, 1f)]
    [Space]
    [Header("Framework")]
    [Tooltip("The speed at which the player launches off")]
    [Range(1, 100)]
    public float Speed;
    [Tooltip("The amount of time player control is locked")]
    [Range(0, 50)]
    public float Timer;
    private bool UseTimer = true;
    void OnValidate()
    {
        name = "WidejumpPadSpawner" + "/" + Speed.ToString() + "/" + Timer.ToString();
        if (Timer < 0 || Speed < 0)
        {
            Timer = 0;
            Speed = 0;
        }
        name = name.Replace(",", ".");
    }

    void OnDrawGizmos()
    {
        {
            Vector3 EndPos = transform.GetChild(0).position + (Direction(transform.GetChild(0)) * Speed * (UseTimer ? 0f : Timer));
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.GetChild(0).position, EndPos);
            int Steps = 4 * (int)Speed;
            float FixedDelta = 1f / 100f;
            Vector3 PredVelocity = Direction(transform.GetChild(0)) * Speed;
            Gizmos.color = Color.green;
            Vector3 PredPos = EndPos;
            Vector3 LastPredPos = EndPos;
            for (var i = 0; i < Steps; i++)
            {
                PredVelocity.y -= 9.81f * FixedDelta *0.0025f;
                PredPos += PredVelocity * FixedDelta *1.3f;
                Gizmos.DrawLine(LastPredPos, PredPos);
                LastPredPos = PredPos;
            }
        }
    }
    private Vector3 Direction(Transform _transform)
    {
        Vector3 normalized = (transform.position - _transform.position).normalized;
        normalized.y = Mathf.Max(normalized.y, base.transform.forward.y);
        return normalized.normalized;
    }
}
