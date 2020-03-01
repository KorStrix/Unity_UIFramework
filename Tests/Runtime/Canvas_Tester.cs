using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.TestTools;
using NUnit.Framework;
using UIFramework;

namespace StrixLibrary_Test
{
    public class CanvasManager_Example : CanvasManager<CanvasManager_Example, CanvasManager_Example.ECanvasName>
    {
        public static readonly bool const_bIsDebug = false;

        public enum ECanvasName
        {
            Single,
        }

        protected override IEnumerator OnCreate_Instance(ECanvasName eName, System.Action<ICanvas> OnFinishCreate)
        {
            switch (eName)
            {
                case ECanvasName.Single:  OnFinishCreate(new GameObject(nameof(Canvas_ForTest)).AddComponent<Canvas_ForTest>()); break;

                default:
                    Debug.LogError("Error");
                    break;
            }

            yield break;
        }

        protected override void OnInit_ManagerLogic(CanvasManagerLogicFactory pLogicFactory)
        {
        }

        public override Canvas GetParentCavnas(ECanvasName eName, ICanvas pCanvas)
        {
            return null;
        }
    }

    #region PopupExample

    public class Canvas_ForTest : MonoBehaviour, ICanvas
    {
        public enum EState
        {
            Init,
            Created,
            Show,
            Hide,
        }
        public IUIManager pUIManager { get; set; }

        public bool bIsPlay_Coroutine;
        public int iWaitFrameCount { get; private set; }
        public int iWaitFrameCount_Current { get; private set; }

        public int iID { get; private set; } = -1;
        public EState eState { get; private set; } = EState.Init;
        public string strText;

        static int g_iCreateInstanceCount = 0;
        void Awake()
        {
            iID = g_iCreateInstanceCount++;
            name += "_" + iID.ToString();
         
            eState = EState.Created;
        }

        public void DoSetWaitFrameCount(int iWaitFrameCount)
        {
            if (CanvasManager_Example.const_bIsDebug)
                Debug.Log(name + " " + nameof(DoSetWaitFrameCount) + " iWaitFrameCount: " + iWaitFrameCount);

            this.iWaitFrameCount = iWaitFrameCount;
            this.iWaitFrameCount_Current = iWaitFrameCount;
        }

        public IEnumerator OnShowCoroutine()
        {
            bIsPlay_Coroutine = true;
            eState = EState.Show;

            iWaitFrameCount_Current = iWaitFrameCount;
            
            if(CanvasManager_Example.const_bIsDebug)
                Debug.Log(name + " " + nameof(OnShowCoroutine) + " Start iWaitFrameCount_Current: " + iWaitFrameCount_Current);

            while (iWaitFrameCount_Current > 0)
            {
                if (CanvasManager_Example.const_bIsDebug)
                    Debug.Log(name + " " + nameof(OnShowCoroutine) + " Playing.. iWaitFrameCount_Current : " + iWaitFrameCount_Current);
                --iWaitFrameCount_Current;
                yield return null;
            }

            if (CanvasManager_Example.const_bIsDebug)
                Debug.Log(name + " " + nameof(OnShowCoroutine) + " Finish");
            bIsPlay_Coroutine = false;
        }

        public IEnumerator OnHideCoroutine()
        {
            bIsPlay_Coroutine = true;
            eState = EState.Hide;

            if (CanvasManager_Example.const_bIsDebug)
                Debug.Log(name + " " + nameof(OnHideCoroutine) + " Start iWaitFrameCount_Current: " + iWaitFrameCount_Current);

            iWaitFrameCount_Current = iWaitFrameCount;
            while (iWaitFrameCount_Current > 0)
            {
                if (CanvasManager_Example.const_bIsDebug)
                    Debug.Log(name + " " + nameof(OnHideCoroutine) + " Playing.. iWaitFrameCount_Current : " + iWaitFrameCount_Current);
                --iWaitFrameCount_Current;
                yield return null;
            }

            if (CanvasManager_Example.const_bIsDebug)
                Debug.Log(name + " " + nameof(OnHideCoroutine) + " Finish");

            bIsPlay_Coroutine = false;
        }
    }


    #endregion

    /// <summary>
    /// protected에 접근하기 위해 <see cref="CanvasManager_Example"/>를 상속
    /// </summary>
    [Category("StrixLibrary")]
    public class Canvas_Tester : CanvasManager_Example
    {
        int iWaitFrameCount;

