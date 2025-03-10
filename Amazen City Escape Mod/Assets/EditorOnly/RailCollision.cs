using UnityEngine;
using System.Collections;

public class RailCollision : MonoBehaviour {

	public RailSystem BezierScript;
	public enum Type {
		Metal,
		Wind,
		Nature
	}
	public Type RailType;
	public bool Unswitchable;

}