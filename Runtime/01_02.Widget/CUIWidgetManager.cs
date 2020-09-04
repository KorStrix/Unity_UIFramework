#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-21 오후 6:42:02
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIFramework;

/// <summary>
/// Owner Canvas가 없는 Widget를 관리하는 관리자
/// </summary>
public class CUIWidgetManager : UIObjectManagerBase<CUIWidgetManager, IUIWidget_Managed>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    HashSet<IUIObject> _setPlayHiding = new HashSet<IUIObject>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/


    // ========================================================================== //

    /* protected - Override & Unity API         */

    public override UICommandHandle<CLASS_UIOBJECT> IUIManager_Show<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
    {
        if (this.IsNull())
            return null;

        UICommandHandle<CLASS_UIOBJECT> sUICommandHandle = UICommandHandle<CLASS_UIOBJECT>.GetInstance(pUIObject);
        sUICommandHandle.Set_UIObject(pUIObject);
        StartCoroutine(Coroutine_Show(sUICommandHandle, pUIObject));

        return sUICommandHandle;
    }

    public override UICommandHandle<CLASS_UIOBJECT> IUIManager_Hide<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject, bool bPlayAnimation)
    {
        if (this.IsNull())
            return null;

        UICommandHandle<CLASS_UIOBJECT> sUICommandHandle = UICommandHandle<CLASS_UIOBJECT>.GetInstance(pUIObject);
        sUICommandHandle.Set_UIObject(pUIObject);
        StartCoroutine(Coroutine_Hide(sUICommandHandle, pUIObject));

        return sUICommandHandle;
    }

    public override EUIObjectState IUIManager_GetUIObjectState<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
    {
        Debug.Log($"아직 미지원.. {nameof(IUIManager_GetUIObjectState)} - {pUIObject.gameObject.name}");

        return EUIObjectState.Error;
    }

    /* protected - [abstract & virtual]         */

    // ========================================================================== //

    #region Private

    IEnumerator Coroutine_Show<CLASS_UIOBJECT>(UICommandHandle<CLASS_UIOBJECT> sUICommandHandle, CLASS_UIOBJECT pUIObject)
        where CLASS_UIOBJECT : IUIObject
    {
        // hiding 중인 경우는 Show Coroutine을 켜지않습니다.
        // 하이딩할 때 Active가 꺼져있을 때 True로 인해 들어올 수 있음
        if (_setPlayHiding.Contains(pUIObject))
            yield break;

        sUICommandHandle.Event_OnBeforeShow();
        GameObject pObject = pUIObject.gameObject;
        if (pObject != null && pObject.activeSelf == false)
            pObject.SetActive(true);

        sUICommandHandle.Event_OnShow_BeforeAnimation();
        yield return pUIObject.OnShowCoroutine();
        sUICommandHandle.Event_OnShow_AfterAnimation();
    }

    IEnumerator Coroutine_Hide<CLASS_UIOBJECT>(UICommandHandle<CLASS_UIOBJECT> sUICommandHandle, CLASS_UIOBJECT pUIObject)
        where CLASS_UIOBJECT : IUIObject
    {
        if (_setPlayHiding.Contains(pUIObject))
            yield break;
        _setPlayHiding.Add(pUIObject);

        GameObject pObject = pUIObject.gameObject;
        if (pObject != null && pObject.activeSelf == false)
            pObject.SetActive(true);

        yield return pUIObject.OnHideCoroutine();
        sUICommandHandle.Event_OnHide(true);

        if(pObject.Equals(null) == false)
            pObject.SetActive(false);

        _setPlayHiding.Remove(pUIObject);
    }

    #endregion Private

}
