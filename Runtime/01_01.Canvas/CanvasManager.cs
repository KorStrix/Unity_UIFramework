#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-21 오후 12:46:22
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIWidgetContainerManager_Logic;
using System.Linq;

/// <summary>
/// 
/// </summary>
abstract public class CanvasManager<CLASS_DRIVEN_MANAGER, ENUM_CANVAS_NAME> : UIObjectManagerBase<CLASS_DRIVEN_MANAGER, ICanvas>, IUIManager
    where CLASS_DRIVEN_MANAGER : CanvasManager<CLASS_DRIVEN_MANAGER, ENUM_CANVAS_NAME>
{
    /* const & readonly declaration             */

    static readonly bool const_bIsDebug = false;

    /* enum & struct declaration                */

    #region Wrapper

    public class CanvasWrapper
    {
        public enum EState
        {
            Enable,
            Process_Show_BeforeAnimation,
            Process_Show_AfterAnimation,

            Process_Hide_BeforeAnimation,
            Disable,
        }

        public EState eState { get; private set; }
        public ENUM_CANVAS_NAME eName { get; private set; }
        public ICanvas pInstance { get; private set; }

        // 로직 캐싱 - 일일이 파싱체크 방지
        List<CanvasManager_LogicUndo_Wrapper> listUndoLogic_AfterShow = new List<CanvasManager_LogicUndo_Wrapper>();
        List<CanvasManager_LogicUndo_Wrapper> listUndoLogic_BeforeHide = new List<CanvasManager_LogicUndo_Wrapper>();
        List<CanvasManager_LogicUndo_Wrapper> listUndoLogic_AfterHide = new List<CanvasManager_LogicUndo_Wrapper>();

        List<IUIWidget> _listChildrenWidget = new List<IUIWidget>();
        List<Coroutine> _listCoroutine = new List<Coroutine>();
        List<Coroutine> _listManager_CoroutineLogic = new List<Coroutine>();
        List<Coroutine> _listManager_Coroutine_UndoLogic = new List<Coroutine>();

        System.Func<IEnumerator, Coroutine> _OnStartCoroutine;
        System.Action<Coroutine> _OnStopCoroutine;


        public void Init(ENUM_CANVAS_NAME eName, ICanvas pInstance, System.Func<IEnumerator, Coroutine> OnStartCoroutine, System.Action<Coroutine> OnStopCoroutine)
        {
            this.eName = eName; this.pInstance = pInstance; this._OnStartCoroutine = OnStartCoroutine; this._OnStopCoroutine = OnStopCoroutine;

            eState = EState.Disable;
            _listChildrenWidget.Clear();
            pInstance.gameObject?.GetComponentsInChildren(true, _listChildrenWidget);
            for(int i = 0; i < _listChildrenWidget.Count; i++)
            {
                if(_listChildrenWidget[i] is IUIObject_Managed)
                    _listChildrenWidget.RemoveAt(i--);
            }
        }

        public IEnumerator DoExecute_ShowCoroutine<CLASS_DRIVEN_CANVAS>(Canvas pCanvas, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            StopCoroutine();
            if (pInstance.Equals(null))
                yield break;

            SettingUI(pCanvas);

            if (const_bIsDebug && pInstance.gameObject != null)
                Debug.Log(pInstance.gameObject.name + " Execute Show Before Anim", pInstance.gameObject);
            sUICommandHandle.Event_OnShow_BeforeAnimation();

            StartCoroutine_Show();
            eState = EState.Process_Show_BeforeAnimation;
            
            yield return _listCoroutine.GetEnumerator_Safe();

            eState = EState.Process_Show_AfterAnimation;

            if (const_bIsDebug && pInstance.gameObject != null)
                Debug.Log(pInstance.gameObject.name + " Finish Show Animation", pInstance.gameObject);
        }

        public IEnumerator DoExecute_HideCoroutine()
        {
            StopCoroutine();

            if (pInstance.Equals(null))
                yield break;

            string strGameObjectName = pInstance.gameObject != null ? pInstance.gameObject.name : eName.ToString();

            if (const_bIsDebug)
                Debug.Log(strGameObjectName + " DoExecute_HideCoroutine Start", pInstance.gameObject);

            StartCoroutine_Hide();
            eState = EState.Process_Hide_BeforeAnimation;

            yield return _listCoroutine.GetEnumerator_Safe();

            if (const_bIsDebug)
            {
                if (pInstance.Equals(null))
                    Debug.Log(strGameObjectName + " DoExecute_HideCoroutine Finish pInstance is Null");
                else
                    Debug.Log(strGameObjectName + " DoExecute_HideCoroutine Finish pInstance is NotNull");
            }
        }

        public IEnumerator DoExecute_Manager_CoroutineLogic(MonoBehaviour pManager, EUIShowHideEvent eEvent, IEnumerable<ICanvasManager_Logic> listManagerLogic, System.Func<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper> GetUndoLogic, bool bIsDebug)
        {
            _listManager_CoroutineLogic.Clear();

            if(eEvent == EUIShowHideEvent.After_Hide_UIObject)
            {
                foreach (ICanvasManager_Logic pLogic in listManagerLogic)
                // foreach (ICanvasManager_Logic pLogic in listManagerLogic.ToArray())
                {
                    Coroutine pCoroutine = _OnStartCoroutine(pLogic.Execute_LogicCoroutine(pManager, pInstance, const_bIsDebug));
                    if (pCoroutine != null)
                        _listManager_CoroutineLogic.Add(pCoroutine);
                }

                Clear_Manager_UndoLogic();
            }
            else
            {
                foreach (ICanvasManager_Logic pLogic in listManagerLogic)
                // foreach (ICanvasManager_Logic pLogic in listManagerLogic.ToArray())
                {
                    _listManager_CoroutineLogic.Add(_OnStartCoroutine(pLogic.Execute_LogicCoroutine(pManager, pInstance, const_bIsDebug)));

                    CanvasManager_LogicUndo_Wrapper pUndoLogic = GetUndoLogic(pLogic);
                    if (pUndoLogic == null)
                        continue;

                    EUIShowHideEvent eUndoEvent = pUndoLogic.eWhenUndo;
                    switch (eUndoEvent)
                    {
                        case EUIShowHideEvent.After_Show_UIObject: listUndoLogic_AfterShow.Add(pUndoLogic); break;
                        case EUIShowHideEvent.Before_Hide_UIObject: listUndoLogic_BeforeHide.Add(pUndoLogic); break;
                        case EUIShowHideEvent.After_Hide_UIObject: listUndoLogic_AfterHide.Add(pUndoLogic); break;
                    }
                }
            }

            return _listManager_CoroutineLogic.GetEnumerator_Safe();
        }

        public IEnumerator DoExecute_Manager_UndoCoroutineLogic(MonoBehaviour pManager, EUIShowHideEvent eEvent)
        {
            List<CanvasManager_LogicUndo_Wrapper> listUndoLogic = null;
            switch (eEvent)
            {
                case EUIShowHideEvent.After_Show_UIObject: listUndoLogic = listUndoLogic_AfterShow; break;
                case EUIShowHideEvent.Before_Hide_UIObject: listUndoLogic = listUndoLogic_BeforeHide; break;
                case EUIShowHideEvent.After_Hide_UIObject: listUndoLogic = listUndoLogic_AfterHide; break;
            }

            if (listUndoLogic == null)
                return null;

            _listManager_Coroutine_UndoLogic.Clear();
            for (int i = 0; i < listUndoLogic.Count; i++)
                _listManager_Coroutine_UndoLogic.Add(_OnStartCoroutine(listUndoLogic[i].Execute_UndoLogicCoroutine(pManager, pInstance, const_bIsDebug)));
            listUndoLogic.Clear();

            return _listManager_Coroutine_UndoLogic.GetEnumerator_Safe();
        }

        public void DoSet_State_IsEnable()
        {
            eState = EState.Enable;
        }

        public void DoSet_State_Is_Disable()
        {
            if (pInstance.Equals(null) == false)
                pInstance.gameObject?.SetActive(false);
            eState = EState.Disable;
        }

        public bool Check_IsEnable()
        {
            return eState != EState.Disable;
        }

        public void Clear_Manager_UndoLogic()
        {
            listUndoLogic_AfterShow.Clear();
            listUndoLogic_BeforeHide.Clear();
            listUndoLogic_AfterHide.Clear();
        }


        private void StartCoroutine_Show()
        {
            _listCoroutine.Add(_OnStartCoroutine(pInstance.OnShowCoroutine()));

            for (int i = 0; i < _listChildrenWidget.Count; i++)
                _listCoroutine.Add(_OnStartCoroutine(_listChildrenWidget[i].OnShowCoroutine()));
        }

        private void StartCoroutine_Hide()
        {
            _listCoroutine.Add(_OnStartCoroutine(pInstance.OnHideCoroutine()));

            for (int i = 0; i < _listChildrenWidget.Count; i++)
                _listCoroutine.Add(_OnStartCoroutine(_listChildrenWidget[i].OnHideCoroutine()));
        }

        private void StopCoroutine()
        {
            for (int i = 0; i < _listCoroutine.Count; i++)
            {
                if (_listCoroutine[i] != null)
                    _OnStopCoroutine(_listCoroutine[i]);
            }
            _listCoroutine.Clear();
        }

        private void SettingUI(Canvas pCanvas)
        {
            Transform pTransformPopup = pInstance.transform;
            if(pCanvas != null)
                pTransformPopup.SetParent(pCanvas.transform);

            pTransformPopup.gameObject.SetActive(true);
        }
    }
    #endregion PopupInfo

    /* public - Field declaration            */


    /* protected & private - Field declaration         */

    static HashSet<System.IDisposable> g_setCommandHandle = new HashSet<System.IDisposable>();

    protected Dictionary<ENUM_CANVAS_NAME, List<CanvasWrapper>> _mapWrapper = new Dictionary<ENUM_CANVAS_NAME, List<CanvasWrapper>>();
    protected Dictionary<ICanvas, CanvasWrapper> _mapWrapper_Key_Is_Instance = new Dictionary<ICanvas, CanvasWrapper>();

    Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>> _mapManagerLogic = new Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>>();
    Dictionary<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper> _mapManagerUndoLogic_Parser = new Dictionary<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper>();

    List<CanvasWrapper> _list_CanvasShow = new List<CanvasWrapper>();
    HashSet<ENUM_CANVAS_NAME> _setProcessCreating = new HashSet<ENUM_CANVAS_NAME>();
    SimplePool<CanvasWrapper> _pWrapperPool;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public UICommandHandle<CLASS_DRIVEN_CANVAS> DoShow<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
        where CLASS_DRIVEN_CANVAS : class, ICanvas
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = UICommandHandle<CLASS_DRIVEN_CANVAS>.GetInstance(null);
        g_setCommandHandle.Add(pHandle);

        pInstance.StartCoroutine(pInstance.CoProcess_Showing(eName, pHandle, false));

        return pHandle;
    }

    static public UICommandHandle<CLASS_DRIVEN_CANVAS> DoShow_Multiple<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
        where CLASS_DRIVEN_CANVAS : class, ICanvas
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = UICommandHandle<CLASS_DRIVEN_CANVAS>.GetInstance(null);
        g_setCommandHandle.Add(pHandle);

        pInstance.StartCoroutine(pInstance.CoProcess_Showing(eName, pHandle, true));

        return pHandle;
    }

    static public UICommandHandle<CLASS_DRIVEN_CANVAS> DoHide<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
        where CLASS_DRIVEN_CANVAS : class, ICanvas
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = UICommandHandle<CLASS_DRIVEN_CANVAS>.GetInstance(null);
        g_setCommandHandle.Add(pHandle);

        pInstance.StartCoroutine(pInstance.CoProcess_Hiding(eName, pHandle));

        return pHandle;
    }
    

    static public UICommandHandle<ICanvas> DoShowOnly(ENUM_CANVAS_NAME eName)
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        UICommandHandle<ICanvas> pHandle = UICommandHandle<ICanvas>.GetInstance(null);
        g_setCommandHandle.Add(pHandle);

        pInstance.StartCoroutine(pInstance.CoProcess_Showing(eName, pHandle, false));

        return pHandle;
    }

    static public UICommandHandle<ICanvas> DoHideOnly(ENUM_CANVAS_NAME eName)
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        UICommandHandle<ICanvas> pHandle = UICommandHandle<ICanvas>.GetInstance(null);
        g_setCommandHandle.Add(pHandle);

        pInstance.StartCoroutine(pInstance.CoProcess_Hiding(eName, pHandle));

        return pHandle;
    }

    static public void DoAllHide_ShowedPopup(bool bPlayAnimation = true)
    {
        List<ICanvas> listCanavs = GetAlreadyShow_PopupList<ICanvas>();
        if (bPlayAnimation)
        {
            for (int i = 0; i < listCanavs.Count; i++)
                listCanavs[i].DoHide();
        }
        else
        {
            for (int i = 0; i < listCanavs.Count; i++)
                listCanavs[i].DoHide_NotPlayHideCoroutine();
        }
    }

    static public CLASS_DRIVEN_CANVAS GetAlreadyShow_Popup_OrNull<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
        where CLASS_DRIVEN_CANVAS : MonoBehaviour, ICanvas
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        var listWrapper = pInstance.Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
        if (listWrapper.Count > 0)
            return listWrapper[0].pInstance as CLASS_DRIVEN_CANVAS;
        else
            return null;
    }

    static public List<CLASS_DRIVEN_CANVAS> GetAlreadyShow_PopupList<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
        where CLASS_DRIVEN_CANVAS : class, ICanvas
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        var listWrapper = pInstance.Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
        List<CLASS_DRIVEN_CANVAS> listCanvas = new List<CLASS_DRIVEN_CANVAS>(listWrapper.Count);
        for(int i = 0; i < listWrapper.Count; i++)
        {
            CLASS_DRIVEN_CANVAS pCanvasInstance = listWrapper[i].pInstance as CLASS_DRIVEN_CANVAS;
            if (pCanvasInstance.Equals(null) == false)
                listCanvas.Add(pCanvasInstance);
        }

        return listCanvas;
    }

    static public List<CLASS_DRIVEN_CANVAS> GetAlreadyShow_PopupList<CLASS_DRIVEN_CANVAS>()
        where CLASS_DRIVEN_CANVAS : class, ICanvas
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        List<CanvasWrapper> listCanvas_AlreadyShow = pInstance._list_CanvasShow;
        List<CLASS_DRIVEN_CANVAS> listCanvas = new List<CLASS_DRIVEN_CANVAS>(listCanvas_AlreadyShow.Count);
        for (int i = 0; i < listCanvas_AlreadyShow.Count; i++)
        {
            CLASS_DRIVEN_CANVAS pCanvasParsing = listCanvas_AlreadyShow[i].pInstance as CLASS_DRIVEN_CANVAS;
            if (pCanvasParsing != null)
                listCanvas.Add(pCanvasParsing);
        }

        return listCanvas;
    }

    static public void DoClear()
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;

        if (pInstance == null)
            return;

        foreach(var pHandle in g_setCommandHandle)
            pHandle.Dispose();
        g_setCommandHandle.Clear();

        List<CanvasWrapper>[] arrData = pInstance._mapWrapper.Values.ToArray();
        for (int i = 0; i < arrData.Length; i++)
        {
            var list = arrData[i];
            for(int j = 0; j < list.Count; j++)
                pInstance.RemoveWrapper(list[j]);
        }

        pInstance._mapWrapper.Clear();
        pInstance._mapWrapper_Key_Is_Instance.Clear();

        pInstance._setProcessCreating.Clear();
        pInstance._list_CanvasShow.Clear();

        pInstance._mapManagerLogic.Clear();
        pInstance._mapManagerUndoLogic_Parser.Clear();

        pInstance._pWrapperPool.DoDestroyPool(false);
    }

    static public ICanvas GetLastShowCanvas_OrNull()
    {
        CLASS_DRIVEN_MANAGER pInstance = instance;
        if (pInstance.IsNull())
            return null;

        List<CanvasWrapper> list_CanvasShow = pInstance._list_CanvasShow;
        if (list_CanvasShow.Count == 0)
            return null;
        else
            return list_CanvasShow[list_CanvasShow.Count - 1].pInstance;
    }

    static public bool GetEnumKey(ICanvas pObject, out ENUM_CANVAS_NAME eName)
    {
        eName = default(ENUM_CANVAS_NAME);

        if (pObject.IsNull())
            return false;

        CLASS_DRIVEN_MANAGER pInstance = instance;
        if (pInstance.IsNull())
            return false;

        bool bGet_IsSuccess = true;
        CanvasWrapper pWrapper = null;

        if (pInstance._mapWrapper_Key_Is_Instance.TryGetValue(pObject, out pWrapper))
        {
            eName = pWrapper.eName;
        }
        else
        {
            bGet_IsSuccess = pInstance.GetEnumKey_Custom(pObject, out eName);

            if (bGet_IsSuccess == false)
                Debug.LogWarning("GetEnumKey - Fail " + pObject.gameObject.name);
        }

        return bGet_IsSuccess;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void OnCreate_ManagerInstance()
    {
        base.OnCreate_ManagerInstance();
        _pWrapperPool = new SimplePool<CanvasWrapper>((iCount) => new CanvasWrapper(), null, 10);

        // Null 체크를 안하기 위해, 개발편의를 위해 Empty List 삽입
        _mapManagerLogic.Clear();
        int iLoopMax = (int)EUIShowHideEvent.After_Hide_UIObject;
        for (int i = 0; i < iLoopMax; i++)
            _mapManagerLogic.Add((EUIShowHideEvent)i, new List<ICanvasManager_Logic>());

        OnInit_ManagerLogic(_mapManagerLogic);
        Init_ManagerUndoLogic();
    }

    protected override void OnDestroy_ManagerInstance()
    {
        base.OnDestroy_ManagerInstance();

        DoClear();
    }


    #region IUIWidgetContainerManager

    public override UICommandHandle<CLASS_UIOBJECT> IUIManager_Show<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
    {
        CanvasWrapper pWrapper;
        UICommandHandle<CLASS_UIOBJECT> pHandle = null;

        if (pUIObject.IsNull() == false)
        {
            GetWrapper_And_Handle(pUIObject, out pWrapper, out pHandle);
            StartCoroutine(CoProcess_Showing(pWrapper, pHandle));
        }

        return pHandle;
    }

    public override UICommandHandle<CLASS_UIOBJECT> IUIManager_Hide<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject, bool bPlayHideCoroutine)
    {
        CanvasWrapper pWrapper;
        UICommandHandle<CLASS_UIOBJECT> pHandle = null;

        if (pUIObject.IsNull() == false)
        {
            GetWrapper_And_Handle(pUIObject, out pWrapper, out pHandle);
            StartCoroutine(CoProcess_Hiding(pWrapper, pHandle, bPlayHideCoroutine));
        }

        return pHandle;
    }

    #endregion

    /* protected - [abstract & virtual]         */

    abstract protected void OnInit_ManagerLogic(Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>> mapManagerLogic);

    /// <summary>
    /// 컨테이너의 인스턴스를 만드는 방법을 구현합니다.
    /// <para> 컨테이너의 인스턴스가 없는 경우에만 호출됩니다. </para> 
    /// </summary>
    /// <param name="eName">인스턴스를 만들 팝업 이름 Enum</param>
    abstract protected IEnumerator OnCreate_Instance(ENUM_CANVAS_NAME eName, System.Action<ICanvas> OnFinish);

    /// <summary>
    /// 인스턴스에 알맞는 캔버스를 얻어오는 방법을 구현합니다.
    /// </summary>
    /// <param name="eName">캔버스가 필요한 이름 Enum</param>
    /// <param name="pCanvas">캔버스가 필요한 인스턴스</param>
    abstract public Canvas GetParentCavnas(ENUM_CANVAS_NAME eName, ICanvas pCanvas);


    virtual protected IEnumerator CoProcess_Showing<CLASS_DRIVEN_CANVAS>(CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
        where CLASS_DRIVEN_CANVAS : IUIObject
    {
        if (pWrapper == null || pWrapper.pInstance.Equals(null))
        {
            Debug.LogWarning(name + " CoProcess_Showing - pContainerWrapper == null || pContainerWrapper.pInstance.Equals(null)", this);
            yield break;
        }

        bool bIsShow = false;
        yield return sUICommandHandle.DoExecute_Check_IsShowCoroutine(
            (bool bIsShowParameter) => bIsShow = bIsShowParameter);

        if (bIsShow == false)
        {
            sUICommandHandle.Event_OnHide(false);
            pWrapper.DoSet_State_Is_Disable();

            yield break;
        }

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.Log(sUICommandHandle.pUIObject.gameObject.name + " Execute " + nameof(EUIShowHideEvent.Before_Show_UIObject), pWrapper.pInstance.gameObject);
        yield return Execute_ManagerLogic(EUIShowHideEvent.Before_Show_UIObject, pWrapper);
        sUICommandHandle.Event_OnBeforeShow();

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.Log(sUICommandHandle.pUIObject.gameObject.name + " Execute Show Coroutine", pWrapper.pInstance.gameObject);
        Canvas pCanvas = GetParentCavnas(pWrapper.eName, pWrapper.pInstance);
        yield return pWrapper.DoExecute_ShowCoroutine(pCanvas, sUICommandHandle);

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.Log(sUICommandHandle.pUIObject.gameObject.name + " Execute After Show", pWrapper.pInstance.gameObject);
        yield return Execute_ManagerLogic(EUIShowHideEvent.After_Show_UIObject, pWrapper);

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.Log(sUICommandHandle.pUIObject.gameObject.name + " Execute " + nameof(EUIShowHideEvent.After_Show_UIObject), pWrapper.pInstance.gameObject);
        sUICommandHandle.Event_OnShow_AfterAnimation();
        OnShow_AfterAnimation(pWrapper.eName, pWrapper.pInstance);

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.LogWarning(sUICommandHandle.pUIObject.gameObject.name + " Show Done", sUICommandHandle.pUIObject.gameObject);

        _list_CanvasShow.Add(pWrapper);

        yield break;
    }

    virtual protected IEnumerator CoProcess_Hiding<CLASS_DRIVEN_CANVAS>(CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle, bool bPlayHideCoroutine)
        where CLASS_DRIVEN_CANVAS : IUIObject
    {
        if (pWrapper == null)
            yield break;

        _list_CanvasShow.Remove(pWrapper);

        if (pWrapper.eState == CanvasWrapper.EState.Process_Hide_BeforeAnimation)
            yield break;

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.Log(sUICommandHandle.pUIObject.gameObject.name + " Execute Before Hide", sUICommandHandle.pUIObject.gameObject);
        yield return Execute_ManagerLogic(EUIShowHideEvent.Before_Hide_UIObject, pWrapper);

        if(bPlayHideCoroutine)
        {
            if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
                Debug.Log(sUICommandHandle.pUIObject.gameObject.name + " Execute Hide Coroutine", sUICommandHandle.pUIObject.gameObject);
            yield return pWrapper.DoExecute_HideCoroutine();
        }

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.Log(sUICommandHandle.pUIObject.gameObject.name + " Execute After Hide", sUICommandHandle.pUIObject.gameObject);
        yield return Execute_ManagerLogic(EUIShowHideEvent.After_Hide_UIObject, pWrapper);

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.Log(sUICommandHandle.pUIObject.gameObject.name + " Execute Hide", sUICommandHandle.pUIObject.gameObject);
        sUICommandHandle.Event_OnHide(true);

        int iInstanceCount = Get_MatchWrapperList(pWrapper.eName, (x) => x.Check_IsEnable()).Count;
        OnHide(pWrapper.eName, pWrapper.pInstance, iInstanceCount);

        pWrapper.DoSet_State_Is_Disable();
        if (Check_Wrapper_IsNull(pWrapper))
            RemoveWrapper(pWrapper);

        if (const_bIsDebug && sUICommandHandle.pUIObject.gameObject != null)
            Debug.LogWarning(sUICommandHandle.pUIObject.gameObject.name + " Hide Done", sUICommandHandle.pUIObject.gameObject);

        yield break;
    }

    /// <summary>
    /// 오브젝트가 켜질 때 호출됩니다.
    /// <para> 매니져 Show(OnBeforeShow) -> <see cref="IUIObject.OnShowCoroutine"/> -> 매니져 Show(OnShow) -> 이 함수 순으로 호출됩니다. </para> 
    /// </summary>
    /// <param name="eName">켜지는 오브젝트의 이름 Enum</param>
    /// <param name="pInstance">켜지는 오브젝트의 인스턴스</param>
    virtual protected void OnShow_AfterAnimation(ENUM_CANVAS_NAME eName, ICanvas pInstance) { }

    /// <summary>
    /// 오브젝트가 꺼질 때 호출됩니다.
    /// <para> <see cref="IUIObject.OnHideCoroutine"/> -> 매니져 Hide(OnHide) -> 이 함수 순으로 호출됩니다. </para> 
    /// </summary>
    /// <param name="eName">꺼지는 오브젝트의 이름 Enum</param>
    /// <param name="pInstance">꺼지는 오브젝트의 인스턴스</param>
    virtual protected void OnHide(ENUM_CANVAS_NAME eName, ICanvas pInstance, int iInstanceCount) { }

    /// <summary>
    /// Default <see cref="GetEnumKey(ICanvas, out ENUM_CANVAS_NAME)"/>를 Fail하면 호출되는 함수입니다.
    /// </summary>
    /// <param name="pInstance"></param>
    /// <param name="eName"></param>
    /// <returns></returns>
    virtual protected bool GetEnumKey_Custom(ICanvas pInstance, out ENUM_CANVAS_NAME eName) { eName = default(ENUM_CANVAS_NAME); return false; }

    virtual protected ICanvas Get_CanvasInstance(ENUM_CANVAS_NAME eName)
    {
        CanvasWrapper pContainer = null;
        if (Get_UnUsedWrapper(eName, out pContainer))
            return pContainer.pInstance;
        else
            return default(ICanvas);
    }

    // ========================================================================== //

    #region Private

    IEnumerator CoProcess_Showing<T>(ENUM_CANVAS_NAME eName, UICommandHandle<T> sUICommandHandle, bool bIsMultiple)
        where T : class, ICanvas
    {
        CanvasWrapper pWrapper = null;
        bool bCreateInstance = Check_IsCreateInstance(eName, bIsMultiple, out pWrapper);
        pWrapper?.DoSet_State_IsEnable();

        if (bCreateInstance)
        {
            ICanvas pInstance = null;
            yield return OnCreate_Instance(eName,
                (ICanvas pCanvas) =>
                {
                    pInstance = pCanvas;
                });

            if (pInstance == null)
            {
                Debug.LogError(name + " CoProcess_Showing - eName : " + eName + "pInstance == null", this);
                yield break;
            }

            pWrapper = CreateWrapper_OrNull(eName, pInstance);
            pWrapper?.DoSet_State_IsEnable();

            _setProcessCreating.Remove(eName);
            if (const_bIsDebug)
                Debug.LogWarning(name + " removed Creating " + eName);
        }

        if (pWrapper == null)
            yield break;

        sUICommandHandle.Set_UIObject(pWrapper.pInstance as T);

        yield return CoProcess_Showing(pWrapper, sUICommandHandle);
    }

    IEnumerator CoProcess_Hiding<T>(ENUM_CANVAS_NAME eName, UICommandHandle<T> sUICommandHandle)
        where T : class, ICanvas
    {
        CanvasWrapper pWrapper = null;
        var listEnableWrapper = Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
        if (listEnableWrapper.Count == 0)
        {
            Debug.LogWarning(name + " CoProcess_Hiding - eName : " + eName + " listEnableWrapper.Count == 0", this);
            yield break;
        }

        pWrapper = listEnableWrapper[0];
        pWrapper.DoSet_State_IsEnable();
        sUICommandHandle.Set_UIObject(pWrapper.pInstance as T);

        yield return CoProcess_Hiding(pWrapper, sUICommandHandle, true);
    }

    #region Manage_Wrapper

    private void GetWrapper_And_Handle<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject, out CanvasWrapper pWrapper, out UICommandHandle<CLASS_UIOBJECT> pHandle)
        where CLASS_UIOBJECT : IUIObject
    {
        ICanvas pCanvas = pUIObject as ICanvas;
        if (_mapWrapper_Key_Is_Instance.TryGetValue(pCanvas, out pWrapper) == false)
        {
            ENUM_CANVAS_NAME eKey;
            GetEnumKey(pCanvas, out eKey);
            pWrapper = CreateWrapper_OrNull(eKey, pCanvas);
        }

        pHandle = UICommandHandle<CLASS_UIOBJECT>.GetInstance(pUIObject);
        g_setCommandHandle.Add(pHandle);

        pHandle.Set_UIObject(pUIObject);
    }

    protected bool Get_UnUsedWrapper(ENUM_CANVAS_NAME eName, out CanvasWrapper pWrapper)
    {
        bool bGet_IsSuccess = false;
        pWrapper = null;

        var listDisableWrapper = Get_MatchWrapperList(eName, (x) => x.Check_IsEnable() == false);
        if (listDisableWrapper.Count > 0)
        {
            pWrapper = listDisableWrapper[listDisableWrapper.Count - 1];
            bGet_IsSuccess = true;
        }

        return bGet_IsSuccess;
    }

    List<CanvasWrapper> _listTempWrapper = new List<CanvasWrapper>();
    private List<CanvasWrapper> Get_MatchWrapperList(ENUM_CANVAS_NAME eName, System.Func<CanvasWrapper, bool> OnCheck_IsMatch)
    {
        _listTempWrapper.Clear();
        if (_mapWrapper.ContainsKey(eName) == false)
            return _listTempWrapper;

        List<CanvasWrapper> listWrapper = _mapWrapper[eName];
        for(int i = 0; i < listWrapper.Count; i++)
        {
            CanvasWrapper pWrapper = listWrapper[i];
            if (pWrapper.pInstance.Equals(null))
            {
                RemoveWrapper(pWrapper);
                i--;
                continue;
            }

            if (OnCheck_IsMatch(pWrapper))
                _listTempWrapper.Add(pWrapper);
        }

        return _listTempWrapper;
    }

    private static bool Check_Wrapper_IsNull(CanvasWrapper pWrapper)
    {
        return (pWrapper == null || pWrapper.pInstance.Equals(null));
    }

    bool Check_IsCreateInstance(ENUM_CANVAS_NAME eName, bool bIsMultiple, out CanvasWrapper pWrapper)
    {
        pWrapper = null;
        bool bCreateInstance = false;
        if (bIsMultiple)
        {
            // 멀티 팝업일 때 지금 쓸수 있는 팝업이 없으면 무조건 인스턴스 생성
            bCreateInstance = Get_UnUsedWrapper(eName, out pWrapper) == false;
        }
        else
        {
            // 싱글 팝업일 때 현재 쓰고있는 팝업이 없으면 인스턴스 생성
            bCreateInstance = Get_MatchWrapperList(eName, (x) => x.Check_IsEnable()).Count == 0;
            if (bCreateInstance)
            {
                if (_setProcessCreating.Contains(eName) == false)
                {
                    _setProcessCreating.Add(eName);

                    if (const_bIsDebug)
                        Debug.LogWarning(name + " added Creating " + eName + " Count : " + Get_MatchWrapperList(eName, (x) => x.Check_IsEnable()).Count);
                }
            }
            else
            {
                // 싱글 팝업일 때 인스턴스를 만들 필요가 없으면 지금 쓸수 있는 팝업 사용
                Get_UnUsedWrapper(eName, out pWrapper);
            }
        }

        return bCreateInstance;
    }

    protected CanvasWrapper CreateWrapper_OrNull<T>(ENUM_CANVAS_NAME eName, T pInstance)
        where T : class, ICanvas
    {
        CanvasWrapper pWrapper = null;
        try
        {
            pWrapper = _pWrapperPool.DoPop();
            pWrapper.Init(eName, pInstance, StartCoroutine, StopCoroutine);

            if (_mapWrapper.ContainsKey(eName) == false)
                _mapWrapper.Add(eName, new List<CanvasWrapper>());

            _mapWrapper[eName].Add(pWrapper);
            _mapWrapper_Key_Is_Instance.Add(pInstance, pWrapper);

            pWrapper.pInstance.pUIManager = this;
        }
        catch (System.Exception e)
        {
            Debug.LogError("CreateWrapper_OrNull eName : " + eName + " pInstance : " + pInstance + "\nException : " + e);
            _pWrapperPool.DoPush(pWrapper);
        }

        return pWrapper;
    }

    protected void RemoveWrapper(CanvasWrapper pWrapper)
    {
        if(_mapWrapper.ContainsKey(pWrapper.eName))
            _mapWrapper[pWrapper.eName].Remove(pWrapper);

        _mapWrapper_Key_Is_Instance.Remove(pWrapper.pInstance);

        _pWrapperPool.DoPush(pWrapper);
    }

    #endregion Manage_Wrapper

    #region ManagerLogic

    private void Init_ManagerUndoLogic()
    {
        _mapManagerUndoLogic_Parser.Clear();
        foreach (var pLogicList in _mapManagerLogic.Values)
        {
            for (int i = 0; i < pLogicList.Count; i++)
            {
                ICanvasManager_Logic pLogic = pLogicList[i];
                if (_mapManagerUndoLogic_Parser.ContainsKey(pLogic))
                    continue;

                CanvasManager_LogicUndo_Wrapper pUndoLogic = pLogic as CanvasManager_LogicUndo_Wrapper;
                if (pUndoLogic == null)
                    continue;

                _mapManagerUndoLogic_Parser.Add(pLogic, pUndoLogic);
            }
        }
    }

    private IEnumerator Execute_ManagerLogic(EUIShowHideEvent eUIEvent, CanvasWrapper pContainerWrapper)
    {
        if (eUIEvent != EUIShowHideEvent.Before_Show_UIObject)
            yield return pContainerWrapper.DoExecute_Manager_UndoCoroutineLogic(this, eUIEvent);

        if (_mapManagerLogic.ContainsKey(eUIEvent) == false)
            yield break;

        yield return pContainerWrapper.DoExecute_Manager_CoroutineLogic(this, eUIEvent, _mapManagerLogic[eUIEvent], GetUndoLogic, const_bIsDebug);
    }

    CanvasManager_LogicUndo_Wrapper GetUndoLogic(ICanvasManager_Logic pLogic)
    {
        if (_mapManagerUndoLogic_Parser.ContainsKey(pLogic) == false)
            return null;

        return _mapManagerUndoLogic_Parser[pLogic];
    }

    #endregion ManagerLogic

    #endregion Private
}