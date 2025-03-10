using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnerType : MonoBehaviour
{

	public enum Mode
	{
		Chase,
		Fix,
		Fix_Vulcan,
		Fix_Rocket,
		Fix_Missile,
		Normal,
		Trans
	}
	[Header("Enemy Type")]
	public Mode TargetMode;
    private void OnValidate()
    {
        gameObject.name = "eGunnerSpawner/" + (int)TargetMode;
    }
}
