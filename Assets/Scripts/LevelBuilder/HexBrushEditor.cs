#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexBrush))]
public class HexBrushEditor : Editor {
    public override void OnInspectorGUI()
    {
        HexBrush brush = (HexBrush)target;

        DrawDefaultInspector();

        Event current = Event.current;
        if (current.type != EventType.KeyDown)
            return;

        switch(current.keyCode)
        {
            case KeyCode.LeftArrow:
                brush.lowerHealth();
                current.Use();
                break;
            case KeyCode.RightArrow:
                brush.raiseHealth();
                current.Use();
                break;
            default:
                break;
        }
    
    }

}
#endif