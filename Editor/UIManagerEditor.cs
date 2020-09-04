#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-03-15
 *	Summary 		        : 
 *
 *
 * 참고한 코드
 * - ReorderableList - https://unityindepth.tistory.com/56
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;
using UIFramework;
using System.Linq;

/// <summary>
/// 
/// </summary>
public class UIManagerEditor : EditorWindow
{
    /* const & readonly declaration             */


    /* enum & struct declaration                */


    /* public - Field declaration               */

    public UISetting pSetting;

    /* protected & private - Field declaration  */

    // ========================================================================== //

    /* public - [Do~Something] Function 	        */

    [MenuItem("Tools/UIManager Editor")]
    static void ShowWindow()
    {
        UIManagerEditor pWindow = (UIManagerEditor)GetWindow(typeof(UIManagerEditor), false);

        pWindow.pSetting = GetCurrentSetting();

        pWindow.minSize = new Vector2(300, 500);
        pWindow.Show();
    }

    private static UISetting GetCurrentSetting()
    {
        string[] arrFileGUID = AssetDatabase.FindAssets($"t:{nameof(UISetting)}");

        UISetting pCurrentSetting;
        if (arrFileGUID.Length > 0)
        {
            UISetting[] arrSetting = arrFileGUID.Select(p => AssetDatabase.LoadAssetAtPath<UISetting>(AssetDatabase.GUIDToAssetPath(p))).ToArray();

            pCurrentSetting = arrSetting.FirstOrDefault(p => p.bIsCurrent);
            if (pCurrentSetting == null)
                pCurrentSetting = arrSetting.First();
        }
        else
            pCurrentSetting = CreateAsset<UISetting>();

        pCurrentSetting.bIsCurrent = true;

        return pCurrentSetting;
    }

    // ========================================================================== //

    /* protected - [Override & Unity API]       */

    Vector2 _vecScrollPos_EditorSetting;

    private void OnGUI()
    {
        SerializedObject pSO = new SerializedObject(this);

        _vecScrollPos_EditorSetting = EditorGUILayout.BeginScrollView(_vecScrollPos_EditorSetting, GUILayout.Height(300f));
        {
            SerializedProperty pProperty = pSO.FindProperty($"{nameof(pSetting)}");
            EditorGUILayout.PropertyField(pProperty);
        }
        EditorGUILayout.EndScrollView();


        Draw_CSExportButton();

        if (GUI.changed)
        {
            pSO.ApplyModifiedProperties();
            EditorUtility.SetDirty(this);
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private
    private void Draw_CSExportButton()
    {
        string strExportCS = $"Export Editor Setting // Path is (Assets/{pSetting.strExportCSPath}.cs)";

        EditorGUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(strExportCS))
            {
                if (string.IsNullOrEmpty(pSetting.strExportCSPath))
                {
                    UnityEngine.Debug.LogError($"{nameof(pSetting.strExportCSPath)} - string.IsNullOrEmpty({(pSetting.strExportCSPath)})");
                    return;
                }

                ExportCS();
            }
        }
        EditorGUILayout.EndHorizontal();
    }

    private void ExportCS()
    {
        CustomCodedom pCodeDom = new CustomCodedom();
        pCodeDom.DoExportCS($"{Application.dataPath}/{pSetting.strExportCSPath}");

        AssetDatabase.Refresh();
        UnityEngine.Debug.Log($"{nameof(ExportCS)} Complete");
    }

    public static T CreateAsset<T>() where T : ScriptableObject
    {
        T pAsset = ScriptableObject.CreateInstance<T>();

#if UNITY_EDITOR
        const string strCreateAssetPath = "Resources";

        string strAbsoluteDirectory = Application.dataPath + $"/{strCreateAssetPath}";
        if (Directory.Exists(strAbsoluteDirectory) == false)
            Directory.CreateDirectory(strAbsoluteDirectory);

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath($"Assets/{strCreateAssetPath}/New {typeof(T)}.asset");

        AssetDatabase.CreateAsset(pAsset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();

        pAsset = (T)AssetDatabase.LoadAssetAtPath(assetPathAndName, typeof(T));
        Selection.activeObject = pAsset;
#endif

        return pAsset;
    }
    #endregion Private
}
