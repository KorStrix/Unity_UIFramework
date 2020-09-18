using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.TestTools;
using NUnit.Framework;
using UIFramework;

namespace UIFramework_Test
{
    namespace About_Canvas
    {
        /// <summary>
        /// protected에 접근하기 위해 <see cref="CanvasManager_Example"/>를 상속
        /// </summary>
        [Category("UIFramework")]/* [Timeout(7000)]*/
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
                while (pHandle.pUIObject.bIsPlay_Coroutine == false)
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


            [UnityTest]
            public IEnumerator ICanvas_확장함수_테스트()
            {
                Debug.LogWarning(nameof(ICanvas_확장함수_테스트).ConvertRichText_SetColor(EColor_ForRichText.red));

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
                pHandle.pUIObject.DoHide_Coroutine();
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

                DoDestroy_Manager(true);

                iWaitFrameCount = Random.Range(3, 6);

                // Set_OnCheck_IsShow에서 Return False를 했기 때문에 Show가 되지 않습니다.
                var pHandle = DoShow<Canvas_ForTest>(ECanvasName.Single).
                        Set_OnCheck_IsShow(CoReturn_False).
                        Set_OnBeforeShow((x) => x.DoSetWaitFrameCount(iWaitFrameCount)).
                        Set_OnShow_BeforeAnimation((x) => x.strText = "1").
                        Set_OnShow_AfterAnimation((x) => x.strText += "2").
                        Set_OnHide((x) => x.strText += "3");


                // 애니메이션을 WaitCoroutine해도 Show 관련 동작을 안하기 때문에 Frame은 그대로
                yield return pHandle.Yield_WaitForAnimation();
                Assert.AreEqual(pHandle.pUIObject.iWaitFrameCount, pHandle.pUIObject.iWaitFrameCount_Current);
                Assert.IsNull(pHandle.pUIObject.strText); // Animation Event가 들어왔는지 확인 안들어왔기 때문에 string default인 null


                // Set_OnCheck_IsShow에서 Return True를 했기 때문에 이번엔 Show가 됩니다.
                pHandle = DoShow<Canvas_ForTest>(ECanvasName.Single).
                    Set_OnCheck_IsShow(CoReturn_True).
                    Set_OnBeforeShow((Canvas_ForTest pPopup) => pPopup.DoSetWaitFrameCount(iWaitFrameCount)).
                    Set_OnShow_BeforeAnimation((x) => x.strText = "1").
                    Set_OnShow_AfterAnimation((x) => x.strText += "2").
                    Set_OnHide((x) => x.strText += "3");

                yield return pHandle.Yield_WaitForAnimation();
                Assert.AreEqual(pHandle.pUIObject.iWaitFrameCount_Current, 0);
                Assert.AreEqual(pHandle.pUIObject.strText, "12"); // Animation Event가 들어왔는지 확인

                yield return pHandle.Yield_WaitForAnimation();

                yield break;
            }

            [UnityTest]
            public IEnumerator 캔버스매니져의_ShowMultiple함수는_인스턴스여러개를_띄울수있습니다()
            {
                Debug.Log(nameof(캔버스매니져의_ShowMultiple함수는_인스턴스여러개를_띄울수있습니다).ConvertRichText_SetColor(EColor_ForRichText.red));

                CanvasManager_Example.DoDestroy_Manager(true);
                Assert.AreEqual(UICommandHandle<Canvas_ForTest>.g_iInstanceCount, 0);

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

                    Assert.AreEqual(UICommandHandle<Canvas_ForTest>.g_iInstanceCount, 0);
                }

                yield break;
            }

            [UnityTest]
            public IEnumerator ICanvas_GetShowedPopup_And_AllHide_Test()
            {
                Debug.Log(nameof(ICanvas_GetShowedPopup_And_AllHide_Test).ConvertRichText_SetColor(EColor_ForRichText.red));

                DoDestroy_Manager(true);
                Assert.AreEqual(UICommandHandle<Canvas_ForTest>.g_iInstanceCount, 0);

                int iMultipleOpenCount = Random.Range(10, 20);
                yield return MultiplePopup_ShowHideTest(iMultipleOpenCount, 2, false);

                List<Canvas_ForTest> listCanvas_1 = GetAlreadyShow_CanvasList<Canvas_ForTest>(ECanvasName.Single);
                List<ICanvas> listCanvas_2 = GetAlreadyShow_CanvasList();

                Assert.AreEqual(iMultipleOpenCount, listCanvas_1.Count);
                Assert.AreEqual(iMultipleOpenCount, listCanvas_2.Count);

                DoAllHide_ShowedCanvas(true);

                for (int i = 0; i < listCanvas_1.Count; i++)
                    Assert.AreEqual(listCanvas_1[i].eState, Canvas_ForTest.EState.Show);

                for (int i = 0; i < 10; i++)
                    yield return null;

                for (int i = 0; i < listCanvas_1.Count; i++)
                    Assert.AreEqual(listCanvas_1[i].eState, Canvas_ForTest.EState.Hide);
            }

