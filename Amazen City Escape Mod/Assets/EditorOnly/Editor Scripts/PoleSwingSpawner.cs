using UnityEngine;

public class PoleSwingSpawner : MonoBehaviour
{
	[Header("Framework")]
	public float Radius;

	public float Height;

	public float Power;

	public float Pitch;

	public float Yaw;

	public float Rotation_Time;

	public float Out_Time;



	void OnValidate()
	{
		name = "PoleSwingSpawner" + "/" + Radius + "/" + Height + "/" + Power + "/" + Pitch + "/" + Yaw + "/" + Rotation_Time + "/" + Out_Time;
		name = name.Replace(",", ".");
	}
	private Vector3 LaunchDirection()
	{
		Vector3 eulerAngles = base.transform.eulerAngles;
		eulerAngles.x = 0f - Pitch;
		eulerAngles.y += 270f - Yaw;
		return Quaternion.Euler(eulerAngles) * Vector3.forward;
	}

	private Vector3 PlayerPoint()
	{
		return base.transform.position ;
		//return base.transform.position + base.transform.forward * (Height * 0.5f);
	}
	private void OnDrawGizmos()
	{
		Vector3 vector = PlayerPoint() + LaunchDirection() * Power * (Power * 0.025f);
		Gizmos.color = Color.white;
		Gizmos.DrawLine(PlayerPoint(), vector);
		int num = 10 * (int)Power;
		float num2 = 0.01f;
		Vector3 a = LaunchDirection() * Power;
		Gizmos.color = Color.green;
		Vector3 vector2 = vector;
		Vector3 from = vector;
		for (int i = 0; i < num; i++)
		{
			a.y -= 20f * num2;
			vector2 += a * num2;
			Gizmos.DrawLine(from, vector2);
			from = vector2;
		}
	}

}