        /// <summary>
        /// 매니져를 통해 Show를 한 뒤 <see cref="IUIObject.OnShowCoroutine"/>을 테스트
        /// </summary>
        [UnityTest]
        public IEnumerator ICanvas_Basic_Test()
        {
            Debug.LogWarning(nameof(ICanvas_Basic_Test).ConvertRichText_SetColor(EColor_ForRichText.red));

            CanvasManager_Example.DoDestroy_Manager(true);

            iWaitFrameCount = Random.Range(3, 6);
            // 매니져를 통한 팝업 켜기
            var pHandle = CanvasManager_Example.DoShow<Canvas_ForTest>(CanvasManager_Example.ECanvasName.Single).
                Set_OnBeforeShow((Canvas_ForTest pPopup) => pPopup.DoSetWaitFrameCount(iWaitFrameCount));

            yield return null; // 핸들을 통해 UI Object를 얻어올때는 인스턴스가 Null일시 한프레임 기다려야 합니다.
            while(pHandle.pUIObject.bIsPlay_Coroutine == false)
            {
                if (CanvasManager_Example.const_bIsDebug)
                    Debug.Log("Wait..");
                yield return null;
            }

            // OnShow 코루틴 대기
            var pTestTarget = pHandle.pUIObject;
            Assert.IsTrue(pTestTarget.bIsPlay_Coroutine);

            // 팝업에서 세팅한 WaitFrameCount와 여기서 세팅한 WaitFrameCount를 일치시키고
            // 똑같이 기다립니다.
            int iWaitFrame = iWaitFrameCount;
            while (pTestTarget.bIsPlay_Coroutine)
            {
                iWaitFrame--;
                yield return null;
            }

            // 기다린 Frame Count가 거의 일치합니다.
            Assert.LessOrEqual(iWaitFrame, 1);
            Assert.IsFalse(pTestTarget.bIsPlay_Coroutine);
        }


        /// <summary>
        /// <see cref="ICanvas"/>를 상속받으면 <see cref="ICanvasHelper.DoShow"/>, <see cref="ICanvasHelper.DoHide"/> 등의 확장 함수를 지원합니다.
        /// </summary>
        [UnityTest]
        public IEnumerator ICanvas_ExtensionMethod_Test()
        {
            Debug.LogWarning(nameof(ICanvas_ExtensionMethod_Test).ConvertRichText_SetColor(EColor_ForRichText.red));

            CanvasManager_Example.DoDestroy_Manager(true);

            iWaitFrameCount = Random.Range(3, 6);
            // 매니져를 통한 팝업 켜기
            var pHandle = CanvasManager_Example.DoShow<Canvas_ForTest>(CanvasManager_Example.ECanvasName.Single).
                Set_OnBeforeShow((Canvas_ForTest pPopup) => pPopup.DoSetWaitFrameCount(iWaitFrameCount));

            yield return null; // 핸들을 통해 UI Object를 얻어올때는 인스턴스가 Null일시 한프레임 기다려야 합니다.
            while (pHandle.pUIObject.bIsPlay_Coroutine == false)
            {
                if (CanvasManager_Example.const_bIsDebug)
                    Debug.Log("Wait..");
                yield return null;
            }
            Assert.IsTrue(pHandle.pUIObject.bIsPlay_Coroutine);

            // 팝업의 코루틴 대기
            int iWaitFrame = iWaitFrameCount;
            while (pHandle.pUIObject.bIsPlay_Coroutine)
            {
                iWaitFrame--;
                yield return null;
            }

            Assert.IsFalse(pHandle.pUIObject.bIsPlay_Coroutine);
            // 팝업에게 바로 Hide 명령
            pHandle.pUIObject.DoHide();
            while (pHandle.pUIObject.bIsPlay_Coroutine == false)
            {
                if (CanvasManager_Example.const_bIsDebug)
                    Debug.Log("Wait..");
                yield return null;
            }

            Assert.IsTrue(pHandle.pUIObject.bIsPlay_Coroutine);

            // 팝업의 코루틴 대기
            iWaitFrame = iWaitFrameCount;
            while (pHandle.pUIObject.bIsPlay_Coroutine)
            {
                iWaitFrame--;
                yield return null;
            }

            Assert.LessOrEqual(iWaitFrame, 1);
            Assert.IsFalse(pHandle.pUIObject.bIsPlay_Coroutine);
        }


