#region Header
/*	============================================
 *	Aurthor 			  : Strix
 *	Initial Creation Date : 2020-03-16 오전 11:08:18
 *	Summary 			  : 
 *  Template 		      : Visual Studio ItemTemplate For Unity V7
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIFramework;

namespace UIFramework_Test
{
    namespace About_Canvas
    {
        /// <summary>
        /// 테스트용 캔버스 매니져 예시
        /// </summary>
        public class CanvasManager_Example : CanvasManager<CanvasManager_Example, CanvasManager_Example.ECanvasName>
        {
            public static readonly bool const_bIsDebug = false;

            public enum ECanvasName
            {
                Single,
                SecondSingle,
            }

            protected override IEnumerator OnCreate_Instance(ECanvasName eName, System.Action<ICanvas> OnFinishCreate)
            {
                switch (eName)
                {
                    case ECanvasName.Single: OnFinishCreate(new GameObject(nameof(Canvas_ForTest)).AddComponent<Canvas_ForTest>()); break;
                    case ECanvasName.SecondSingle: OnFinishCreate(new GameObject(nameof(Canvas_ForTest)).AddComponent<Canvas_ForTest>()); break;

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


        /// <summary>
        /// 테스트용 캔버스 예시
        /// </summary>
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

                if (CanvasManager_Example.const_bIsDebug)
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
    }
}