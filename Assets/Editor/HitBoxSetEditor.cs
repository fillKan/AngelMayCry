using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HitBoxSet))]
public class HitBoxSetEditor : Editor
{
    [TextArea(1, 1)] public string FileName;
    [TextArea(1, 1)] public string TableName;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space(16f);
        if (GUILayout.Button("Load", GUILayout.Height(31f)))
        {
            if (!Application.isPlaying)
            {
                var hitBox = target as HitBoxSet;

                string fileName = hitBox.FileName;
                string id = hitBox.ID;

                hitBox.Set(
                    GetData("Damage"), 
                    GetData("Hitbox_Duration"), 
                    GetData("Stun"), new Vector2(GetData("Push_Force_X"), GetData("Push_Force_Y")), 
                    GetData("Camera_Shaking_Force"), 
                    GetData("HitStop_Time"));
                hitBox.SetDirty();

                float GetData(string value)
                {
                    return float.Parse(DataUtil.GetDataValue(fileName, "ID", id, value));
                }
            }
        }
    }
}
