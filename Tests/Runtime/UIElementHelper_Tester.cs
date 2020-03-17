using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace UIFramework_Test
{
    /// <summary>
    /// 버튼을 가지고 있는 테스트용 Monobehaviour
    /// </summary>
    public class HasButton_TestObject : MonoBehaviour, IHas_UIButton<HasButton_TestObject.EButtonObjectName>
    {
        public enum EButtonObjectName
        {
            None,

            A, B,
        }

        public EButtonObjectName eLastClickButton { get; private set; } = EButtonObjectName.None;

        public void IHas_UIButton_OnClickButton(UIButtonMessage<EButtonObjectName> sButtonMsg)
        {
            eLastClickButton = sButtonMsg.eButtonName;
        }
    }

    /// <summary>
    /// 토글을 가지고 있는 테스트용 Monobehaviour
    /// </summary>
    public class HasToggle_TestObject : MonoBehaviour, IHas_UIToggle
    {
        public enum EToggleObjectName
        {
            None,

            ToggleA, ToggleB,
        }

        public EToggleObjectName eLastClickToggle { get; private set; } = EToggleObjectName.None;
        public bool bLastToggle { get; private set; } = false;

        public void IHas_UIToggle_OnToggle(UIToggleMessage sToggleMessage)
        {
            eLastClickToggle = (EToggleObjectName)System.Enum.Parse(typeof(EToggleObjectName), sToggleMessage.strToggleName);
            bLastToggle = sToggleMessage.bToggle;
        }
    }

    [Category("UIFramework")]
    public class UIElementHelper_Tester : MonoBehaviour
    {
        [Test]
        static public void HasButton_Test()
        {
            GameObject pObjectTest = new GameObject(nameof(HasButton_Test));

            GameObject pObjectButton_A = new GameObject(HasButton_TestObject.EButtonObjectName.A.ToString());
            Button pButtonA = pObjectButton_A.AddComponent<Button>();
            pObjectButton_A.transform.parent = pObjectTest.transform;

            GameObject pObjectButton_B = new GameObject(HasButton_TestObject.EButtonObjectName.B.ToString());
            Button pButtonB = pObjectButton_B.AddComponent<Button>();
            pObjectButton_B.transform.parent = pObjectTest.transform;



            // 테스트 시작
            HasButton_TestObject pTestScript = pObjectTest.AddComponent<HasButton_TestObject>();

            // 처음 디폴트 값은 None입니다.
            Assert.AreEqual(pTestScript.eLastClickButton, HasButton_TestObject.EButtonObjectName.None);

            // 스크립트로 수동으로 버튼 A 클릭
            pButtonA.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));

            // Helper를 통한 Init을 안했기 때문에 여전히 None
            Assert.AreEqual(pTestScript.eLastClickButton, HasButton_TestObject.EButtonObjectName.None);


            // Helper를 통한 Init
            SCUIElementEventHelper.DoInit_HasUIElement(pTestScript);

            // 스크립트로 수동으로 버튼 A 클릭 테스트
            pButtonA.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));
            Assert.AreEqual(pTestScript.eLastClickButton, HasButton_TestObject.EButtonObjectName.A);

            // 스크립트로 수동으로 버튼 B 클릭 테스트
            pButtonB.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));
            Assert.AreEqual(pTestScript.eLastClickButton, HasButton_TestObject.EButtonObjectName.B);
        }

        [Test]
        static public void HasToggle_Test()
        {
            GameObject pObjectTest = new GameObject(nameof(HasToggle_Test));

            GameObject pObjectToggle_A = new GameObject(HasToggle_TestObject.EToggleObjectName.ToggleA.ToString());
            Toggle pToggleA = pObjectToggle_A.AddComponent<Toggle>();
            pObjectToggle_A.transform.SetParent(pObjectTest.transform);

            GameObject pObjectToggle_B = new GameObject(HasToggle_TestObject.EToggleObjectName.ToggleB.ToString());
            Toggle pToggleB = pObjectToggle_B.AddComponent<Toggle>();
            pObjectToggle_B.transform.SetParent(pObjectTest.transform);



            // 테스트 시작
            HasToggle_TestObject pTestScript = pObjectTest.AddComponent<HasToggle_TestObject>();

            // 처음 디폴트 값은 None입니다.
            Assert.AreEqual(pTestScript.eLastClickToggle, HasToggle_TestObject.EToggleObjectName.None);

            // 스크립트로 수동으로 토글 A 클릭
            pToggleA.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));

            // Helper를 통한 Init을 안했기 때문에 여전히 None
            Assert.AreEqual(pTestScript.eLastClickToggle, HasToggle_TestObject.EToggleObjectName.None);


            // Helper를 통한 Init
            SCUIElementEventHelper.DoInit_HasUIElement(pTestScript);

            // 테스트 하기 전 Toggle의 isOn상태 저장
            bool bIsOn_Original = pToggleA.isOn;

            // 스크립트로 수동으로 토글 A 클릭 테스트
            pToggleA.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));
            Assert.AreEqual(pTestScript.eLastClickToggle, HasToggle_TestObject.EToggleObjectName.ToggleA);
            Assert.AreEqual(pTestScript.bLastToggle, !bIsOn_Original);

            // 스크립트로 수동으로 토글 A 클릭 테스트
            pToggleA.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));
            Assert.AreEqual(pTestScript.eLastClickToggle, HasToggle_TestObject.EToggleObjectName.ToggleA);
            Assert.AreEqual(pTestScript.bLastToggle, !!bIsOn_Original);


            // 스크립트로 수동으로 토글 B 클릭 테스트
            pToggleB.OnPointerClick(new UnityEngine.EventSystems.PointerEventData(null));
            Assert.AreEqual(pTestScript.eLastClickToggle, HasToggle_TestObject.EToggleObjectName.ToggleB);
        }
    }
}
