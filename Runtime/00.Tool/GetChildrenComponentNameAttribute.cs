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
/// 
/// </summary>
public class GetChildrenComponentNameAttribute : PropertyAttribute
{
    public System.Type[] arrType { get; private set; }

    public GetChildrenComponentNameAttribute(params System.Type[] arrType)
    {
        this.arrType = arrType;
    }
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(GetChildrenComponentNameAttribute))]
public class GetComponentNameAttributeDrawer : PropertyDrawer
{
    readonly List<string> _listName = new List<string>();

    // ========================================================================== //

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.propertyType != SerializedPropertyType.String)
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        _listName.Clear();

        EditorGUI.BeginProperty(position, label, property);

        Object pObjectOwner = property.serializedObject?.targetObject;
        if (pObjectOwner == null)
            return;

        Component pOwnerComponent = pObjectOwner as Component;
        if (pOwnerComponent == null)
            return;

        GetChildrenComponentNameAttribute pAttribute = attribute as GetChildrenComponentNameAttribute;
        if (pAttribute.arrType == null || pAttribute.arrType.Length == 0)
            return;

        Type[] arrType = pAttribute.arrType;
        foreach(Type pType in arrType)
        {
            Component[] arrComponent = pOwnerComponent.GetComponentsInChildren(pType, true);
            _listName.AddRange(arrComponent.Select(p => p.name));
        }
      


        int iIndex = CalculateIndex(property);

        //Draw the popup box with the current selected index
        iIndex = EditorGUI.Popup(position, label.text, iIndex, _listName.ToArray());
        property.stringValue = iIndex >= 1 ? _listName[iIndex] : "";

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
            for (int i = 1; i < _listName.Count; i++)
            {
                if (_listName[i] == strValue)
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
