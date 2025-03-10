using System;
using UnityEngine;

public class SnowBoardJump : MonoBehaviour
{
	public void SetParameters(float _Power, float _Pitch, float _Rate, float _BPower_Rate, float _Time)
	{
		this.Power = _Power;
		this.Pitch = _Pitch;
		this.Rate = _Rate;
		this.BPower_Rate = _BPower_Rate;
		this.Time = _Time;
	}

	private void OnDrawGizmosSelected()
	{
		Vector3 vector = base.transform.position + (base.transform.forward * this.Power + base.transform.up * this.Pitch * 0.5f) * this.Time;
		Gizmos.color = Color.white;
		Gizmos.DrawLine(base.transform.position, vector);
		int num = 8 * (int)this.Power;
		float num2 = 0.01f;
		Vector3 vector2 = base.transform.forward * this.Power + base.transform.up * this.Pitch * 0.5f;
		Gizmos.color = Color.green;
		Vector3 vector3 = vector;
		Vector3 vector4 = vector;
		for (int i = 0; i < num; i++)
		{
			vector2.y -= 9.81f * num2;
			vector3 += vector2 * num2;
			Gizmos.DrawLine(vector4, vector3);
			vector4 = vector3;
		}
	}

	[Header("Framework")]
	public float Power;
	public float Pitch;
	public float Rate;
	public float BPower_Rate;
	public float Time;
}
