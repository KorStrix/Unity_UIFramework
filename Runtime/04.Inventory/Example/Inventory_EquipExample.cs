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
using static UIFramework.InventoryExample;

namespace UIFramework
{

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Inventory))]
    public class Inventory_EquipExample : MonoBehaviour
    {
        /* const & readonly declaration             */


        /* enum & struct declaration                */

        /* public - Field declaration               */

        public List<SomthingData> listSomthingData = new List<SomthingData>();

        /* protected & private - Field declaration  */

        Inventory _pInventory;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


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

            _pInventory.OnSwap_Slot += _pInventory_OnSwap_Slot;
            _pInventory.OnSwap_Slot_OtherInventory -= Inventory_OnSwap_OtherInventory;
            _pInventory.OnSwap_Slot_OtherInventory += Inventory_OnSwap_OtherInventory;
        }

        private void Inventory_OnSwap_OtherInventory(Inventory.OnSwapSlot_Msg obj)
        {
            if (obj.pSlot_OnDraging.pData == null)
                return;

            obj.pSlot_OnDraging.DoSwapSlot(obj.pSlot_Dest);
            obj.pSlot_OnDraging.Event_SetSelected(false, false);
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
