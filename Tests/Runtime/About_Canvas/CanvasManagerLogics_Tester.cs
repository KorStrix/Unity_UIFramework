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

using UIFramework;
using UIFramework.UIWidgetContainerManager_Logic;
using static UIFramework_Test.About_Canvas.CanvasManager_ForLogicTest;

namespace UIFramework_Test
{
    namespace About_Canvas
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

            static CanvasManagerLogicFactory g_pLogicFactory = new CanvasManagerLogicFactory();

            public static void DoInit(CanvasManagerLogicFactory pLogicFactory)
            {
                g_pLogicFactory = pLogicFactory;
            }

            protected override IEnumerator OnCreate_Instance(ECanvasName eName, bool bIsMultiple, System.Action<ICanvas> OnFinishCreate)
            {
                switch (eName)
                {
                    case ECanvasName.Single: OnFinishCreate(new GameObject(nameof(Canvas_ForLogicTest)).AddComponent<Canvas_ForLogicTest>()); break;

                    default: Debug.LogError("Error"); break;
                }

                yield break;
            }

            protected override void OnInit_ManagerLogic(CanvasManagerLogicFactory pLogicFactory)
            {
                if (g_pLogicFactory.mapLogicContainer.Count == 0)
                {
                    Debug.LogError("Error");
                    return;
                }

                Debug.Log(nameof(OnInit_ManagerLogic));


                foreach (var pLogic in g_pLogicFactory.mapLogicContainer)
                    pLogicFactory.DoAddLogic(pLogic.Key, pLogic.Value.ToArray());
            }

            public override Canvas GetParentCanvas(ECanvasName eName, ICanvas pCanvas)
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

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                Canvas_ForLogicTest pTest = pCanvas as Canvas_ForLogicTest;
                pTest.iValue = _iOriginValue;

                yield break;
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                Canvas_ForLogicTest pTest = pCanvas as Canvas_ForLogicTest;
                pTest.iValue = _iOriginValue;
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

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (_pTransformParents.childCount == 0)
                    yield break;

                GameObject.DestroyImmediate(_pTransformParents.GetChild(_pTransformParents.childCount - 1).gameObject);

                yield break;
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (_pTransformParents.childCount == 0)
                    return;

