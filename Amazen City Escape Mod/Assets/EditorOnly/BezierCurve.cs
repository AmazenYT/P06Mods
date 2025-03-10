using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Linq;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Knot {
	public Vector3 position;
	public Vector3 ctrl1;
	public Vector3 ctrl2;
	public enum HandleType {
		Free,
		Aligned,
		Broken,
		Auto
	};	
	public HandleType type;
	public Knot (Vector3 _position, Vector3 _ctrl1, Vector3 _ctrl2) {
		position = _position;
		ctrl1 = _ctrl1;
		ctrl2 = _ctrl2;		
	}
}

[System.Serializable]
public class BezierCurve : MonoBehaviour {

	public List<Knot> knots;
	private List<Knot> lastKnots;

	private float length;
	private float[] lookup;

	public void Reset() {
		knots = new List<Knot>();
		knots.Add(new Knot(Vector3.left * 2f, Vector3.left * 3f, Vector3.left + Vector3.up));
		knots.Add(new Knot(Vector3.right * 2f, Vector3.right + Vector3.down, Vector3.right * 3f));
	}

	public void AddKnot() {
		Vector3 position = knots[knots.Count - 1].position;
		Vector3 direction = (position - knots[knots.Count - 2].position).normalized;
		knots.Add(new Knot(position + direction * 3f, position + direction * 2f, position + direction * 4f));
	}

	public void InsertKnot(int index) {
		int indexMax = knots.Count - 1;
		if (index > indexMax) {
			AddKnot();
			return;
		} else if (index == 0) {
			Vector3 position = knots[0].position;
			Vector3 direction = (position - knots[1].position).normalized;
			knots.Insert(index, new Knot(position + direction * 3f, position + direction * 4f, position + direction * 2f));
			return;
		}
		Vector3 avgPosition = (knots[index - 1].position + knots[index].position) * 0.5f;
		Vector3 dir_ctrl1 = (knots[index - 1].ctrl2 - avgPosition).normalized; //(knots[index-1].position-avgPosition).normalized;
		Vector3 dir_ctrl2 = (knots[index].ctrl1 - avgPosition).normalized; //(knots[index].position-avgPosition).normalized;
		knots.Insert(index, new Knot(avgPosition, avgPosition + dir_ctrl1, avgPosition + dir_ctrl2));
	}

	public void UpdateKnot(int index) {
		Knot knot = knots[index];
		Knot.HandleType type = knot.type;
		bool start = (index == 0);
		bool end = (index == knots.Count - 1);
		if (type == Knot.HandleType.Free) {
			return;
		}
		if (type == Knot.HandleType.Aligned) {
			Vector3 dir1 = knot.position - knot.ctrl1;
			Vector3 dir2 = knot.ctrl2 - knot.position;
			Vector3 avgDir = (dir1 + dir2) * 0.5f;
			knot.ctrl1 = knot.position - avgDir;
			knot.ctrl2 = knot.position + avgDir;
			return;
		}
		if (type == Knot.HandleType.Broken || type == Knot.HandleType.Auto) {
			if (!start) {
				Knot knot1 = knots[index - 1];
				Vector3 dir1 = knot1.position - knot.position;
				knot.ctrl1 = knot.position + dir1 * 0.25f;
			}
			if (!end) {
				Knot knot2 = knots[index + 1];
				Vector3 dir2 = knot2.position - knot.position;
				knot.ctrl2 = knot.position + dir2 * 0.25f;
			}
			if (type == Knot.HandleType.Broken) {
				return;
			}
		}
		if (type == Knot.HandleType.Auto) {
			Vector3 dir1 = knot.position - knot.ctrl1;
			Vector3 dir2 = knot.ctrl2 - knot.position;
			Vector3 avgDir = (dir1 + dir2) * 0.5f;
			knot.ctrl1 = knot.position - avgDir;
			knot.ctrl2 = knot.position + avgDir;
		}
	}

	public int GetSegmentAtTime(ref float t) {
		int nbSegment = knots.Count - 1;
		float tRect = t * nbSegment;
		int seg = (int)tRect; // cast to int to get segment
		tRect -= seg; // 0-1 for that segment
		t = tRect;
		if (seg > nbSegment - 1) {
			t = 1.0f;
			return nbSegment - 1;
		} else {
			return seg;
		}
	}

	void CopyList() {
		lastKnots = new List<Knot>();
		for (int i = 0; i < knots.Count; i++) {
			lastKnots.Add(new Knot(knots[i].position, knots[i].ctrl1, knots[i].ctrl2));
		}
	}

	bool ShouldUpdate() {
		if (lastKnots == null || knots.Count != lastKnots.Count) {
			CopyList();
			return true;
		}
		for (int i = 0; i < knots.Count; i++) {
			if (knots[i].position != lastKnots[i].position || knots[i].ctrl1 != lastKnots[i].ctrl1 || knots[i].ctrl2 != lastKnots[i].ctrl2) {
				CopyList();
				return true;
			}
		}
		return false;
	}

	void Parametrize() {
		int points = (knots.Count - 1) * 25;
		length = 0.0f;
		List<float> lookup_list = new List<float>();
		Vector3 lastPoint = InterpolatePosition(0.0f);
		lookup_list.Add(0.0f);
		for (int i = 0; i < points; i++) {
			float time = (1.0f + i) / points;
			Vector3 point = InterpolatePosition(time);
			length += Vector3.Distance(point, lastPoint);
			lastPoint = point;
			lookup_list.Add(length);
		}
		lookup = lookup_list.ToArray();
	}

	public float Length() {
		if (ShouldUpdate()) {
			Parametrize();
		}
		return length;
	}

