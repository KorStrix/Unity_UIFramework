#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-02-15 오전 10:32:24
 *	개요 : 
 *	
 *	참고 코드
 *	https://forum.unity.com/threads/creating-a-spriteatlas-from-code.511400/
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine.U2D;
using UnityEditor.U2D;
using System.IO;

/// <summary>
/// 
/// </summary>
public class SpriteAtlasHelper
{
    private const string const_strSpriteAtlasExtensionName = ".spriteatlas";

    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    // [MenuItem("Assets/Create SpriteAtlas for selected Sprites.")]
    public static void DoCreateAtlas_ForSelectedSprites()
    {
        SpriteAtlas pSpriteAtlas = CreateSpriteAtlas();
        UpdateAtlas(pSpriteAtlas, Selection.objects);
    }

    [MenuItem("Assets/UpdateAtlas_AllFile_InFolder")]
    static public void DoUpdateAtlas()
    {
        Debug.Log(nameof(DoUpdateAtlas));

        for (int i = 0; i < Selection.objects.Length; i++)
        {
            DefaultAsset pFolderAsset = Selection.objects[i] as DefaultAsset; // 폴더인지 체크
            if (pFolderAsset == null)
                continue;

            string strFolderAssetPath = AssetDatabase.GetAssetPath(pFolderAsset);
            SpriteAtlas pAtlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(strFolderAssetPath + const_strSpriteAtlasExtensionName);
            if (pAtlas != null)
                AssetDatabase.DeleteAsset(strFolderAssetPath + const_strSpriteAtlasExtensionName);

            pAtlas = CreateSpriteAtlas(strFolderAssetPath);
            SpriteAtlasPackingSettings pSetting = new SpriteAtlasPackingSettings();
            pSetting.blockOffset = 0;
            pSetting.padding = 4;
            pSetting.enableTightPacking = false;
            pSetting.enableRotation = true;

            pAtlas.SetPackingSettings(pSetting);

            string strCurrentDirectoryPath = Directory.GetCurrentDirectory();
            DirectoryInfo pDirectory = new DirectoryInfo(Directory.GetCurrentDirectory() + "/" + strFolderAssetPath);
            FileInfo[] arrFile = pDirectory.GetFiles();
            List<Object> listFile = new List<Object>();
            for(int j = 0; j < arrFile.Length; j++)
            {
                string strFileAssetPath = strFolderAssetPath + "/" + arrFile[j].Name;
                Object pAsset = AssetDatabase.LoadAssetAtPath<Sprite>(strFileAssetPath);
                if(pAsset != null)
                    listFile.Add(pAsset);
            }
            UpdateAtlas(pAtlas, listFile);
        }
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private
    private static SpriteAtlas CreateSpriteAtlas(string strAssetPath_WithName = "Assets/sample")
    {
        
        SpriteAtlas pSpriteAtlas = new SpriteAtlas();
        AssetDatabase.CreateAsset(pSpriteAtlas, strAssetPath_WithName + const_strSpriteAtlasExtensionName);
        return pSpriteAtlas;
    }

    private static void UpdateAtlas(SpriteAtlas pSpriteAtlas, IEnumerable<Object> arrObject)
    {
        Sprite[] arrSprite = new Sprite[pSpriteAtlas.spriteCount];
        pSpriteAtlas.GetSprites(arrSprite);
        pSpriteAtlas.Remove(arrSprite);

        foreach (var pObject in arrObject)
        {
            Sprite pSprite = pObject as Sprite;
            if (pSprite == null)
                continue;

            if (pSpriteAtlas.GetSprite(pSprite.name) == false)
                Debug.Log($"{nameof(SpriteAtlasHelper)} - Added Sprite{pSprite.name} In Sprite Atlas{pSpriteAtlas.name}");
            pSpriteAtlas.Add(new Object[] { pSprite });

        }

        AssetDatabase.SaveAssets();
    }

    #endregion Private
}