                GameObject.DestroyImmediate(_pTransformParents.GetChild(_pTransformParents.childCount - 1).gameObject);
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

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                yield return new WaitForSeconds(fWaitSecond);
            }

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                yield return new WaitForSeconds(fWaitSecond_On_Undo);
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
            }
        }


        #endregion CanvasManagerLogic

        [Category("UIFramework")]
        public class CanvasManagerLogics_Tester
        {
            [UnityTest]
            public IEnumerator Logic_Is_Working()
            {
                // 이걸 안하면 테스트를 동시에 여러번 했을 때 
                // 인스턴스가 이미 있기 때문에 OnInit_ManagerLogic을 호출하지 않습니다.
                CanvasManager_ForLogicTest.DoDestroy_Manager(true);

                // 로직은 string을 Canvas에 Set하는 기능입니다.
                CanvasManagerLogicFactory pLogicFactory = new CanvasManagerLogicFactory();
                pLogicFactory.DoAddLogic(EUIObjectState.Process_Before_ShowCoroutine, new Logic_SetString_ForTest(nameof(EUIObjectState.Process_Before_ShowCoroutine)));
                pLogicFactory.DoAddLogic(EUIObjectState.Process_After_ShowCoroutine, new Logic_SetString_ForTest(nameof(EUIObjectState.Process_After_ShowCoroutine)));
                pLogicFactory.DoAddLogic(EUIObjectState.Process_Before_HideCoroutine, new Logic_SetString_ForTest(nameof(EUIObjectState.Process_Before_HideCoroutine)));
                pLogicFactory.DoAddLogic(EUIObjectState.Process_After_HideCoroutine, new Logic_SetString_ForTest(nameof(EUIObjectState.Process_After_HideCoroutine)));
                DoInit(pLogicFactory);

                var pHandle_Show = CanvasManager_ForLogicTest.DoShow<Canvas_ForLogicTest>(ECanvasName.Single);
                Assert.IsNull(pHandle_Show.pUIObject);

                yield return pHandle_Show.Yield_WaitForSetUIObject();

                Assert.IsNotNull(pHandle_Show.pUIObject);
                var pUICanvas = pHandle_Show.pUIObject;

                yield return pHandle_Show.Yield_WaitForAnimation();
                Assert.AreEqual(pUICanvas.strText, nameof(EUIObjectState.Process_After_ShowCoroutine));

                var pHandle_Hide = CanvasManager_ForLogicTest.DoHide<Canvas_ForLogicTest>(ECanvasName.Single);
                yield return null;
                Assert.AreEqual(pUICanvas.strText, nameof(EUIObjectState.Process_Before_HideCoroutine));

                yield return pHandle_Hide.Yield_WaitForAnimation();
                Assert.AreEqual(pUICanvas.strText, nameof(EUIObjectState.Process_After_HideCoroutine));


                // UI Object에게 직접 명령 후 Handle 및 Logic 동작 이벤트 테스트
                pHandle_Show = pUICanvas.DoShowCoroutine();
                yield return pHandle_Show.Yield_WaitForAnimation();
                Assert.AreEqual(pUICanvas.strText, nameof(EUIObjectState.Process_After_ShowCoroutine));

                pHandle_Hide = pUICanvas.DoHide_Coroutine();
                yield return null;
                Assert.AreEqual(pUICanvas.strText, nameof(EUIObjectState.Process_Before_HideCoroutine));

                yield return pHandle_Hide.Yield_WaitForAnimation();
                Assert.AreEqual(pUICanvas.strText, nameof(EUIObjectState.Process_After_HideCoroutine));

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
                CanvasManagerLogicFactory pLogicFactory = new CanvasManagerLogicFactory();
                pLogicFactory.DoAddLogic_PossibleUndo(EUIObjectState.Process_Before_ShowCoroutine, EUIObjectState.Process_Before_HideCoroutine, new Logic_Calculate_Add_And_Undo(iRandomValue));

                DoInit(pLogicFactory);

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
            public IEnumerator 캔버스의_Undologic_기능테스트_YieldSecond()
            {
                // Arrange
                // 이걸 안하면 테스트를 동시에 여러번 했을 때 
                // 인스턴스가 이미 있기 때문에 OnInit_ManagerLogic을 호출하지 않습니다.
                CanvasManager_ForLogicTest.DoDestroy_Manager(true);

                float fWaitSecond = Random.Range(0.1f, 0.2f);
                float fWaitSecond_Undo = Random.Range(0.1f, 0.2f);



                // Action
                // 로직은 실행되면 Canvas의 원래 값에 어떠한 값을 더하고,
                // 로직을 취소할 경우 원래값으로 복구합니다.
                CanvasManagerLogicFactory pLogicFactory = new CanvasManagerLogicFactory();
                pLogicFactory.DoAddLogic_PossibleUndo(EUIObjectState.Process_After_ShowCoroutine, EUIObjectState.Process_After_HideCoroutine, new Logic_WaitForSecond(fWaitSecond, fWaitSecond_Undo));
                DoInit(pLogicFactory);

                System.Diagnostics.Stopwatch pWatch = new System.Diagnostics.Stopwatch();

                pWatch.Start();
                var pHandle_Show = CanvasManager_ForLogicTest.DoShow<Canvas_ForLogicTest>(ECanvasName.Single);
                yield return pHandle_Show.Yield_WaitForAnimation();
                pWatch.Stop();

                float fTimeOffset = (Time.deltaTime * 2f);

                // 기다렸는지 체크하는 로직은
                // 실제 기다리는 시간을 지정한 시간의 +- 오차 time.DeltaTime이면 True
                Assert.Greater(pWatch.Elapsed.TotalSeconds, fWaitSecond - fTimeOffset);
                // Assert.Less(pWatch.Elapsed.TotalSeconds, fWaitSecond + Time.deltaTime);

                pWatch.Reset();
                pWatch.Start();
                var pHandle_Hide = CanvasManager_ForLogicTest.DoHide<Canvas_ForLogicTest>(ECanvasName.Single);
                yield return pHandle_Hide.Yield_WaitForAnimation();
                pWatch.Stop();

                Assert.Greater(pWatch.Elapsed.TotalSeconds, fWaitSecond_Undo - fTimeOffset);
                // Assert.Less(pWatch.Elapsed.TotalSeconds, fWaitSecond + Time.deltaTime);
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
                CanvasManagerLogicFactory pLogicFactory = new CanvasManagerLogicFactory();
                pLogicFactory.DoAddLogic_PossibleUndo(EUIObjectState.Process_Before_ShowCoroutine, EUIObjectState.Process_Before_HideCoroutine, new Logic_CreateObject_And_SetParents_And_Undo_Is_Destroy(pTransformParents));
                DoInit(pLogicFactory);

                int iRandomShow = Random.Range(3, 7);
                List<UICommandHandle<Canvas_ForLogicTest>> listHandle = new List<UICommandHandle<Canvas_ForLogicTest>>();
                for (int i = 0; i < iRandomShow; i++)
                    listHandle.Add(CanvasManager_ForLogicTest.DoShow_Multiple<Canvas_ForLogicTest>(ECanvasName.Single));

                yield return listHandle[listHandle.Count - 1].Yield_WaitForAnimation();

                Assert.AreEqual(pTransformParents.childCount, iRandomShow);
                CanvasManager_ForLogicTest.DoDestroy_Manager(true);

                yield return null;
                Assert.AreEqual(pTransformParents.childCount, 0);

                yield break;
            }
        }
    }
}