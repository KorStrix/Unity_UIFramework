﻿#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-21 오후 12:46:22
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UIFramework.UIWidgetContainerManager_Logic;

namespace UIFramework
{

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

        /// <summary>
        /// <see cref="ICanvas"/> 래퍼
        /// <para>1. <see cref="ENUM_CANVAS_NAME"/> 관리</para>
        /// <para>2. <see cref="ICanvasManager_Logic"/> 관리</para>
        /// <para>3. <see cref="EUIObjectState"/> 관리</para>
        /// <para>4. Show/Hide Coroutine 관리</para>
        /// </summary>
        public class CanvasWrapper
        {
            public EUIObjectState eState { get; private set; }
            public ENUM_CANVAS_NAME eName { get; private set; }
            public ICanvas pInstance { get; private set; }

            // 로직 캐싱 - 일일이 파싱체크 방지
            Dictionary<EUIObjectState, List<CanvasManager_LogicUndo_Wrapper>> mapUndoLogic = new Dictionary<EUIObjectState, List<CanvasManager_LogicUndo_Wrapper>>()
        {
            { EUIObjectState.Process_Before_ShowCoroutine, new List<CanvasManager_LogicUndo_Wrapper>() },
            { EUIObjectState.Process_Before_HideCoroutine, new List<CanvasManager_LogicUndo_Wrapper>() },
            { EUIObjectState.Process_After_ShowCoroutine, new List<CanvasManager_LogicUndo_Wrapper>() },
            { EUIObjectState.Process_After_HideCoroutine, new List<CanvasManager_LogicUndo_Wrapper>() },
            { EUIObjectState.Showing, new List<CanvasManager_LogicUndo_Wrapper>() },
            { EUIObjectState.Disable, new List<CanvasManager_LogicUndo_Wrapper>() },
        };


            List<IUIWidget> _listChildrenWidget = new List<IUIWidget>();
            List<Coroutine> _listCoroutine = new List<Coroutine>();
            List<Coroutine> _listManager_CoroutineLogic = new List<Coroutine>();
            List<Coroutine> _listManager_Coroutine_UndoLogic = new List<Coroutine>();

            System.Func<IEnumerator, Coroutine> _OnStartCoroutine;
            System.Action<Coroutine> _OnStopCoroutine;


            public void Init(ENUM_CANVAS_NAME eName, ICanvas pInstance, System.Func<IEnumerator, Coroutine> OnStartCoroutine, System.Action<Coroutine> OnStopCoroutine)
            {
                this.eName = eName; this.pInstance = pInstance; this._OnStartCoroutine = OnStartCoroutine; this._OnStopCoroutine = OnStopCoroutine;

                DoSet_State(EUIObjectState.Disable);
                _listChildrenWidget.Clear();
                pInstance.gameObject?.GetComponentsInChildren(true, _listChildrenWidget);
                for (int i = 0; i < _listChildrenWidget.Count; i++)
                {
                    if (_listChildrenWidget[i] is IUIObject_Managed)
                        _listChildrenWidget.RemoveAt(i--);
                }
            }

