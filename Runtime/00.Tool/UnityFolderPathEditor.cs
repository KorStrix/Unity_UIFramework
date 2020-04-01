#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-05
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;


/// <summary>
/// 자주 들어가는 폴더경로 모음
/// </summary>
public static class UnityFolderPathEditor
{
    [MenuItem("Tools/OpenFolder/Open Persistent Path")]
    static public void Open_PersistentPath()
    {
        System.Diagnostics.Process.Start(Application.persistentDataPath);
    }

    [MenuItem("Tools/OpenFolder/Open LocalLow Unity Path")]
    static public void Open_CahcedBundle_Path()
    {
        string strPath_Unity = Application.persistentDataPath + "/../../Unity/";
        string strPath_Include_Compnay_And_Product = $"{strPath_Unity}{Application.companyName}_{Application.productName}";

        if(System.IO.Directory.Exists(strPath_Include_Compnay_And_Product))
            System.Diagnostics.Process.Start(strPath_Include_Compnay_And_Product);
        else
        {
            Debug.LogWarning("Not Contain Path - " + strPath_Include_Compnay_And_Product);
            System.Diagnostics.Process.Start(strPath_Unity);
        }
    }

    [MenuItem("Tools/OpenFolder/Open AndroidSDK Path")]
    static public void Open_AndroidSDK_Path()
    {
        System.Diagnostics.Process.Start(EditorPrefs.GetString("AndroidSdkRoot"));
    }


    [MenuItem("Tools/OpenFolder/Open Editor Log")]
    static public void Open_EditorLog_Path()
    {
        string strPath_Unity = Application.persistentDataPath + "/../../../Local/";
        if (System.IO.Directory.Exists(strPath_Unity))
        {
            string strPath = "Unity/Editor/";
            if (System.IO.Directory.Exists(strPath_Unity + strPath))
                System.Diagnostics.Process.Start(strPath_Unity + strPath);
            else
                System.Diagnostics.Process.Start(strPath_Unity);
        }
    }
}
#endif