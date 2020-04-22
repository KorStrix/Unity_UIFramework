#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-02 오후 4:47:34
 *	개요 :
 *	
 *	UI Element(Button, Toggle 등)의 주요 이벤트(OnClick, OnToggle 등)을
 *	인스펙터에서 수동으로 작업하지 않고
 *	Script에서 바로 사용하게 해주는 Helper 클래스입니다.
 *	
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

/// <summary>
/// <see cref="Button"/>을 관리해주는 Interface입니다
/// </summary>
/// <typeparam name="Enum_ButtonName">버튼 이름이 담긴 Enum 타입</typeparam>
public interface IHas_UIButton<Enum_ButtonName>
{
    void IHas_UIButton_OnClickButton(UIButtonMessage<Enum_ButtonName> sButtonMsg);
}

/// <summary>
/// <see cref="Toggle"/>을 관리해주는 Interface입니다.
/// </summary>
public interface IHas_UIToggle
{
    void IHas_UIToggle_OnToggle(UIToggleMessage sToggleMessage);
}

/// <summary>
/// UI Button
/// </summary>
/// <typeparam name="Enum_ButtonName">버튼 이름이 담긴 Enum 타입</typeparam>
public struct UIButtonMessage<Enum_ButtonName>
{
    /// <summary>
    /// 클릭한 버튼의 이름입니다.
    /// </summary>
    public Enum_ButtonName eButtonName { get; private set; }

    /// <summary>
    /// 클릭한 버튼의 인스턴스입니다. Null이 될 수 있습니다.
    /// </summary>
    public Button pButtonInstance_OrNull { get; private set; }

    public UIButtonMessage(Enum_ButtonName eButtonName, Button pButton = null)
    {
        this.eButtonName = eButtonName; this.pButtonInstance_OrNull = pButton;
    }
}

public struct UIToggleMessage
{
    public string strToggleName { get; private set; }
    public Toggle pToggle { get; private set; }
    public bool bToggle { get; private set; }

    public UIToggleMessage(Toggle pToggle, bool bToggle)
    {
        this.strToggleName = pToggle.name; this.pToggle = pToggle; this.bToggle = bToggle;
    }
}

/// <summary>
/// UI Element(<see cref="Button"/>, <see cref="Toggle"/> 등)의 주요 이벤트(OnClick, OnToggle 등)을 Script에서 바로 사용하게 해주는 Helper 클래스입니다.
/// </summary>
public static class SCUIElementEventHelper
{
    /// <summary>
    /// Interface - "IHas_UI~..." 를 상속받은 경우 주요 이벤트를 자동으로 연결합니다.
    /// </summary>
    public static void DoInit_HasUIElement(MonoBehaviour pUIElementOwner)
    {
        if(pUIElementOwner == null)
        {
            Debug.LogError("Error - pUIElementOwner == null");
            return;
        }

        System.Type[] arrInterfacesType = pUIElementOwner.GetType().GetInterfaces();
        if (arrInterfacesType == null || arrInterfacesType.Length == 0)
            return;

        Init_HasButton(pUIElementOwner, arrInterfacesType);
        Init_HasToggle(pUIElementOwner, arrInterfacesType);
    }

    static void Init_HasButton(MonoBehaviour pOwner, System.Type[] arrInterfacesType)
    {
        System.Type pType_InterfaceHasButton = arrInterfacesType.FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHas_UIButton<>));
        if (pType_InterfaceHasButton == null)
            return;

        System.Type pType_EnumButtonName = pType_InterfaceHasButton.GetGenericArguments()[0];
        HashSet<string> setEnumName = new HashSet<string>(System.Enum.GetNames(pType_EnumButtonName));
        var arrButton = pOwner.GetComponentsInChildren<Button>(true).Where(p => setEnumName.Contains(p.name));
        foreach (Button pButton in arrButton)
        {
            object pEnumValue;
            if (TryParsing_NameToEnum(pType_EnumButtonName, pButton, out pEnumValue) == false)
                continue;

            UnityEngine.Events.UnityAction pAction = () => 
            {
                System.Type pButtonMessageGenericType = typeof(UIButtonMessage<>).MakeGenericType(pType_EnumButtonName);
                object pButtonMsg = System.Activator.CreateInstance(pButtonMessageGenericType, pEnumValue, pButton);
                pOwner.SendMessage(nameof(IHas_UIButton<object>.IHas_UIButton_OnClickButton), pButtonMsg, SendMessageOptions.DontRequireReceiver); 
            };

            pButton.onClick.AddListener(pAction);
        }
    }

    static void Init_HasToggle(MonoBehaviour pOwner, System.Type[] arrInterfacesType)
    {
        if (arrInterfacesType.Contains(typeof(IHas_UIToggle)) == false)
            return;

        Toggle[] arrToggle = pOwner.GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < arrToggle.Length; i++)
        {
            Toggle pToggle = arrToggle[i];

            UnityEngine.Events.UnityAction<bool> pAction = (bool bIsOn) =>
            {
                pOwner.SendMessage(nameof(IHas_UIToggle.IHas_UIToggle_OnToggle), new UIToggleMessage(pToggle, bIsOn), SendMessageOptions.DontRequireReceiver);
            };

            pToggle.onValueChanged.AddListener(pAction);
        }
    }

    private static bool TryParsing_NameToEnum(System.Type pType_EnumButtonName, Component pButton, out object pEnum)
    {
        bool bResult = true;
        pEnum = null;
        if (pType_EnumButtonName.IsEnum)
        {
            try
            {
                pEnum = System.Enum.Parse(pType_EnumButtonName, pButton.name);
            }
            catch
            {
                bResult = false;
            }
        }
        else
            pEnum = pButton.name;

        return bResult;
    }
}