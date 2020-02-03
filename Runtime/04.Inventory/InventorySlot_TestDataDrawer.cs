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
    public class InventorySlot_TestDataDrawer : UIWidgetObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */


        /* protected & private - Field declaration  */

        Image _pImage_Icon;
        Text _pText_Name;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pImage_Icon = GetComponentInChildren<Image>();
            _pText_Name = GetComponentInChildren<Text>();

            InventorySlot pSlot = GetComponent<InventorySlot>();
            pSlot.OnChangeSlotData += OnChangeSlotData;
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void OnChangeSlotData(InventorySlot.OnChangeSlotData_Msg obj)
        {
            if (obj.pData_Current == null)
            {
                _pImage_Icon.sprite = null;
                _pImage_Icon.color = new Color(0f, 0f, 0f, 0f);
                _pText_Name.text = "";
            }
            else
            {
                InventoryTester.SomthingData pData = obj.pData_Current as InventoryTester.SomthingData;
                _pImage_Icon.sprite = pData.pSpriteIcon;
                _pImage_Icon.color = pData.pColor;
                _pText_Name.text = pData.strName;
            }
        }

        #endregion Private
    }
}