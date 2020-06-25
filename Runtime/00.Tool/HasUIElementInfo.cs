#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-06-19
 *	Summary 		        : 
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = System.Object;

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UI;

#endif

namespace UIFramework
{
    [Serializable]
    public class UIElementInfo
    {
        public enum EType
        {
            Button,
            Toggle,
        }

        [UIHasEnumSelector] 
        public string strEnumName;

        public MonoBehaviour pMono;

        public EType eType;

        public UIElementInfo(string strEnumName, MonoBehaviour pMono, EType eType)
        {
            this.strEnumName = strEnumName;
            this.pMono = pMono;
            this.eType = eType;
        }

        public override string ToString()
        {
            return strEnumName;
        }
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(UIElementInfo))]
    public class UIElementInfoDrawer : PropertyDrawer
    {
        readonly List<UIElementInfo> _listEnum = new List<UIElementInfo>();

        // ========================================================================== //

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Object pObjectOwner = property.serializedObject?.targetObject;
            if (pObjectOwner == null)
                return;

            Component pComponent = pObjectOwner as Component;
            if (pComponent == null)
                return;

            _listEnum.Clear();
            MonoBehaviour[] arrMono = pComponent.GetComponents<MonoBehaviour>();
            foreach (var pMono in arrMono)
            {
                if (pMono == null)
                    continue;

                Button[] arrButton = pMono.GetComponentsInChildren<Button>(true);
                _listEnum.AddRange(arrButton.Select(pButton => new UIElementInfo(pButton.name, pMono, UIElementInfo.EType.Button)));
            }

            if (_listEnum.Count == 0)
                return;



            EditorGUI.BeginProperty(position, label, property);
            SerializedProperty pProperty_strEnumName = property.FindPropertyRelative(nameof(UIElementInfo.strEnumName));

            int iIndex = CalculateIndex(pProperty_strEnumName);
            iIndex = EditorGUI.Popup(position, label.text, iIndex, _listEnum.Select(p => p.strEnumName).ToArray());



            SerializedProperty pProperty_pMono = property.FindPropertyRelative(nameof(UIElementInfo.pMono));
            pProperty_strEnumName.stringValue = _listEnum[iIndex].strEnumName;
            pProperty_pMono.objectReferenceValue = _listEnum[iIndex].pMono;


            EditorGUI.EndProperty();
        }

        // ========================================================================== //

        private int CalculateIndex(SerializedProperty property)
        {
            string strValue = property.stringValue;
            if (string.IsNullOrEmpty(strValue))
                return 0;

            int iIndex = 0;
            for (int i = 1; i < _listEnum.Count; i++)
            {
                if (_listEnum[i].strEnumName == strValue)
                {
                    iIndex = i;
                    break;
                }
            }

            return iIndex;
        }
    }
#endif
}