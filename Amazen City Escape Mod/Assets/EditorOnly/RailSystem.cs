using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

public class RailSystem : MonoBehaviour {

	public enum Mode {
		Single,
		Dual
	}

	public BezierCurve bezierCurve;
	public BezierCurve bezierCurve_R;
	public float ColliderWidth = 0.2f;
	public Mode SplineMode;

	[SerializeField]
	internal RailData[] RailPathData;

	void Start() {
		if (RailPathData == null) {
			RailPathData = GetRailDataArray();
		}
	}
	public RailData[] GetRailPathData() {
		return RailPathData;
	}

	public RailData GetRailData(float time) {
		Vector3 Position = Vector3.zero;
		Vector3 Tangent = Vector3.forward;
		Vector3 Normal = Vector3.up;
		if (SplineMode == Mode.Single) {
			Position = bezierCurve.GetPosition(time);
			Tangent = bezierCurve.GetTangent(time);
		} else {
			Vector3 LeftPosition = bezierCurve.GetPosition(time);
			Vector3 RightPosition = bezierCurve_R.GetPosition(time);
			Position = (LeftPosition + RightPosition) * 0.5f;
			Tangent = ((bezierCurve.GetTangent(time) + bezierCurve_R.GetTangent(time)) * 0.5f).normalized;
			Normal = Vector3.Cross(Tangent, (RightPosition - Position).normalized);
		}
		return new RailData(Position, Normal, Tangent);
	}

	public float Length() {
		if (SplineMode == Mode.Single) {
			return bezierCurve.Length();
		} else {
			return (bezierCurve.Length() + bezierCurve_R.Length()) * 0.5f;
		}
	}

	public Vector3[] TangentArray(float Distance = 1f) {
		int Count = Mathf.FloorToInt(bezierCurve.Length() / Distance);
		Vector3[] Tangent = new Vector3[Count + 1];
		Tangent[0] = bezierCurve.GetTangent(0f);
		for (int i = 0; i < Count; i++) {
			float Time = (i + 1f) / (Count * 1f);
			if (Time > 1f) {
				Debug.LogError("Time is greater than 1.0f; index: " + i + ", count: " + Count);
				Time = 1f;
			}
			Tangent[i] = bezierCurve.GetTangent(Time);
		}
		return Tangent;
	}

