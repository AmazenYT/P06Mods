using UnityEngine;

public class HurtPlayer : MonoBehaviour
{
	public enum Type
	{
		Enemy = 0,
		Projectile = 1,
		Object = 2
	}

	public Type hurtType;

	public bool OnCollision;

	public bool OnlyMachSpeed;

}
