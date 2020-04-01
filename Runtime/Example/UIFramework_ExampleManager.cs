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
using System.Linq;

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

        public List<ECanvas> listShowCanvas = new List<ECanvas>();

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

            _pCanvas = GetComponent<Canvas>();

            UIFramework_ExampleCanvasBase[] arrCanvas = GetComponentsInChildren<UIFramework_ExampleCanvasBase>();
            _mapExampleCanvas = arrCanvas.ToDictionary(p => (ECanvas)System.Enum.Parse(typeof(ECanvas), p.name));

            for (int i = 0; i < arrCanvas.Length; i++)
                arrCanvas[i].gameObject.SetActive(false);

            for (int i = 0; i < listShowCanvas.Count; i++)
                DoShowOnly(listShowCanvas[i]);
        }

        protected override void OnInit_ManagerLogic(CanvasManagerLogicFactory pLogicFactory)
        {
        }

        protected override IEnumerator OnCreate_Instance(ECanvas eName, bool bIsMultiple, Action<ICanvas> OnFinish)
        {
            OnFinish(_mapExampleCanvas[eName]);

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