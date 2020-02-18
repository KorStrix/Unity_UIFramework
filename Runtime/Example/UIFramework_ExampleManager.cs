#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-29
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using UIWidgetContainerManager_Logic;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class UIFramework_ExampleManager : CanvasManager<UIFramework_ExampleManager, UIFramework_ExampleManager.ECanvas>
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum ECanvas
        {
            Canvas_InventoryExample,
            Canvas_PopupTextExample,
        }

        /* public - Field declaration               */


        /* protected & private - Field declaration  */

        Dictionary<ECanvas, UIFramework_ExampleCanvasBase> _mapExampleCanvas = new Dictionary<ECanvas, UIFramework_ExampleCanvasBase>();
        Canvas _pCanvas;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            UIFramework_ExampleCanvasBase[] arrCanvas = GetComponentsInChildren<UIFramework_ExampleCanvasBase>();
            for (int i = 0; i < arrCanvas.Length; i++)
                arrCanvas[i].gameObject.SetActive(false);

            _pCanvas = GetComponent<Canvas>();
        }

        protected override void OnInit_ManagerLogic(Dictionary<EUIObjectState, List<ICanvasManager_Logic>> mapManagerLogic)
        {
        }

        protected override IEnumerator OnCreate_Instance(ECanvas eName, Action<ICanvas> OnFinish)
        {
            yield break;
        }

        public override Canvas GetParentCavnas(ECanvas eName, ICanvas pCanvas)
        {
            return _pCanvas;
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}