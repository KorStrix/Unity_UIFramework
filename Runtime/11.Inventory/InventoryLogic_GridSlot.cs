#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-03-31
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UIFramework
{
	/// <summary>
	/// 일단 Inventory Slot만
    /// 가로 기준으로 구현
	/// </summary>
    [RequireComponent(typeof(Inventory))]
	public class InventoryLogic_GridSlot : UIWidgetObjectBase
	{
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public int iGridSlotCount;

        /* protected & private - Field declaration  */

        List<InventorySlot> _listSlotInstance = new List<InventorySlot>();
        Inventory _pInventory;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pInventory = GetComponent<Inventory>();
            _pInventory.OnEmptySlot += Inventory_OnEmptySlot;
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private InventorySlot Inventory_OnEmptySlot(InventorySlot pSlotOrigin)
        {
            _listSlotInstance.Clear();
            Transform pTransform_SlotParents = pSlotOrigin.transform.parent;

            for (int i = 0; i < iGridSlotCount; i++)
            {
                InventorySlot pSlotCopy = GameObject.Instantiate(pSlotOrigin, pTransform_SlotParents);
                pSlotCopy.EventAwake();

                _listSlotInstance.Add(pSlotCopy);
            }

            return _listSlotInstance.FirstOrDefault();
        }

        #endregion Private
    }
}