        /// <summary>
        /// <see cref="UICommandHandle{T}"/>의 Event 동작 테스트
        /// </summary>
        [UnityTest]
        public IEnumerator ICanvas_Event_Hooking_Test()
        {
            Debug.LogWarning(nameof(ICanvas_Event_Hooking_Test).ConvertRichText_SetColor(EColor_ForRichText.red));

            CanvasManager_Example.DoDestroy_Manager(true);

            iWaitFrameCount = Random.Range(3, 6);

            // Set_OnCheck_IsShow에서 Return False를 했기 때문에 Show가 되지 않습니다.
            var pHandle = CanvasManager_Example.DoShow<Canvas_ForTest>(CanvasManager_Example.ECanvasName.Single).
                    Set_OnCheck_IsShow(CoReturn_False).
                    Set_OnBeforeShow((x) => x.DoSetWaitFrameCount(iWaitFrameCount)).
                    Set_OnShow_BeforeAnimation((x) => x.strText = "1").
                    Set_OnShow_AfterAnimation((x) => x.strText += "2");


            // 애니메이션을 WaitCoroutine해도 Frame은 그대로
            yield return pHandle.Yield_WaitForAnimation();
            Assert.AreEqual(pHandle.pUIObject.iWaitFrameCount, pHandle.pUIObject.iWaitFrameCount_Current);
            Assert.IsNull(pHandle.pUIObject.strText); // Animation Event가 들어왔는지 확인 안들어왔기 때문에 string default인 null


            // Set_OnCheck_IsShow에서 Return True를 했기 때문에 이번엔 Show가 됩니다.
            pHandle = CanvasManager_Example.DoShow<Canvas_ForTest>(CanvasManager_Example.ECanvasName.Single).
                Set_OnCheck_IsShow(CoReturn_True).
                Set_OnBeforeShow((Canvas_ForTest pPopup) => pPopup.DoSetWaitFrameCount(iWaitFrameCount)).
                Set_OnShow_BeforeAnimation((x) => x.strText = "1").
                Set_OnShow_AfterAnimation((x) => x.strText += "2");

            yield return pHandle.Yield_WaitForAnimation();
            Assert.AreEqual(pHandle.pUIObject.iWaitFrameCount_Current, 0);
            Assert.AreEqual(pHandle.pUIObject.strText, "12"); // Animation Event가 들어왔는지 확인

            yield break;
        }

        [UnityTest]
        public IEnumerator ICanvas_MultiplePopup_Test()
        {
            Debug.Log(nameof(ICanvas_MultiplePopup_Test).ConvertRichText_SetColor(EColor_ForRichText.red));

            CanvasManager_Example.DoDestroy_Manager(true);
            Assert.AreEqual(UICommandHandle<Canvas_ForTest>.iInstanceCount, 0);

            int iRandomCountMax = 0;
            int iTestCase = 3;
            for (int i = 0; i < iTestCase; i++)
            {
                int iMultipleOpenCount = Random.Range(10, 20);
                iRandomCountMax += iMultipleOpenCount;

                yield return MultiplePopup_ShowHideTest(iMultipleOpenCount, 2);
                Debug.LogWarning("------------------------------------");
                Debug.LogWarning(string.Format("Test Count : {0} // Test Cycle : {1} / {2}", iMultipleOpenCount, i + 1, iTestCase));
                Debug.LogWarning("------------------------------------");

                yield return null; // Handle이 초기화될때까지 기다린다.

                Assert.AreEqual(UICommandHandle<Canvas_ForTest>.iInstanceCount, 0);
            }

            yield break;
        }

        [UnityTest]
        public IEnumerator ICanvas_GetShowedPopup_And_AllHide_Test()
        {
            Debug.Log(nameof(ICanvas_GetShowedPopup_And_AllHide_Test).ConvertRichText_SetColor(EColor_ForRichText.red));

            DoDestroy_Manager(true);
            Assert.AreEqual(UICommandHandle<Canvas_ForTest>.iInstanceCount, 0);

            int iMultipleOpenCount = Random.Range(10, 20);
            yield return MultiplePopup_ShowHideTest(iMultipleOpenCount, 2, false);

            List<Canvas_ForTest> listCanvas_1 = GetAlreadyShow_CanvasList<Canvas_ForTest>(ECanvasName.Single);
            List<ICanvas> listCanvas_2 = GetAlreadyShow_CanvasList();

            Assert.AreEqual(iMultipleOpenCount, listCanvas_1.Count);
            Assert.AreEqual(iMultipleOpenCount, listCanvas_2.Count);

            DoAllHide_ShowedCanvas(true);

            for (int i = 0; i < listCanvas_1.Count; i++)
                Assert.AreEqual(listCanvas_1[i].eState, Canvas_ForTest.EState.Show);

            for(int i = 0; i < 10; i++)
                yield return null;

            for (int i = 0; i < listCanvas_1.Count; i++)
                Assert.AreEqual(listCanvas_1[i].eState, Canvas_ForTest.EState.Hide);
        }

