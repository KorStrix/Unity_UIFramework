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
using System.Linq;
using UIFramework.UIWidgetContainerManager_Logic;

namespace UIFramework
{
    [System.Flags]
    public enum EDebugLevelFlags
    {
        None = 0,

        /// <summary>
        /// <see cref="CanvasManager{CLASS_DRIVEN_MANAGER,ENUM_CANVAS_NAME}"/> 클래스의 일반 로직(뭐가 Show됐는지, Hide됐는지) 디버깅용
        /// </summary>
        Manager = 1 << 0,

        /// <summary>
        /// <see cref="CanvasManager{CLASS_DRIVEN_MANAGER,ENUM_CANVAS_NAME}"/> 클래스의 코어 로직(뭐가 List에서 Add됐는지, Remove됐는지) 디버깅용
        /// </summary>
        Manager_Core = 1 << 1,

        /// <summary>
        /// <see cref="ICanvasManager_Logic"/> 로직 디버깅용
        /// </summary>
        ManagerLogic = 1 << 2,

        Detail = 1 << 3,
    }

    public static class UIDebug
    {
        static Dictionary<EDebugLevelFlags, string> _mapColorCode = new Dictionary<EDebugLevelFlags, string>()
        {
            {EDebugLevelFlags.Manager, "#aaaa00"},
            {EDebugLevelFlags.Manager_Core, "#ffff00"},
            {EDebugLevelFlags.ManagerLogic, "#ff0000"},
            {EDebugLevelFlags.Detail, "#ff00ff"},
        };

        public static string LogFormat(EDebugLevelFlags eLogType)
        {
            return $"<color={_mapColorCode[eLogType]}>[{eLogType}]</color> ";
        }
    }

