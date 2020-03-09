#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-29
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class ButtonExtension : Button
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum ESelectionState
        {
            Normal = 0,
            Highlighted = 1,
            Pressed = 2,
            Selected = 3,
            Disabled = 4
        }


        /* public - Field declaration               */

        public delegate void delOnStateTransition_Color(ESelectionState state, Color sColorBase, float fDuration);
        public event delOnStateTransition_Color OnStateTransition_Color;

        /* protected & private - Field declaration  */

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
            base.DoStateTransition(state, instant);

            switch (state)
            {
                case SelectionState.Normal: OnStateTransition_Color?.Invoke((ESelectionState)(int)state, this.colors.normalColor, this.colors.fadeDuration); break;
                case SelectionState.Highlighted: OnStateTransition_Color?.Invoke((ESelectionState)(int)state, this.colors.highlightedColor, this.colors.fadeDuration); break;
                case SelectionState.Pressed: OnStateTransition_Color?.Invoke((ESelectionState)(int)state, this.colors.pressedColor, this.colors.fadeDuration); break;

#if UNITY_2019
                case SelectionState.Selected: OnStateTransition_Color?.Invoke((ESelectionState)(int)state, this.colors.selectedColor, this.colors.fadeDuration); break;
#endif


                case SelectionState.Disabled: OnStateTransition_Color?.Invoke((ESelectionState)(int)state, this.colors.disabledColor, this.colors.fadeDuration); break;
            }
        }

        /* protected - [abstract & virtual]         */

        // ========================================================================== //

        #region Private

        #endregion Private
    }

#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ButtonExtension), true)]
    public class ButtonExtensionInspector : ButtonEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }

#endif
}