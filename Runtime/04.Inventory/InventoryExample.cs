#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-30
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Inventory))]
    public class InventoryExample : MonoBehaviour
    {
        /* const & readonly declaration             */


        /* enum & struct declaration                */

        [System.Serializable]
        public class SomthingData
        {
            public string strName;
            public Sprite pSpriteIcon;
            public Color pColor;
        }

        /* public - Field declaration               */

        public List<SomthingData> listSomthingData = new List<SomthingData>();

        /* protected & private - Field declaration  */

        Inventory _pInventory;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        private void Awake()
        {
            _pInventory = GetComponent<Inventory>();
        }

        private void OnEnable()
        {
            if(_pInventory.bIsExecute_Awake == false)
                _pInventory.EventAwake();

            _pInventory.DoClearData();
            _pInventory.DoAddRangeData(listSomthingData.ToArray());
            _pInventory.DoInit_SlotLogic_Command(
                new InventorySlotLogic_Command.InventorySlot_CommandLogic(InventorySlot.EInventorySlot_CommandEvent.OnDragBegin, 
                                                                          InventorySlot.EInventorySlot_CommandEvent.OnDragEnd,
                                                                          new InventorySlotLogic_Command.Instantiate_CloneSlot((p => p.GetComponent<Image>().enabled = false)))
                );

            _pInventory.OnSwap_Slot += _pInventory_OnSwap_Slot;
        }

        private void _pInventory_OnSwap_Slot(InventorySlot pStart, InventorySlot pDest)
        {
            pStart.DoSwapSlot(pDest);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}