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

using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
using Object = UnityEngine.Object;


#if UNITY_EDITOR
using System;
using UnityEditor;
#endif

/// <summary>
/// UIElement를 보유했다는 인터페이스
/// 빈몸통 && 하위 인터페이스 상속용
/// </summary>
// ReSharper disable once UnusedTypeParameter
public interface IHas_UIElement<TEnum_ElementName>
{
    GameObject gameObject { get; }
}


/// <summary>
/// <see cref="Button"/>을 관리해주는 Interface입니다
/// </summary>
/// <typeparam name="TEnum_ButtonName">버튼 이름이 담긴 Enum 타입</typeparam>
public interface IHas_UIButton<TEnum_ButtonName> : IHas_UIElement<TEnum_ButtonName>
{
    void IHas_UIButton_OnClickButton(UIButtonMessage<TEnum_ButtonName> sButtonMsg);
}

/// <summary>
/// <see cref="Toggle"/>을 관리해주는 Interface입니다.
/// </summary>
public interface IHas_UIToggle<TEnum_ToggleName> : IHas_UIElement<TEnum_ToggleName>
{
    void IHas_UIToggle_OnToggle(UIToggleMessage<TEnum_ToggleName> sToggleMessage);
}

/// <summary>
/// UI Button
/// </summary>
/// <typeparam name="TEnum_ButtonName">버튼 이름이 담긴 Enum 타입</typeparam>
public struct UIButtonMessage<TEnum_ButtonName>
{
    /// <summary>
    /// 클릭한 버튼의 이름입니다.
    /// </summary>
    public TEnum_ButtonName eButtonName { get; private set; }

    /// <summary>
    /// 클릭한 버튼의 인스턴스입니다. Null이 될 수 있습니다.
    /// </summary>
    public Button pButtonInstance_OrNull { get; private set; }

    public UIButtonMessage(TEnum_ButtonName eButtonName, Button pButton = null)
    {
        this.eButtonName = eButtonName; pButtonInstance_OrNull = pButton;
    }
}

public struct UIToggleMessage<TEnum_ToggleName>
{
    public TEnum_ToggleName eToggleName { get; private set; }
    public Toggle pToggle_OrNull { get; private set; }
    public bool bToggleInput { get; private set; }

    public UIToggleMessage(TEnum_ToggleName eToggleName, bool bToggleInput, Toggle pToggleOrNull = null)
    {
        this.eToggleName = eToggleName; pToggle_OrNull = pToggleOrNull; this.bToggleInput = bToggleInput;
    }
}

/// <summary>
/// UI Element(<see cref="Button"/>, <see cref="Toggle"/> 등)의 주요 이벤트(OnClick, OnToggle 등)을 Script에서 바로 사용하게 해주는 Helper 클래스입니다.
/// </summary>
public static class HasUIElementHelper
{
    /// <summary>
    /// Interface - "IHas_UI~..." 를 상속받은 경우 주요 이벤트를 자동으로 연결합니다.
    /// <para>하단은 지원되는 인터페이스 목록</para>
    /// <para><see cref="IHas_UIButton{TEnum_ButtonName}"/></para>
    /// <para><see cref="IHas_UIToggle{TEnum_ToggleName}"/></para>
    /// </summary>
    public static void DoInit_HasUIElement(MonoBehaviour pUIElementOwner)
    {
        if(pUIElementOwner == null)
        {
            Debug.LogError($"Error - {nameof(HasUIElementHelper)}.{nameof(DoInit_HasUIElement)} pUIElementOwner == null");
            return;
        }

        Type[] arrInterfaces = pUIElementOwner.GetType().
            GetInterfaces().
            Where(p => p.IsGenericType).
            ToArray();

        if (arrInterfaces.Length == 0)
            return;

        Init_HasButton(pUIElementOwner, arrInterfaces.FirstOrDefault(p => p.GetGenericTypeDefinition() == typeof(IHas_UIButton<>)));
        Init_HasToggle(pUIElementOwner, arrInterfaces.FirstOrDefault(p => p.GetGenericTypeDefinition() == typeof(IHas_UIToggle<>)));
    }


    public static bool GetInterfaceEnumName(Type pType_Interface, out HashSet<string> setEnumName)
    {
        return GetInterfaceEnumName(pType_Interface, out setEnumName, out _);
    }

    public static bool GetInterfaceEnumName(Type pType_Interface, out HashSet<string> setEnumName, out Type pType_ElementEnumName)
    {
        setEnumName = new HashSet<string>();
        pType_ElementEnumName = null;

        if (pType_Interface == null)
            return false;

        Get_GenericEnum(pType_Interface, out setEnumName, out pType_ElementEnumName);
        return true;
    }

    // ========================================================================== //

