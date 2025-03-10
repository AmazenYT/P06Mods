using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class SaveHierarchyToJson : EditorWindow
{
    private static string lastSavedFilePath = null;

    [MenuItem("Sonic P-06/Save Set Data Only #E")]
    static void SaveHierarchy()
    {
        GameObject parentObject = GameObject.Find("Set Data");
        if (parentObject == null)
        {
            Debug.LogError("Parent object 'Set Data' not found.");
            return;
        }

        List<HierarchyItem> hierarchyItems = new List<HierarchyItem>();
        TraverseHierarchy(parentObject.transform, hierarchyItems);

        HierarchyData data = new HierarchyData { items = hierarchyItems };

        string jsonFilePath = EditorUtility.SaveFilePanel("Save Hierarchy as JSON", "", "SetData", "json");
        if (!string.IsNullOrEmpty(jsonFilePath))
        {
            File.WriteAllText(jsonFilePath, JsonUtility.ToJson(data, true));
            Debug.Log("Hierarchy saved to JSON: " + jsonFilePath);
            lastSavedFilePath = jsonFilePath;
        }
    }

    [MenuItem("Sonic P-06/Save Set Data Only #E")]
    static void SaveHierarchyShortcut()
    {
        GameObject parentObject = GameObject.Find("Set Data");
        if (parentObject == null)
        {
            Debug.LogError("Parent object 'Set Data' not found.");
            return;
        }

        List<HierarchyItem> hierarchyItems = new List<HierarchyItem>();
        TraverseHierarchy(parentObject.transform, hierarchyItems);

        HierarchyData data = new HierarchyData { items = hierarchyItems };

        // Use the previously saved path or prompt for a new location if it's the first save
        string jsonFilePath = string.IsNullOrEmpty(lastSavedFilePath)
            ? EditorUtility.SaveFilePanel("Save Hierarchy as JSON", "", "SetData", "json")
            : lastSavedFilePath;

        if (!string.IsNullOrEmpty(jsonFilePath))
        {
            File.WriteAllText(jsonFilePath, JsonUtility.ToJson(data, true));
            lastSavedFilePath = jsonFilePath; // Update the last saved file path
            Debug.Log("Hierarchy saved to JSON: " + jsonFilePath);
        }
    }

    static void TraverseHierarchy(Transform parent, List<HierarchyItem> hierarchyItems)
    {
        foreach (Transform child in parent)
        {
            if (child.gameObject.CompareTag("EditorOnly"))
            {
                continue;
            }

            if (child.gameObject.name.Contains("/"))
            {
                hierarchyItems.Add(new HierarchyItem
                {
                    name = child.gameObject.name,
                    position = child.position,
                    rotation = child.rotation
                });
            }
            else
            {
              //  Debug.Log("Found gameobject that isn't supported: " + child.gameObject.name);
            }

            TraverseHierarchy(child, hierarchyItems);
        }
    }

    [System.Serializable]
    public class HierarchyData
    {
        public List<HierarchyItem> items;
    }

    [System.Serializable]
    public class HierarchyItem
    {
        public string name;
        public Vector3 position;
        public Quaternion rotation;
    }
}
