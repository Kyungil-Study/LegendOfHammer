using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ReadmeSO))]
public class ReadmeSOEditor : Editor
{
    private ReadmeSO so => (ReadmeSO)target;
    
    public override void OnInspectorGUI()
    {
        GUI.skin.label.richText = true;

        if (so.icon != null)
        {
            GUILayout.Label(so.icon.texture, GUILayout.Height(64));
        }

        if (string.IsNullOrEmpty(so.title) == false)
        {
            GUILayout.Label($"<size=18><b>{so.title}</b></size>");
        }

        GUILayout.Space(8);

        if (string.IsNullOrEmpty(so.description) == false)
        {
            var wordWrapped = new GUIStyle(GUI.skin.label) { wordWrap = true };
            GUILayout.Label(so.description, wordWrapped);
        }

        GUILayout.Space(8);
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        if (string.IsNullOrEmpty(so.guideUrl) == false)
        {
            if (GUILayout.Button("몬스터 테스트 가이드 문서로 이동"))
            {
                Application.OpenURL(so.guideUrl);
            }
        }

        GUILayout.Space(4);
        EditorGUILayout.LabelField($"Asset Path: {AssetDatabase.GetAssetPath(so)}", EditorStyles.miniLabel);
        
        // Readme 수정하기
        DrawDefaultInspector();
    }
}
