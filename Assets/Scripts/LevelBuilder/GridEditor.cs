#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Grid))]
public class GridEditor : Editor {

    // Use this for initialization
    public override void OnInspectorGUI()
    {
        //Show NOTHING we only want the user to edit through the HexGridScript.
    }
}
#endif