    public abstract class CanvasManager<TDRIVEN_MANAGER, TENUM_CANVAS_NAME> : UIObjectManagerBase<TDRIVEN_MANAGER, ICanvas>
        where TDRIVEN_MANAGER : CanvasManager<TDRIVEN_MANAGER, TENUM_CANVAS_NAME>
    {
        /* const & readonly declaration             */

        //static readonly EDebugLevelFlags eDebugLevel = EDebugLevelFlags.Manager | EDebugLevelFlags.ManagerLogic | EDebugLevelFlags.Detail | EDebugLevelFlags.Manager_Core;

        // static readonly EDebugLevelFlags eDebugLevel = EDebugLevelFlags.Manager | EDebugLevelFlags.Manager_Core | EDebugLevelFlags.ManagerLogic;
        private static readonly EDebugLevelFlags eDebugLevel = EDebugLevelFlags.None;

        /* enum & struct declaration                */

        #region Wrapper

        /// <summary>
        /// <see cref="ICanvas"/> 래퍼
        /// <para>1. <see cref="TENUM_CANVAS_NAME"/> 관리</para>
        /// <para>2. <see cref="ICanvasManager_Logic"/> 관리</para>
        /// <para>3. <see cref="EUIObjectState"/> 관리</para>
        /// <para>4. Show/Hide Coroutine 관리</para>
        /// </summary>
        public class CanvasWrapper
        {
            public EUIObjectState eState { get; private set; }
            public TENUM_CANVAS_NAME eName { get; private set; }
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

            System.Func<CanvasWrapper, IEnumerator, Coroutine> _OnStartCoroutine;
            System.Action<Coroutine> _OnStopCoroutine;


            public void DoInit(TENUM_CANVAS_NAME eName, System.Func<CanvasWrapper, IEnumerator, Coroutine> OnStartCoroutine, System.Action<Coroutine> OnStopCoroutine)
            {
                this.eName = eName; this.pInstance = pInstance; this._OnStartCoroutine = OnStartCoroutine; this._OnStopCoroutine = OnStopCoroutine;

                DoSet_State(EUIObjectState.Creating);
            }

            public void DoSet_CanvasInstance(ICanvas pInstance)
            {
                this.pInstance = pInstance;

                pInstance.gameObject?.GetComponentsInChildren(true, _listChildrenWidget);
                _listChildrenWidget.Clear();
                for (int i = 0; i < _listChildrenWidget.Count; i++)
                {
                    if (_listChildrenWidget[i] is IUIObject_Managed)
                        _listChildrenWidget.RemoveAt(i--);
                }

                DoSet_State(EUIObjectState.Disable);
            }

            public IEnumerator DoExecute_ShowCoroutine<CLASS_DRIVEN_CANVAS>(Canvas pCanvas, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
                where CLASS_DRIVEN_CANVAS : IUIObject
            {
                StopCoroutine();
                if (pInstance.IsNull())
                    yield break;

                _listChildrenWidget.ForEach(p => p.IUIWidget_OnBeforeShow());
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

            public IEnumerator DoExecute_Manager_CoroutineLogic(MonoBehaviour pManager, EUIObjectState eEvent, IEnumerable<ICanvasManager_Logic> listManagerLogic, System.Func<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper> GetUndoLogic, EDebugLevelFlags eDebugLevel)
            {
                _listManager_CoroutineLogic.Clear();

                if (eEvent == EUIObjectState.Process_After_HideCoroutine)
                {
                    foreach (ICanvasManager_Logic pLogic in listManagerLogic)
                    {
                        Coroutine pCoroutine = _OnStartCoroutine(this, pLogic.Execute_LogicCoroutine(pManager, pInstance, eDebugLevel));
                        if (pCoroutine != null)
                            _listManager_CoroutineLogic.Add(pCoroutine);
                    }

                    Clear_Manager_UndoLogic();
                }
                else
                {
                    foreach (ICanvasManager_Logic pLogic in listManagerLogic)
                    {
                        _listManager_CoroutineLogic.Add(_OnStartCoroutine(this, pLogic.Execute_LogicCoroutine(pManager, pInstance, eDebugLevel)));

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
                    _listManager_Coroutine_UndoLogic.Add(_OnStartCoroutine(this, listUndoLogic[i].Execute_UndoLogic_Coroutine(pManager, pInstance, eDebugLevel)));
                listUndoLogic.Clear();

                return _listManager_Coroutine_UndoLogic.GetEnumerator_Safe();
            }

            public void DoExecute_Manager_UndoLogic(MonoBehaviour pManager, EUIObjectState eEvent)
            {
                List<CanvasManager_LogicUndo_Wrapper> listUndoLogic = mapUndoLogic[eEvent];

                for (int i = 0; i < listUndoLogic.Count; i++)
                    listUndoLogic[i].Execute_UndoLogic_NotCoroutine(pManager, pInstance, eDebugLevel);
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
                return eState != EUIObjectState.Disable && eState != EUIObjectState.Creating;
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

            public string GetObjectName()
            {
                return pInstance.GetObjectName_Safe();
            }

            private void StartCoroutine_Show()
            {
                _listCoroutine.Add(_OnStartCoroutine(this, pInstance.OnShowCoroutine()));

                for (int i = 0; i < _listChildrenWidget.Count; i++)
                    _listCoroutine.Add(_OnStartCoroutine(this, _listChildrenWidget[i].OnShowCoroutine()));
            }

            private void StartCoroutine_Hide()
            {
                _listCoroutine.Add(_OnStartCoroutine(this, pInstance.OnHideCoroutine()));

                for (int i = 0; i < _listChildrenWidget.Count; i++)
                    _listCoroutine.Add(_OnStartCoroutine(this, _listChildrenWidget[i].OnHideCoroutine()));
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

        // ReSharper disable once StaticMemberInGenericType
        static HashSet<System.IDisposable> g_setCommandHandle = new HashSet<System.IDisposable>();

        protected Dictionary<TENUM_CANVAS_NAME, List<CanvasWrapper>> _mapWrapper = new Dictionary<TENUM_CANVAS_NAME, List<CanvasWrapper>>();
        protected Dictionary<ICanvas, CanvasWrapper> _mapWrapper_Key_Is_Instance = new Dictionary<ICanvas, CanvasWrapper>();

        Dictionary<EUIObjectState, List<ICanvasManager_Logic>> _mapManagerLogic = new Dictionary<EUIObjectState, List<ICanvasManager_Logic>>();
        Dictionary<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper> _mapManagerUndoLogic_Parser = new Dictionary<ICanvasManager_Logic, CanvasManager_LogicUndo_Wrapper>();

        List<ICanvas> _list_CanvasShowInstance = new List<ICanvas>();
        Dictionary<TENUM_CANVAS_NAME, KeyValuePair<CanvasWrapper, object>> _mapProcessCreating = new Dictionary<TENUM_CANVAS_NAME, KeyValuePair<CanvasWrapper, object>>();
        SimplePool<CanvasWrapper> _pWrapperPool;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 캔버스를 Show합니다. 리턴하는 <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="EUIObjectState.Showing"/>인 상태인 캔버스가 있을 경우</para>
        /// <para>아무 동작하지 않습니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS"></typeparam>
        /// <param name="eName">Show 할 캔버스 이름</param>
        public static UICommandHandle<CLASS_DRIVEN_CANVAS> DoShow<CLASS_DRIVEN_CANVAS>(TENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : class, ICanvas
        {
            TDRIVEN_MANAGER pInstance = instance;

            UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle;
            if (pInstance._mapProcessCreating.TryGetValue(eName, out var sCreatingCanvas) == false)
            {
                pHandle = GetCommandHandle<CLASS_DRIVEN_CANVAS>(null);
                pInstance.CreateInstance(eName, false, pHandle);
            }
            else
                pHandle = (UICommandHandle<CLASS_DRIVEN_CANVAS>)sCreatingCanvas.Value;

            return pHandle;
        }

        /// <summary>
        /// 캔버스를 Show합니다. 리턴하는 <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="EUIObjectState.Showing"/>인 상태인 캔버스가 있을 경우</para>
        /// <para>캔버스의 인스턴스를 새로 만들어서 보여줍니다</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS"></typeparam>
        /// <param name="eName">Show 할 캔버스 이름</param>
        public static UICommandHandle<CLASS_DRIVEN_CANVAS> DoShow_Multiple<CLASS_DRIVEN_CANVAS>(TENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : class, ICanvas
        {
            TDRIVEN_MANAGER pInstance = instance;

            UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = GetCommandHandle<CLASS_DRIVEN_CANVAS>(null);
            pInstance.CreateInstance(eName, true, pHandle);

            return pHandle;
        }

        /// <summary>
        /// 캔버스를 Show합니다. <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="EUIObjectState.Showing"/>인 상태인 캔버스가 있을 경우</para>
        /// <para>아무 동작하지 않습니다.</para>
        /// </summary>
        /// <param name="eName">Show 할 캔버스 이름</param>
        public static UICommandHandle<ICanvas> DoShowOnly(TENUM_CANVAS_NAME eName)
        {
            TDRIVEN_MANAGER pInstance = instance;

            UICommandHandle<ICanvas> pHandle;
            if (pInstance._mapProcessCreating.TryGetValue(eName, out var sCreatingCanvas) == false)
            {
                pHandle = GetCommandHandle<ICanvas>(null);
                pInstance.CreateInstance(eName, false, pHandle);
            }
            else
                pHandle = (UICommandHandle<ICanvas>)sCreatingCanvas.Value;

            return pHandle;
        }

        /// <summary>
        /// 캔버스를 Hide합니다. 리턴하는 <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="EUIObjectState.Showing"/>인 상태인 캔버스가 있을때만 동작합니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS"></typeparam>
        /// <param name="eName">Hide 할 캔버스 이름</param>
        public static UICommandHandle<CLASS_DRIVEN_CANVAS> DoHide<CLASS_DRIVEN_CANVAS>(TENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : class, ICanvas
        {
            TDRIVEN_MANAGER pInstance = instance;

            UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = GetCommandHandle<CLASS_DRIVEN_CANVAS>(null);
            pInstance.StartCoroutine(pInstance.Process_HideCoroutine(eName, pHandle));

            return pHandle;
        }

        /// <summary>
        /// 캔버스를 Hide합니다. <see cref="UICommandHandle{CLASS_DRIVEN_CANVAS}"/>를 통해 주요 이벤트를 구독하여 사용합니다.
        /// <para>이미 <see cref="EUIObjectState.Showing"/>인 상태인 캔버스가 없을 경우</para>
        /// <para>아무 동작하지 않습니다.</para>
        /// </summary>
        /// <param name="eName">Hide 할 캔버스 이름</param>
        public static UICommandHandle<ICanvas> DoHideOnly(TENUM_CANVAS_NAME eName)
        {
            TDRIVEN_MANAGER pInstance = instance;

            UICommandHandle<ICanvas> pHandle = GetCommandHandle<ICanvas>(null);
            pInstance.StartCoroutine(pInstance.Process_HideCoroutine(eName, pHandle));

            return pHandle;
        }

        /// <summary>
        /// 모든 팝업을 닫습니다
        /// </summary>
        /// <param name="bPlayHideCoroutine"><see cref="IUIObject.OnHideCoroutine"/>실행 유무</param>
        public static void DoAllHide_ShowedCanvas(bool bPlayHideCoroutine = true)
        {
            List<ICanvas> listCanavs = GetAlreadyShow_CanvasList();
            if (bPlayHideCoroutine)
            {
                for (int i = 0; i < listCanavs.Count; i++)
                    listCanavs[i].DoHide_Coroutine();
            }
            else
            {
                for (int i = 0; i < listCanavs.Count; i++)
                    listCanavs[i].DoHide_NotPlayHideCoroutine();
            }

            instance?.OnAllHide_ShowedCanvas();
        }

        /// <summary>
        /// 이미 보여지고 있는 Canvas를 Return합니다. 없으면 null을 리턴합니다.
        /// <para>여러개일 경우 오래된 Canvas 1개만 리턴합니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS">형변환 할 Canvas 타입</typeparam>
        /// <param name="eName">얻고자 하는 캔버스 이름</param>
        public static CLASS_DRIVEN_CANVAS GetAlreadyShow_Canvas_OrNull<CLASS_DRIVEN_CANVAS>(TENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : MonoBehaviour, ICanvas
        {
            var pCanvas = GetAlreadyShow_Canvas_OrNull(eName);
            return pCanvas as CLASS_DRIVEN_CANVAS;
        }

        /// <summary>
        /// 이미 보여지고 있는 Canvas를 Return합니다. 없으면 null을 리턴합니다.
        /// <para>여러개일 경우 오래된 Canvas 1개만 리턴합니다.</para>
        /// </summary>
        /// <param name="eName">얻고자 하는 캔버스 이름</param>
        public static ICanvas GetAlreadyShow_Canvas_OrNull(TENUM_CANVAS_NAME eName)
        {
            TDRIVEN_MANAGER pInstance = instance;

            var listWrapper = pInstance.Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
            if (listWrapper.Count > 0)
                return listWrapper[0].pInstance;
            else
                return null;
        }

        /// <summary>
        /// 이미 보여지고 있는 Canvas List를 Return합니다. 없으면 count == 0인 list를 리턴합니다.
        /// <para><see cref="DoShow_Multiple{CLASS_DRIVEN_CANVAS}(TENUM_CANVAS_NAME)"/>을 통해 Show를 할 경우 유효합니다.</para>
        /// </summary>
        /// <typeparam name="CLASS_DRIVEN_CANVAS">형변환 할 Canvas 타입</typeparam>
        /// <param name="eName">얻고자 하는 캔버스 이름</param>
        public static List<CLASS_DRIVEN_CANVAS> GetAlreadyShow_CanvasList<CLASS_DRIVEN_CANVAS>(TENUM_CANVAS_NAME eName)
            where CLASS_DRIVEN_CANVAS : class, ICanvas
        {
            TDRIVEN_MANAGER pInstance = instance;

            var listWrapper = pInstance.Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
            return listWrapper.Where(p => p.pInstance is CLASS_DRIVEN_CANVAS && p.pInstance.IsNull() == false).Select(p => p.pInstance as CLASS_DRIVEN_CANVAS).ToList();
        }

        /// <summary>
        /// 이미 보여지고 있는 Canvas List를 Return합니다. 없으면 count == 0인 list를 리턴합니다.
        /// </summary>
        public static List<ICanvas> GetAlreadyShow_CanvasList()
        {
            if (_bApplication_IsQuit)
                return new List<ICanvas>();

            return new List<ICanvas>(instance._list_CanvasShowInstance.Where(p => p.IsNull() == false));
        }

        /// <summary>
        /// 마지막에 Show를 한 Canvas를 리턴합니다. 없으면 Null을 리턴합니다.
        /// </summary>
        public static ICanvas GetLastShowCanvas_OrNull()
        {
            TDRIVEN_MANAGER pInstance = instance;
            if (pInstance.IsNull())
                return null;

            return pInstance._list_CanvasShowInstance.LastOrDefault();
        }

        /// <summary>
        /// 캔버스의 Key를 얻습니다. 리턴은 Key 얻기 유무입니다.
        /// </summary>
        /// <param name="pObject">Key를 얻을 캔버스 인스턴스</param>
        /// <param name="eName">리턴받을 Key</param>
        public static bool GetEnumKey(ICanvas pObject, out TENUM_CANVAS_NAME eName)
        {
            eName = default(TENUM_CANVAS_NAME);

            if (pObject.IsNull())
                return false;

            TDRIVEN_MANAGER pInstance = instance;
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

            if ((eDebugLevel & EDebugLevelFlags.Manager) != 0)
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

            TDRIVEN_MANAGER pInstance = instance;
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

            pInstance._mapProcessCreating.Clear();
            pInstance._list_CanvasShowInstance.Clear();

            pInstance._mapManagerLogic.Clear();
            pInstance._mapManagerUndoLogic_Parser.Clear();

            pInstance._pWrapperPool.DoDestroyPool();
        }


        #region IUIWidgetContainerManager

        public override UICommandHandle<CLASS_UIOBJECT> IUIManager_Show<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
        {
            UICommandHandle<CLASS_UIOBJECT> pHandle = null;

            if (pUIObject.IsNull() == false)
            {
                GetWrapper_And_Handle(pUIObject, out var pWrapper, out pHandle);
                GetEnumKey_Custom(pUIObject as ICanvas, out var eName);
                StartCoroutine(Process_ShowCoroutine(eName, pWrapper, pHandle));
            }

            return pHandle;
        }

        public override UICommandHandle<CLASS_UIOBJECT> IUIManager_Hide<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject, bool bPlayHideCoroutine)
        {
            UICommandHandle<CLASS_UIOBJECT> pHandle = null;

            if (pUIObject.IsNull() == false)
            {
                GetWrapper_And_Handle(pUIObject, out var pWrapper, out pHandle);

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
            if (_mapWrapper_Key_Is_Instance.TryGetValue(pCanvas, out var pCanvasWrapper) == false)
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
        protected abstract void OnInit_ManagerLogic(CanvasManagerLogicFactory pLogicFactory);

        /// <summary>
        /// 컨테이너의 인스턴스를 만드는 방법을 구현합니다.
        /// <para> 컨테이너의 인스턴스가 없는 경우에만 호출됩니다. </para> 
        /// </summary>
        /// <param name="eName">인스턴스를 만들 팝업 이름 Enum</param>
        /// <param name="bIsMultiple">이미 같은 팝업이 떴을 때 또 띄울지</param>
        /// <param name="OnFinish">인스턴스를 만들었을 때</param>
        protected abstract IEnumerator OnCreate_Instance(TENUM_CANVAS_NAME eName, bool bIsMultiple, System.Action<ICanvas> OnFinish);

        /// <summary>
        /// 인스턴스에 알맞는 캔버스를 얻어오는 방법을 구현합니다.
        /// </summary>
        /// <param name="eName">캔버스가 필요한 이름 Enum</param>
        /// <param name="pCanvas">캔버스가 필요한 인스턴스</param>
        public abstract Canvas GetParentCanvas(TENUM_CANVAS_NAME eName, ICanvas pCanvas);

        /// <summary>
        /// 오브젝트가 켜질 때 호출됩니다.
        /// <para> 매니져 Show(OnBeforeShow) -> <see cref="IUIObject.OnShowCoroutine"/> -> 매니져 Show(OnShow) -> 이 함수 순으로 호출됩니다. </para> 
        /// </summary>
        /// <param name="eName">켜지는 오브젝트의 이름 Enum</param>
        /// <param name="pInstance">켜지는 오브젝트의 인스턴스</param>
        protected virtual void OnShow_BeforeAnimation(TENUM_CANVAS_NAME eName, ICanvas pInstance) { }


        /// <summary>
        /// 오브젝트가 켜질 때 호출됩니다.
        /// <para> 매니져 Show(OnBeforeShow) -> <see cref="IUIObject.OnShowCoroutine"/> -> 매니져 Show(OnShow) -> 이 함수 순으로 호출됩니다. </para> 
        /// </summary>
        /// <param name="eName">켜지는 오브젝트의 이름 Enum</param>
        /// <param name="pInstance">켜지는 오브젝트의 인스턴스</param>
        protected virtual void OnShow_AfterAnimation(TENUM_CANVAS_NAME eName, ICanvas pInstance) { }

        /// <summary>
        /// 오브젝트가 꺼질 때 호출됩니다.
        /// <para> <see cref="IUIObject.OnHideCoroutine"/> -> 매니져 Hide(OnHide) -> 이 함수 순으로 호출됩니다. </para> 
        /// </summary>
        /// <param name="eName">꺼지는 오브젝트의 이름 Enum</param>
        /// <param name="pInstance">꺼지는 오브젝트의 인스턴스</param>
        /// <param name="iInstanceCount">현재 켜져있는 인스턴스의 개수</param>
        protected virtual void OnHide(TENUM_CANVAS_NAME eName, ICanvas pInstance, int iInstanceCount) { }

        protected virtual void OnAllHide_ShowedCanvas() { }

        /// <summary>
        /// Default <see cref="GetEnumKey(ICanvas, out TENUM_CANVAS_NAME)"/>를 Fail하면 호출되는 함수입니다.
        /// </summary>
        /// <param name="pInstance"></param>
        /// <param name="eName"></param>
        /// <returns></returns>
        protected virtual bool GetEnumKey_Custom(ICanvas pInstance, out TENUM_CANVAS_NAME eName) { eName = default(TENUM_CANVAS_NAME); return false; }

        protected virtual ICanvas Get_CanvasInstance(TENUM_CANVAS_NAME eName)
        {
            if (Get_UnUsedWrapper(eName, out var pContainer))
                return pContainer.pInstance;
            else
                return default(ICanvas);
        }

        // ========================================================================== //

        #region Private

        protected IEnumerator Process_ShowCoroutine<CLASS_DRIVEN_CANVAS>(TENUM_CANVAS_NAME eName, CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            if ((eDebugLevel & EDebugLevelFlags.Manager_Core) != 0)
                Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager_Core)}/{nameof(Process_ShowCoroutine)} - Name : {eName} // {pWrapper.GetObjectName()}");

            if (pWrapper == null || pWrapper.pInstance.IsNull())
            {
                Debug.LogWarning(name + " " + eName + " CoProcess_Showing - pWrapper == null || pWrapper.pInstance.Equals(null)", this);
                yield break;
            }

            bool bIsShow = false;
            yield return sUICommandHandle.DoExecute_Check_IsShowCoroutine(
                (bIsShowParameter) => bIsShow = bIsShowParameter);

            if (bIsShow == false)
            {
                sUICommandHandle.Event_OnHide(false);
                pWrapper.DoSet_State_Is_Disable_Force();

                yield break;
            }

            _list_CanvasShowInstance.Add(pWrapper.pInstance);
            sUICommandHandle.DoResetFlag();

            OnShow_BeforeAnimation(pWrapper.eName, pWrapper.pInstance);

            // BeforeShow만 Handle 이벤트를 임시로 CanvasManager Logic 전에 호출
            sUICommandHandle.Event_OnBeforeShow();
            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Process_Before_ShowCoroutine, pWrapper, sUICommandHandle);

            Canvas pCanvas = GetParentCanvas(pWrapper.eName, pWrapper.pInstance);
            yield return pWrapper.DoExecute_ShowCoroutine(pCanvas, sUICommandHandle);

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Process_After_ShowCoroutine, pWrapper, sUICommandHandle);
            sUICommandHandle.Event_OnShow_AfterAnimation();

            OnShow_AfterAnimation(pWrapper.eName, pWrapper.pInstance);

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Showing, pWrapper, sUICommandHandle);

            // _list_CanvasShow.Add(pWrapper);
        }

        protected virtual IEnumerator Process_HideCoroutine<CLASS_DRIVEN_CANVAS>(CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            if ((eDebugLevel & EDebugLevelFlags.Manager_Core) != 0)
                Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager_Core)}/{nameof(Process_HideCoroutine)} - Wrapper :  {pWrapper.GetObjectName()}");

            if (pWrapper == null)
                yield break;

            _list_CanvasShowInstance.Remove(pWrapper.pInstance);

            if (pWrapper.eState == EUIObjectState.Process_Before_HideCoroutine)
                yield break;

            sUICommandHandle.DoResetFlag();

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Process_Before_HideCoroutine, pWrapper, sUICommandHandle);

            yield return pWrapper.DoExecute_HideCoroutine();

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Process_After_HideCoroutine, pWrapper, sUICommandHandle);

            sUICommandHandle.Event_OnHide();

            int iInstanceCount = Get_MatchWrapperList(pWrapper.eName, x => x.Check_IsEnable()).Count;
            OnHide(pWrapper.eName, pWrapper.pInstance, iInstanceCount);

            yield return Execute_ManagerLogicCoroutine(EUIObjectState.Disable, pWrapper, sUICommandHandle);

            DisableWrapper(pWrapper);
        }

        protected virtual void Process_Hide<CLASS_DRIVEN_CANVAS>(CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            if ((eDebugLevel & EDebugLevelFlags.Manager_Core) != 0)
                Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager_Core)}/{nameof(Process_Hide)} - Wrapper :  {pWrapper.GetObjectName()}");

            if (pWrapper == null)
                return;

            _list_CanvasShowInstance.Remove(pWrapper.pInstance);

            if (pWrapper.eState == EUIObjectState.Process_Before_HideCoroutine)
                return;

            Execute_ManagerUndoLogic(EUIObjectState.Process_Before_HideCoroutine, pWrapper, sUICommandHandle);

            // pWrapper.DoExecute_HideCoroutine();

            Execute_ManagerUndoLogic(EUIObjectState.Process_After_HideCoroutine, pWrapper, sUICommandHandle);

            sUICommandHandle.Event_OnHide();

            int iInstanceCount = Get_MatchWrapperList(pWrapper.eName, x => x.Check_IsEnable()).Count;
            OnHide(pWrapper.eName, pWrapper.pInstance, iInstanceCount);

            Execute_ManagerUndoLogic(EUIObjectState.Disable, pWrapper, sUICommandHandle);

            DisableWrapper(pWrapper);
        }


        IEnumerator Process_CreateInstance<T>(TENUM_CANVAS_NAME eName, CanvasWrapper pWrapper, bool bCreateInstance, bool bIsMultiple, UICommandHandle<T> pUICommandHandle)
            where T : class, ICanvas
        {
            if (bCreateInstance)
            {
                ICanvas pInstance = null;
                yield return OnCreate_Instance(eName, bIsMultiple,
                    (pCanvas) =>
                    {
                        if (pCanvas.IsNull())
                            Debug.LogError($"Error {eName} - OnCreate_Instance Fail");

                        pInstance = pCanvas;
                    });

                if (pInstance.IsNull())
                {
                    RemoveWrapper(pWrapper);
                    Debug.LogError(name + " CoProcess_Showing - eName : " + eName + " pInstance == null", this);
                    yield break;
                }

                Wrapper_SetCanvasInstance(pWrapper, pInstance);
                _mapProcessCreating.Remove(eName);
                if ((eDebugLevel & EDebugLevelFlags.Manager_Core) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager_Core)}{name} Removed Creating " + eName);
            }

            if (pWrapper == null || pWrapper.pInstance.IsNull())
                yield break;

            if (pWrapper.Check_IsEnable())
            {
                pUICommandHandle.Set_UIObject(pWrapper.pInstance as T);
                yield break;
            }

            EnableWrapper(pWrapper, pUICommandHandle);
            yield return Process_ShowCoroutine(eName, pWrapper, pUICommandHandle);
        }

        IEnumerator Process_HideCoroutine<T>(TENUM_CANVAS_NAME eName, UICommandHandle<T> pUICommandHandle)
            where T : class, ICanvas
        {
            var listEnableWrapper = Get_MatchWrapperList(eName, (x) => x.Check_IsEnable());
            if (listEnableWrapper.Count == 0)
            {
                // Hiding 요청이 왔는데 Canvas Instance가 없는 경우
                Debug.LogWarning(name + " CoProcess_Hiding - eName : " + eName + " listEnableWrapper.Count == 0", this);
                pUICommandHandle.Event_OnHide();
                yield break;
            }

            CanvasWrapper pWrapper = listEnableWrapper[0];
            EnableWrapper(pWrapper, pUICommandHandle);
            yield return Process_HideCoroutine(pWrapper, pUICommandHandle);
        }


        private void EnableWrapper<T>(CanvasWrapper pWrapper, UICommandHandle<T> pUICommandHandle)
            where T : class, ICanvas
        {
            pWrapper.DoSet_State_IsEnable();
            pUICommandHandle.Set_UIObject(pWrapper.pInstance as T);
        }

        private void DisableWrapper(CanvasWrapper pWrapper)
        {
            pWrapper.DoSet_State_Is_Disable_Force();
            if (Check_Wrapper_IsNull(pWrapper))
                RemoveWrapper(pWrapper);
        }

        private static UICommandHandle<CLASS_DRIVEN_CANVAS> GetCommandHandle<CLASS_DRIVEN_CANVAS>(CLASS_DRIVEN_CANVAS pInstance_OrNull) 
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            UICommandHandle<CLASS_DRIVEN_CANVAS> pHandle = UICommandHandle<CLASS_DRIVEN_CANVAS>.GetInstance(pInstance_OrNull);
            g_setCommandHandle.Add(pHandle);

            return pHandle;
        }

