#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-12-19 오후 2:29:34
 *	개요 : 
   ============================================ */
#endregion Header

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using UIWidgetContainerManager_Logic;
using static StrixLibrary_Test.CanvasManager_ForLogicTest;

namespace StrixLibrary_Test
{
    public class CanvasManager_ForLogicTest : CanvasManager<CanvasManager_ForLogicTest, CanvasManager_ForLogicTest.ECanvasName>
    {
        #region Canvas
        public class Canvas_ForLogicTest : MonoBehaviour, ICanvas
        {
            public IUIManager pUIManager { get; set; }
            public string strText { get; private set; }
            public int iValue;

            public void DoSetText(string strText)
            {
                this.strText = strText;
            }

            public IEnumerator OnShowCoroutine()
            {
                strText = nameof(OnShowCoroutine);
                yield break;
            }
            public IEnumerator OnHideCoroutine()
            {
                strText = nameof(OnHideCoroutine);
                yield break;
            }
        }

        #endregion Canvas

        public enum ECanvasName
        {
            Single,
        }

        static Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>> _mapManagerLogicTest = new Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>>();

        static public void DoInit(Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>> mapManagerLogic)
        {
            _mapManagerLogicTest = mapManagerLogic;
        }

        protected override IEnumerator OnCreate_Instance(ECanvasName eName, System.Action<ICanvas> OnFinishCreate)
        {
            switch (eName)
            {
                case ECanvasName.Single: OnFinishCreate(new GameObject(nameof(Canvas_ForLogicTest)).AddComponent<Canvas_ForLogicTest>()); break;

                default: Debug.LogError("Error"); break;
            }

            yield break;
        }

        protected override void OnInit_ManagerLogic(Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>> mapManagerLogic)
        {
            if(mapManagerLogic.Count == 0)
            {
                Debug.LogError("Error");
                return;
            }

            Debug.Log(nameof(OnInit_ManagerLogic));
            foreach(var pLogic in _mapManagerLogicTest)
                mapManagerLogic[pLogic.Key].AddRange(pLogic.Value);
        }

        public override Canvas GetParentCavnas(ECanvasName eName, ICanvas pCanvas)
        {
            return null;
        }
    }

    #region CanvasManagerLogic
    public class Logic_SetString_ForTest : ICanvasManager_Logic
    {
        public string _strTestText { get; private set; }

        public Logic_SetString_ForTest(string strTestText)
        {
            _strTestText = strTestText;
        }

        public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
        {
            Canvas_ForLogicTest pTest = pCanvas as Canvas_ForLogicTest;
            pTest.DoSetText(_strTestText);

            yield break;
        }
    }

    public class Logic_Calculate_Add_And_Undo : ICanvasManager_Logic_IsPossible_Undo
    {
        int _iAddValue;
        int _iOriginValue;

        public Logic_Calculate_Add_And_Undo(int iAddValue)
        {
            _iAddValue = iAddValue;
        }

        public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
        {
            Canvas_ForLogicTest pTest = pCanvas as Canvas_ForLogicTest;
            _iOriginValue = pTest.iValue;
            pTest.iValue += _iAddValue;

            yield break;
        }

        public IEnumerator Execute_UndoLogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
        {
            Canvas_ForLogicTest pTest = pCanvas as Canvas_ForLogicTest;
            pTest.iValue = _iOriginValue;

            yield break;
        }
    }

    public class Logic_CreateObject_And_SetParents_And_Undo_Is_Destroy : ICanvasManager_Logic_IsPossible_Undo
    {
        Transform _pTransformParents;

        public Logic_CreateObject_And_SetParents_And_Undo_Is_Destroy(Transform pTransformParents)
        {
            _pTransformParents = pTransformParents;
        }

        public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
        {
            new GameObject(nameof(Logic_CreateObject_And_SetParents_And_Undo_Is_Destroy)).transform.SetParent(_pTransformParents);

            yield break;
        }

        public IEnumerator Execute_UndoLogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
        {
            if (_pTransformParents.childCount == 0)
                yield break;

            GameObject.DestroyImmediate(_pTransformParents.GetChild(_pTransformParents.childCount - 1).gameObject);

            yield break;
        }
    }

    public class Logic_WaitForSecond : ICanvasManager_Logic_IsPossible_Undo
    {
        public float fWaitSecond { get; private set; }
        public float fWaitSecond_On_Undo { get; private set; }


        public Logic_WaitForSecond(float fWaitSecond, float fWaitSecond_On_Undo)
        {
            this.fWaitSecond = fWaitSecond; this.fWaitSecond_On_Undo = fWaitSecond_On_Undo;
        }

        public IEnumerator Execute_UndoLogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
        {
            yield return new WaitForSeconds(fWaitSecond);
        }

        public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
        {
            yield return new WaitForSeconds(fWaitSecond_On_Undo);
        }
    }


