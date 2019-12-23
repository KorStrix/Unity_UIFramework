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

public interface IHas_UIButton<Enum_ButtonName>
{
    void IHas_UIButton_OnClickButton(Enum_ButtonName eButtonName);
}

public interface IHas_UIToggle
{
    void IHas_UIToggle_OnToggle(UIToggleMessage sToggleMessage);
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
        Button[] arrButton = pOwner.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < arrButton.Length; i++)
        {
            Button pButton = arrButton[i];

            bool bParseSuccess;
            object pEnum;
            Parsing_NameToEnum(pType_EnumButtonName, pButton, out bParseSuccess, out pEnum);

            if (bParseSuccess == false)
                continue;

            UnityEngine.Events.UnityAction pAction = () => 
            {
                pOwner.SendMessage(nameof(IHas_UIButton<object>.IHas_UIButton_OnClickButton), pEnum, SendMessageOptions.DontRequireReceiver); 
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

            bool bParseSuccess = true;

            if (bParseSuccess == false)
                continue;

            UnityEngine.Events.UnityAction<bool> pAction = (bool bIsOn) =>
            {
                pOwner.SendMessage(nameof(IHas_UIToggle.IHas_UIToggle_OnToggle), new UIToggleMessage(pToggle, bIsOn), SendMessageOptions.DontRequireReceiver);
            };

            pToggle.onValueChanged.AddListener(pAction);
        }
    }

    private static void Parsing_NameToEnum(System.Type pType_EnumButtonName, Component pButton, out bool bParseSuccess, out object pEnum)
    {
        bParseSuccess = true;
        pEnum = null;
        if (pType_EnumButtonName.IsEnum)
        {
            try
            {
                pEnum = System.Enum.Parse(pType_EnumButtonName, pButton.name);
            }
            catch
            {
                bParseSuccess = false;
            }
        }
        else
            pEnum = pButton.name;
    }

}