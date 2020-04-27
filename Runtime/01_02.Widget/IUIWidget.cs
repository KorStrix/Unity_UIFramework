#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-18 오전 11:02:15
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIFramework;

namespace UIFramework
{
    /// <summary>
    /// <see cref="ICanvas"/> 제외하고, 고유한 객체로 동작하는 작은 UI Object
    /// <para>예시) <see cref="CUIAnimatedBar"/></para>
    /// </summary>
    public interface IUIWidget : IUIObject
    {
        void IUIWidget_OnBeforeShow();
    }

    public interface IUIWidget_Managed : IUIWidget, IUIObject_Managed
    {
    }
}


public static class IUIWidgetHelper
{
    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 켭니다.
    /// </summary>
    public static UICommandHandle<T> DoShow<T>(this T pObject)
        where T : class, IUIWidget_Managed
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager.IsNull())
            pObject.pUIManager = CUIWidgetManager.instance;

        return pObject.pUIManager.IUIManager_Show(pObject);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// </summary>
    public static UICommandHandle<T> DoHide<T>(this T pObject)
        where T : class, IUIWidget_Managed
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager.IsNull())
            pObject.pUIManager = CUIWidgetManager.instance;

        return pObject.pUIManager.IUIManager_Hide(pObject, true);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// </summary>
    public static UICommandHandle<T> DoPlay_ShowOrHide<T>(this T pObject, bool bShow)
        where T : class, IUIWidget_Managed
    {
        if (pObject == null || pObject.gameObject.activeSelf == bShow)
            return null;

        if (pObject.pUIManager.IsNull())
            pObject.pUIManager = CUIWidgetManager.instance;

        if(bShow)
            return pObject.pUIManager.IUIManager_Show(pObject);
        else
            return pObject.pUIManager.IUIManager_Hide(pObject, true);
    }
}
