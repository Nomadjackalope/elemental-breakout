#if (UNITY_EDITOR) 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(SkillDataList))]
public class SkillDataListEditor : Editor {
    public override void OnInspectorGUI()
    {
        SkillDataList dataList = (SkillDataList)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Reload list"))
        {
            reloadList(dataList);
        }
    
    }

    void reloadList(SkillDataList dataList) {
        dataList.skills.Clear();
        dataList.skills.AddRange(Resources.FindObjectsOfTypeAll<Skill>());
        dataList.skills = dataList.skills.OrderBy(x => (int) (x.powerUp)).ToList();
    }

}
#endif