#if UNIT_TEST

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace StrixLibrary_Test
{
    public class PopupManager_Example : CPopupManager<PopupManager_Example, PopupManager_Example.EPopupName>
    {

        public enum EPopupName
        {
            Single,
            Multiple,
        }

        public void DoRegist_PopupInstance(EPopupName ePopupName, IPopup pPopup)
        {
            CreatePopupInfo(ePopupName, pPopup);
        }

        public override Canvas GetPopupParentCavnas(EPopupName ePopupName, IPopup pPopup)
        {
            return null;
        }

        protected override IEnumerator OnRequire_Create_PopupInstance(EPopupName ePopupName)
        {
            yield break;
        }

    }

    #region PopupExample

    public class Popup_ForTest_Coroutine : IPopup
    {
        public Transform transform { get; }
        public IUIManager pUIManager { get; set; }

        public bool bIsPlay_Coroutine;
        public int iWaitFrameCount;

        public IEnumerator IPopup_OnShowCoroutine()
        {
            bIsPlay_Coroutine = true;

            Debug.Log("Start " + nameof(IPopup_OnShowCoroutine));

            int iWaitFrame = iWaitFrameCount;
            while (iWaitFrame-- > 0)
            {
                yield return null;
            }

            Debug.Log("Finish " + nameof(IPopup_OnShowCoroutine));
            bIsPlay_Coroutine = false;
        }

        public IEnumerator IPopup_OnHideCoroutine()
        {
            bIsPlay_Coroutine = true;

            Debug.Log("Start " + nameof(IPopup_OnHideCoroutine));

            int iWaitFrame = iWaitFrameCount;
            while (iWaitFrame-- > 0)
            {
                yield return null;
            }

            Debug.Log("Finish " + nameof(IPopup_OnHideCoroutine));

            bIsPlay_Coroutine = false;
        }
    }


    public class Popup_ForTest_Simple : IPopup
    {
        public Transform transform { get; }
        public IUIManager pUIManager { get; set; }

        public List<string> listString = new List<string>();


        public IEnumerator IPopup_OnShowCoroutine()
        {
            yield break;
        }

        public IEnumerator IPopup_OnHideCoroutine()
        {
            yield break;
        }
    }


    #endregion

    public class Popup_Test
    {
        int iWaitFrameCount;

        [UnityTest]
        public IEnumerator IPopup_Basic_Test()
        {
            PopupManager_Example.DoClear_PopupInstance();

            // 테스트용 팝업 생성
            Popup_ForTest_Coroutine pTestPopup = new Popup_ForTest_Coroutine();

            // 테스트용 팝업 매니져에 등록
            PopupManager_Example.instance.DoRegist_PopupInstance(PopupManager_Example.EPopupName.Single, pTestPopup);

            // 팝업 인스턴스 얻는 법 테스트
            Popup_ForTest_Coroutine pTestPopup_Get = PopupManager_Example.GetPopupInstance<Popup_ForTest_Coroutine>(PopupManager_Example.EPopupName.Single);
            Assert.AreEqual(pTestPopup, pTestPopup_Get);



            // 팝업의 OnShowCoroutine, Show(OnShowBeforePopup) 테스트
            iWaitFrameCount = Random.Range(3, 6);

            Assert.IsFalse(pTestPopup_Get.bIsPlay_Coroutine);
            // 매니져를 통한 팝업 켜기
            PopupManager_Example.DoShow_Popup(PopupManager_Example.EPopupName.Single,
                (Popup_ForTest_Coroutine pPopup) => pPopup.iWaitFrameCount = iWaitFrameCount,
                null);

            yield return null;
            Assert.IsTrue(pTestPopup_Get.bIsPlay_Coroutine);



            // 팝업의 OnShow 코루틴 대기
            int iWaitFrame = iWaitFrameCount;
            while (pTestPopup_Get.bIsPlay_Coroutine)
            {
                iWaitFrame--;
                yield return null;
            }

            Assert.LessOrEqual(iWaitFrame, 1);
            Assert.IsFalse(pTestPopup_Get.bIsPlay_Coroutine);
        }



        [UnityTest]
        public IEnumerator IPopup_ExtensionMethod_Test()
        {
            PopupManager_Example.DoClear_PopupInstance();

            // 테스트용 팝업 생성
            Popup_ForTest_Coroutine pTestPopup = new Popup_ForTest_Coroutine();

            // 테스트용 팝업 매니져에 등록
            PopupManager_Example.instance.DoRegist_PopupInstance(PopupManager_Example.EPopupName.Single, pTestPopup);
            Popup_ForTest_Coroutine pTestPopup_Get = PopupManager_Example.GetPopupInstance<Popup_ForTest_Coroutine>(PopupManager_Example.EPopupName.Single);
            Assert.AreEqual(pTestPopup, pTestPopup_Get);

            // 팝업의 OnShowCoroutine, Show(OnShowBeforePopup) 테스트
            iWaitFrameCount = Random.Range(3, 6);
            pTestPopup.iWaitFrameCount = iWaitFrameCount;

            Assert.IsFalse(pTestPopup_Get.bIsPlay_Coroutine);
            // 팝업에게 바로 명령
            pTestPopup.DoShow();

            yield return null;
            Assert.IsTrue(pTestPopup_Get.bIsPlay_Coroutine);



            // 팝업의 OnShow 코루틴 대기
            int iWaitFrame = iWaitFrameCount;
            while (pTestPopup_Get.bIsPlay_Coroutine)
            {
                iWaitFrame--;
                yield return null;
            }

            Assert.LessOrEqual(iWaitFrame, 1);
            Assert.IsFalse(pTestPopup_Get.bIsPlay_Coroutine);
        }

        [UnityTest]
        public IEnumerator IPopup_Event_Test()
        {
            PopupManager_Example.DoClear_PopupInstance();

            // 테스트용 팝업 생성
            Popup_ForTest_Simple pTestPopup = new Popup_ForTest_Simple();

            // 테스트용 팝업 매니져에 등록
            PopupManager_Example.instance.DoRegist_PopupInstance(PopupManager_Example.EPopupName.Single, pTestPopup);
            Popup_ForTest_Simple pTestPopup_Get = PopupManager_Example.GetPopupInstance<Popup_ForTest_Simple>(PopupManager_Example.EPopupName.Single);
            Assert.AreEqual(pTestPopup, pTestPopup_Get);

            pTestPopup.listString.Clear();
            Assert.AreEqual(pTestPopup.listString.Count, 0);

            pTestPopup.DoAdd_Listener_OnBeforeShowPopup((Popup_ForTest_Simple pPopup) => { pPopup.listString.Add("Add_Listener_BeforeShow"); });
            pTestPopup.DoAdd_Listener_OnShowPopup((Popup_ForTest_Simple pPopup) => { pPopup.listString.Add("Add_Listener_Show"); });
            pTestPopup.DoAdd_Listener_OnHidePopup((Popup_ForTest_Simple pPopup) => { pPopup.listString.Add("Add_Listener_Hide"); });

            pTestPopup.DoShow(
                OnBeforeShow: (Popup_ForTest_Simple pPopup) => { pPopup.listString.Add("Before_Show"); },
                OnShow: (Popup_ForTest_Simple pPopup) => { pPopup.listString.Add("Show"); }
                );

            // Executed OnBeforeShow
            Assert.AreEqual(pTestPopup.listString.Count, 2);

            yield return null;

            // Executed OnShow
            Assert.AreEqual(pTestPopup.listString.Count, 4);

            pTestPopup.DoHide();
            yield return null;
            Assert.AreEqual(pTestPopup.listString.Count, 5);


            pTestPopup.listString.Clear();
            Assert.AreEqual(pTestPopup.listString.Count, 0);

            pTestPopup.DoRemove_AllListener_OnBeforeShow();
            pTestPopup.DoRemove_AllListener_OnShow();
            pTestPopup.DoRemove_AllListener_OnHide();

            pTestPopup.DoShow();

            Assert.AreEqual(pTestPopup.listString.Count, 0);

            yield return null;

            Assert.AreEqual(pTestPopup.listString.Count, 0);

            pTestPopup.DoHide();
            yield return null;
            Assert.AreEqual(pTestPopup.listString.Count, 0);


        }
    }
}

#endif