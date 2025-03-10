using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnerSettings : MonoBehaviour
{
    public Vector2 Spacing;
    public Vector2Int Tilling;

    public PlacementMode placementMode = PlacementMode.Straight;
    public enum PlacementMode
    {
        Straight,
        Circle,
        Spline
    }
    public GameObject Duplicate;
}