    static void Init_HasButton(MonoBehaviour pOwner, Type pInterface)
    {
        if (GetInterfaceEnumName(pInterface, out HashSet<string> setEnumName, out Type pType_ElementEnumName) == false)
            return;

        IEnumerable<Button> arrButton = pOwner.GetComponentsInChildren<Button>(true).
            Where(p => setEnumName.Contains(p.name));

        foreach (Button pButton in arrButton)
        {
            if (TryParsing_NameToEnum(pType_ElementEnumName, pButton, out object pEnumValue) == false)
                continue;


            UnityEngine.Events.UnityAction pAction = () => 
            {
                Type pEnumGenericType = typeof(UIButtonMessage<>).MakeGenericType(pType_ElementEnumName);
                object pMsg = Activator.CreateInstance(pEnumGenericType, pEnumValue, pButton);
                pOwner.SendMessage(nameof(IHas_UIButton<object>.IHas_UIButton_OnClickButton), pMsg, SendMessageOptions.DontRequireReceiver); 
            };

            pButton.onClick.AddListener(pAction);
        }
    }

    static void Init_HasToggle(MonoBehaviour pOwner, Type pInterface)
    {
        if (GetInterfaceEnumName(pInterface, out HashSet<string> setEnumName, out Type pType_ElementEnumName) == false)
            return;

        IEnumerable<Toggle> arrToggle = pOwner.GetComponentsInChildren<Toggle>(true).
            Where(p => setEnumName.Contains(p.name));

        foreach (Toggle pToggle in arrToggle)
        {
            if (TryParsing_NameToEnum(pType_ElementEnumName, pToggle, out object pEnumValue) == false)
                continue;


            UnityEngine.Events.UnityAction<bool> pAction = (bIsOn) =>
            {
                Type pEnumGenericType = typeof(UIToggleMessage<>).MakeGenericType(pType_ElementEnumName);
                object pMsg = Activator.CreateInstance(pEnumGenericType, pEnumValue, bIsOn, pToggle);
                pOwner.SendMessage(nameof(IHas_UIToggle<object>.IHas_UIToggle_OnToggle), pMsg, SendMessageOptions.DontRequireReceiver);
            };

            pToggle.onValueChanged.AddListener(pAction);
        }
    }

    private static bool TryParsing_NameToEnum(Type pType_EnumButtonName, Component pButton, out object pEnum)
    {
        bool bResult = true;
        pEnum = null;
        if (pType_EnumButtonName.IsEnum)
        {
            try
            {
                pEnum = Enum.Parse(pType_EnumButtonName, pButton.name);
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

    private static void Get_GenericEnum(Type pType_Interface, out HashSet<string> setEnumName, out Type pType_EnumName)
    {
        pType_EnumName = pType_Interface.GetGenericArguments()[0];
        setEnumName = new HashSet<string>(Enum.GetNames(pType_EnumName));
    }
}


/// <summary>
/// 
/// </summary>
public class UIHasEnumSelectorAttribute : PropertyAttribute
{

}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(UIHasEnumSelectorAttribute))]
public class UIEnumSelectorDrawer : PropertyDrawer
{
    readonly List<string> _listEnum = new List<string>();

    // ========================================================================== //

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        _listEnum.Clear();
        _listEnum.Add("Nothing");

        EditorGUI.BeginProperty(position, label, property);

        Object pObjectOwner = property.serializedObject?.targetObject;
        if (pObjectOwner == null)
            return;

        Component pComponent = pObjectOwner as Component;
        if (pComponent == null)
            return;

        MonoBehaviour[] arrMono = pComponent.GetComponents<MonoBehaviour>();
        foreach (var pMono in arrMono)
        {
            Type[] arrInterfaces = pMono.GetType().
                GetInterfaces().
                Where(p => p.IsGenericType).
                ToArray();

            Type pButtonInterface = arrInterfaces.FirstOrDefault(p => p.GetGenericTypeDefinition() == typeof(IHas_UIButton<>));
            if (pButtonInterface == null)
                continue;

            if (HasUIElementHelper.GetInterfaceEnumName(pButtonInterface, out var setEnumName))
                _listEnum.AddRange(setEnumName);
        }


        int iIndex = CalculateIndex(property);

        //Draw the popup box with the current selected index
        iIndex = EditorGUI.Popup(position, label.text, iIndex, _listEnum.ToArray());
        property.stringValue = iIndex >= 1 ? _listEnum[iIndex] : "";

        EditorGUI.EndProperty();
    }

    // ========================================================================== //

    private int CalculateIndex(SerializedProperty property)
    {
        string strValue = property.stringValue;
        int iIndex = -1;
        if (strValue == "")
        {
            //The tag is empty
            iIndex = 0; //first index is the special <notag> entry
        }
        else
        {
            //check if there is an entry that matches the entry and get the index
            //we skip index 0 as that is a special custom case
            for (int i = 1; i < _listEnum.Count; i++)
            {
                if (_listEnum[i] == strValue)
                {
                    iIndex = i;
                    break;
                }
            }
        }

        return iIndex;
    }
}
#endif
