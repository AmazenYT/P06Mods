using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BackgroundColorAttribute))]
public class BackgroundColorEditor : DecoratorDrawer
{
    BackgroundColorAttribute attr { get { return ((BackgroundColorAttribute)attribute); } }
    public override float GetHeight() { return 0; }

    public override void OnGUI(Rect position)
    {
        GUI.backgroundColor = attr.color;
    }
}