using UnityEngine;

public class ChainJumpSpawner : MonoBehaviour
{
	public enum Mode
	{
		Land,
		GameObject
	}

	public Mode TargetMode;

	[HideInInspector]public bool JumpSplinter;

	public bool ReleaseOnLand;

	[HideInInspector]public GameObject LandTarget;

	[HideInInspector]public Vector3 LandPosition;

	[HideInInspector] public GameObject[] EditorVisualizer;
	private bool HasTarget()
	{
		if (TargetMode == Mode.Land)
		{
			return LandPosition != Vector3.zero;
		}
		return LandTarget != null;
	}
	private int JumpSplinterInt;
	private int ReleaseOnLandInt;

	private void OnValidate()
    {
		if (TargetMode == ChainJumpSpawner.Mode.Land)
        {
			EditorVisualizer[0].SetActive(true);
			EditorVisualizer[1].SetActive(false);
			LandTarget.SetActive(false);
		}
		if (TargetMode == ChainJumpSpawner.Mode.GameObject)
		{
			EditorVisualizer[1].SetActive(true);
			EditorVisualizer[0].SetActive(false);
			LandTarget.SetActive(true);
		}
		JumpSplinterInt = JumpSplinter == true ? 1 : 0;
		ReleaseOnLandInt = ReleaseOnLand == true ? 1 : 0;
		gameObject.name = "ChainJumpSpawner/" + (int)TargetMode + "/" + JumpSplinterInt + "/" + ReleaseOnLandInt+ "/" + LandPosition.x + "/" + LandPosition.y + "/" + LandPosition.z;
		name = name.Replace(",", ".");
	}

    private void OnDrawGizmosSelected()
	{
		if (HasTarget())
		{
			Gizmos.DrawLine(base.transform.position, (TargetMode == Mode.Land) ? LandPosition : LandTarget.transform.position);
			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube((TargetMode == Mode.Land) ? LandPosition : LandTarget.transform.position, new Vector3(1.5f, 1.5f, 1.5f));
			Gizmos.DrawWireSphere((TargetMode == Mode.Land) ? LandPosition : LandTarget.transform.position, 0.1f);
		}
	}
}
