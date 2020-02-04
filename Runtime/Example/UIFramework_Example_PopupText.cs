#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-30
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
    public class UIFramework_Example_PopupText : UIFramework_ExampleCanvasBase, IHas_UIButton<UIFramework_Example_PopupText.EUIButton>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EUIButton
        {
            Button_ShowPopupText,
        }

        /* public - Field declaration               */

        public PopupText pPopupText;

        /* protected & private - Field declaration  */

        Canvas _pCanvas;
        InputField _pInputField;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pCanvas = GetComponent<Canvas>();

            _pInputField = GetComponentInChildren<InputField>();
            if (string.IsNullOrEmpty(_pInputField.text))
                _pInputField.text = Random.Range(0, 1000).ToString();
        }

        public void IHas_UIButton_OnClickButton(UIButtonMessage<EUIButton> sButtonMsg)
        {
            switch (sButtonMsg.eButtonName)
            {
                case EUIButton.Button_ShowPopupText:

                    Vector3 vecSpawnPos = sButtonMsg.pButtonInstance_OrNull.transform.position;
                    vecSpawnPos.x += Random.Range(-5f, 5f);
                    vecSpawnPos.y += Random.Range(-5f, 5f);

                    PopupText pPopupTextClone = Instantiate(pPopupText);
                    pPopupTextClone.transform.position = vecSpawnPos;
                    pPopupTextClone.DoShow(_pCanvas, _pInputField.text, vecSpawnPos);
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