    #endregion CanvasManagerLogic

    [Category("StrixLibrary")]
    public class CanvasManagerLogics_Tester
    {
        Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>> mapManagerLogic;

        [UnityTest]
        public IEnumerator Logic_Is_Working()
        {
            // 이걸 안하면 테스트를 동시에 여러번 했을 때 
            // 인스턴스가 이미 있기 때문에 OnInit_ManagerLogic을 호출하지 않습니다.
            CanvasManager_ForLogicTest.DoDestroy_Manager(true);

            // 로직은 string을 Canvas에 Set하는 기능입니다.
            mapManagerLogic = CreateDictionary();
            mapManagerLogic[EUIShowHideEvent.Before_Show_UIObject].Add(new Logic_SetString_ForTest(nameof(EUIShowHideEvent.Before_Show_UIObject)));
            mapManagerLogic[EUIShowHideEvent.After_Show_UIObject].Add(new Logic_SetString_ForTest(nameof(EUIShowHideEvent.After_Show_UIObject)));
            mapManagerLogic[EUIShowHideEvent.Before_Hide_UIObject].Add(new Logic_SetString_ForTest(nameof(EUIShowHideEvent.Before_Hide_UIObject)));
            mapManagerLogic[EUIShowHideEvent.After_Hide_UIObject].Add(new Logic_SetString_ForTest(nameof(EUIShowHideEvent.After_Hide_UIObject)));
            DoInit(mapManagerLogic);

            var pHandle_Show = CanvasManager_ForLogicTest.DoShow<Canvas_ForLogicTest>(ECanvasName.Single);
            Assert.IsNull(pHandle_Show.pUIObject);

            yield return pHandle_Show.Yield_WaitForSetUIObject();

            Assert.IsNotNull(pHandle_Show.pUIObject);
            var pUICanvas = pHandle_Show.pUIObject;

            yield return pHandle_Show.Yield_WaitForAnimation();
            Assert.AreEqual(pUICanvas.strText, nameof(EUIShowHideEvent.After_Show_UIObject));

            var pHandle_Hide = CanvasManager_ForLogicTest.DoHide<Canvas_ForLogicTest>(ECanvasName.Single);
            yield return null;
            Assert.AreEqual(pUICanvas.strText, nameof(EUIShowHideEvent.Before_Hide_UIObject));

            yield return pHandle_Hide.Yield_WaitForAnimation();
            Assert.AreEqual(pUICanvas.strText, nameof(EUIShowHideEvent.After_Hide_UIObject));


            // UI Object에게 직접 명령 후 Handle 및 Logic 동작 이벤트 테스트
            pHandle_Show = pUICanvas.DoShow();
            yield return pHandle_Show.Yield_WaitForAnimation();
            Assert.AreEqual(pUICanvas.strText, nameof(EUIShowHideEvent.After_Show_UIObject));

            pHandle_Hide = pUICanvas.DoHide();
            yield return null;
            Assert.AreEqual(pUICanvas.strText, nameof(EUIShowHideEvent.Before_Hide_UIObject));

            yield return pHandle_Hide.Yield_WaitForAnimation();
            Assert.AreEqual(pUICanvas.strText, nameof(EUIShowHideEvent.After_Hide_UIObject));

            yield break;
        }

        [UnityTest]
        public IEnumerator UndoLogic_Is_Working()
        {
            // 이걸 안하면 테스트를 동시에 여러번 했을 때 
            // 인스턴스가 이미 있기 때문에 OnInit_ManagerLogic을 호출하지 않습니다.
            CanvasManager_ForLogicTest.DoDestroy_Manager(true);

            int iRandomRange = 100;
            int iRandomValue = Random.Range(-iRandomRange, iRandomRange);

            // 로직은 실행되면 Canvas의 원래 값에 어떠한 값을 더하고,
            // 로직을 취소할 경우 원래값으로 복구합니다.
            mapManagerLogic = CreateDictionary();
            mapManagerLogic[EUIShowHideEvent.Before_Show_UIObject].Add(
                new CanvasManager_LogicUndo_Wrapper(
                    pLogic: new Logic_Calculate_Add_And_Undo(iRandomValue),
                    eWhenUndo: EUIShowHideEvent.Before_Hide_UIObject));
            DoInit(mapManagerLogic);

            int iOriginValue = Random.Range(int.MinValue + iRandomRange, int.MaxValue - iRandomRange);

            var pHandle_Show = CanvasManager_ForLogicTest.DoShow<Canvas_ForLogicTest>(ECanvasName.Single);
            yield return pHandle_Show.Yield_WaitForSetUIObject();
            var pUICanvas = pHandle_Show.pUIObject;
            pUICanvas.iValue = iOriginValue;

            yield return pHandle_Show.Yield_WaitForAnimation();
            Assert.AreEqual(pUICanvas.iValue, iOriginValue + iRandomValue);

            CanvasManager_ForLogicTest.DoHide<Canvas_ForLogicTest>(ECanvasName.Single);
            yield return null;
            Assert.AreEqual(pUICanvas.iValue, iOriginValue);

            yield break;
        }

