using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionTextEditor : MonoBehaviour
{
    public string MissionText;

    private void OnValidate()
    {
        name = "MISSION TEXT" + "/" + MissionText;
    }
}
