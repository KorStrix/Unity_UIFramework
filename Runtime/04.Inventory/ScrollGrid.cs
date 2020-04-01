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
	/// 
	/// </summary>
	public class ScrollGrid : UIWidgetObjectBase
	{
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public int iGridObjectCount;

		/* protected & private - Field declaration  */

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        #region IEnumerable

        public IEnumerator<InventorySlot> GetEnumerator()
		{
			return _listSlot.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _listSlot.GetEnumerator();
		}

        #endregion

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}