#region Header
/*	============================================
 *	Author 			      : Strix
 *	Initial Creation Date : 2020-07-14 오전 11:13:29
 *	Summary 			  : 
 *  Template 		      : Visual Studio ItemTemplate For Unity V7
   ============================================ */
#endregion Header

using UnityEngine.EventSystems;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// UGUI 베이스 버튼 컬라이더
    /// <para>기존 <see cref="Button"/>의 기능을 끄고</para>
    /// <para><see cref="Collider"/>에 <see cref="IPointerClickHandler"/>를 붙여서 기존 <see cref="Button"/>의 <see cref="Button.onClick"/>을 호출</para>
    /// </summary>
    public class ButtonCollider : Selectable, IPointerClickHandler
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            	*/

        [Header("타겟 버튼")]
        public Button pButton;

        [Header("타겟 버튼의 Collider 동작유무")]
        public bool bIsInteractiveOn_TargetButton = true;

        /* protected & private - Field declaration  */

        MethodInfo _pMethodInfo_DoStateTransition;

        // ========================================================================== //

        /* public - [Do~Something] Function 	    */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void Awake()
        {
            if (pButton == null)
                pButton = GetComponent<Button>();

            if (pButton == null)
            {
                Debug.LogError($"{name} - pButton == null");
                return;
            }

            _pMethodInfo_DoStateTransition = pButton.GetType().GetMethod(nameof(DoStateTransition), BindingFlags.Instance | BindingFlags.NonPublic);

            pButton.enabled = bIsInteractiveOn_TargetButton;
            pButton.interactable = bIsInteractiveOn_TargetButton;

            base.Awake();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            pButton.onClick?.Invoke();
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            _pMethodInfo_DoStateTransition.Invoke(pButton, new object[] {state, instant});
            // pButton.SendMessage(nameof(DoStateTransition), new object[] {state, instant});
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private

    }
}
