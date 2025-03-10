#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RailSystem))]
public class RailSystemEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		RailSystem RailSystem = (RailSystem)target;
		if (GUILayout.Button("Create Path Data")) {
			RailSystem.UpdateRailPathData();
		}
		if (GUILayout.Button("Create Collision Mesh")) {
			RailSystem.UpdateCollisionMesh();
		}
		if(GUILayout.Button("Create WindRoad Mesh")) {
			RailSystem.UpdateWindRoadMesh();
		}
		if (GUI.changed) {
			EditorUtility.SetDirty(RailSystem);
		}
	}
}
#endif