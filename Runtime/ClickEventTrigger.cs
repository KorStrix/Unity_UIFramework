#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-21
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

/// <summary>
/// 
/// </summary>
public class ClickEventTrigger : MonoBehaviour, IPointerClickHandler
{
	/* const & readonly declaration             */

	/* enum & struct declaration                */

	/* public - Field declaration               */

	public bool bDebugMode = true;

	public UnityEvent OnClick;

	/* protected & private - Field declaration  */


	// ========================================================================== //

	/* public - [Do~Somthing] Function 	        */


	// ========================================================================== //

	/* protected - [Override & Unity API]       */

	public void OnPointerClick(PointerEventData eventData)
	{
		if (bDebugMode)
			Debug.Log($"{name} - {nameof(OnPointerClick)}", this);

		OnClick.Invoke();
	}

	/* protected - [abstract & virtual]         */


	// ========================================================================== //

	#region Private

	#endregion Private}
}