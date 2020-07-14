#region Header
/*	============================================
 *	Author 			    	: Require PlayerPref Key : "Author"
 *	Initial Creation Date 	: 2020-07-14
 *	Summary 		        : 
 *  Template 		        : New Behaviour For Unity Editor V2
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class ButtonCollider_Example : MonoBehaviour, IHas_UIButton<ButtonCollider_Example.EButtonName>
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

    public enum EButtonName
    {
        ButtonColliderExample,
    }

	/* public - Field declaration               */


	/* protected & private - Field declaration  */


	// ========================================================================== //

	/* public - [Do~Something] Function 	        */


	// ========================================================================== //

	/* protected - [Override & Unity API]       */

    public void ButtonTest_Inspector()
    {
        Debug.Log(nameof(ButtonTest_Inspector), this);
    }

    void ButtonTest_Interface()
    {
        Debug.Log(nameof(ButtonTest_Interface), this);
    }

    public void IHas_UIButton_OnClickButton(UIButtonMessage<EButtonName> sButtonMsg)
    {
        switch (sButtonMsg.eButtonName)
        {
            case EButtonName.ButtonColliderExample:
                ButtonTest_Interface();
                break;
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private

}
