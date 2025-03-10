using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
	public class PathSystem : MonoBehaviour
	{
		public BezierCurve LeftSpline;

		public BezierCurve RightSpline;

		public bool CanJumpOff;

		public float YOffset;

		public float Distance = 1f;

		public int TotalPath;

		public PathData LeftPathArray;

		public PathData CenterPathArray;

		public PathData RightPathArray;

		private void OnDrawGizmosSelected()
		{
			if (TotalPath != -1 && (bool)LeftSpline && (bool)RightSpline)
			{
				for (int i = 0; i < CenterPathArray.position.Length; i++)
				{
					Vector3 direction = CenterPathArray.normal[i];
					Gizmos.DrawRay(LeftPathArray.position[i], direction);
					Gizmos.DrawRay(RightPathArray.position[i], direction);
				}
			}
		}

		private void DrawArrayLine(int i, int max, Vector3[] position, Color color, float duration = 0f)
		{
			if (i < max - 1)
			{
				Debug.DrawLine(position[i], position[i + 1], color, duration);
			}
		}

		public int FindClosestPoint(PathData pathData, Vector3 position, Vector3 direction)
		{
			int result = -1;
			float num = 250f;
			for (int i = 0; i < pathData.position.Length; i++)
			{
				float sqrMagnitude = (pathData.position[i] - position).sqrMagnitude;
				if (sqrMagnitude > 0.1f && sqrMagnitude < num && Vector3.Dot((pathData.position[i] - position).normalized, direction) > 0f)
				{
					result = i;
					num = sqrMagnitude;
				}
			}
			return result;
		}

		public int FindClosestPointCenter(Vector3 position, Vector3 direction)
		{
			int result = -1;
			float num = 100f;
			for (int i = 0; i < CenterPathArray.position.Length; i++)
			{
				float sqrMagnitude = (CenterPathArray.position[i] - position).sqrMagnitude;
				if (sqrMagnitude > 0.5f && sqrMagnitude < num && Vector3.Dot((CenterPathArray.position[i] - position).normalized, direction) > 0f)
				{
					result = i;
					num = sqrMagnitude;
				}
			}
			return result;
		}

		public float PathDist(Vector3 position, Vector3 direction)
		{
			int num = FindClosestPointCenter(position, direction);
			if (num == -1)
			{
				return -1f;
			}
			Vector3 vector = LeftPathArray.position[num];
			Vector3 vector2 = RightPathArray.position[num];
			float num2 = Vector3.Distance(vector, vector2);
			return Mathf.Min(Vector3.Distance(Math3D.ClosestPointOnLine(vector, vector2, position), vector) / num2, 1f);
		}

		public PathData BuildPathData(float distance)
		{
			Vector3[] array = new Vector3[CenterPathArray.position.Length];
			for (int i = 0; i < array.Length; i++)
			{
				Vector3 vector = LeftPathArray.position[i];
				Vector3 vector2 = RightPathArray.position[i];
				Vector3 normalized = (vector2 - vector).normalized;
				float d = Vector3.Distance(vector, vector2);
				array[i] = vector + normalized * distance * d;
				Debug.DrawRay(array[i], CenterPathArray.normal[i], Color.yellow, 10f);
			}
			for (int j = 0; j < array.Length; j++)
			{
				DrawArrayLine(j, array.Length, array, Color.yellow, 10f);
			}
			return new PathData(array, CenterPathArray.normal);
		}

		public void UpdatePath()
		{
			float length = (LeftSpline.Length() + RightSpline.Length()) / 2f;
			LeftPathArray = BezierToPathData(LeftSpline, length, Distance);
			RightPathArray = BezierToPathData(RightSpline, length, Distance);
			TotalPath = Mathf.Min(LeftPathArray.position.Length, RightPathArray.position.Length);
			CenterPathArray = PathDataCenter(TotalPath, LeftPathArray, RightPathArray);
		}

		internal PathData BezierToPathData_Distance(PolyBezier bezierScript)
		{
			return new PathData(bezierScript.bez.GetPointAtDistance(1f, base.transform), bezierScript.bez.GetTangentAtDistance());
		}

		private PathData BezierToPathData(BezierCurve bezierCurve, float length, float distance = 1f)
		{
			float num = 0f;
			float num2 = distance / length;
			List<Vector3> list = new List<Vector3>();
			List<Vector3> list2 = new List<Vector3>();
			for (num = 0f; num < 1f; num += num2)
			{
				list.Add(bezierCurve.GetPosition(num));
				list2.Add(bezierCurve.GetTangent(num));
			}
			if (num - num2 < 1f)
			{
				list.Add(bezierCurve.GetPosition(1f));
				list2.Add(bezierCurve.GetTangent(1f));
			}
			return new PathData(list.ToArray(), list2.ToArray());
		}

		internal PathData PathDataCenter(int length, PathData left, PathData right)
		{
			Vector3[] array = new Vector3[length];
			Vector3[] array2 = new Vector3[length];
			for (int i = 0; i < length; i++)
			{
				Vector3 normalized = (left.normal[i] + right.normal[i]).normalized;
				Vector3 normalized2 = (right.position[i] - left.position[i]).normalized;
				array2[i] = Vector3.Cross(normalized, normalized2);
				array[i] = (left.position[i] + right.position[i]) * 0.5f;
			}
			return new PathData(array, array2);
		}
	}
public static class Math3D
{
	// Token: 0x06000FF8 RID: 4088 RVA: 0x000BB198 File Offset: 0x000B9398
	public static Vector3 ClosestPointOnLine(Vector3 start, Vector3 end, Vector3 pnt)
	{
		Vector3 vector = end - start;
		float magnitude = vector.magnitude;
		vector.Normalize();
		float num = Vector3.Dot(pnt - start, vector);
		if (num < 0f)
		{
			return start;
		}
		if (num > magnitude)
		{
			return end;
		}
		num = Mathf.Clamp(num, 0f, magnitude);
		return start + vector * num;
	}
}