	public float[] LookupTable() {
		if (ShouldUpdate() || lookup == null) {
			Parametrize();
		}
		return lookup;
	}

	float LengthTime(int index) {
		return index / (LookupTable().Length - 1.0f);
	}

	public float GetUnscaledTime(float t) {
		if (t == 0f || t == 1f) {
			return t;
		}
		float targetDistance = Length() * t;
		float[] lookupTable = LookupTable();
		int index = -1;
		for (int i = 0; i < lookupTable.Length; i++) {
			float lookupLength = lookupTable[i];
			if (lookupLength == targetDistance) {
				return LengthTime(i);
			}
			if (lookupLength > targetDistance) {
				index = i;
				break;
			}
		}
		if (index == 0) {
			index += 1;
		}
		float lookupLength1 = lookupTable[index - 1];
		float lookupLength2 = lookupTable[index];
		float length = lookupLength2 - lookupLength1;
		float position = (targetDistance - lookupLength1) / length;
		return Mathf.Lerp(LengthTime(index - 1), LengthTime(index), position);
	}

	public Vector3 GetPosition(float t, bool worldSpace = true) {
		t = Mathf.Clamp(t, 0f, 1f);
		if (worldSpace) {
			return transform.TransformPoint(InterpolatePosition(GetUnscaledTime(t)));
		} else {
			return InterpolatePosition(GetUnscaledTime(t));
		}
	}

	public Vector3 InterpolatePosition(float t) {
		t = Mathf.Clamp(t, 0f, 1f);
		if (knots == null) {
			Debug.LogError("CubicBezierSpline knots are null");
			return Vector3.zero;
		}
		if (knots.Count < 2) {
			return Vector3.zero;
		}
		if (t == 0f) {
			return knots[0].position;
		}
		if (t == 1f) {
			return knots[knots.Count - 1].position;
		}
		int seg = GetSegmentAtTime(ref t);
		if (t == 0f) {
			return knots[seg].position;
		}
		float d = 1f - t;
		return d * d * d * knots[seg].position + 3f * d * d * t * knots[seg].ctrl2 + 3f * d * t * t * knots[seg + 1].ctrl1 + t * t * t * knots[seg + 1].position;
	}

	public Vector3 GetTangent(float t, bool worldSpace = true) {
		t = Mathf.Clamp(t, 0f, 1f);
		if (knots == null) {
			Debug.LogError("CubicBezierSpline knots are null");
			return Vector3.zero;
		}
		if (knots.Count < 2) {
			return Vector3.zero;
		}
		float time = GetUnscaledTime(t);
		int seg = GetSegmentAtTime(ref time);
		Vector3 tangent = Vector3.zero;
		if (t == 0.0f) {
			tangent = (knots[seg].ctrl2 - knots[seg].position).normalized;
		} else {
			tangent = InterpolateTangent(time, knots[seg].position, knots[seg].ctrl2, knots[seg + 1].ctrl1, knots[seg + 1].position).normalized;
		}
		if (worldSpace) {
			return transform.TransformDirection(tangent);
		} else {
			return tangent;
		}
	}

	///////////////////// NEW STUFF
	public float FindNearestPointToProgress(Vector3 WorldPos, float Accuracy = 100f) {
		float NormalizedT = -1f;
		float Step = AccuracyToStepSize(Accuracy);
		float MinDistance = Mathf.Infinity;
		for (float i = 0f; i < 1f; i += Step) {
			Vector3 ThisPoint = GetPosition(i);
			float ThisDistance = (WorldPos - ThisPoint).sqrMagnitude;
			if (ThisDistance < MinDistance) {
				MinDistance = ThisDistance;
				NormalizedT = i;
			}
		}
		return NormalizedT;
	}
	private float AccuracyToStepSize(float Accuracy) {
		if (Accuracy <= 0f) {
			return 0.2f;
		}
		return Mathf.Clamp(1f / Accuracy, 0.001f, 0.2f);
	}
	////////////////////

	private Vector3 InterpolateTangent(float t, Vector3 p0, Vector3 h0, Vector3 h1, Vector3 p1) {
		float a = 1 - t;
		float b = a * 6 * t;
		a = a * a * 3;
		float c = t * t * 3;
		return -a * p0 + a * h0 - b * h0 - c * h1 + b * h1 + c * p1;
	}

	public void LogDistance() {
		Vector3 prevPt = GetPosition(0);
		for (int i = 1; i <= 20; i++) {
			float pm = (float)i / 20f;
			Vector3 currPt = GetPosition(pm);
			Debug.Log(Vector3.Distance(currPt, prevPt));
			prevPt = currPt;
		}
		Debug.Log("Total Length: " + Length());
	}

#if UNITY_EDITOR
	public void DrawBezier() {
		for (int seg = 0; seg < knots.Count - 1; seg++) {
			Handles.color = Color.white;
			Handles.DrawBezier(
			transform.TransformPoint(knots[seg].position),
			transform.TransformPoint(knots[seg + 1].position),
			transform.TransformPoint(knots[seg].ctrl2),
			transform.TransformPoint(knots[seg + 1].ctrl1),
			Color.white, null, 2.5f);
		}
	}

	public void OnDrawGizmosSelected() {
		if (knots == null || knots.Count < 2) {
			return;
		}
		DrawBezier();
		float handleSize = 0.125f;
		for (int i = 0; i < knots.Count; i++) {
			bool start = (i == 0);
			bool end = (i == knots.Count - 1);
			float scale = (start || end ? 1.25f : 1f);
			Handles.color = (start || end ? Color.red : Color.white);
			Handles.DotHandleCap(0, transform.TransformPoint(knots[i].position), Quaternion.identity, handleSize * scale, EventType.Repaint);
		}
	}
#endif
}