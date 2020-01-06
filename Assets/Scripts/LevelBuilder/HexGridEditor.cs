#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HexGrid))]
public class HexGridEditor : Editor {
    public override void OnInspectorGUI()
    {
        HexGrid hexGrid = (HexGrid)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Update Grid Size"))
        {
            hexGrid.UpdateGridSize();
        }
    }

}
#endif