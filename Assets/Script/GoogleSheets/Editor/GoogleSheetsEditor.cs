using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataManager))]
public class GoogleSheetsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(16f);
        if (GUILayout.Button("Load", GUILayout.Height(31f)))
        {
            if (!Application.isPlaying)
            {
                DataManager.Instance.Init();
            }
        }
    }
}
