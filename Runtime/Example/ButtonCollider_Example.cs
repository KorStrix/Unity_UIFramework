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
using UIFramework;
using UnityEngine.UI;

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

    Button pButton;

    // ========================================================================== //

    /* public - [Do~Something] Function 	        */


    // ========================================================================== //

    /* protected - [Override & Unity API]       */

    private void Awake()
    {
        HasUIElementHelper.DoInit_HasUIElement(this);

        pButton = GetComponentInChildren<ButtonCollider>().pButton;
    }

    public void ButtonTest_Inspector()
    {
        Debug.Log(nameof(ButtonTest_Inspector), this);
        StartCoroutine(ButtonCoroutine(pButton));
    }

    void ButtonTest_Interface(Button pButton)
    {
        Debug.Log(nameof(ButtonTest_Interface), this);
        StartCoroutine(ButtonCoroutine(pButton));
    }

    public void IHas_UIButton_OnClickButton(UIButtonMessage<EButtonName> sButtonMsg)
    {
        switch (sButtonMsg.eButtonName)
        {
            case EButtonName.ButtonColliderExample:
                ButtonTest_Interface(sButtonMsg.pButtonInstance_OrNull);
                break;
        }
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    IEnumerator ButtonCoroutine(Button pButton)
    {
        Text pText = pButton.GetComponentInChildren<Text>();

        pText.text = "Clicked!";

        yield return new WaitForSeconds(1f);

        pText.text = "Normal";

        yield break;
    }

    #endregion Private

}
