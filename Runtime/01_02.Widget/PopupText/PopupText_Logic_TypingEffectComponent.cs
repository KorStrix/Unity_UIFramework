#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-02-04
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIFramework.PopupText_Logic;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class PopupText_Logic_TypingEffectComponent : PopupText_Logic_ComponentBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public Logic_TextTyping pLogic = new Logic_TextTyping(1f);

        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        public override IEnumerator OnAnimation(PopupText pTextOwner, string strText)
        {
            yield return pLogic.ExecuteLogic_Coroutine(pTextOwner.pTextWrapper, strText);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private

    }
}