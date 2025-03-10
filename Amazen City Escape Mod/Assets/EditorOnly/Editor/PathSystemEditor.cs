#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(PathSystem))]
public class PathSystemEditor : Editor {
	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		PathSystem PathSystem = (PathSystem)target;
		if (GUILayout.Button("Update Path System")) {
			PathSystem.UpdatePath();
		}
	}
}
#endif