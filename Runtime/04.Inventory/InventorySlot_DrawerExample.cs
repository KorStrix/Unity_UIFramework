#region Header
/*	============================================
 *	Author   			    : Strix
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
    public class InventorySlot_DrawerExample : UIWidgetObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public Image pImage_IsSelected;
        public Image pImage_Icon;

        /* protected & private - Field declaration  */

        Text _pText_Name;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pText_Name = GetComponentInChildren<Text>();

            InventorySlot pSlot = GetComponent<InventorySlot>();
            pImage_IsSelected.enabled = pSlot.bIsSelected;
            OnChangeSlotData(new InventorySlot.OnChangeSlotData_Msg(pSlot, null, null));

            pSlot.OnChange_SlotData += OnChangeSlotData;
            pSlot.OnChange_IsSelected += (bIsSelected) => pImage_IsSelected.enabled = bIsSelected;
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void OnChangeSlotData(InventorySlot.OnChangeSlotData_Msg obj)
        {
            if (obj.pData_Current == null)
            {
                pImage_Icon.sprite = null;
                pImage_Icon.color = new Color(0f, 0f, 0f, 0f);
                _pText_Name.text = "";
            }
            else
            {
                InventoryExample.SomthingData pData = obj.pData_Current as InventoryExample.SomthingData;
                pImage_Icon.sprite = pData.pSpriteIcon;
                pImage_Icon.color = pData.pColor;
                _pText_Name.text = pData.strName;
            }
        }

        #endregion Private
    }
}