            [UnityTest]
            public IEnumerator 캔버스매니져를_Destroy하면_현재보여지는_모든팝업은_Hide됩니다()
            {
                Debug.Log(nameof(캔버스매니져를_Destroy하면_현재보여지는_모든팝업은_Hide됩니다).ConvertRichText_SetColor(EColor_ForRichText.red));

                // Arrange
                DoDestroy_Manager(true);
                Assert.IsNull(_instance);
                Assert.IsTrue(_instance.IsNull());
                int iMultipleOpenCount = Random.Range(10, 20);


                // Act
                yield return MultiplePopup_ShowHideTest(iMultipleOpenCount, 2, false);


                // Assert
                Assert.AreEqual(GetAlreadyShow_CanvasList().Count, iMultipleOpenCount);

                DoDestroy_Manager(false);
                Assert.IsNull(_instance);
                Assert.AreEqual(GetAlreadyShow_CanvasList().Count, 0);
            }

            [UnityTest]
            public IEnumerator 캔버스매니져의_Show함수는_인스턴스한개만_띄워야합니다()
            {
                // Arrange
                var pHandle = DoShowOnly(ECanvasName.Single);
                var pHandle2 = DoShowOnly(ECanvasName.Single);


                // Act
                yield return pHandle.Yield_WaitForSetUIObject();
                yield return pHandle2.Yield_WaitForSetUIObject();


                // Assert
                Assert.AreEqual(pHandle.pUIObject, pHandle2.pUIObject);
                Assert.AreEqual(pHandle.pUIObject.GetHandle(), pHandle2.pUIObject.GetHandle());
            }

            /// <summary>
            /// 기존에는 여러번 호출하면 무한 yield가 돌았음
            /// </summary>
            /// <returns></returns>
            [UnityTest]
            public IEnumerator 캔버스매니져_Hide함수를_여러번_호출했을때_동작해야합니다()
            {
                // Arrange
                const int iRandomCount = 3;

                List<UICommandHandle<ICanvas>> listHandle = new List<UICommandHandle<ICanvas>>();
                List<IEnumerator> listCoroutine = new List<IEnumerator>();
                Dictionary<ICanvas, bool> mapCanvas_IsHided = new Dictionary<ICanvas, bool>();

                // 일단 Random으로 같은 Canvas을 Show를 합니다
                {
                    for (int i = 0; i < iRandomCount; i++)
                    {
                        var pHandle = DoShowOnly(ECanvasName.Single).
                            Set_OnShow_AfterAnimation(pCanvas => mapCanvas_IsHided[pCanvas]= false);

                        listHandle.Add(pHandle);
                        listCoroutine.Add(pHandle.Yield_WaitForAnimation());
                    }

                    yield return listCoroutine.GetEnumerator();
                    listCoroutine.Clear();
                }

                // Act, Handle을 통해 Canvas를 Hide합니다.
                {
                    for (int i = 0; i < iRandomCount; i++)
                    {
                        listCoroutine.Add(listHandle[i].pUIObject.DoHide_Coroutine()
                            .Set_OnHide(pCanvas => mapCanvas_IsHided[pCanvas] = true)
                            .Yield_WaitForAnimation());
                    }
                    yield return listCoroutine.GetEnumerator();
                }



                // Assert
                foreach (var bIsHide in mapCanvas_IsHided.Values)
                    Assert.IsTrue(bIsHide);
            }

            /// <summary>
            /// 기존에는 Show가 안됐음
            /// </summary>
            /// <returns></returns>
            [UnityTest]
            public IEnumerator 캔버스매니져_같은프레임에_Hide와_Show를하면_정상인지확인()
            {
                // Arrange
                var pHandle = DoShow<Canvas_ForTest>(ECanvasName.Single);
                yield return pHandle.Yield_WaitForSetUIObject();
                Canvas_ForTest pCanvas = pHandle.pUIObject;

                // Act
                pCanvas.DoHide_NotPlayHideCoroutine();
                var pHandle_New = DoShow<Canvas_ForTest>(ECanvasName.Single);
                yield return pHandle_New.Yield_WaitForSetUIObject();
                Canvas_ForTest pCanvas_New = pHandle_New.pUIObject;

                // Assert
                Assert.AreEqual(pCanvas, pCanvas_New);
                // Assert.IsTrue(pCanvas_New.eState == Canvas_ForTest.EState.Show);
            }

            // ================================================================================================================

            #region private

            IEnumerator MultiplePopup_ShowHideTest(int iMultipleOpenCount, int iMaxFrame, bool bIsHide = true)
        {
            List<UICommandHandle<Canvas_ForTest>> listCommandHandle = new List<UICommandHandle<Canvas_ForTest>>();

            for (int i = 1; i < iMultipleOpenCount + 1; i++)
            {
                int iWaitFrameCount = i % iMaxFrame;

                var pHandle = DoShow_Multiple<Canvas_ForTest>(ECanvasName.Single).
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
                    pUITarget.DoHide_Coroutine();
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

        #endregion
        }
    }
}