            public IEnumerator DoExecute_ShowCoroutine<CLASS_DRIVEN_CANVAS>(Canvas pCanvas, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
                where CLASS_DRIVEN_CANVAS : IUIObject
            {
                StopCoroutine();
                if (pInstance.IsNull())
                    yield break;

                SettingUI(pCanvas);

                sUICommandHandle.Event_OnShow_BeforeAnimation();
                StartCoroutine_Show();
                yield return _listCoroutine.GetEnumerator_Safe();
            }

            public IEnumerator DoExecute_HideCoroutine()
            {
                StopCoroutine();

                if (pInstance.IsNull())
                    yield break;

                StartCoroutine_Hide();
                yield return _listCoroutine.GetEnumerator_Safe();
            }

            public IEnumerator DoExecute_Manager_CoroutineLogic(MonoBehaviour pManager, EUIObjectState eEvent, IEnumerable<ICanvasManager_Logic> listManagerLogic, System.Func<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper> GetUndoLogic, bool bIsDebug)
            {
                _listManager_CoroutineLogic.Clear();

                if (eEvent == EUIObjectState.Process_After_HideCoroutine)
                {
                    foreach (ICanvasManager_Logic pLogic in listManagerLogic)
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
                    {
                        _listManager_CoroutineLogic.Add(_OnStartCoroutine(pLogic.Execute_LogicCoroutine(pManager, pInstance, const_bIsDebug)));

                        CanvasManager_LogicUndo_Wrapper pUndoLogic = GetUndoLogic(pLogic);
                        if (pUndoLogic == null)
                            continue;

                        mapUndoLogic[pUndoLogic.eWhenUndo].Add(pUndoLogic);
                    }
                }

                return _listManager_CoroutineLogic.GetEnumerator_Safe();
            }

            public IEnumerator DoExecute_Manager_UndoLogic_Coroutine(MonoBehaviour pManager, EUIObjectState eEvent)
            {
                List<CanvasManager_LogicUndo_Wrapper> listUndoLogic = mapUndoLogic[eEvent];

                _listManager_Coroutine_UndoLogic.Clear();
                for (int i = 0; i < listUndoLogic.Count; i++)
                    _listManager_Coroutine_UndoLogic.Add(_OnStartCoroutine(listUndoLogic[i].Execute_UndoLogic_Coroutine(pManager, pInstance, const_bIsDebug)));
                listUndoLogic.Clear();

                return _listManager_Coroutine_UndoLogic.GetEnumerator_Safe();
            }

            public void DoExecute_Manager_UndoLogic(MonoBehaviour pManager, EUIObjectState eEvent)
            {
                List<CanvasManager_LogicUndo_Wrapper> listUndoLogic = mapUndoLogic[eEvent];

                for (int i = 0; i < listUndoLogic.Count; i++)
                    listUndoLogic[i].Execute_UndoLogic_NotCoroutine(pManager, pInstance, const_bIsDebug);
                listUndoLogic.Clear();
            }

            public void DoSet_State_IsEnable()
            {
                DoSet_State(EUIObjectState.Showing);
            }

            public void DoSet_State_Is_Disable_Force()
            {
                if (pInstance.Equals(null) == false)
                    pInstance.gameObject?.SetActive(false);
                DoSet_State(EUIObjectState.Disable);
            }

            public void DoSet_State(EUIObjectState eState)
            {
                // Debug.LogError(eName + "State : " + eState, pInstance.gameObject);
                this.eState = eState;
            }

            public bool Check_IsEnable()
            {
                return eState != EUIObjectState.Disable;
            }

            public bool Check_IsDisable()
            {
                return eState == EUIObjectState.Disable;
            }

            public void Clear_Manager_UndoLogic()
            {
                foreach (var pUndoLogicList in mapUndoLogic.Values)
                    pUndoLogicList.Clear();
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
                if (pCanvas != null)
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

        Dictionary<EUIObjectState, List<ICanvasManager_Logic>> _mapManagerLogic = new Dictionary<EUIObjectState, List<ICanvasManager_Logic>>();
        Dictionary<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper> _mapManagerUndoLogic_Parser = new Dictionary<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper>();

        List<ICanvas> _list_CanvasShowInstance = new List<ICanvas>();
        HashSet<ENUM_CANVAS_NAME> _setProcessCreating = new HashSet<ENUM_CANVAS_NAME>();
        SimplePool<CanvasWrapper> _pWrapperPool;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 캔버스를 Show합니다. 리턴하는 <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="CanvasWrapper.EState.Showing"/>인 상태인 캔버스가 있을 경우</para>
        /// <para>아무 동작하지 않습니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS"></typeparam>
        /// <param name="eName">Show 할 캔버스 이름</param>
        static public UICommandHandle<CLASS_DRIVEN_CANVAS> DoShow<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : class, ICanvas
        {
            CLASS_DRIVEN_MANAGER pInstance = instance;

            UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = GetCommandHandle<CLASS_DRIVEN_CANVAS>(null);
            pInstance.StartCoroutine(pInstance.Process_ShowCoroutine(eName, pHandle, false));

            return pHandle;
        }

        /// <summary>
        /// 캔버스를 Show합니다. 리턴하는 <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="CanvasWrapper.EState.Showing"/>인 상태인 캔버스가 있을 경우</para>
        /// <para>캔버스의 인스턴스를 새로 만들어서 보여줍니다</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS"></typeparam>
        /// <param name="eName">Show 할 캔버스 이름</param>
        static public UICommandHandle<CLASS_DRIVEN_CANVAS> DoShow_Multiple<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : class, ICanvas
        {
            CLASS_DRIVEN_MANAGER pInstance = instance;

            UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = GetCommandHandle<CLASS_DRIVEN_CANVAS>(null);
            pInstance.StartCoroutine(pInstance.Process_ShowCoroutine(eName, pHandle, true));

            return pHandle;
        }

        /// <summary>
        /// 캔버스를 Hide합니다. 리턴하는 <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="CanvasWrapper.EState.Showing"/>인 상태인 캔버스가 있을때만 동작합니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS"></typeparam>
        /// <param name="eName">Hide 할 캔버스 이름</param>
        static public UICommandHandle<CLASS_DRIVEN_CANVAS> DoHide<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : class, ICanvas
        {
            CLASS_DRIVEN_MANAGER pInstance = instance;

            UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = GetCommandHandle<CLASS_DRIVEN_CANVAS>(null);
            pInstance.StartCoroutine(pInstance.Process_HideCoroutine(eName, pHandle));

            return pHandle;
        }

        /// <summary>
        /// 캔버스를 Show합니다. <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="CanvasWrapper.EState.Showing"/>인 상태인 캔버스가 있을 경우</para>
        /// <para>아무 동작하지 않습니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS"></typeparam>
        /// <param name="eName">Show 할 캔버스 이름</param>
        static public UICommandHandle<ICanvas> DoShowOnly(ENUM_CANVAS_NAME eName)
        {
            CLASS_DRIVEN_MANAGER pInstance = instance;

            UICommandHandle<ICanvas> pHandle = GetCommandHandle<ICanvas>(null);
            pInstance.StartCoroutine(pInstance.Process_ShowCoroutine(eName, pHandle, false));

            return pHandle;
        }

        /// <summary>
        /// 캔버스를 Hide합니다. <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="CanvasWrapper.EState.Showing"/>인 상태인 캔버스가 없을 경우</para>
        /// <para>아무 동작하지 않습니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS"></typeparam>
        /// <param name="eName">Hide 할 캔버스 이름</param>
        static public UICommandHandle<ICanvas> DoHideOnly(ENUM_CANVAS_NAME eName)
        {
            CLASS_DRIVEN_MANAGER pInstance = instance;

            UICommandHandle<ICanvas> pHandle = GetCommandHandle<ICanvas>(null);
            pInstance.StartCoroutine(pInstance.Process_HideCoroutine(eName, pHandle));

            return pHandle;
        }

        /// <summary>
        /// 모든 팝업을 닫습니다
        /// </summary>
        /// <param name="bPlayHideCoroutine"><see cref="IUIObject.OnHideCoroutine"/>실행 유무</param>
        static public void DoAllHide_ShowedCanvas(bool bPlayHideCoroutine = true)
        {
            List<ICanvas> listCanavs = GetAlreadyShow_CanvasList();
            if (bPlayHideCoroutine)
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

        /// <summary>
        /// 이미 보여지고 있는 Canvas를 Return합니다. 없으면 null을 리턴합니다.
        /// <para>여러개일 경우 오래된 Canvas 1개만 리턴합니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS">형변환 할 Canvas 타입</typeparam>
        /// <param name="eName">얻고자 하는 캔버스 이름</param>
        static public CLASS_DRIVEN_CANVAS GetAlreadyShow_Canvas_OrNull<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : MonoBehaviour, ICanvas
        {
            CLASS_DRIVEN_MANAGER pInstance = instance;

            var listWrapper = pInstance.Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
            if (listWrapper.Count > 0)
                return listWrapper[0].pInstance as CLASS_DRIVEN_CANVAS;
            else
                return null;
        }

        /// <summary>
        /// 이미 보여지고 있는 Canvas List를 Return합니다. 없으면 count == 0인 list를 리턴합니다.
        /// <para><see cref="DoShow_Multiple{CLASS_DRIVEN_CANVAS}(ENUM_CANVAS_NAME)"/>을 통해 Show를 할 경우 유효합니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS">형변환 할 Canvas 타입</typeparam>
        /// <param name="eName">얻고자 하는 캔버스 이름</param>
        static public List<CLASS_DRIVEN_CANVAS> GetAlreadyShow_CanvasList<CLASS_DRIVEN_CANVAS>(ENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : class, ICanvas
        {
            CLASS_DRIVEN_MANAGER pInstance = instance;

            var listWrapper = pInstance.Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
            return listWrapper.Where(p => p.pInstance is CLASS_DRIVEN_CANVAS && p.pInstance.IsNull() == false).Select(p => p.pInstance as CLASS_DRIVEN_CANVAS).ToList();
        }

        /// <summary>
        /// 이미 보여지고 있는 Canvas List를 Return합니다. 없으면 count == 0인 list를 리턴합니다.
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS">형변환 할 Canvas 타입</typeparam>
        static public List<ICanvas> GetAlreadyShow_CanvasList()
        {
            if (_bApplication_IsQuit)
                return new List<ICanvas>();

            return new List<ICanvas>(instance._list_CanvasShowInstance.Where(p => p.IsNull() == false));
        }

        /// <summary>
        /// 마지막에 Show를 한 Canvas를 리턴합니다. 없으면 Null을 리턴합니다.
        /// </summary>
        static public ICanvas GetLastShowCanvas_OrNull()
        {
            CLASS_DRIVEN_MANAGER pInstance = instance;
            if (pInstance.IsNull())
                return null;

            return pInstance._list_CanvasShowInstance.LastOrDefault();
        }

        /// <summary>
        /// 캔버스의 Key를 얻습니다. 리턴은 Key 얻기 유무입니다.
        /// </summary>
        /// <param name="pObject">Key를 얻을 캔버스 인스턴스</param>
        /// <param name="eName">리턴받을 Key</param>
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

        /// <summary>
        /// 매니져의 인스턴스가 생성될 때
        /// </summary>
        protected override void OnCreate_ManagerInstance()
        {
            base.OnCreate_ManagerInstance();

            _pWrapperPool = new SimplePool<CanvasWrapper>((iCount) => new CanvasWrapper(), null, 10);

            CanvasManagerLogicFactory pFactory = new CanvasManagerLogicFactory();
            int iLoopMax = (int)EUIObjectState.MAX;

            if (const_bIsDebug)
            {
                for (int i = 0; i < iLoopMax; i++)
                    pFactory.DoAddLogic((EUIObjectState)i, new Print_CanvasState((EUIObjectState)i));
            }

            OnInit_ManagerLogic(pFactory);

            // Null 체크를 안하기 위해, 개발편의를 위해 Empty List 삽입
            _mapManagerLogic = new Dictionary<EUIObjectState, List<ICanvasManager_Logic>>(pFactory.mapLogicContainer);
            for (int i = 0; i < iLoopMax; i++)
            {
                if(_mapManagerLogic.ContainsKey((EUIObjectState)i) == false)
                    _mapManagerLogic.Add((EUIObjectState)i, new List<ICanvasManager_Logic>());
            }

            Init_ManagerUndoLogic();
        }

        /// <summary>
        /// 매니져의 인스턴스가 파괴될 때
        /// </summary>
        protected override void OnDestroy_ManagerInstance()
        {
            base.OnDestroy_ManagerInstance();

            DoAllHide_ShowedCanvas(false);

            CLASS_DRIVEN_MANAGER pInstance = instance;
            if (pInstance.IsNull())
                return;

            foreach (var pHandle in g_setCommandHandle)
                pHandle.Dispose();
            g_setCommandHandle.Clear();

            List<CanvasWrapper>[] arrData = pInstance._mapWrapper.Values.ToArray();
            for (int i = 0; i < arrData.Length; i++)
            {
                var list = arrData[i];
                for (int j = 0; j < list.Count; j++)
                    pInstance.RemoveWrapper(list[j]);
            }

            pInstance._mapWrapper.Clear();
            pInstance._mapWrapper_Key_Is_Instance.Clear();

            pInstance._setProcessCreating.Clear();
            pInstance._list_CanvasShowInstance.Clear();

            pInstance._mapManagerLogic.Clear();
            pInstance._mapManagerUndoLogic_Parser.Clear();

            pInstance._pWrapperPool.DoDestroyPool(false);
        }


        #region IUIWidgetContainerManager

        public override UICommandHandle<CLASS_UIOBJECT> IUIManager_Show<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
        {
            CanvasWrapper pWrapper;
            UICommandHandle<CLASS_UIOBJECT> pHandle = null;

            if (pUIObject.IsNull() == false)
            {
                GetWrapper_And_Handle(pUIObject, out pWrapper, out pHandle);
                StartCoroutine(Process_ShowCoroutine(pWrapper, pHandle));
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

                if (bPlayHideCoroutine)
                    StartCoroutine(Process_HideCoroutine(pWrapper, pHandle));
                else
                    Process_Hide(pWrapper, pHandle);
            }

            return pHandle;
        }

        public override EUIObjectState IUIManager_GetUIObjectState<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
        {
            ICanvas pCanvas = pUIObject as ICanvas;
            CanvasWrapper pCanvasWrapper;
            if (_mapWrapper_Key_Is_Instance.TryGetValue(pCanvas, out pCanvasWrapper) == false)
                return EUIObjectState.Error;

            return pCanvasWrapper.eState;
        }

        #endregion

        /* protected - [abstract & virtual]         */

        /// <summary>
        /// 매니져의 로직을 Init할 때, 매개변수의 Dictionary에 원하는 `로직`을 Add를 해야 합니다.
        /// <para>`로직`은 <see cref="ICanvasManager_Logic"/>을 상속받은 클래스입니다.</para>
        /// </summary>
        /// <param name="pLogicFactory"></param>
        abstract protected void OnInit_ManagerLogic(CanvasManagerLogicFactory pLogicFactory);

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

        /// <summary>
        /// 오브젝트가 켜질 때 호출됩니다.
        /// <para> 매니져 Show(OnBeforeShow) -> <see cref="IUIObject.OnShowCoroutine"/> -> 매니져 Show(OnShow) -> 이 함수 순으로 호출됩니다. </para> 
        /// </summary>
        /// <param name="eName">켜지는 오브젝트의 이름 Enum</param>
        /// <param name="pInstance">켜지는 오브젝트의 인스턴스</param>
        virtual protected void OnShow_BeforeAnimation(ENUM_CANVAS_NAME eName, ICanvas pInstance) { }


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

        protected IEnumerator Process_ShowCoroutine<CLASS_DRIVEN_CANVAS>(CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            if (pWrapper == null || pWrapper.pInstance.IsNull())
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
                pWrapper.DoSet_State_Is_Disable_Force();

                yield break;
            }

            _list_CanvasShowInstance.Add(pWrapper.pInstance);

            OnShow_BeforeAnimation(pWrapper.eName, pWrapper.pInstance);

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Process_Before_ShowCoroutine, pWrapper);
            sUICommandHandle.Event_OnBeforeShow();

            Canvas pCanvas = GetParentCavnas(pWrapper.eName, pWrapper.pInstance);
            yield return pWrapper.DoExecute_ShowCoroutine(pCanvas, sUICommandHandle);

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Process_After_ShowCoroutine, pWrapper);
            sUICommandHandle.Event_OnShow_AfterAnimation();

            OnShow_AfterAnimation(pWrapper.eName, pWrapper.pInstance);

            // _list_CanvasShow.Add(pWrapper);

            yield break;
        }

        virtual protected IEnumerator Process_HideCoroutine<CLASS_DRIVEN_CANVAS>(CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            if (pWrapper == null)
                yield break;

            _list_CanvasShowInstance.Remove(pWrapper.pInstance);

            if (pWrapper.eState == EUIObjectState.Process_Before_HideCoroutine)
                yield break;

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Process_Before_HideCoroutine, pWrapper);

            yield return pWrapper.DoExecute_HideCoroutine();

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Process_After_HideCoroutine, pWrapper);

