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

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}