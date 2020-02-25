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
using System.Linq;

/// <summary>
/// 
/// </summary>
public class HierarchyChecker : Editor
{
    private const string const_strSpriteAtlasExtensionName = ".spriteatlas";

    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */


    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    [MenuItem("GameObject/UI/Custom/" + nameof(DoCheck_ObjectNameConvention))]
    public static void DoCheck_ObjectNameConvention(MenuCommand pCommand)
    {
        IEnumerable<GameObject> arrSelectObject = Selection.objects.Where(p => p is GameObject).Select(p => p as GameObject);
        foreach (GameObject pObject in arrSelectObject)
        {
            Component[] arrComponent = pObject.GetComponentsInChildren<Component>();

            IEnumerable<Component> arrFilteredComponent = arrComponent.Where(p => p is Transform == false);
            foreach(Component pComponent in arrFilteredComponent)
            {
                Debug.Log(pComponent.name + pComponent.GetType().FullName, pComponent);
                EditorGUIUtility.PingObject(pComponent);
            }
        }
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private
   
    #endregion Private
}