            sUICommandHandle.Event_OnHide(true);

            int iInstanceCount = Get_MatchWrapperList(pWrapper.eName, x => x.Check_IsEnable()).Count;
            OnHide(pWrapper.eName, pWrapper.pInstance, iInstanceCount);

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Disable, pWrapper);

            pWrapper.DoSet_State_Is_Disable_Force();
            if (Check_Wrapper_IsNull(pWrapper))
                RemoveWrapper(pWrapper);

            yield break;
        }

        virtual protected void Process_Hide<CLASS_DRIVEN_CANVAS>(CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            if (pWrapper == null)
                return;

            _list_CanvasShowInstance.Remove(pWrapper.pInstance);

            if (pWrapper.eState == EUIObjectState.Process_Before_HideCoroutine)
                return;

            Execute_ManagerUndoLogic(EUIObjectState.Process_Before_HideCoroutine, pWrapper);

            // pWrapper.DoExecute_HideCoroutine();

            Execute_ManagerUndoLogic(EUIObjectState.Process_After_HideCoroutine, pWrapper);

            sUICommandHandle.Event_OnHide(true);

            int iInstanceCount = Get_MatchWrapperList(pWrapper.eName, x => x.Check_IsEnable()).Count;
            OnHide(pWrapper.eName, pWrapper.pInstance, iInstanceCount);

            Execute_ManagerUndoLogic(EUIObjectState.Disable, pWrapper);

            pWrapper.DoSet_State_Is_Disable_Force();
            if (Check_Wrapper_IsNull(pWrapper))
                RemoveWrapper(pWrapper);
        }

        IEnumerator Process_ShowCoroutine<T>(ENUM_CANVAS_NAME eName, UICommandHandle<T> sUICommandHandle, bool bIsMultiple)
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

                if (pInstance.IsNull())
                {
                    Debug.LogError(name + " CoProcess_Showing - eName : " + eName + "pInstance == null", this);
                    yield break;
                }

                pWrapper = CreateWrapper_OrNull(eName, pInstance);
                pWrapper?.DoSet_State_IsEnable();

                _setProcessCreating.Remove(eName);
                if (const_bIsDebug)
                    Debug.LogWarning(name + " Removed Creating " + eName);
            }

            if (pWrapper == null)
                yield break;

            sUICommandHandle.Set_UIObject(pWrapper.pInstance as T);

            yield return Process_ShowCoroutine(pWrapper, sUICommandHandle);
        }

        IEnumerator Process_HideCoroutine<T>(ENUM_CANVAS_NAME eName, UICommandHandle<T> sUICommandHandle)
            where T : class, ICanvas
        {
            CanvasWrapper pWrapper = null;
            var listEnableWrapper = Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
            if (listEnableWrapper.Count == 0)
            {
                Debug.LogWarning(name + " CoProcess_Hiding - eName : " + eName + " listEnableWrapper.Count == 0", this);
                sUICommandHandle.Event_OnHide(true);
                yield break;
            }

            pWrapper = listEnableWrapper[0];
            pWrapper.DoSet_State_IsEnable();
            sUICommandHandle.Set_UIObject(pWrapper.pInstance as T);

            yield return Process_HideCoroutine(pWrapper, sUICommandHandle);
        }

        private static UICommandHandle<CLASS_DRIVEN_CANVAS> GetCommandHandle<CLASS_DRIVEN_CANVAS>(CLASS_DRIVEN_CANVAS pInstance_OrNull) where CLASS_DRIVEN_CANVAS : IUIObject
        {
            UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = UICommandHandle<CLASS_DRIVEN_CANVAS>.GetInstance(pInstance_OrNull);
            g_setCommandHandle.Add(pHandle);

            return pHandle;
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

            pHandle = GetCommandHandle<CLASS_UIOBJECT>(pUIObject);
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
            else
            {
                Debug.LogWarning(name + " Get_UnUsedWrapper - eName : " + eName + " listDisableWrapper.Count == 0", this);
                Get_MatchWrapperList(eName, (x) => x.Check_IsEnable() == false);
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
            for (int i = 0; i < listWrapper.Count; i++)
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
                // 싱글 팝업일 때 현재 쓸 수 있는 팝업이 없으면 인스턴스 생성
                // bCreateInstance = Get_MatchWrapperList(eName, (x) => x.Check_IsEnable()).Count == 0;
                bCreateInstance = Get_MatchWrapperList(eName, (x) => x.Check_IsDisable()).Count == 0;
                if (bCreateInstance)
                {
                    if (_setProcessCreating.Contains(eName) == false)
                    {
                        _setProcessCreating.Add(eName);

                        if (const_bIsDebug)
                            Debug.LogWarning(name + " added Creating " + eName + " Count : " + Get_MatchWrapperList(eName, (x) => x.Check_IsDisable()).Count);
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

                if (_mapWrapper_Key_Is_Instance.ContainsKey(pInstance) == false)
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
            if (_mapWrapper.ContainsKey(pWrapper.eName))
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

        private IEnumerator Execute_ManagerLogicCoroutine(EUIObjectState eUIEvent, CanvasWrapper pContainerWrapper)
        {
            pContainerWrapper.DoSet_State(eUIEvent);

            if (eUIEvent != EUIObjectState.Process_Before_ShowCoroutine)
                yield return pContainerWrapper.DoExecute_Manager_UndoLogic_Coroutine(this, eUIEvent);

            List<ICanvasManager_Logic> listLogic;
            if (_mapManagerLogic.TryGetValue(eUIEvent, out listLogic) == false)
                yield break;

            yield return pContainerWrapper.DoExecute_Manager_CoroutineLogic(this, eUIEvent, listLogic, GetUndoLogic, const_bIsDebug);
        }

        private void Execute_ManagerUndoLogic(EUIObjectState eUIEvent, CanvasWrapper pContainerWrapper)
        {
            pContainerWrapper.DoSet_State(eUIEvent);

            if (eUIEvent != EUIObjectState.Process_Before_ShowCoroutine)
                pContainerWrapper.DoExecute_Manager_UndoLogic(this, eUIEvent);
        }

        CanvasManager_LogicUndo_Wrapper GetUndoLogic(ICanvasManager_Logic pLogic)
        {
            CanvasManager_LogicUndo_Wrapper pWrapper;
            _mapManagerUndoLogic_Parser.TryGetValue(pLogic, out pWrapper);

            return pWrapper;
        }

        #endregion ManagerLogic

        #endregion Private

    }
}