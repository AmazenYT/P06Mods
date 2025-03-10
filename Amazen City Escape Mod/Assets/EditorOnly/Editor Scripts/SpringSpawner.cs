using UnityEditor;
using UnityEngine;

//
public class SpringSpawner : MonoBehaviour
{
    [BackgroundColor(0.9f, 1f, 0.9f, 1f)]
    [Space][Header("")]
    [Tooltip("The speed at which the player launches off")]
    [Range(1, 100)]
    public float Speed;
    [Space]
    private bool UseTimer;
    [Tooltip("The amount of time player control is locked")]
    [Range(0, 50)]
    public float Timer;
    [Space]
    private bool UseTargetPosition;
    [HideInInspector]public Transform TargetPosition;
    [HideInInspector]public Vector3 TargetPositionLocation;
    [Space][Header("Spring Trajectory Properties")]
    [Tooltip("The location of target position you want the Spring to send Sonic into")]
    public Transform AutoTargetLocation;
    [Tooltip("Manual Mode will allow you to rotate spring, while Automatic can lock onto objects (make sure to increase the Speed more if you don't reach the target location")]
    public SpringSpawner.Mode SpringMode;


    public enum Mode
    {
        ManualMode,
        AutomaticLockOn
    }

    void OnValidate()
    {
        if (this.SpringMode == SpringSpawner.Mode.AutomaticLockOn)
        {
            UseTargetPosition = true;
            if (AutoTargetLocation != null)
            {
                TargetPosition.position = AutoTargetLocation.position;
            }
            transform.LookAt(AutoTargetLocation.position, AutoTargetLocation.position);
            name = "SpringObjSpawner" + "/" + Speed.ToString() + "/" + Timer.ToString() + "/" + TargetPosition.position.x + "/" + TargetPosition.position.y + "/" + TargetPosition.position.z + "/" + 1;
        }
        if (this.SpringMode == SpringSpawner.Mode.ManualMode)
        {
            UseTargetPosition = false;
            name = "SpringObjSpawner" + "/" + Speed.ToString() + "/" + Timer.ToString() + "/" + 0 + "/" + 0 + "/" + 0 + "/" + 0;
            TargetPositionLocation = new Vector3(0f, 0f, 0f);
            TargetPosition.localPosition = TargetPositionLocation;
            AutoTargetLocation = TargetPosition;
        }
        if (Timer < 0 || Speed < 0)
        {
            Timer = 0;
            Speed = 0;
        }
        name = name.Replace(",", ".");
    }

    void OnDrawGizmos()
    {
        if (this.SpringMode == SpringSpawner.Mode.AutomaticLockOn)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, AutoTargetLocation.position);
            Gizmos.DrawWireSphere(AutoTargetLocation.position, 1f);
        }
        if (this.SpringMode == SpringSpawner.Mode.ManualMode)
        {
            Vector3 EndPos = transform.GetChild(0).position + (Direction(transform.GetChild(0)) * Speed * (UseTimer ? 0f : Timer));
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.GetChild(0).position, EndPos);
            int Steps = 4 * (int)Speed;
            float FixedDelta = 1f / 100f;
            Vector3 PredVelocity = Direction(transform.GetChild(0)) * Speed;
            Gizmos.color = !UseTargetPosition ? Color.green : Color.white;
            Vector3 PredPos = EndPos;
            Vector3 LastPredPos = EndPos;
            for (var i = 0; i < Steps; i++)
            {
                PredVelocity.y -= 9.81f * FixedDelta;
                PredPos += PredVelocity * FixedDelta;
                Gizmos.DrawLine(LastPredPos, PredPos);
                LastPredPos = PredPos;
            }
        }
    }
    private Vector3 Direction(Transform _transform)
    {
        if (this.SpringMode == SpringSpawner.Mode.AutomaticLockOn)
        {
            return (this.TargetPosition.position - _transform.position).normalized;
        }
        if (this.SpringMode == SpringSpawner.Mode.ManualMode)
        {
            return base.transform.forward;
        }
        Vector3 normalized = (TargetPosition.position - _transform.position).normalized;
        normalized.y = Mathf.Max(normalized.y, base.transform.forward.y);
        return normalized.normalized;
    }
}
