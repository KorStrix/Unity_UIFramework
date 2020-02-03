#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-29
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class UIFramework_ExampleManager : UIWidgetObjectBase, IHas_UIButton<UIFramework_ExampleManager.EUIButton>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EUIButton
        {
            Button_ShowPopupText,
        }

        enum EUIElement
        {
            InputField_PopupText,
        }

        /* public - Field declaration               */


        /* protected & private - Field declaration  */

        InputField _pInputField;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pInputField = GetComponentInChildren<InputField>();
            if (string.IsNullOrEmpty(_pInputField.text))
                _pInputField.text = Random.Range(0, 1000).ToString();
        }

        public void IHas_UIButton_OnClickButton(UIButtonMessage<EUIButton> sButtonMsg)
        {
            switch (sButtonMsg.eButtonName)
            {
                case EUIButton.Button_ShowPopupText:
                    break;

                default:
                    break;
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}