        #region Manage_Wrapper

        // ReSharper disable once UnusedMethodReturnValue.Local
        private CanvasWrapper CreateInstance<T>(TENUM_CANVAS_NAME eName, bool bIsMultiple, UICommandHandle<T> pHandle)
            where T : class, ICanvas
        {
            bool bIsCreateInstance = Check_IsCreateInstance(eName, bIsMultiple, pHandle, out var pWrapper);
            StartCoroutine(Process_CreateInstance(eName, pWrapper, bIsCreateInstance, bIsMultiple, pHandle));

            return pWrapper;
        }

        private void GetWrapper_And_Handle<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject, out CanvasWrapper pWrapper, out UICommandHandle<CLASS_UIOBJECT> pHandle)
            where CLASS_UIOBJECT : IUIObject
        {
            ICanvas pCanvas = pUIObject as ICanvas;
            if (_mapWrapper_Key_Is_Instance.TryGetValue(pCanvas, out pWrapper) == false)
            {
                GetEnumKey(pCanvas, out var eKey);
                pWrapper = CreateWrapper_OrNull(eKey);
                Wrapper_SetCanvasInstance(pWrapper, pCanvas);
            }

            pHandle = GetCommandHandle(pUIObject);
            pHandle.Set_UIObject(pUIObject);
        }

        protected bool Get_UnUsedWrapper(TENUM_CANVAS_NAME eName, out CanvasWrapper pWrapper)
        {
            pWrapper = null;
            bool bGet_IsSuccess = false;
            var listDisableWrapper = Get_MatchWrapperList(eName, (x) => x.eState != EUIObjectState.Creating && x.Check_IsEnable() == false);
            if (listDisableWrapper.Count > 0)
            {
                pWrapper = listDisableWrapper[listDisableWrapper.Count - 1];
                bGet_IsSuccess = true;
            }
            else
            {
                pWrapper = CreateWrapper_OrNull(eName);
            }
                // Debug.LogWarning(name + " Get_UnUsedWrapper - eName : " + eName + " listDisableWrapper.Count == 0", this);

            return bGet_IsSuccess;
        }

        List<CanvasWrapper> _listTempWrapper = new List<CanvasWrapper>();
        private List<CanvasWrapper> Get_MatchWrapperList(TENUM_CANVAS_NAME eName, System.Func<CanvasWrapper, bool> OnCheck_IsMatch)
        {
            _listTempWrapper.Clear();
            if (_mapWrapper.ContainsKey(eName) == false)
                return _listTempWrapper;

            List<CanvasWrapper> listWrapper = _mapWrapper[eName];
            for (int i = 0; i < listWrapper.Count; i++)
            {
                CanvasWrapper pWrapper = listWrapper[i];
                if (pWrapper.pInstance.IsNull() && pWrapper.eState != EUIObjectState.Creating)
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

        bool Check_IsCreateInstance<T>(TENUM_CANVAS_NAME eName, bool bIsMultiple, UICommandHandle<T> pUICommandHandle, out CanvasWrapper pWrapper)
            where T : class, ICanvas
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
                // 싱글 팝업일 때 먼저 현재 활성화된 인스턴스가 있는지부터 체크
                // 있으면 아무것도 하지 않음
                pWrapper = Get_MatchWrapperList(eName, x => x.Check_IsEnable()).FirstOrDefault();
                if(pWrapper != null)
                    return false;

                // 현재 쓸 수 있는 팝업이 없으면 인스턴스 생성
                bCreateInstance = Get_MatchWrapperList(eName, x => x.Check_IsDisable()).Count == 0;
                if (bCreateInstance)
                {
                    bCreateInstance = _mapProcessCreating.TryGetValue(eName, out var sKeyPair) == false;
                    if (bCreateInstance)
                    {
                        pWrapper = CreateWrapper_OrNull(eName);
                        sKeyPair = new KeyValuePair<CanvasWrapper, object>(pWrapper, pUICommandHandle);
                        _mapProcessCreating.Add(eName, sKeyPair);
                    }
                    else
                        pWrapper = sKeyPair.Key;

                    if ((eDebugLevel & EDebugLevelFlags.Manager_Core) != 0)
                        Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager_Core)}{name} Check_IsCreateInstance // bCreateInstance : {bCreateInstance} " + eName + " Disable Count : " + Get_MatchWrapperList(eName, (x) => x.Check_IsDisable()).Count);
                }
                else
                {
                    // 싱글 팝업일 때 인스턴스를 만들 필요가 없으면 지금 쓸수 있는 팝업 사용
                    Get_UnUsedWrapper(eName, out pWrapper);
                }
            }

            return bCreateInstance;
        }

        protected CanvasWrapper CreateWrapper_OrNull(TENUM_CANVAS_NAME eName)
        {
            CanvasWrapper pWrapper = null;
            try
            {
                pWrapper = _pWrapperPool.DoPop();
                pWrapper.DoInit(eName, StartCoroutineWrapper, StopCoroutine);

                if (_mapWrapper.ContainsKey(eName) == false)
                    _mapWrapper.Add(eName, new List<CanvasWrapper>());

                _mapWrapper[eName].Add(pWrapper);
            }
            catch (System.Exception e)
            {
                Debug.LogError("CreateWrapper_OrNull eName : " + eName + "\nException : " + e);
                _pWrapperPool.DoPush(pWrapper);
            }

            return pWrapper;
        }

        Coroutine StartCoroutineWrapper(CanvasWrapper pWrapper, IEnumerator pEnumerator)
        {
            if((eDebugLevel & EDebugLevelFlags.Detail) != 0)
                Debug.Log($"{pWrapper.GetObjectName()} - StartCoroutine {pEnumerator}");

            return StartCoroutine(pEnumerator);
        }

        protected void Wrapper_SetCanvasInstance<T>(CanvasWrapper pWrapper, T pInstance)
            where T : class, ICanvas
        {
            pWrapper.DoSet_CanvasInstance(pInstance);

            if (_mapWrapper_Key_Is_Instance.ContainsKey(pInstance) == false)
                _mapWrapper_Key_Is_Instance.Add(pInstance, pWrapper);

            pWrapper.pInstance.pUIManager = this;
        }

        protected void RemoveWrapper(CanvasWrapper pWrapper)
        {
            if ((eDebugLevel & EDebugLevelFlags.Manager) != 0)
                Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager)} Remove Wrapper - {pWrapper.eName} - Instance == Null : {pWrapper.pInstance == null}");

            if (_mapWrapper.ContainsKey(pWrapper.eName))
                _mapWrapper[pWrapper.eName].Remove(pWrapper);

            if(pWrapper.pInstance != null)
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

        private IEnumerator Execute_ManagerLogicCoroutine<CLASS_DRIVEN_CANVAS>(EUIObjectState eUIState, CanvasWrapper pWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            if ((eDebugLevel & EDebugLevelFlags.Manager_Core) != 0)
                Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager_Core)} {nameof(Execute_ManagerLogicCoroutine)} - {pWrapper.eName} // Set UIState : {eUIState}");


            sUICommandHandle.Set_UIState(eUIState);
            pWrapper.DoSet_State(eUIState);

            if (eUIState != EUIObjectState.Process_Before_ShowCoroutine)
                yield return pWrapper.DoExecute_Manager_UndoLogic_Coroutine(this, eUIState);

            if (_mapManagerLogic.TryGetValue(eUIState, out var listLogic) == false)
                yield break;

            yield return pWrapper.DoExecute_Manager_CoroutineLogic(this, eUIState, listLogic, GetUndoLogic, eDebugLevel);
        }

        private void Execute_ManagerUndoLogic<CLASS_DRIVEN_CANVAS>(EUIObjectState eUIEvent, CanvasWrapper pContainerWrapper, UICommandHandle<CLASS_DRIVEN_CANVAS> sUICommandHandle)
            where CLASS_DRIVEN_CANVAS : IUIObject
        {
            sUICommandHandle.Set_UIState(eUIEvent);
            pContainerWrapper.DoSet_State(eUIEvent);

            if (eUIEvent != EUIObjectState.Process_Before_ShowCoroutine)
                pContainerWrapper.DoExecute_Manager_UndoLogic(this, eUIEvent);
        }

        CanvasManager_LogicUndo_Wrapper GetUndoLogic(ICanvasManager_Logic pLogic)
        {
            _mapManagerUndoLogic_Parser.TryGetValue(pLogic, out var pWrapper);

            return pWrapper;
        }

        #endregion ManagerLogic

        #endregion Private

    }
}
