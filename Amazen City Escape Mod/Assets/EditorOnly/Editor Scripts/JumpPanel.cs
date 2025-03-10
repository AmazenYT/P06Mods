using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class JumpPanel : MonoBehaviour
{
    public enum Mode
    {
        Manual,
        AutoLockOn
    }
    private bool RefreshValues;

    [Header("Framework")]
    public float YOffset;

    public float Speed;

    public float Timer;

    public Vector3 TargetPosition;

    public Transform TargetPositionReference;

    [Header("Prefab")]
    public bool UseTimerToExit;

    public bool UseTimerToRelease;

    public bool AlwaysLocked;

    public Mode JumpPanelMode;

    public bool AddYOffsetVel;
#if UNITY_EDITOR
    [Button(ButtonSizes.Medium)]
    private void ResetTargetPosition()
    {
        TargetPosition = transform.position;
    }
    [Button(ButtonSizes.Medium)]
    private void TargetPositionToCamera()
    {
        Camera cam = SceneView.lastActiveSceneView.camera;
        TargetPosition = cam.transform.position;
    }
    [Button(ButtonSizes.Medium)]
    private void FocusOnCameraEvent()
    {

        SceneView.lastActiveSceneView.LookAt(base.transform.position);
        SceneView.lastActiveSceneView.Repaint();

    }
#endif
    void OnValidate()
    {
        if (RefreshValues)
        {
            RefreshValues = false;
        }
        if (TargetPositionReference.transform.localPosition != Vector3.zero)
        {
            TargetPosition = TargetPositionReference.transform.position;
        }
        else
        {
            TargetPosition = Vector3.zero;
        }
        int UseTimerToExitInt;
        int UseTimerToReleaseInt;
        int AlwaysLockedInt;
        int AddYOffsetVell;

        UseTimerToExitInt = UseTimerToExit == true ? 1 : 0;
        UseTimerToReleaseInt = UseTimerToRelease == true ? 1 : 0;
        AlwaysLockedInt = AlwaysLocked == true ? 1 : 0;
        AddYOffsetVell = AddYOffsetVel == true ? 1 : 0;
        if (JumpPanelMode == JumpPanel.Mode.Manual)
        {
            name = "JumpPanelSpawner" + "/" + YOffset.ToString() + "/" + Speed.ToString() + "/" + Timer.ToString() + "/" + TargetPosition.x + "/" + TargetPosition.y + "/" + TargetPosition.z + "/" + UseTimerToExitInt.ToString() + "/" + UseTimerToReleaseInt.ToString() + "/" + AlwaysLockedInt.ToString() + "/" + 0 + "/" + AddYOffsetVell.ToString();
        }
        if (JumpPanelMode == JumpPanel.Mode.AutoLockOn)
        {
            name = "JumpPanelSpawner" + "/" + YOffset.ToString() + "/" + Speed.ToString() + "/" + Timer.ToString() + "/" + TargetPosition.x + "/" + TargetPosition.y + "/" + TargetPosition.z + "/" + UseTimerToExitInt.ToString() + "/" + UseTimerToReleaseInt.ToString() + "/" + AlwaysLockedInt.ToString() + "/" + 1 + "/" + AddYOffsetVell.ToString();
        }
        
        name = name.Replace(",", ".");
    }

    void OnDrawGizmos()
    {
        if (TargetPosition != Vector3.zero)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(TargetPosition, 1f);
        }
        if (!UseTimerToExit)
        {
            Vector3 EndPos = transform.position + (Direction(transform) * Speed * Timer);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, EndPos);
            float FixedDelta = 1f / 100f;
            Vector3 PredVelocity = Direction(transform) * Speed;
            Gizmos.color = !AlwaysLocked ? Color.green : Color.white;
            Vector3 PredPos = EndPos;
            Vector3 LasPredPos = EndPos;
            while (PredPos.y > TargetPosition.y)
            {
                PredVelocity.y -= (9.81f * FixedDelta);
                PredPos += PredVelocity * FixedDelta;
                Gizmos.DrawLine(LasPredPos, PredPos);
                LasPredPos = PredPos;
            }
        }
        
        else
        {
            Vector3 EndPos = transform.position + (Direction(transform) * Speed * Timer);
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, EndPos);
            float FixedDelta = 1f / 100f;
            Vector3 PredVelocity = Direction(transform) * Speed;
            Gizmos.color = !AlwaysLocked ? Color.green : Color.white;
            Vector3 PredPos = EndPos;
            Vector3 LasPredPos = EndPos;
            while (PredPos.y > TargetPosition.y)
            {
                PredVelocity.y -= (9.81f * FixedDelta);
                PredPos += PredVelocity * FixedDelta;
                Gizmos.DrawLine(LasPredPos, PredPos);
                LasPredPos = PredPos;
            }
        }
    }
    //[Button(ButtonSizes.Medium)]
    //private void RefreshButton()
    //{
    //    RefreshValues = true;
    //}


    private Vector3 Direction(Transform _transform)
    {
        if (this.TargetPosition == Vector3.zero)
        {
            return base.transform.forward;
        }
        Vector3 normalized = (this.TargetPosition - _transform.position).normalized;
        if (this.JumpPanelMode == JumpPanel.Mode.AutoLockOn)
        {
            return normalized;
        }
        return new Vector3(normalized.x, base.transform.forward.y + (this.AddYOffsetVel ? this.YOffset : 0f), normalized.z).normalized;
    }
}