        [UnityTest]
        public IEnumerator CanvasManager_Destroy_And_Init_Loop()
        {
            Debug.Log(nameof(ICanvas_GetShowedPopup_And_AllHide_Test).ConvertRichText_SetColor(EColor_ForRichText.red));
            
            DoDestroy_Manager(true);
            Assert.IsNull(_instance);
            Assert.IsTrue(_instance.IsNull());

            int iMultipleOpenCount = Random.Range(10, 20);
            yield return MultiplePopup_ShowHideTest(iMultipleOpenCount, 2, false);

            Assert.AreEqual(GetAlreadyShow_CanvasList().Count, iMultipleOpenCount);

            DoDestroy_Manager(false);
            Assert.IsNull(_instance);
            Assert.AreEqual(GetAlreadyShow_CanvasList().Count, 0);

            yield break;
        }

        // ================================================================================================================

        IEnumerator MultiplePopup_ShowHideTest(int iMultipleOpenCount, int iMaxFrame, bool bIsHide = true)
        {
            List<UICommandHandle<Canvas_ForTest>> listCommandHandle = new List<UICommandHandle<Canvas_ForTest>>();

            for (int i = 1; i < iMultipleOpenCount + 1; i++)
            {
                int iWaitFrameCount = i % iMaxFrame;

                var pHandle = CanvasManager_Example.DoShow_Multiple<Canvas_ForTest>(CanvasManager_Example.ECanvasName.Single).
                    Set_OnBeforeShow(x => x.DoSetWaitFrameCount(iWaitFrameCount));

                listCommandHandle.Add(pHandle);
            }

            // 모든 Yield For Animation 동시에 기다리기
            List<IEnumerator> listWaitCoroutine = new List<IEnumerator>();
            for (int i = 0; i < listCommandHandle.Count; i++)
                listWaitCoroutine.Add(listCommandHandle[i].Yield_WaitForAnimation());
            yield return listWaitCoroutine.GetEnumerator();

            if (bIsHide)
            {
                HashSet<Canvas_ForTest> setCheckOverlap = new HashSet<Canvas_ForTest>();
                for (int i = 0; i < listCommandHandle.Count; i++)
                {
                    Canvas_ForTest pUITarget = listCommandHandle[i].pUIObject;
                    if (setCheckOverlap.Add(pUITarget) == false)
                    {
                        Debug.LogError("CheckOverlap Fail");
                        break;
                    }

                    try
                    {
                        Assert.AreEqual(pUITarget.eState, Canvas_ForTest.EState.Show);
                        Assert.AreEqual(pUITarget.iWaitFrameCount_Current, 0);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Error - " + pUITarget.iID.ToString() + " " + e);
                    }

                    // 다시 Hide 시키기
                    pUITarget.DoHide();
                }

                // 모든 Yield For Animation 동시에 기다리기
                listWaitCoroutine.Clear();
                for (int i = 0; i < listCommandHandle.Count; i++)
                    listWaitCoroutine.Add(listCommandHandle[i].Yield_WaitForAnimation());
                yield return listWaitCoroutine.GetEnumerator();

                for (int i = 0; i < listCommandHandle.Count; i++)
                {
                    Canvas_ForTest pUITarget = listCommandHandle[i].pUIObject;

                    try
                    {
                        Assert.AreEqual(pUITarget.eState, Canvas_ForTest.EState.Hide);
                        Assert.AreEqual(pUITarget.iWaitFrameCount_Current, 0);
                    }
                    catch (System.Exception e)
                    {
                        Debug.LogError("Error - " + pUITarget.iID.ToString() + " " + e);
                    }
                }
            }

            yield return null;
        }

        IEnumerator CoReturn_True(System.Action<bool> CheckIsShow)
        {
            Debug.Log(nameof(CoReturn_True));

            CheckIsShow(true);

            yield return null;
        }

        IEnumerator CoReturn_False(System.Action<bool> CheckIsShow)
        {
            Debug.Log(nameof(CoReturn_False));

            CheckIsShow(false);

            yield return null;
        }
    }
}