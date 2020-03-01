#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-03-01
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace UIFramework
{
	/// <summary>
	/// 
	/// </summary>
	public class ButtonStateListener : Selectable
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		/* public - Field declaration               */

		public ButtonExtension pListenTarget;

		/* protected & private - Field declaration  */

		bool _bUseTransition = false;

		// ========================================================================== //

		/* public - [Do~Somthing] Function 	        */


		// ========================================================================== //

		/* protected - [Override & Unity API]       */

		protected override void Awake()
		{
			base.Awake();

			pListenTarget.OnStateTransition_Color += OnStateTransition_Color;
			interactable = false;
		}

		protected override void DoStateTransition(SelectionState state, bool instant)
		{
			if(_bUseTransition)
			{
				_bUseTransition = false;
				base.DoStateTransition(state, instant);
			}
		}

		/* protected - [abstract & virtual]         */


		// ========================================================================== //

		#region Private

		private void OnStateTransition_Color(ButtonExtension.ESelectionState state, Color sColorBase, float fDuration)
		{
			_bUseTransition = true;
			DoStateTransition((SelectionState)(int)state, false);
		}

		#endregion Private
	}


#if UNITY_EDITOR

	[CanEditMultipleObjects]
	[CustomEditor(typeof(ButtonStateListener), true)]
	public class ButtonStateListenerInspector : SelectableEditor
	{
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();

			EditorGUILayout.PropertyField(serializedObject.FindProperty("pListenTarget"), true);
			serializedObject.ApplyModifiedProperties();
		}
	}

#endif
}