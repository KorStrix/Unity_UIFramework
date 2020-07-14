#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-06-19
 *	Summary 		        : 
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    [ExecuteInEditMode]
    public class UIEventTrigger : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        [Serializable]
        public class TriggerInfo
        {
            public UIElementInfo pElementInfo;

            [Header("딜레이(초)")]
            public float fDelaySec;
            [Header("이벤트 리스트")] 
            public UnityEvent OnEvent;
        }

        /* public - Field declaration               */

        public List<TriggerInfo> listTriggerInfo;

        /* protected & private - Field declaration  */


        // ========================================================================== //

        /* public - [Do~Something] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        private void Awake()
        {
            int iIndex = -1;
            foreach (TriggerInfo pTriggerInfo in listTriggerInfo)
            {
                iIndex++;
                if (string.IsNullOrEmpty(pTriggerInfo.pElementInfo.strEnumName))
                {
                    Debug.Log($"{name} - string.IsNullOrEmpty(pTriggerInfo.pElementInfo.strEnumName) iIndex : {iIndex}");
                    continue;
                }

                // System.Type pType = pTriggerInfo.pElementInfo.pInterface_HasUIElementType;
                //switch (pType.Name)
                //{
                //    case typeof(IHas_UIButton<>).Name:

                //        break;

                //    case typeof(IHas_UIToggle<>).Name:

                //        break;

                //}
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}
