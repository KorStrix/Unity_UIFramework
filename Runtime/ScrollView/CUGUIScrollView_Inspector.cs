#if UNITY_EDITOR

using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

[CustomEditor(typeof(CUGUIScrollView), true)]
public class CUGUIScrollView_Inspector : Editor
{
    int index = 0;
    float speed = 1000;
    int iTestCount;
	public override void OnInspectorGUI ()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        CUGUIScrollView scroll = (CUGUIScrollView)target;

        //iTestCount = EditorGUILayout.IntField("Test Object Count : ", iTestCount);
        //if (GUILayout.Button("Create Scroll Item"))
        //{
        //    scroll.DoCreate_ScrollItem(iTestCount);
        //}

        GUI.enabled = Application.isPlaying;

        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("Clear"))
        {
            scroll.ClearCells();
        }
        if (GUILayout.Button("Refresh"))
        {
            scroll.RefreshCells();
		}
		if(GUILayout.Button("Refill"))
		{
			scroll.RefillCells();
		}
		if(GUILayout.Button("RefillFromEnd"))
		{
			scroll.RefillCellsFromEnd();
		}
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 45;
        float w = (EditorGUIUtility.currentViewWidth - 100) / 2;
        EditorGUILayout.BeginHorizontal();
        index = EditorGUILayout.IntField("Index", index, GUILayout.Width(w));
        speed = EditorGUILayout.FloatField("Speed", speed, GUILayout.Width(w));
        if(GUILayout.Button("Scroll", GUILayout.Width(45)))
        {
            scroll.SrollToCell(index, speed);
        }
        EditorGUILayout.EndHorizontal();
	}
}

#endif