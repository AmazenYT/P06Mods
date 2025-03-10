using UnityEngine;

public class Ring : MonoBehaviour
{
	[Header("Framework")]
	public bool GroundLightDash;

	public float SplineTime;

	public string SplineName;

	private int GroundLightDashCheck;
	private void OnValidate()
	{
		if (GroundLightDash)
		{
			GroundLightDashCheck = 1;
		}
		if (!GroundLightDash)
		{
			GroundLightDashCheck = 0;
		}
		gameObject.transform.GetChild(0).gameObject.name = SplineName;
		gameObject.name = "RingObjectSpawner/" + GroundLightDashCheck + "/" + SplineTime;
		name = name.Replace(",", ".");
	}
}