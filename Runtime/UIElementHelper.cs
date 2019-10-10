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
    void IHas_UIButton_OnClickButton(Enum_ButtonName eButtonName, Button pButton);
}

public interface IHas_UIToggle<Enum_ToggleName>
{
    void IHas_UIToggle_OnToggle(Enum_ToggleName eToggleName, Toggle pToggle, bool bToggle);
}

/// <summary>
/// UI Element(Button, Toggle 등)의 주요 이벤트(OnClick, OnToggle 등)을 Script에서 바로 사용하게 해주는 Helper 클래스입니다.
/// </summary>
public static class SCUIElementHelper
{
    /// <summary>
    /// Interface - "IHas_UI~..." 를 상속받은 경우 주요 이벤트를 자동으로 연결합니다.
    /// </summary>
    public static void DoInit_HasUIElement(MonoBehaviour pUIElementOwner)
    {
        Init_HasButton(pUIElementOwner);
        Init_HasToggle(pUIElementOwner);
    }

    public static void Init_HasButton(MonoBehaviour pUIButtonOwner)
    {
        System.Type pType_InterfaceHasButton = pUIButtonOwner.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHas_UIButton<>));
        if (pType_InterfaceHasButton == null)
            return;

        System.Type pType_EnumButtonName = pType_InterfaceHasButton.GetGenericArguments()[0];
        var pMethod = pType_InterfaceHasButton.GetMethod("IHas_UIButton_OnClickButton");

        Button[] arrButton = pUIButtonOwner.GetComponentsInChildren<Button>(true);
        for (int i = 0; i < arrButton.Length; i++)
        {
            Button pButton = arrButton[i];

            bool bParseSuccess;
            object pEnum;
            Parsing_NameToEnum(pType_EnumButtonName, pButton, out bParseSuccess, out pEnum);

            if (bParseSuccess)
            {
                UnityEngine.Events.UnityAction pAction = delegate { pMethod.Invoke(pUIButtonOwner, new object[2] { pEnum, pButton }); };

                pButton.onClick.RemoveListener(pAction);
                pButton.onClick.AddListener(pAction);
            }
        }
    }

    public static void Init_HasToggle(MonoBehaviour pUIToggleOwner)
    {
        System.Type pType_InterfaceHasToggle = pUIToggleOwner.GetType().GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IHas_UIToggle<>));
        if (pType_InterfaceHasToggle == null)
            return;

        System.Type pType_EnumButtonName = pType_InterfaceHasToggle.GetGenericArguments()[0];
        var pMethod = pType_InterfaceHasToggle.GetMethod("IHas_UIToggle_OnToggle");

        Toggle[] arrToggle = pUIToggleOwner.GetComponentsInChildren<Toggle>(true);
        for (int i = 0; i < arrToggle.Length; i++)
        {
            Toggle pToggle = arrToggle[i];

            bool bParseSuccess = true;
            object pEnum = null;
            Parsing_NameToEnum(pType_EnumButtonName, pToggle, out bParseSuccess, out pEnum);

            if (bParseSuccess)
            {
                UnityEngine.Events.UnityAction<bool> pAction = delegate (bool bIsOn) { pMethod.Invoke(pUIToggleOwner, new object[3] { pEnum, pToggle, bIsOn }); };

                pToggle.onValueChanged.RemoveListener(pAction);
                pToggle.onValueChanged.AddListener(pAction);
            }
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