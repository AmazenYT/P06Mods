using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
[ExecuteInEditMode]
public class CameraEvents : MonoBehaviour
{
	public CameraParameters cameraParameters;

#if UNITY_EDITOR
	[Button(ButtonSizes.Medium)]
	private void CenterCamera()
	{
		cameraParameters.Position = transform.position;
	}
	[Button(ButtonSizes.Medium)]
	private void SetPositionToEditorCamera()
	{
		Camera cam = SceneView.lastActiveSceneView.camera;
		cameraParameters.Position = cam.transform.position;
	}
	[Button(ButtonSizes.Medium)]
	private void FocusOnCameraEvent()
	{
		
		 SceneView.lastActiveSceneView.LookAt(base.transform.position);
		 SceneView.lastActiveSceneView.Repaint();
        
	}
#endif
	private void OnDrawGizmos()
	{
		if (cameraParameters != null)
		{
			if (cameraParameters.Mode == 1 || cameraParameters.Mode == 4 || cameraParameters.Mode == 40 || cameraParameters.Mode == 5 || cameraParameters.Mode == 50)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.transform.position, cameraParameters.Position);
			}
			if (cameraParameters.Mode == 3 || cameraParameters.Mode == 30)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(base.transform.position, cameraParameters.Position);
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(cameraParameters.Position, 0.5f);
				Gizmos.DrawLine((cameraParameters.Position != Vector3.zero) ? cameraParameters.Position : base.transform.position, cameraParameters.Target);
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(cameraParameters.Target, 1f);
			}
			if (cameraParameters.Mode == 4 || cameraParameters.Mode == 40)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawWireSphere(cameraParameters.Position, 0.5f);
			}
			if (cameraParameters.Mode == 41)
			{
				Gizmos.color = Color.red;
				Gizmos.DrawWireSphere(cameraParameters.Position, 0.5f);
				Gizmos.color = Color.yellow;
				Gizmos.DrawLine(base.transform.position, cameraParameters.Position);
			}
			if (cameraParameters.Mode == 42)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawWireSphere(cameraParameters.Position, 0.5f);
				Gizmos.color = Color.cyan;
				Gizmos.DrawLine(base.transform.position, cameraParameters.Position);
			}
		}
	}
}