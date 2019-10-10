#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-07 오전 11:17:04
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 
/// </summary>
public interface IUIManager
{
    void IUIManager_Show(IUIObject pUIObject);
    void IUIManager_Show<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject, System.Action<CLASS_UIOBJECT_DRIVEN> OnBeforeShow, System.Action<CLASS_UIOBJECT_DRIVEN> OnShow)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject;

    void IUIManager_Hide(IUIObject pUIObject);
    void IUIManager_Hide(IUIObject pUIObject, Action OnHide);

    void IUIManager_Add_Listener_OnBeforeShow<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject, System.Action<CLASS_UIOBJECT_DRIVEN> OnBeforeShow)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject;

    void IUIManager_Add_Listener_OnShow<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject, System.Action<CLASS_UIOBJECT_DRIVEN> OnShow)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject;

    void IUIManager_Add_Listener_OnHide<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject, System.Action<CLASS_UIOBJECT_DRIVEN> OnHide)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject;



    void IUIManager_Remove_AllListener_OnBeforeShow<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject;

    void IUIManager_Remove_AllListener_OnShow<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject;

    void IUIManager_Remove_AllListener_OnHide<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject;
}

static public class IPopupHelper
{
    /// <summary>
    /// 이 팝업을 관리하는 팝업 매니져를 찾아 매니져를 통해 팝업을 켭니다.
    /// </summary>
    static public void DoShow(this IPopup pPopup)
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Show(pPopup);
    }

    /// <summary>
    /// 이 팝업을 관리하는 팝업 매니져를 찾아 매니져를 통해 팝업을 켭니다.
    /// </summary>
    static public void DoShow<POPUP>(this POPUP pPopup, System.Action<POPUP> OnBeforeShow, System.Action<POPUP> OnShow)
        where POPUP : class, IPopup
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Show(pPopup, OnBeforeShow, OnShow);
    }

    /// <summary>
    /// 이 팝업을 관리하는 팝업 매니져를 찾아 매니져를 통해 팝업을 끕니다.
    /// </summary>
    static public void DoHide(this IPopup pPopup)
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Hide(pPopup);
    }

    /// <summary>
    /// 이 팝업을 관리하는 팝업 매니져를 찾아 매니져를 통해 팝업을 끕니다.
    /// </summary>
    static public void DoHide(this IPopup pPopup, System.Action OnHide)
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Hide(pPopup, OnHide);
    }

    /// <summary>
    /// 이 팝업이 Show가 되어 켜지기 직전에 이벤트를 구독합니다.
    /// </summary>
    static public void DoAdd_Listener_OnBeforeShowPopup<POPUP>(this POPUP pPopup, System.Action<POPUP> OnBeforeShowPopup)
        where POPUP : class, IPopup
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Add_Listener_OnBeforeShow(pPopup, OnBeforeShowPopup);
    }

    /// <summary>
    /// 이 팝업이 Show가 될 때 이벤트를 구독합니다.
    /// </summary>
    static public void DoAdd_Listener_OnShowPopup<POPUP>(this POPUP pPopup, System.Action<POPUP> OnShowPopup)
        where POPUP : class, IPopup
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Add_Listener_OnShow(pPopup, OnShowPopup);
    }

    /// <summary>
    /// 이 팝업이 Hide가 될 때 이벤트를 구독합니다.
    /// </summary>
    static public void DoAdd_Listener_OnHidePopup<POPUP>(this POPUP pPopup, System.Action<POPUP> OnHidePopup)
        where POPUP : class, IPopup
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Add_Listener_OnHide(pPopup, OnHidePopup);
    }

    /// <summary>
    /// 이 팝업이 Show가 되어 켜지기 직전에 이벤트를 비웁니다.
    /// <para> 익명 메소드는 변수에 저장하지 않는 이상 개별로 구독을 취소할 수 없기 때문에 비우는 것밖에 없습니다. </para>
    /// </summary>
    static public void DoRemove_AllListener_OnBeforeShow<POPUP>(this POPUP pPopup)
        where POPUP : class, IPopup
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Remove_AllListener_OnBeforeShow(pPopup);
    }

    /// <summary>
    /// 이 팝업이 Show가 될 때 이벤트를 비웁니다.
    /// <para> 익명 메소드는 변수에 저장하지 않는 이상 개별로 구독을 취소할 수 없기 때문에 비우는 것밖에 없습니다. </para>
    /// </summary>
    static public void DoRemove_AllListener_OnShow<POPUP>(this POPUP pPopup)
        where POPUP : class, IPopup
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Remove_AllListener_OnShow(pPopup);
    }

    /// <summary>
    /// 이 팝업이 Hide가 될 때 이벤트를 비웁니다.
    /// <para> 익명 메소드는 변수에 저장하지 않는 이상 개별로 구독을 취소할 수 없기 때문에 비우는 것밖에 없습니다. </para>
    /// </summary>
    static public void DoRemove_AllListener_OnHide<POPUP>(this POPUP pPopup)
        where POPUP : class, IPopup
    {
        if (pPopup == null)
            return;

        pPopup.pUIManager.IUIManager_Remove_AllListener_OnHide(pPopup);
    }
}