        [UnityTest]
        public IEnumerator UndoLogic_WaitForSecond()
        {
            // 이걸 안하면 테스트를 동시에 여러번 했을 때 
            // 인스턴스가 이미 있기 때문에 OnInit_ManagerLogic을 호출하지 않습니다.
            CanvasManager_ForLogicTest.DoDestroy_Manager(true);

            float fWaitSecond = Random.Range(0.1f, 0.2f);
            float fWaitSecond_Undo = Random.Range(0.1f, 0.2f);

            // 로직은 실행되면 Canvas의 원래 값에 어떠한 값을 더하고,
            // 로직을 취소할 경우 원래값으로 복구합니다.
            mapManagerLogic = CreateDictionary();
            mapManagerLogic[EUIShowHideEvent.After_Show_UIObject].Add(
                new CanvasManager_LogicUndo_Wrapper(
                    pLogic: new Logic_WaitForSecond(fWaitSecond, fWaitSecond_Undo),
                    eWhenUndo: EUIShowHideEvent.After_Hide_UIObject));
            DoInit(mapManagerLogic);

            System.Diagnostics.Stopwatch pWatch = new System.Diagnostics.Stopwatch();

            pWatch.Start();
            var pHandle_Show = CanvasManager_ForLogicTest.DoShow<Canvas_ForLogicTest>(ECanvasName.Single);
            yield return pHandle_Show.Yield_WaitForAnimation();
            pWatch.Stop();

            // 기다렸는지 체크하는 로직은
            // 실제 기다리는 시간보다 더 오래 기다리면 True로 체크
            Assert.Greater(pWatch.Elapsed.TotalSeconds, fWaitSecond);

            pWatch.Reset();
            pWatch.Start();
            var pHandle_Hide = CanvasManager_ForLogicTest.DoHide<Canvas_ForLogicTest>(ECanvasName.Single);
            yield return pHandle_Hide.Yield_WaitForAnimation();
            pWatch.Stop();

            Assert.Greater(pWatch.Elapsed.TotalSeconds, fWaitSecond_Undo);
        }

        [UnityTest]
        public IEnumerator UndoLogic_Is_Working_OnDestroyManager()
        {
            // 이걸 안하면 테스트를 동시에 여러번 했을 때 
            // 인스턴스가 이미 있기 때문에 OnInit_ManagerLogic을 호출하지 않습니다.
            CanvasManager_ForLogicTest.DoDestroy_Manager(true);

            Transform pTransformParents = new GameObject(nameof(UndoLogic_Is_Working_OnDestroyManager)).transform;

            // 로직은 실행되면 특정 트렌스폼에 차일드 게임오브젝트를 붙이고
            // 로직을 취소할 경우 마지막 차일드 오브젝트를 Destroy합니다.
            mapManagerLogic = CreateDictionary();
            mapManagerLogic[EUIShowHideEvent.Before_Show_UIObject].Add(
                new CanvasManager_LogicUndo_Wrapper(
                    pLogic: new Logic_CreateObject_And_SetParents_And_Undo_Is_Destroy(pTransformParents),
                    eWhenUndo: EUIShowHideEvent.Before_Hide_UIObject));
            DoInit(mapManagerLogic);

            int iRandomShow = Random.Range(3, 7);
            List<UICommandHandle<Canvas_ForLogicTest>> listHandle = new List<UICommandHandle<Canvas_ForLogicTest>>();
            for (int i = 0; i < iRandomShow; i++)
                listHandle.Add(CanvasManager_ForLogicTest.DoShow_Multiple<Canvas_ForLogicTest>(CanvasManager_ForLogicTest.ECanvasName.Single));

            yield return listHandle[listHandle.Count - 1].Yield_WaitForAnimation();

            Assert.AreEqual(pTransformParents.childCount, iRandomShow);
            CanvasManager_ForLogicTest.DoDestroy_Manager(true);

            yield return null;
            Assert.AreEqual(pTransformParents.childCount, 0);

            yield break;
        }

        // =======================================================================================

        static Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>> CreateDictionary()
        {
            return new Dictionary<EUIShowHideEvent, List<ICanvasManager_Logic>>()
            {
                { EUIShowHideEvent.Before_Show_UIObject, new List<ICanvasManager_Logic>() },
                { EUIShowHideEvent.After_Show_UIObject, new List<ICanvasManager_Logic>() },
                { EUIShowHideEvent.Before_Hide_UIObject, new List<ICanvasManager_Logic>() },
                { EUIShowHideEvent.After_Hide_UIObject, new List<ICanvasManager_Logic>() },
            };
        }
    }
}