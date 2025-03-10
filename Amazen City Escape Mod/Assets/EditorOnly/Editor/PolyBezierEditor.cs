using UnityEditor;
using UnityEngine;
using System.Collections;
using System;
using System.Reflection;

public class DefaultHandles {
	public static bool Hidden {
		get {
			Type type = typeof(Tools);
			FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
			return ((bool)field.GetValue(null));
		}
		set {
			Type type = typeof(Tools);
			FieldInfo field = type.GetField("s_Hidden", BindingFlags.NonPublic | BindingFlags.Static);
			field.SetValue(null, value);
		}
	}
}

[CustomEditor(typeof(PolyBezier))]
public class PolyBezierEditor : Editor {

	private enum cpte { Node, HandleA, HandleB };
	private cpte curPointType = cpte.Node;
	int curPointIndex = -1;
	bool hideDefaultHandle = true;

	void OnEnable() {
		DefaultHandles.Hidden = hideDefaultHandle;
	}
	void OnDisable() {
		DefaultHandles.Hidden = false;
	}

	void MoveNode(PolyBezier polyBezier, Vector3 localPos, int i) {
		polyBezier.bez.MoveNode(localPos, i);
		EditorUtility.SetDirty(target);
	}

	void MoveHandle(PolyBezier polyBezier, Vector3 localPos, int i, bool handleA) {
		polyBezier.bez.MoveHandle(localPos, i, handleA);
		EditorUtility.SetDirty(target);
	}

	void RotateHandles(PolyBezier polyBezier, Quaternion qDelta, int i) {
		Vector3 p = polyBezier.bez.nodes[i];
		Vector3 ha = polyBezier.bez.handlesA[i];
		Vector3 hb = polyBezier.bez.handlesB[i];
		polyBezier.bez.handlesA[i] = p + qDelta * (ha - p);
		polyBezier.bez.handlesB[i] = p + qDelta * (hb - p);
		EditorUtility.SetDirty(target);
	}

	void DeleteNode(PolyBezier polyBezier, int index) {
		int nl = polyBezier.bez.nodes.Length;
		if (index < 0 || index >= nl) {
			return;
		}
		polyBezier.bez.DeleteNode(index);
		if (nl == 0) {
			curPointIndex = -1;
			RectifyCurrentPointEnum(polyBezier);
		} else {
			if (index != 0) {
				curPointIndex -= 1;
				RectifyCurrentPointEnum(polyBezier);
			}
		}
		EditorUtility.SetDirty(target);
	}

	void AddNode(PolyBezier polyBezier, int position) {
		int nl = polyBezier.bez.nodes.Length;
		Vector3 p;
		Vector3 h;
		Vector3 p2;
		Vector3[] nodes = polyBezier.bez.nodes;
		if (nl == 0) {
			p = Vector3.zero;
			h = Vector3.right * 10;
			polyBezier.bez.InsertNode(0, p, p + h, p - h);
		} else if (nl == 1) {
			if (position == 0) {
				p = Vector3.zero;
				h = Vector3.right * 10;
				polyBezier.bez.InsertNode(0, p, p + h, p - h);
			} else {
				p = nodes[0] * 2;
				p2 = p - nodes[0];
				polyBezier.bez.InsertNode(1, p, polyBezier.bez.handlesA[0] + p2, polyBezier.bez.handlesB[0] + p2);
			}
		} else {
			if (position == -1) {
				return;
			}
			if (position == 0) {
				p = nodes[position];
				p2 = p - nodes[position + 1];
				polyBezier.bez.InsertNode(position, p + p2 * 2 / 4, p + p2 * 3 / 4, p + p2 * 1 / 4);
			} else if (position == nl) {
				p = nodes[position - 1];
				p2 = p - nodes[position - 2];
				polyBezier.bez.InsertNode(position, p + p2 * 2 / 4, p + p2 * 1 / 4, p + p2 * 3 / 4);
			} else {
				p = 0.5f * (nodes[position - 1] + nodes[position]);
				Vector3 pMid = 0.5f * (p - polyBezier.bez.nodes[position - 1]);
				polyBezier.bez.InsertNode(position, p, p - pMid, p + pMid);
			}
		}
		EditorUtility.SetDirty(target);
	}

	void ResetHandles(PolyBezier polyBezier, int i) {
		Vector3 p = polyBezier.bez.nodes[i];
		polyBezier.bez.handlesA[i] = p - Vector3.right * 4;
		polyBezier.bez.handlesB[i] = p + Vector3.right * 4;
		EditorUtility.SetDirty(target);
	}

	void ClearAll(PolyBezier polyBezier) {
		polyBezier.bez.DeleteAllNodes();
	}