	public RailData[] GetRailDataArray(float Distance = 1f) {
		int Count = Mathf.FloorToInt(Length() / Distance);
		RailData[] Rail_Data = new RailData[Count + 1];
		Rail_Data[0] = GetRailData(0f);
		for (int i = 0; i < Count; i++) {
			float Time = (i + 1f) / (Count * 1f);
			if (Time > 1f) {
				Debug.LogError("Time is greater than 1.0f; index: " + i + ", count: " + Count);
				Time = 1f;
			}
			Rail_Data[i + 1] = GetRailData(Time);
		}
		return Rail_Data;
	}

#if UNITY_EDITOR
	private void CreateRailPathData() {
		transform.position = bezierCurve.transform.position;
		transform.rotation = bezierCurve.transform.rotation;
		RailData[] RailData = GetRailDataArray();
		string TargetsName = "targets";

		GameObject Targetsbj = gameObject.FindInChildren(TargetsName);
		if (Targetsbj) {
			DestroyImmediate(Targetsbj);
		}
		Targetsbj = new GameObject(TargetsName);
		Targetsbj.transform.parent = transform;
		Targetsbj.name = TargetsName;
		Targetsbj.transform.localPosition = Vector3.zero;
		Targetsbj.transform.localRotation = Quaternion.identity;
		RailPathData = new RailData[RailData.Length];
		for (int i = 0; i < RailData.Length; i++) {
			string TargetName = gameObject.name + "_htarget" + i.ToString("D2");
			GameObject HomingTarget = new GameObject(TargetName) {
				tag = "RailHomingTarget"
			};
			HomingTarget.transform.parent = Targetsbj.transform;
			HomingTarget.transform.position = RailData[i].position;
			HomingTarget.transform.rotation = Quaternion.LookRotation(RailData[i].tangent, RailData[i].normal);
			if (i == 0 || i == RailData.Length - 1) {
				DestroyImmediate(HomingTarget);
			}
			RailPathData[i] = new RailData(RailData[i].position, RailData[i].tangent);
		}
	}
	private void CreateCollMesh() {
		transform.position = bezierCurve.transform.position;
		transform.rotation = bezierCurve.transform.rotation;
		RailData[] RailData = GetRailDataArray(2f);
		int Count = RailData.Length;
		Mesh CollisionMesh = new Mesh();
		if (CollisionMesh != null) {
			CollisionMesh.Clear();
		}
		Vector3[] Vertices = new Vector3[Count * 2];
		Vector2[] UVs = new Vector2[Count * 2];
		int[] Triangles = new int[(Count - 1) * 6];
		for (int i = 0; i < Count; i++) {
			Vector3 Right = Vector3.Cross(RailData[i].normal, RailData[i].tangent).normalized;
			Vector3 LocalPosition = transform.InverseTransformPoint(RailData[i].position);
			Vertices[i * 2] = LocalPosition + (Right * ColliderWidth); //Right vertex
			Vertices[(i * 2) + 1] = LocalPosition - (Right * ColliderWidth); //Left Vertex
			UVs[i * 2] = new Vector2(0f, i);
			UVs[(i * 2) + 1] = new Vector2(1f, i);
			if (i > 0) {
				int triIndex = (i - 1) * 6;
				int vertIndex = i * 2;
				Triangles[triIndex] = vertIndex - 2;
				Triangles[triIndex + 1] = vertIndex - 1;
				Triangles[triIndex + 2] = vertIndex;
				Triangles[triIndex + 3] = vertIndex;
				Triangles[triIndex + 4] = vertIndex - 1;
				Triangles[triIndex + 5] = vertIndex + 1;
			}
		}
		CollisionMesh.vertices = Vertices;
		CollisionMesh.uv = UVs;
		CollisionMesh.triangles = Triangles;
		CollisionMesh.RecalculateNormals();
		CollisionMesh.RecalculateBounds();
		string MeshColliderName = "collider";
		GameObject ColliderObj = gameObject.FindInChildren(MeshColliderName);
		if (!ColliderObj) {
			ColliderObj = new GameObject(MeshColliderName);
			ColliderObj.transform.parent = transform;
		}
		ColliderObj.name = MeshColliderName;
		ColliderObj.transform.localPosition = Vector3.zero;
		ColliderObj.transform.localRotation = Quaternion.identity;
		MeshCollider MeshCollider = ColliderObj.GetComponent<MeshCollider>();
		if (!MeshCollider) {
			MeshCollider = ColliderObj.AddComponent<MeshCollider>();
			ColliderObj.layer = 17;
		}
		string[] Folder = EditorSceneManager.GetActiveScene().path.Split(char.Parse("."));
		string AssetPath = Folder[0] + "/" + gameObject.name + "_mesh" + ".asset";
		if (AssetDatabase.LoadAssetAtPath(AssetPath, typeof(Mesh)) as Mesh) {
			AssetDatabase.DeleteAsset(AssetPath);
		}
		AssetDatabase.Refresh();
		AssetDatabase.CreateAsset(CollisionMesh, AssetPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Mesh MeshAsset = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(Mesh)) as Mesh;
		MeshCollider.sharedMesh = null;
		MeshCollider.sharedMesh = MeshAsset;
	}
	private void CreateWindRoadMesh() {
		transform.position = bezierCurve.transform.position;
		transform.rotation = bezierCurve.transform.rotation;
		RailData[] RailData = GetRailDataArray();
		int Count = RailData.Length;
		Mesh WindRoadMesh = new Mesh();
		WindRoadMesh.Clear();
		var Vert = 5;
		var Faces = 8;
		Vector3[] Vertices = new Vector3[Count * Vert];
		Color[] Colors = new Color[Count * Vert];
		Vector2[] UVs = new Vector2[Count * Vert];
		var Tris = (Count - 1) * Vert * Faces;
		while (Tris % 3 != 0) { // MUST be a multiple of three
			Tris++;
		}
        int[] Triangles = new int[Tris];
		for (var i = 0; i < Count; i++) {
			var iN = i * Vert;
			for (var v = 0; v < Vert; v++) {
				var ThisV = iN + v;
				Colors[ThisV] = Color.white;
				if (i == 0 || i == Count - 1) {
					Colors[ThisV] = Color.clear;
				}
			}
			var Up = RailData[i].normal;
			var Right = Vector3.Cross(Up, RailData[i].tangent).normalized;
			var LocalPosition = transform.InverseTransformPoint(RailData[i].position);
			Vertices[iN] = LocalPosition + (Right * 0.85f) + (Up * 0.45f); //Rightmost vertex
			Vertices[iN + 1] = LocalPosition + (Right * 0.5f) + (Up * 0.125f); //Right vertex			
			Vertices[iN + 2] = LocalPosition; //Center vertex			
			Vertices[iN + 3] = LocalPosition - (Right * 0.5f) + (Up * 0.125f); //Left Vertex
			Vertices[iN + 4] = LocalPosition - (Right * 0.85f) + (Up * 0.45f); //Leftmost Vertex
			UVs[iN] = new Vector2(0f, i);
			UVs[iN + 1] = new Vector2(0.25f, i);
			UVs[iN + 2] = new Vector2(0.5f, i);
			UVs[iN + 3] = new Vector2(0.75f, i);
			UVs[iN + 4] = new Vector2(1f, i);
			if (i > 0) {
				var triIndex = (i - 1) * Vert * 6; // I have no idea why this works, should be Faces(8) instead of 6
				var next_rightmost = iN;
				var next_right = iN + 1;
				var next_center = iN + 2;
				var next_left = iN + 3;
				var next_leftmost = iN + 4;
				var next_iN = (i - 1) * Vert;
				var rightmost = next_iN;
				var right = next_iN + 1;
				var center = next_iN + 2;
				var left = next_iN + 3;
				var leftmost = next_iN + 4;
				Triangles[triIndex] = rightmost;
				Triangles[triIndex + 1] = right;
				Triangles[triIndex + 2] = next_rightmost;
				Triangles[triIndex + 3] = next_right;
				Triangles[triIndex + 4] = next_rightmost;
				Triangles[triIndex + 5] = right;
				Triangles[triIndex + 6] = right;
				Triangles[triIndex + 7] = center;
				Triangles[triIndex + 8] = next_right;
				Triangles[triIndex + 9] = next_center;
				Triangles[triIndex + 10] = next_right;
				Triangles[triIndex + 11] = center;
				Triangles[triIndex + 12] = center;
				Triangles[triIndex + 13] = left;
				Triangles[triIndex + 14] = next_center;
				Triangles[triIndex + 15] = next_left;
				Triangles[triIndex + 16] = next_center;
				Triangles[triIndex + 17] = left;
				Triangles[triIndex + 18] = left;
				Triangles[triIndex + 19] = leftmost;
				Triangles[triIndex + 20] = next_left;
				Triangles[triIndex + 21] = next_leftmost;
				Triangles[triIndex + 22] = next_left;
				Triangles[triIndex + 23] = leftmost;
			}
		}
		WindRoadMesh.vertices = Vertices;
		WindRoadMesh.colors = Colors;
		WindRoadMesh.uv = UVs;
		WindRoadMesh.triangles = Triangles;
		WindRoadMesh.RecalculateNormals();
		WindRoadMesh.RecalculateBounds();
		var MeshObj = gameObject.FindInChildren("mesh");
		if (!MeshObj) {
			MeshObj = gameObject.FindInChildren("mesh");
		}
		if (!MeshObj) {
			MeshObj = new GameObject("mesh");
			MeshObj.transform.parent = transform;
		}
		MeshObj.transform.localPosition = Vector3.zero;
		MeshObj.transform.localRotation = Quaternion.identity;
		string[] Folder = EditorSceneManager.GetActiveScene().path.Split(char.Parse("."));
		string AssetPath = Folder[0] + "/" + gameObject.name + "_windroadmesh" + ".asset";
		if (AssetDatabase.LoadAssetAtPath(AssetPath, typeof(Mesh)) as Mesh) {
			AssetDatabase.DeleteAsset(AssetPath);
		}
		AssetDatabase.Refresh();
		AssetDatabase.CreateAsset(WindRoadMesh, AssetPath);
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();
		Mesh MeshAsset = AssetDatabase.LoadAssetAtPath(AssetPath, typeof(Mesh)) as Mesh;
		var MeshFilter = MeshObj.GetComponent<MeshFilter>();
		if (!MeshFilter) {
			MeshFilter = MeshObj.AddComponent<MeshFilter>();
		}
		MeshFilter.mesh = null;
		MeshFilter.mesh = MeshAsset;
	}
	public void UpdateRailPathData() {
		CreateRailPathData();
	}
	public void UpdateCollisionMesh() {
		CreateCollMesh();
	}
	public void UpdateWindRoadMesh() {
		CreateWindRoadMesh();
	}
	public void BuildWindRoad() {
		UpdateRailPathData();
		UpdateCollisionMesh();
		UpdateWindRoadMesh();
	}

#endif
}