/// <summary>
/// <seealso cref="IPopup"/>을 관리하는 팝업 매니져.
/// <para> 추상 클래스, 싱글턴, 주 함수는 Static Public로 이루어졌습니다. </para>
/// </summary>
abstract public class CPopupManager<CLASS_DRIVEN, ENUM_POPUP_NAME> : MonoBehaviour, IUIManager
    where CLASS_DRIVEN : CPopupManager<CLASS_DRIVEN, ENUM_POPUP_NAME>
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    #region PopupInfo
    public class PopupInfo
    {
        public event System.Action<IPopup> _OnBeforeShowPopup;
        public event System.Action<IPopup> _OnShowPopup;
        public event System.Action<IPopup> _OnHidePopup;

        public ENUM_POPUP_NAME ePopupName { get; private set; }
        public IPopup pPopupInstance { get; private set; }


        System.Func<IEnumerator, Coroutine> _OnStartCoroutine;
        System.Action<IEnumerator> _OnStopCoroutine;

        Coroutine _pCoroutine_ShowHide;

        public PopupInfo(ENUM_POPUP_NAME ePopupName, IPopup pPopupInstance, System.Func<IEnumerator, Coroutine> OnStartCoroutine, System.Action<IEnumerator> OnStopCoroutine)
        {
            this.ePopupName = ePopupName; this.pPopupInstance = pPopupInstance; this._OnStartCoroutine = OnStartCoroutine; this._OnStopCoroutine = OnStopCoroutine;
        }

        public void DoRemove_AllListener_OnBeforeShowPopup()
        {
            _OnBeforeShowPopup = null;
        }

        public void DoRemove_AllListener_OnShowPopup()
        {
            _OnShowPopup = null;
        }

        public void DoRemove_AllListener_OnHidPopup()
        {
            _OnHidePopup = null;
        }

        public IEnumerator DoExecute_ShowCoroutine(Canvas pCanvas)
        {
            if (_pCoroutine_ShowHide != null)
                _OnStopCoroutine(pPopupInstance.IPopup_OnShowCoroutine());

            Transform pTransformPopup = pPopupInstance.transform;
            if(pTransformPopup != null)
            {
                pTransformPopup.SetParent(pCanvas.transform);
                pTransformPopup.SetAsLastSibling();
                pTransformPopup.gameObject.SetActive(true);
            }

            _OnBeforeShowPopup?.Invoke(pPopupInstance);
            _pCoroutine_ShowHide = _OnStartCoroutine(pPopupInstance.IPopup_OnShowCoroutine());
            yield return _pCoroutine_ShowHide;
            _OnShowPopup?.Invoke(pPopupInstance);
        }

        public IEnumerator DoExecute_HideCoroutine()
        {
            if (_pCoroutine_ShowHide != null)
                _OnStopCoroutine(pPopupInstance.IPopup_OnHideCoroutine());

            _pCoroutine_ShowHide = _OnStartCoroutine(pPopupInstance.IPopup_OnHideCoroutine());
            yield return _pCoroutine_ShowHide;

            if(pPopupInstance.transform != null)
                pPopupInstance.transform.gameObject.SetActive(false);
            _OnHidePopup?.Invoke(pPopupInstance);
        }
    }
    #endregion PopupInfo

    /* public - Field declaration            */

    public static CLASS_DRIVEN instance
    {
        get 
        {
            if(_instance == null)
            {
                GameObject pObjectInstance = new GameObject(typeof(CLASS_DRIVEN).Name);
                _instance = pObjectInstance.AddComponent<CLASS_DRIVEN>();
                _instance.OnCreate_ManagerInstance();
            }

            return _instance;
        }
    }

    /* protected & private - Field declaration         */

    static CLASS_DRIVEN _instance;

    Dictionary<ENUM_POPUP_NAME, PopupInfo> _mapPopupInstance = new Dictionary<ENUM_POPUP_NAME, PopupInfo>();
    Dictionary<IPopup, PopupInfo> _mapPopupInstance_Key_Is_IPopup = new Dictionary<IPopup, PopupInfo>();

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    /// <summary>
    /// 팝업 Instance를 얻어옵니다.
    /// </summary>
    /// <typeparam name="POPUP">얻어올 팝업의 타입</typeparam>
    /// <param name="ePopupName">팝업의 이름 Enum</param>
    static public POPUP GetPopupInstance<POPUP>(ENUM_POPUP_NAME ePopupName)
        where POPUP : class, IPopup
    {
        PopupInfo pPopupInfo = null;
        if (instance.GetPoupInfo(ePopupName, out pPopupInfo))
            return pPopupInfo.pPopupInstance as POPUP;
        else
            return null;
    }

    /// <summary>
    /// 팝업 Instance를 얻어옵니다. 
    /// <para> 팝업의 Instance가 없을 때 코루틴으로 생성 후 대리자를 통해 받아옵니다. </para> 
    /// </summary>
    /// <typeparam name="POPUP">얻어올 팝업의 타입</typeparam>
    /// <param name="ePopupName">팝업의 이름 Enum</param>
    /// <param name="OnGetPopupInstance">팝업을 얻어올 때 대리자</param>
    /// <returns></returns>
    static public IEnumerator GetPopupInstance_Coroutine<POPUP>(ENUM_POPUP_NAME ePopupName, System.Action<POPUP> OnGetPopupInstance)
        where POPUP : class, IPopup
    {
        PopupInfo pPopupInfo = null;
        if (instance.GetPoupInfo(ePopupName, out pPopupInfo))
        {
            OnGetPopupInstance?.Invoke(pPopupInfo.pPopupInstance as POPUP);
            yield break;
        }

        yield return _instance.StartCoroutine(_instance.CoOnCreatePopup(ePopupName));
        OnGetPopupInstance?.Invoke(_instance.GetPopupInstance(ePopupName) as POPUP);
    }

    /// <summary>
    /// 팝업을 켭니다.
    /// </summary>
    /// <param name="ePopupName">팝업의 이름 Enum</param>
    static public void DoShow_Popup(ENUM_POPUP_NAME ePopupName)
    {
        instance.StartCoroutine(_instance.CoProcess_ShowingPopup<IPopup>(ePopupName, null, null));
    }

    /// <summary>
    /// 팝업을 켭니다.
    /// </summary>
    /// <typeparam name="POPUP">팝업의 타입</typeparam>
    /// <param name="ePopupName">팝업의 이름 Enum</param>
    /// <param name="OnShowBeforePopup">팝업의 Active True전 호출</param>
    /// <param name="OnShowPopup">팝업의 Active True후 호출</param>
    static public void DoShow_Popup<POPUP>(ENUM_POPUP_NAME ePopupName, System.Action<POPUP> OnShowBeforePopup, System.Action<POPUP> OnShowPopup)
        where POPUP : class, IPopup
    {
        instance.StartCoroutine(_instance.CoProcess_ShowingPopup(ePopupName, OnShowBeforePopup, OnShowPopup));
    }

    /// <summary>
    /// 팝업을 끕니다.
    /// </summary>
    /// <param name="ePopupName">팝업의 이름 Enum</param>
    static public void DoHide_Popup(ENUM_POPUP_NAME ePopupName)
    {
        PopupInfo pPopupInfo = null;
        if (instance.GetPoupInfo(ePopupName, out pPopupInfo))
        {
            _instance.StartCoroutine(_instance.CoProcess_HidingPopup(pPopupInfo, null));
        }
    }

    /// <summary>
    /// 팝업을 끕니다.
    /// </summary>
    /// <typeparam name="POPUP">팝업의 타입</typeparam>
    /// <param name="ePopupName">팝업의 이름 Enum</param>
    /// <param name="OnHidePopup">팝업의 Active False 후 호출</param>
    static public void DoHide_Popup<POPUP>(ENUM_POPUP_NAME ePopupName, System.Action<POPUP> OnHidePopup)
        where POPUP : class, IPopup
    {
        PopupInfo pPopupInfo = null;
        if (instance.GetPoupInfo(ePopupName, out pPopupInfo) == false)
        {
            return;
        }

        _instance.StartCoroutine(_instance.CoProcess_HidingPopup(pPopupInfo, () => OnHidePopup?.Invoke(pPopupInfo.pPopupInstance as POPUP)));
    }
    static public void DoClear_PopupInstance()
    {
        PopupInfo[] arrPopupInfo = new PopupInfo[instance._mapPopupInstance.Count];
        _instance._mapPopupInstance.Values.CopyTo(arrPopupInfo, 0);

        for (int i = 0; i < arrPopupInfo.Length; i++)
            _instance.RemovePopupInfo(arrPopupInfo[i]);
    }


    #region IUIManager
    public void IUIManager_Show(IUIObject pUIObject)
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            StartCoroutine(CoShowingPopup<IPopup>(pPopupInfo, null, null));
    }
    
    public void IUIManager_Show<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject, System.Action<CLASS_UIOBJECT_DRIVEN> OnBeforeShow, System.Action<CLASS_UIOBJECT_DRIVEN> OnShow)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            StartCoroutine(CoShowingPopup(pPopupInfo, OnBeforeShow, OnShow));
    }

    public void IUIManager_Hide(IUIObject pUIObject)
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            StartCoroutine(CoProcess_HidingPopup(pPopupInfo, null));
    }
    public void IUIManager_Hide(IUIObject pUIObject, Action OnHide)
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            StartCoroutine(CoProcess_HidingPopup(pPopupInfo, OnHide));
    }

    public void IUIManager_Add_Listener_OnBeforeShow<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject, Action<CLASS_UIOBJECT_DRIVEN> OnBeforeShow)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            pPopupInfo._OnBeforeShowPopup += (IPopup pPopup) => { OnBeforeShow?.Invoke(pPopup as CLASS_UIOBJECT_DRIVEN); };
    }

    public void IUIManager_Add_Listener_OnShow<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject, Action<CLASS_UIOBJECT_DRIVEN> OnShow)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            pPopupInfo._OnShowPopup += (IPopup pPopup) => { OnShow?.Invoke(pPopup as CLASS_UIOBJECT_DRIVEN); };
    }


    public void IUIManager_Add_Listener_OnHide<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject, Action<CLASS_UIOBJECT_DRIVEN> OnHide)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            pPopupInfo._OnHidePopup += (IPopup pPopup) => { OnHide?.Invoke(pPopup as CLASS_UIOBJECT_DRIVEN); };
    }

    public void IUIManager_Remove_AllListener_OnBeforeShow<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            pPopupInfo.DoRemove_AllListener_OnBeforeShowPopup();
    }

    public void IUIManager_Remove_AllListener_OnShow<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            pPopupInfo.DoRemove_AllListener_OnShowPopup();
    }

    public void IUIManager_Remove_AllListener_OnHide<CLASS_UIOBJECT_DRIVEN>(CLASS_UIOBJECT_DRIVEN pUIObject)
        where CLASS_UIOBJECT_DRIVEN : class, IUIObject
    {
        PopupInfo pPopupInfo;
        if (_mapPopupInstance_Key_Is_IPopup.TryGetValue(pUIObject as IPopup, out pPopupInfo))
            pPopupInfo.DoRemove_AllListener_OnHidPopup();
    }

    #endregion

    // ========================================================================== //

    /* protected - Override & Unity API         */

    /// <summary>
    /// 팝업에 알맞는 캔버스를 얻어오는 방법을 구현합니다.
    /// </summary>
    /// <param name="ePopupName">캔버스가 필요한 팝업의 이름 Enum</param>
    /// <param name="pPopup">캔버스가 필요한 팝업의 인스턴스</param>
    abstract public Canvas GetPopupParentCavnas(ENUM_POPUP_NAME ePopupName, IPopup pPopup);

    /// <summary>
    /// 팝업의 인스턴스를 만드는 방법을 구현합니다.
    /// <para> 팝업의 인스턴스가 없는 경우에만 호출됩니다. </para> 
    /// </summary>
    /// <param name="ePopupName">인스턴스를 만들 팝업 이름 Enum</param>
    /// <returns></returns>
    abstract protected IEnumerator OnRequire_Create_PopupInstance(ENUM_POPUP_NAME ePopupName);

    /// <summary>
    /// 팝업의 인스턴스를 얻는 방법을 구현합니다.
    /// <para> 인스턴스가 없는 경우 <see cref="OnRequire_Create_PopupInstance"/>호출 뒤에 호출됩니다.</para> 
    /// </summary>
    /// <param name="ePopupName">인스턴스를 얻을 팝업의 이름 Enum</param>
    virtual protected IPopup GetPopupInstance(ENUM_POPUP_NAME ePopupName)
    {
        PopupInfo pPopupInfo;
        if (GetPoupInfo(ePopupName, out pPopupInfo))
            return pPopupInfo.pPopupInstance;
        else
            return null;
    }

    /// <summary>
    /// 싱글톤으로 instance가 생성될 때 호출됩니다.
    /// </summary>
    virtual protected void OnCreate_ManagerInstance() { }

    /// <summary>
    /// 팝업이 켜질 때 호출됩니다.
    /// <para> 매니져 Show(OnBeforeShow) -> <see cref="IPopup.IPopup_OnShowCoroutine"/> -> 매니져 Show(OnShow) -> 이 함수 순으로 호출됩니다. </para> 
    /// </summary>
    /// <param name="ePopupName">켜지는 팝업의 이름 Enum</param>
    /// <param name="pPopupInstance">켜지는 팝업의 인스턴스</param>
    virtual protected void OnShow_Popup(ENUM_POPUP_NAME ePopupName, IPopup pPopupInstance) { }

    /// <summary>
    /// 팝업이 꺼질 때 호출됩니다.
    /// <para> <see cref="IPopup.IPopup_OnHideCoroutine"/> -> 매니져 Hide(OnHide) -> 이 함수 순으로 호출됩니다. </para> 
    /// </summary>
    /// <param name="ePopupName">꺼지는 팝업의 이름 Enum</param>
    /// <param name="pPopupInstance">꺼지는 팝업의 인스턴스</param>
    virtual protected void OnHide_Popup(ENUM_POPUP_NAME ePopupName, IPopup pPopupInstance) { }

    private void OnDestroy()
    {
        DoClear_PopupInstance();
        _instance = null;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    IEnumerator CoProcess_ShowingPopup<POPUP>(ENUM_POPUP_NAME ePopupName, System.Action<POPUP> OnBeforeShowPopup, System.Action<POPUP> OnShowPopup)
        where POPUP : class, IPopup
    {
        PopupInfo pPopupInfo = null;
        if (GetPoupInfo(ePopupName, out pPopupInfo) == false)
        {
            yield return StartCoroutine(CoOnCreatePopup(ePopupName));

            IPopup pPopupInstance = GetPopupInstance(ePopupName);
            if (pPopupInstance == null)
            {
                yield break;
            }

            pPopupInfo = CreatePopupInfo(ePopupName, pPopupInstance);
        }

        if (pPopupInfo == null)
            yield break;

        StartCoroutine(CoShowingPopup(pPopupInfo, OnBeforeShowPopup, OnShowPopup));
    }

    IEnumerator CoShowingPopup<POPUP>(PopupInfo pPopupInfo, System.Action<POPUP> OnBeforeShowPopup, System.Action<POPUP> OnShowPopup)
        where POPUP : class, IUIObject
    {
        Canvas pCanvas = _instance.GetPopupParentCavnas(pPopupInfo.ePopupName, pPopupInfo.pPopupInstance);
        POPUP pPopup = pPopupInfo.pPopupInstance as POPUP;

        OnBeforeShowPopup?.Invoke(pPopup);
        yield return StartCoroutine(pPopupInfo.DoExecute_ShowCoroutine(pCanvas));
        OnShowPopup?.Invoke(pPopup);
        OnShow_Popup(pPopupInfo.ePopupName, pPopupInfo.pPopupInstance);
    }

    private bool GetPoupInfo(ENUM_POPUP_NAME ePopupName, out PopupInfo pPopupInfo)
    {
        bool bGet_IsSuccess = true;
        if (_instance._mapPopupInstance.TryGetValue(ePopupName, out pPopupInfo))
        {
            if (pPopupInfo.pPopupInstance.Equals(null))
            {
                RemovePopupInfo(pPopupInfo);
                bGet_IsSuccess = false;
            }
        }
        else
        {
            bGet_IsSuccess = false;
        }

        return bGet_IsSuccess;
    }

    IEnumerator CoProcess_HidingPopup(PopupInfo pPopupInfo, System.Action OnHidePopup)
    {
        if (pPopupInfo == null)
        {
            yield break;
        }

        yield return StartCoroutine(pPopupInfo.DoExecute_HideCoroutine());
        OnHidePopup?.Invoke();
        OnHide_Popup(pPopupInfo.ePopupName, pPopupInfo.pPopupInstance);

        if(pPopupInfo.pPopupInstance.Equals(null))
        {
            RemovePopupInfo(pPopupInfo);
        }
    }

    IEnumerator CoOnCreatePopup(ENUM_POPUP_NAME ePopupName)
    {
        yield return _instance.StartCoroutine(_instance.OnRequire_Create_PopupInstance(ePopupName));
    }

    protected PopupInfo CreatePopupInfo(ENUM_POPUP_NAME ePopupName, IPopup pPopupInstance)
    {
        PopupInfo pPopupInfo;
        if(GetPoupInfo(ePopupName, out pPopupInfo) == false)
        {
            pPopupInfo = new PopupInfo(ePopupName, pPopupInstance, instance.StartCoroutine, instance.StopCoroutine);
            _mapPopupInstance.Add(ePopupName, pPopupInfo);
            _mapPopupInstance_Key_Is_IPopup.Add(pPopupInstance, pPopupInfo);

            if (pPopupInfo.pPopupInstance != null)
                pPopupInfo.pPopupInstance.pUIManager = this;
        }

        if(pPopupInfo.pPopupInstance != pPopupInstance)
        {
            Debug.LogError(ePopupName + " Error pPopupInfo.pPopupInstance != pPopupInstance");
        }

        return _mapPopupInstance[ePopupName];
    }

    protected void RemovePopupInfo(PopupInfo pPopupInfo)
    {
        _mapPopupInstance.Remove(pPopupInfo.ePopupName);
        _mapPopupInstance_Key_Is_IPopup.Remove(pPopupInfo.pPopupInstance);
    }

    #endregion Private
}