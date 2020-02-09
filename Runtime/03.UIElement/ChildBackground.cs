#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-02-09 오후 10:02:48
 *	개요 : 
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
    [ExecuteInEditMode]
    public class ChildBackground : MonoBehaviour
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        [System.Serializable]
        public class TextMargin
        {
            public float Left;
            public float Right;
            public float Top;
            public float Bottom;
        }


        /* public - Field declaration            */

        public RectTransform pTransformChild;
        public TextMargin pTextMargin;
        
        /* protected & private - Field declaration         */

        RectTransform _pTransformMe;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/


        // ========================================================================== //

        /* protected - Override & Unity API         */

        private void Awake()
        {
            _pTransformMe = GetComponent<RectTransform>();
            InitChildTransform();
        }

        private void Update()
        {
            if (pTransformChild == null)
                return;

            Vector2 vecSizeDelta = pTransformChild.sizeDelta;
            vecSizeDelta.x += pTextMargin.Left + pTextMargin.Right;
            vecSizeDelta.y += pTextMargin.Top + pTextMargin.Bottom;

            _pTransformMe.sizeDelta = vecSizeDelta;
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        void InitChildTransform()
        {
            if (pTransformChild != null)
                return;

            if (transform.childCount > 0)
                pTransformChild = transform.GetChild(0).GetComponent<RectTransform>();
        }

        #endregion Private
    }
}