	void insideSceneGUI(PolyBezier polyBezier) {
		Rect size = new Rect(0, 0, 300, 160 + 10);
		float sizeButton = 30;
		Handles.BeginGUI();
		GUI.BeginGroup(new Rect(Screen.width - size.width - 10, Screen.height - size.height - 50, size.width, size.height));
		GUI.Box(size, "PolyBezier Tool Bar");
		Rect rc = new Rect(0, 15, size.width, sizeButton);
		GUI.contentColor = Color.black;
		GUI.Label(rc, "Clic on Circles to select a point");
		GUI.contentColor = Color.white;
		rc.y += 15;
		if (curPointIndex != -1) {
			GUI.contentColor = Color.black;
			GUI.Label(rc, "Current Point " + curPointIndex);
			GUI.contentColor = Color.white;
			rc.y -= 25; // back up
			rc.x = size.width / 5 * 4;
			rc.width = size.width / 5;
			if (GUI.Button(rc, "Deselect")) {
				curPointIndex = -1;
				RectifyCurrentPointEnum(polyBezier);
			}
			rc.y += 25; // back down
			rc.x = 0;
			rc.y += 15;
			rc.width = size.width / 3;
			if (GUI.Button(rc, "Insert Before")) {
				AddNode(polyBezier, curPointIndex);
			}
			rc.x += rc.width;
			if (GUI.Button(rc, "Delete"))
				DeleteNode(polyBezier, curPointIndex);
			rc.x += rc.width;
			if (GUI.Button(rc, "Insert After")) {
				AddNode(polyBezier, curPointIndex + 1);
				curPointIndex++;
				RectifyCurrentPointEnum(polyBezier);
			}
			rc.x = 0;
			rc.y += 10;
			rc.y += sizeButton;
			if (GUI.Button(rc, "Reset Handles")) {
				ResetHandles(polyBezier, curPointIndex);
			}
		} else {
			if (polyBezier.bez.nodes.Length == 0) {
				if (GUI.Button(rc, "Insert"))
					AddNode(polyBezier, 0);
				rc.y += sizeButton;
			} else {
				rc.width = size.width / 2;

				if (GUI.Button(rc, "Insert First")) {
					AddNode(polyBezier, 0);
					curPointIndex = 0;
					RectifyCurrentPointEnum(polyBezier);
				}
				rc.x += rc.width;
				if (GUI.Button(rc, "Insert Last")) {
					AddNode(polyBezier, polyBezier.bez.nodes.Length);
					curPointIndex = polyBezier.bez.nodes.Length - 1;
					RectifyCurrentPointEnum(polyBezier);
				}
				rc.y += sizeButton;
			}
		}
		if (polyBezier.bez.nodes.Length > 0) {
			rc.width = size.width / 2;
			rc.x = 0;
			rc.y += sizeButton + 10;
			if (GUI.Button(rc, "Clear All")) {
				ClearAll(polyBezier);
				curPointIndex = -1;
				RectifyCurrentPointEnum(polyBezier);
			}
		}
		rc.x += rc.width;
		if (hideDefaultHandle) {
			if (GUI.Button(rc, "Show Main Transform")) {
				hideDefaultHandle = false;
				DefaultHandles.Hidden = hideDefaultHandle;
			}
		} else {
			if (GUI.Button(rc, "Hide Main Transform")) {
				hideDefaultHandle = true;
				DefaultHandles.Hidden = hideDefaultHandle;
			}
		}
		GUI.EndGroup();
		Handles.EndGUI();
	}

	void OnSceneGUI() {
		PolyBezier polyBezier = (PolyBezier)target;
		DisplayBezier(polyBezier);
		DisplayPointsAndLines(polyBezier);
		SceneView.RepaintAll();
	}

	void DisplayBezier(PolyBezier polyBezier) {
		int l = polyBezier.bez.GetNumberOfSegment();
		if (l == 0) {
			return;
		}
		Transform tr = polyBezier.transform;
		Vector3[,] h = polyBezier.bez.GetBezierInHandlesFormat();
		for (int i = 0; i < l; i++) {
			for (int j = 0; j < 4; j++)
				h[i, j] = tr.TransformPoint(h[i, j]);
			Handles.DrawBezier(h[i, 0], h[i, 1], h[i, 2], h[i, 3], Color.white, null, 2);
		}
	}

	private const float handleSize = 0.05f;
	private const float pickSize = 0.06f;
	private const float lineWidth = 4f;

