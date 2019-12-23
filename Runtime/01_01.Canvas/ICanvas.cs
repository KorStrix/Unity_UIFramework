#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-21 오후 12:31:30
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 관리받는(관리자(<see cref="IUIManager"/>가 있는) <see cref="IUIObject"/> 
/// </summary>
public interface IUIObject_Managed
{
    IUIManager pUIManager { get; set; }
}

/// <summary>
/// <see cref="Canvas"/>와 <see cref="IUIWidget"/> 자식들을 가지고 있는 UI의 한 장면을 담당하는 객체
/// </summary>
public interface ICanvas : IUIObject, IUIObject_Managed
{
    // UIWidgetContainer pWidgetContainer { get; }
}

//public class UIWidgetContainer
//{

//}

static public class ICanvasHelper
{
    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 켭니다.
    /// </summary>
    static public UICommandHandle<T> DoShow<T>(this T pObject)
        where T : MonoBehaviour, ICanvas
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager - Check Regist Manager", pObject.gameObject.name, nameof(DoShow), pObject);
            pObject.gameObject.SetActive(true);

            return null;
        }

        return pObject.pUIManager.IUIManager_Show(pObject);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// </summary>
    static public UICommandHandle<T> DoHide<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager == null)
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager - Check Regist Manager", pObject.gameObject.name, nameof(DoHide), pObject);
            pObject.gameObject.SetActive(false);

            return null;
        }

        return pObject.pUIManager.IUIManager_Hide(pObject, true);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// </summary>
    static public UICommandHandle<T> DoHide_NotPlayHideCoroutine<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager == null)
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager - Check Regist Manager", pObject.gameObject.name, nameof(DoHide), pObject);
            pObject.gameObject.SetActive(false);

            return null;
        }

        return pObject.pUIManager.IUIManager_Hide(pObject, false);
    }
}