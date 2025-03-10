using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RingSpawner : MonoBehaviour
{


	[Header("Ring Properties")]
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
		gameObject.name = "RingSpawner/" + GroundLightDashCheck + "/" + SplineTime;
		name = name.Replace(",", ".");
	}
}
