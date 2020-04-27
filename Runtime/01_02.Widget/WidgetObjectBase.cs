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
    /// <see cref="IUIWidget"/>를 상속받은 <see cref="MonoBehaviour"/> 기반의 Base Class
    /// </summary>
    public class UIWidgetObjectBase : MonoBehaviour, IUIWidget
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public bool bIsExecute_Awake { get; protected set; }

        /* protected & private - Field declaration  */
        protected bool _bIsQuit_Application { get; private set; }

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void EventAwake()
        {
            Awake();
        }

        public void EventAwake_Force()
        {
            bIsExecute_Awake = false;
            Awake();
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected void Awake()
        {
            if (bIsExecute_Awake)
                return;
            bIsExecute_Awake = true;

            OnAwake();
        }

        private void OnApplicationQuit()
        {
            _bIsQuit_Application = true;
        }

        protected virtual void OnDisableObject(bool bIsQuit_Application) { }

        public virtual IEnumerator OnShowCoroutine()
        {
            yield break;
        }

        public virtual IEnumerator OnHideCoroutine()
        {
            yield break;
        }

        /* protected - [abstract & virtual]         */

        protected virtual void OnAwake() { }

        public virtual void IUIWidget_OnBeforeShow()
        {
        }

        // ========================================================================== //

        #region Private

        #endregion Private
    }
}