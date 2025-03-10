using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
	public enum Type
	{
		Static,
		Physics,
		Enemy,
		Lights,
		Effects
	}

	public Type ObjectType;

	[Header("Optional")]
	public bool OverrideDist;

	public float Dist;

}
