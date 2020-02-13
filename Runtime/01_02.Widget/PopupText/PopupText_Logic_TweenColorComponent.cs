#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-04
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
    public class PopupText_Logic_TweenColorComponent : PopupText_Logic_ComponentBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public Logic_TweenColor pLogic;

        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        public override IEnumerator OnAnimation(PopupText pTextOwner, string strText)
        {
            yield return pLogic.ExecuteLogic_Coroutine(pTextOwner.pTextWrapper.arrGraphic);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private

    }
}