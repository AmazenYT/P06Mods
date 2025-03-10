using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PlacementMode
{
    Straight,
    Circle,
    SplineSmooth,
    SplineLinear
}

public class ObjectSpawnerEditor : EditorWindow
{
    private GameObject prefab;

    private static PlacementMode placementMode = PlacementMode.Straight;
    private Vector2 spacing = new Vector2(1.0f, 1.0f);
    private Vector2Int tiling = new Vector2Int(3, 3);

    private bool snapToGround = true;
    private float upwardOffset = 0.0f;
    private float pathSmoothness = 0.5f;

    private GameObject previewParent;
    private bool shouldUpdatePreview = true;
    private GameObject splineController;

    private List<Transform> pseudoSplineControlPoints = new List<Transform>();
    private int controlPointsCount = 4;
    private ObjectSpawnerSettings Settings;
    private bool hasReference = false;


    [MenuItem("Sonic P-06/Tools/Multi Object Spawner #T")]
    public static void ShowWindow()
    {
        ObjectSpawnerEditor window = GetWindow<ObjectSpawnerEditor>();
        window.titleContent = new GUIContent("Sonic Frontiers Object Spawner");
    }

    private void OnEnable()
    {
        RefreshItself();
    }
    void DestroyEmptyParentSpawners()
    {
        // Find all GameObjects in the scene
        GameObject[] allGameObjects = GameObject.FindObjectsOfType<GameObject>();

        // Iterate through all found GameObjects
        foreach (GameObject gameObject in allGameObjects)
        {
            if (gameObject.name == "Object Parent Spawner")
            {
                bool hasValidChild = false;

                // Check all children of the ParentSpawner
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    Transform child = gameObject.transform.GetChild(i);
                    if (child != null)
                    {
                        // At least one child is not null
                        hasValidChild = true;
                        break;
                    }
                }

                // Destroy the ParentSpawner only if no valid children are found
                if (!hasValidChild)
                {
                    DestroyImmediate(gameObject);
                }
            }
        }
    }

    void RefreshItself()
    {      
        if (Selection.activeGameObject != null && Selection.activeGameObject.GetComponent<ObjectSpawnerSettings>() != null)
        {
            Settings = Selection.activeGameObject.GetComponent<ObjectSpawnerSettings>();
            spacing = Settings.Spacing;
            tiling = Settings.Tilling;
            hasReference = true;
            if (Settings.Duplicate == null)
            {
                Settings.Duplicate = prefab;
            }
            else
            {
                prefab = Settings.Duplicate;
            }
          //  placementMode = (PlacementMode)Settings.placementMode;
            previewParent = Selection.activeGameObject;
            shouldUpdatePreview = true;
        }
        else
        {
            previewParent = null;
        }
        if (previewParent == null)
        {
            hasReference = false;
        }
        if (hasReference)
        {
            return;
        }
        // Instance itself upon creation with nothing selected
        if (Selection.activeGameObject == null && prefab == null)
        {
            previewParent = new GameObject("Object Parent Spawner");
            Settings = previewParent.AddComponent<ObjectSpawnerSettings>();
            SetSettings(previewParent.GetComponent<ObjectSpawnerSettings>());
            Selection.activeGameObject = previewParent;
            hasReference = true;
        }

        // If we selected a previously used Object Spawner
        if (Selection.activeGameObject != null && Selection.activeGameObject.name == "Object Parent Spawner")
        {
            Settings = Selection.activeGameObject.GetComponent<ObjectSpawnerSettings>();
            if (Settings.Duplicate != null)
            {
                prefab = Settings.Duplicate;
            }
            previewParent = Selection.activeGameObject;
            Selection.activeGameObject = previewParent;
            hasReference = true;
        }

        // We're creating a new Object Spawner here and have selected a thing to duplicate
        if (Selection.activeGameObject != null && Selection.activeGameObject.name != "Object Parent Spawner")
        {
            previewParent = new GameObject("Object Parent Spawner");
            prefab = Selection.activeGameObject;
            previewParent.transform.position = prefab.transform.position;
            Settings = previewParent.AddComponent<ObjectSpawnerSettings>();
            SetSettings(previewParent.GetComponent<ObjectSpawnerSettings>());
            hasReference = true;
            Selection.activeGameObject = previewParent;
        }

        //if (previewParent == null)
        //{
        //    DestroyEmptyParentSpawners();
        //}
    }

    void SetSettings(ObjectSpawnerSettings SettingsRef)
    {
        SettingsRef.Tilling = tiling;
        SettingsRef.Spacing = spacing;
        SettingsRef.Duplicate = prefab;
      //  SettingsRef.placementMode = (ObjectSpawnerSettings.PlacementMode)ObjectSpawnerEditor.placementMode;
    }
    private void OnGUI()
    {
        RefreshItself();

        prefab = EditorGUILayout.ObjectField("GameObject", prefab, typeof(GameObject), true) as GameObject;

        EditorGUI.BeginChangeCheck();
        placementMode = (PlacementMode)EditorGUILayout.EnumPopup("Placement Mode", placementMode);

        if (placementMode == PlacementMode.SplineSmooth)
        {
            splineController = EditorGUILayout.ObjectField("Control Points Parent", splineController, typeof(GameObject), true) as GameObject;

            while (pseudoSplineControlPoints.Count < controlPointsCount)
            {
                pseudoSplineControlPoints.Add(null);
            }
            while (pseudoSplineControlPoints.Count > controlPointsCount)
            {
                pseudoSplineControlPoints.RemoveAt(pseudoSplineControlPoints.Count - 1);
            }
        }
        if (placementMode == PlacementMode.SplineLinear)
        {
            splineController = EditorGUILayout.ObjectField("Control Points Parent", splineController, typeof(GameObject), true) as GameObject;

            while (pseudoSplineControlPoints.Count < controlPointsCount)
            {
                pseudoSplineControlPoints.Add(null);
            }
            while (pseudoSplineControlPoints.Count > controlPointsCount)
            {
                pseudoSplineControlPoints.RemoveAt(pseudoSplineControlPoints.Count - 1);
            }
        }
        EditorGUILayout.LabelField("Spacing Options", EditorStyles.boldLabel);
        Vector2 newSpacing = EditorGUILayout.Vector2Field("Spacing", spacing);
        if (newSpacing != spacing)
        {
            spacing = newSpacing;
            shouldUpdatePreview = true;
            Settings.Spacing = spacing;
        }

        EditorGUILayout.LabelField("Tiling Options", EditorStyles.boldLabel);
        Vector2Int newTiling = EditorGUILayout.Vector2IntField("Tiling", tiling);
        if (newTiling != tiling)
        {
            tiling = newTiling;
            shouldUpdatePreview = true;
            Settings.Tilling = newTiling;
        }

        snapToGround = EditorGUILayout.Toggle("Snap to Floor", snapToGround);

        float newUpwardOffset = EditorGUILayout.FloatField("Snap Y Offset", upwardOffset);
        if (newUpwardOffset != upwardOffset)
        {
            upwardOffset = newUpwardOffset;
            shouldUpdatePreview = true;
        }

        if (GUILayout.Button("Refresh Objects") || shouldUpdatePreview)
        {
            UpdatePreview();
            shouldUpdatePreview = false;
        }
    }

    private void UpdatePreview()
    {
        pseudoSplineControlPoints.Clear();
        if (splineController != null)
        {
            foreach (Transform child in splineController.transform)
            {
                pseudoSplineControlPoints.Add(child);
            }
        }
        controlPointsCount = pseudoSplineControlPoints.Count;

        ClearPreview();

        if (prefab == null)
        {
            return;
        }

        switch (placementMode)
        {
            case PlacementMode.Straight:
                PlaceStraight();
                break;
            case PlacementMode.Circle:
                PlaceCircle();
                break;
            case PlacementMode.SplineSmooth:
                PlacePseudoSpline();
                break;
            case PlacementMode.SplineLinear:
                PlaceLinear();
                break;
        }
    }
    private void PlaceLinear()
    {
        if (pseudoSplineControlPoints.Count < 2)
        {
            Debug.LogError("Not enough control points for pseudo spline.");
            return;
        }

        float totalDistance = CalculateTotalPseudoSplineDistance();

        for (int i = 0; i < tiling.x; i++)
        {
            float t = i / (float)(tiling.x - 1);
            float distanceAlongSpline = t * totalDistance;

            Vector3 splinePoint = CalculatePseudoSplinePointAtDistance(distanceAlongSpline);

            Vector3 spawnPos = splinePoint;

            if (snapToGround)
            {
                RaycastHit hit;
                if (Physics.Raycast(spawnPos, Vector3.down, out hit))
                {
                    spawnPos = hit.point + Vector3.up * upwardOffset;
                }
            }
            else
            {
                spawnPos += Vector3.up * upwardOffset;
            }

            InstantiateObject(prefab, previewParent.transform, spawnPos, Quaternion.identity);
        }
    }

    private float CalculateTotalPseudoSplineDistance()
    {
        float totalDistance = 0f;

        for (int i = 0; i < tiling.x - 1; i++)
        {
            float t1 = i / (float)(tiling.x - 1);
            float t2 = (i + 1) / (float)(tiling.x - 1);

            Vector3 point1 = CalculatePseudoSplinePoint(t1);
            Vector3 point2 = CalculatePseudoSplinePoint(t2);

            totalDistance += Vector3.Distance(point1, point2);
        }

        return totalDistance;
    }

    private Vector3 CalculatePseudoSplinePointAtDistance(float distance)
    {
        float currentDistance = 0f;

        for (int i = 0; i < tiling.x - 1; i++)
        {
            float t1 = i / (float)(tiling.x - 1);
            float t2 = (i + 1) / (float)(tiling.x - 1);

            Vector3 point1 = CalculatePseudoSplinePoint(t1);
            Vector3 point2 = CalculatePseudoSplinePoint(t2);

            float segmentDistance = Vector3.Distance(point1, point2);

            if (currentDistance + segmentDistance >= distance)
            {
                float u = (distance - currentDistance) / segmentDistance;
                return CalculatePseudoSplinePoint(t1 + u);
            }

            currentDistance += segmentDistance;
        }

        // If the distance is not found, return the last point on the spline
        return CalculatePseudoSplinePoint(1f);
    }

    private void PlaceStraight()
    {
        for (int z = 0; z < tiling.y; z++)
        {
            for (int x = 0; x < tiling.x; x++)
            {
                Vector3 offset = new Vector3(spacing.x * x, upwardOffset, spacing.y * z);
                Vector3 spawnPos = previewParent.transform.position + previewParent.transform.rotation * offset;

                if (snapToGround)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(spawnPos, Vector3.down, out hit))
                    {
                        spawnPos.y = hit.point.y + upwardOffset;
                    }
                }

                InstantiateObject(prefab, previewParent.transform, spawnPos, Quaternion.identity);
            }
        }
    }
    private void InstantiateObject(GameObject prefab, Transform parent, Vector3 position, Quaternion rotation)
    {
        GameObject newObject = Instantiate(prefab, position, rotation, parent);
    }

    private void PlaceCircle()
    {
        for (int i = 0; i < tiling.x; i++)
        {
            float angle = i * (360f / tiling.x);
            float radians = angle * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Sin(radians) * spacing.x * tiling.y, upwardOffset, Mathf.Cos(radians) * spacing.y * tiling.y);
            Vector3 spawnPos = previewParent.transform.position + previewParent.transform.rotation * offset;

            if (snapToGround)
            {
                RaycastHit hit;
                if (Physics.Raycast(spawnPos, Vector3.down, out hit))
                {
                    spawnPos.y = hit.point.y + upwardOffset;
                }
            }

            InstantiateObject(prefab, previewParent.transform, spawnPos, Quaternion.identity);
        }
    }
    private void PlacePseudoSpline()
    {
        if (pseudoSplineControlPoints.Count < 2)
        {
            Debug.LogError("Not enough control points for pseudo spline.");
            return;
        }

        for (int i = 0; i < tiling.x; i++)
        {
            float t = i / (float)(tiling.x - 1);
            Vector3 splinePoint = CalculatePseudoSplinePoint(t);
            Quaternion spawnRotation = CalculatePseudoSplineTangent(t);

            Vector3 spawnPos = splinePoint;

            if (snapToGround)
            {
                RaycastHit hit;
                if (Physics.Raycast(spawnPos, Vector3.down, out hit))
                {
                    spawnPos = hit.point + Vector3.up * upwardOffset;
                }
            }
            else
            {
                spawnPos += Vector3.up * upwardOffset;
            }

            InstantiateObject(prefab, previewParent.transform, spawnPos, spawnRotation);
        }
    }

    private string GenerateUniqueID()
    {
        string id = "{";

        for (int i = 0; i < 32; i++)
        {
            if (i == 8 || i == 12 || i == 16 || i == 20)
            {
                id += "-";
            }

            id += UnityEngine.Random.Range(0, 16).ToString("X");
        }

        id += "}";

        return id;
    }

    private Quaternion CalculatePseudoSplineTangent(float t)
    {
        if (pseudoSplineControlPoints.Count < 2)
        {
            Debug.LogError("Not enough control points for pseudo spline.");
            return Quaternion.identity;
        }

        int segmentIndex = Mathf.Min(Mathf.FloorToInt(t * (pseudoSplineControlPoints.Count - 1)), pseudoSplineControlPoints.Count - 2);
        float u = t * (pseudoSplineControlPoints.Count - 1) - segmentIndex;

        Vector3 p0 = pseudoSplineControlPoints[LoopIndex(segmentIndex - 1)].position;
        Vector3 p1 = pseudoSplineControlPoints[segmentIndex].position;
        Vector3 p2 = pseudoSplineControlPoints[LoopIndex(segmentIndex + 1)].position;
        Vector3 p3 = pseudoSplineControlPoints[LoopIndex(segmentIndex + 2)].position;

        Vector3 tangent = 0.5f *
            ((-p0 + p2) +
            2 * (2 * p0 - 5 * p1 + 4 * p2 - p3) * u +
            3 * (-p0 + 3 * p1 - 3 * p2 + p3) * u * u);

        return Quaternion.LookRotation(tangent);
    }

    private Vector3 CalculatePseudoSplinePoint(float t)
    {
        if (pseudoSplineControlPoints.Count < 2)
        {
            Debug.LogError("Not enough control points for pseudo spline.");
            return Vector3.zero;
        }

        int segmentIndex = Mathf.Min(Mathf.FloorToInt(t * (pseudoSplineControlPoints.Count - 1)), pseudoSplineControlPoints.Count - 2);
        float u = t * (pseudoSplineControlPoints.Count - 1) - segmentIndex;

        Vector3 p0 = pseudoSplineControlPoints[LoopIndex(segmentIndex - 1)].position;
        Vector3 p1 = pseudoSplineControlPoints[segmentIndex].position;
        Vector3 p2 = pseudoSplineControlPoints[LoopIndex(segmentIndex + 1)].position;
        Vector3 p3 = pseudoSplineControlPoints[LoopIndex(segmentIndex + 2)].position;

        Vector3 point = 0.5f *
            ((2 * p1) +
            (-p0 + p2) * u +
            (2 * p0 - 5 * p1 + 4 * p2 - p3) * u * u +
            (-p0 + 3 * p1 - 3 * p2 + p3) * u * u * u);

        return point;
    }


    private int LoopIndex(int i)
    {
        return Mathf.Clamp(i, 0, pseudoSplineControlPoints.Count - 1);
    }

    private void ClearPreview()
    {
        while (previewParent.transform.childCount > 0)
        {
            DestroyImmediate(previewParent.transform.GetChild(0).gameObject);
        }
    }
}
