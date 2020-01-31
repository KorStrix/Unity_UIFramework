#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-31
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class PopupText : WidgetObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */


        /* protected & private - Field declaration  */

        List<IPopupText_Logic> listTextLogic = new List<IPopupText_Logic>();
        List<Coroutine> _listLogicCoroutine = new List<Coroutine>();
        Coroutine _pCoroutine_Show;

        Text _pText;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoShow(string strText, Vector3 vecWorldPos)
        {
            if (_pCoroutine_Show != null)
                StopCoroutine(_pCoroutine_Show);
            _pCoroutine_Show = StartCoroutine(OnShow_Coroutine(strText, vecWorldPos));
        }

        public void DoAddLogic(IPopupText_Logic pPopupText_Logic)
        {
            listTextLogic.Add(pPopupText_Logic);
        }

        public void DoClearLogic()
        {
            listTextLogic.Clear();
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pText = GetComponent<Text>();
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator OnShow_Coroutine(string strText, Vector3 vecWorldPos)
        {
            _pText.text = strText;
            transform.position = vecWorldPos;

            _listLogicCoroutine.Clear();
            for (int i = 0; i < listTextLogic.Count; i++)
                _listLogicCoroutine.Add(StartCoroutine(listTextLogic[i].OnShowText(this)));

            yield return _listLogicCoroutine.GetEnumerator_Safe();

            gameObject.SetActive(false);
            yield break;
        }

        #endregion Private
    }
}