	void DisplayPointsAndLines(PolyBezier polyBezier) {
		Transform tr = polyBezier.transform;
		int nl = polyBezier.bez.nodes.Length;
		Vector3[] worldNodes = new Vector3[nl];
		Vector3[] worldHandlesA = new Vector3[nl];
		Vector3[] worldHandlesB = new Vector3[nl];
		for (int i = 0; i < nl; i++) {
			Vector3 node = tr.TransformPoint(polyBezier.bez.nodes[i]);
			Vector3 hanA = tr.TransformPoint(polyBezier.bez.handlesA[i]);
			Vector3 hanB = tr.TransformPoint(polyBezier.bez.handlesB[i]);
			Vector3 oncam = Camera.current.WorldToScreenPoint(node);
			if ((oncam.x >= 0) && (oncam.x <= Camera.current.pixelWidth) && (oncam.y >= 0) && (oncam.y <= Camera.current.pixelHeight) && (oncam.z > 0) && (oncam.z < 500)) {
				Handles.color = Color.white;
				Handles.Label(node, "   " + i);
			}
			float size = 3f;
			if (i == 0 || i == nl - 1) {
				size *= 1.25f;
				Handles.color = Color.red;
			}
			if (Handles.Button(node, Quaternion.identity, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
				curPointIndex = i;
				curPointType = cpte.Node;
				RectifyCurrentPointEnum(polyBezier);
				Repaint();
			}
			Handles.color = Color.green;
			if (i != 0) {
				if (Handles.Button(hanA, Quaternion.identity, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
					curPointIndex = i;
					curPointType = cpte.HandleA;
					RectifyCurrentPointEnum(polyBezier);
					Repaint();
				}
				Handles.color = Color.yellow;
				Handles.DrawAAPolyLine((size / HandleUtility.GetHandleSize(hanB)) * lineWidth, new[] { hanA, node }); //DrawLine(hanA, node);
			}
			Handles.color = Color.green;
			if (i != nl - 1) {
				if (Handles.Button(hanB, Quaternion.identity, size * handleSize, size * pickSize, Handles.DotHandleCap)) {
					curPointIndex = i;
					curPointType = cpte.HandleB;
					RectifyCurrentPointEnum(polyBezier);
					Repaint();
				}
				Handles.color = Color.yellow;
				Handles.DrawAAPolyLine((size / HandleUtility.GetHandleSize(hanB)) * lineWidth, new[] { hanB, node });//DrawLine(hanB, node);
			}
			if (curPointIndex == i) {
				if (curPointType == cpte.Node) {
					EditorGUI.BeginChangeCheck();
					node = Handles.DoPositionHandle(node, Quaternion.identity);
					if (EditorGUI.EndChangeCheck()) {
						MoveNode(polyBezier, tr.InverseTransformPoint(node), i);
					}
				} else if (curPointType == cpte.HandleA) {
					EditorGUI.BeginChangeCheck();
					hanA = Handles.DoPositionHandle(hanA, Quaternion.identity);
					if (EditorGUI.EndChangeCheck()) {
						MoveHandle(polyBezier, tr.InverseTransformPoint(hanA), i, true);
					}
				} else {
					EditorGUI.BeginChangeCheck();
					hanB = Handles.DoPositionHandle(hanB, Quaternion.identity);
					if (EditorGUI.EndChangeCheck()) {
						MoveHandle(polyBezier, tr.InverseTransformPoint(hanB), i, false);
					}
				}
			}
			worldNodes[i] = node;
			worldHandlesA[i] = hanA;
			worldHandlesB[i] = hanB;
		}
		insideSceneGUI(polyBezier);
	}

	int controlIDBeforeHandle;
	bool isEventUsedBeforeHandle;

	private void SetControlIDState(int hashCode) {
		controlIDBeforeHandle = GUIUtility.GetControlID(hashCode, FocusType.Passive);
		isEventUsedBeforeHandle = (Event.current.type == EventType.Used);
	}

	private bool GetControlIDState(int hashCode) {
		int controlIDAfterHandle = GUIUtility.GetControlID(hashCode, FocusType.Passive);
		bool isEventUsedByHandle = !isEventUsedBeforeHandle && (Event.current.type == EventType.Used);
		if ((controlIDBeforeHandle < GUIUtility.hotControl && GUIUtility.hotControl < controlIDAfterHandle) || isEventUsedByHandle) {
			return true;
		}
		return false;
	}

	void DisplayProgression(PolyBezier polyBezier) {
		float t = polyBezier.t;
		t = Mathf.Clamp01(t);
		polyBezier.t = t;
		if (t == 0) {
			float tAuto = (Time.realtimeSinceStartup / polyBezier.bez.nodes.Length) % 1;
			t = tAuto;
		}
		Transform tr = polyBezier.transform;
		Vector3 p = polyBezier.bez.GetPointAtTime(t);
		p = tr.TransformPoint(p);
		Vector3 tan = polyBezier.bez.GetTangentAtTime(t);
		tan = tr.TransformDirection(tan);
		Handles.color = Color.green;
		Handles.DrawWireDisc(p, tan, 1);
		Handles.DrawLine(p, p + tan * 1);
		float tRect = t;
		int segment = polyBezier.bez.GetSegmentAtTime(ref tRect);
		Handles.Label(p, "     " + Math.Round(t, 2) + "\n" + "segment : " + segment + "\n" + "local t : " + Math.Round(tRect, 2));
	}

	override public void OnInspectorGUI() {
		base.OnInspectorGUI();
	}

	private void RectifyCurrentPointEnum(PolyBezier polyBezier) {
		if (curPointIndex == -1) {
			curPointType = cpte.Node;
		}
		if (curPointIndex == 0 && curPointType == cpte.HandleA) {
			curPointType = cpte.HandleB;
		}
		if (curPointIndex == polyBezier.bez.nodes.Length - 1 && curPointType == cpte.HandleB) {
			curPointType = cpte.HandleA;
		}
	}
}