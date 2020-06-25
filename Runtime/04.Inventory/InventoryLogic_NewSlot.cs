#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-03-31
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UIFramework
{
	/// <summary>
	/// </summary>
	[DefaultExecutionOrder(order:-100)]
    [RequireComponent(typeof(Inventory))]
	public class InventoryLogic_NewSlot : UIWidgetObjectBase
	{
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        /* protected & private - Field declaration  */

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
            Transform pTransform_SlotParents = pSlotOrigin.transform.parent;
            InventorySlot pSlotCopy = GameObject.Instantiate(pSlotOrigin, pTransform_SlotParents);
            pSlotCopy.EventAwake();

            return pSlotCopy;
        }

        #endregion Private
    }
}