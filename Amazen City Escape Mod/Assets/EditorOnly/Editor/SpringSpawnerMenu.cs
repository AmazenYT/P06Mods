using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpringSpawner))]
public class SpringSpawnerMenu : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GUIStyle customText = new GUIStyle()
        {
            fontSize = 12,
            fontStyle = FontStyle.Bold
        };
        customText.normal.textColor = new Color(0.9f, 0.9f, 0.9f);

        GUI.Label(new Rect(15, 25, 300, 20), "Spring Properties", customText);

        var springSpawnerObj = target as SpringSpawner;
        {
            switch (springSpawnerObj.SpringMode)
            {
                case SpringSpawner.Mode.ManualMode:
                    EditorGUILayout.HelpBox("Spring Mode set to Manual - manually define the spring trajectory by aiming the spring.", MessageType.Info);
                    break;

                case SpringSpawner.Mode.AutomaticLockOn:
                    EditorGUILayout.HelpBox("Spring Mode set to Automatic - spring rotation is automated and locked onto an object.", MessageType.Info);
                    break;
            }
        }
    }

}