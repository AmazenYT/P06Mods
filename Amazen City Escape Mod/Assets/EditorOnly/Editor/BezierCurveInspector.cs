using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BezierCurve))]
public class BezierCurveInspector : Editor {

	private BezierCurve curve;

	int SelectedKnot() {
		if (selectedIndex >= curve.knots.Count * 2)
			return selectedIndex - (curve.knots.Count * 2);
		else if (selectedIndex >= curve.knots.Count)
			return selectedIndex - curve.knots.Count;

		return selectedIndex;
	}

	public override void OnInspectorGUI() {
		DrawDefaultInspector();

		curve = target as BezierCurve;

		if (selectedIndex > -1 && selectedIndex < curve.knots.Count) {
			Knot knot = curve.knots[selectedIndex];

			Knot.HandleType handleType = (Knot.HandleType)EditorGUILayout.EnumPopup("Knot Handle Type: ", knot.type);

			if (knot.type != handleType) {
				knot.type = handleType;
				curve.UpdateKnot(selectedIndex);
			}
		}

		if (selectedIndex == -1) {
			if (GUILayout.Button("Insert in Start"))
				curve.InsertKnot(0);

			if (GUILayout.Button("Add to End"))
				curve.AddKnot();
		} else {
			int selectedKnot = SelectedKnot();

			if (GUILayout.Button("Insert Before"))
				curve.InsertKnot(selectedKnot);

			if (GUILayout.Button("Insert After"))
				curve.InsertKnot(selectedKnot + 1);
		}

		if (GUILayout.Button("Reverse Order"))
			curve.knots.Reverse();


		if (GUILayout.Button("Debug - Log Distance"))
			curve.LogDistance();

		EditorUtility.SetDirty(curve);
	}

	private Transform handleTransform;
	private Quaternion handleRotation;
	private int selectedIndex = -1;
	private const float handleSize = 0.1f;
	private const float pickSize = 0.18f;

	private void OnSceneGUI() {
		curve = target as BezierCurve;
		handleTransform = curve.transform;
		handleRotation = Tools.pivotRotation == PivotRotation.Local ?
			handleTransform.rotation : Quaternion.identity;

		// DrawPoint
		for (int i = 0; i < curve.knots.Count; i++) {
			bool start = (i == 0);
			bool end = (i == curve.knots.Count - 1);

			Knot knot = curve.knots[i];

			Vector3 position = knot.position;

			Vector3 screenPos = Camera.current.WorldToScreenPoint(handleTransform.TransformPoint(position));

			if (screenPos.x >= 0 && screenPos.x <= Camera.current.pixelWidth && screenPos.y >= 0 && screenPos.y <= Camera.current.pixelHeight && screenPos.z > 0 && screenPos.z < 500) {
				Handles.color = Color.white;
				Handles.Label(handleTransform.TransformPoint(position), "   " + i);
			}

			Color handleColor = (start || end ? Color.red : Color.white);
			float scale = (start || end ? 1.25f : 1.0f);

			bool updateTangent = !(knot.type == Knot.HandleType.Broken || knot.type == Knot.HandleType.Auto);

			if (!start && updateTangent)
				knot.ctrl1 = ShowPoint(knot.ctrl1, curve.knots.Count + i, Color.green, 1, 1.0f);
			knot.position = ShowPoint(position, i, handleColor, 0, scale);
			if (!end && updateTangent)
				knot.ctrl2 = ShowPoint(knot.ctrl2, (curve.knots.Count * 2) + i, Color.green, 2, 1.0f);

			Handles.color = Color.yellow;
			if (!start)
				Handles.DrawAAPolyLine(2.5f, handleTransform.TransformPoint(knot.position), handleTransform.TransformPoint(knot.ctrl1));
			if (!end)
				Handles.DrawAAPolyLine(2.5f, handleTransform.TransformPoint(knot.position), handleTransform.TransformPoint(knot.ctrl2));
		}
	}

	private Vector3 ShowPoint(Vector3 point, int index, Color color, int alignMode, float scale) {
		Handles.color = color;

		point = handleTransform.TransformPoint(point);

		if (Handles.Button(point, handleRotation, handleSize * scale, pickSize * scale, Handles.DotHandleCap)) {
			selectedIndex = index;
		}

		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck();
			Vector3 lastPoint = point;
			point = Handles.DoPositionHandle(point, Quaternion.identity);

			if (EditorGUI.EndChangeCheck()) {
				Vector3 moveDirection = point - lastPoint;
				int knotIndex = SelectedKnot();

				Knot knot = curve.knots[knotIndex];

				if (selectedIndex < curve.knots.Count) {
					knot.ctrl1 += moveDirection;
					knot.ctrl2 += moveDirection;
					curve.UpdateKnot(selectedIndex);
				}

				if (alignMode != 0 && knot.type == Knot.HandleType.Aligned) {
					if (alignMode == 1) {
						knot.ctrl2 = knot.position + (knot.position - knot.ctrl1);
					} else {
						knot.ctrl1 = knot.position + (knot.position - knot.ctrl2);
					}
				}

				Undo.RecordObject(curve, "Move Point");
				EditorUtility.SetDirty(curve);
			}
		}

		return handleTransform.InverseTransformPoint(point);
		;
	}
}