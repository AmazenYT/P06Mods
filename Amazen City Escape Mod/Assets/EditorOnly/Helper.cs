using System.Linq;
using UnityEngine;
using System.IO;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
#if UNITY_EDITOR
using UnityEditor;
#endif
 
public static class Helper {

	public static Transform FindInChildren(this Transform InTransform, string InName) {
		foreach (Transform _Transform in InTransform) {
			if (_Transform.name == InName) {
				return _Transform;
			}
		}
		return null;
	}

    public static GameObject FindInChildren(this GameObject InTransform, string InName) {
        foreach (Transform _Transform in InTransform.transform) {
			if (_Transform.name == InName) {
				return _Transform.gameObject;
			}
		}
		return null;
    }

	public static Transform FindInChildren(this Transform InTransform, string InName, bool Recursive = false) {
		foreach (Transform _Transform in InTransform) {
			if(_Transform.name == InName) {
				return _Transform;
			}
		}
		if (Recursive) {
			foreach (Transform _Transform in InTransform) {
				Transform result = _Transform.FindInChildren(InName, true);
				if (result != null) {
					return result;
				}
			}
		}
		return null;
	}

}