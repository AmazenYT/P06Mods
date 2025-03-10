using System;
using UnityEngine;

[Serializable]
public class CameraParameters
{
	public int Mode;

	public Vector3 Position;

	public Vector3 Target;

	public CameraParameters(int _Mode, Vector3 _Position, Vector3 _Target)
	{
		Mode = _Mode;
		Position = _Position;
		Target = _Target;
	}
}
