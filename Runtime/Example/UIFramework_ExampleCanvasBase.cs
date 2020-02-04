#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-30
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class UIFramework_ExampleCanvasBase : MonoBehaviour, ICanvas
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public IUIManager pUIManager { get; set; }

        /* protected & private - Field declaration  */

        bool _bExecute_Awake = false;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        private void Awake()
        {
            if (_bExecute_Awake)
                return;
            _bExecute_Awake = true;

            SCUIElementEventHelper.DoInit_HasUIElement(this);
            OnAwake();
        }

        virtual public IEnumerator OnShowCoroutine()
        {
            yield break;
        }

        virtual public IEnumerator OnHideCoroutine()
        {
            yield break;
        }

        /* protected - [abstract & virtual]         */

        virtual protected void OnAwake() { }

        // ========================================================================== //

        #region Private

        #endregion Private
    }
}