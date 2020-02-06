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
    public class PopupText : UIWidgetObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EEventWhen
        {
            Show,
            Hide,
        }

        [System.Serializable]
        public class PopupupTextLogic
        {
            public EEventWhen eEventWhen;
            public PopupText_Logic_ComponentBase pPopupTextLogic;
        }

        /* public - Field declaration               */

        public Text pText;

        public List<PopupupTextLogic> listLogic_Insert_OnAwake = new List<PopupupTextLogic>();

        /* protected & private - Field declaration  */

        List<IPopupText_Logic> _listPopupTextLogic_OnShow = new List<IPopupText_Logic>();
        List<IPopupText_Logic> _listPopupTextLogic_OnHide = new List<IPopupText_Logic>();
        List<Coroutine> _listLogicCoroutine = new List<Coroutine>();
        Coroutine _pCoroutine;


        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoShow(string strText, Vector3 vecWorldPos)
        {
            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);

            if (_pCoroutine != null)
                StopCoroutine(_pCoroutine);
            _pCoroutine = StartCoroutine(OnPopupTextAnimation_Coroutine(strText, vecWorldPos));
        }

        public void DoShow(Canvas pCanvasParents, string strText, Vector3 vecWorldPos)
        {
            transform.SetParent(pCanvasParents.transform);

            DoShow(strText, vecWorldPos);
        }

        public void DoAddLogic(EEventWhen eEventWhen, IPopupText_Logic pPopupText_Logic)
        {
            switch (eEventWhen)
            {
                case EEventWhen.Show: _listPopupTextLogic_OnShow.Add(pPopupText_Logic); break;
                case EEventWhen.Hide: _listPopupTextLogic_OnHide.Add(pPopupText_Logic); break;
            }
        }

        public void DoClearLogic(EEventWhen eEventWhen)
        {
            switch (eEventWhen)
            {
                case EEventWhen.Show: _listPopupTextLogic_OnShow.Clear(); break;
                case EEventWhen.Hide: _listPopupTextLogic_OnHide.Clear(); break;
            }
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            if(pText == null)
                pText = GetComponent<Text>();

            for(int i = 0; i < listLogic_Insert_OnAwake.Count; i++)
                DoAddLogic(listLogic_Insert_OnAwake[i].eEventWhen, listLogic_Insert_OnAwake[i].pPopupTextLogic);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator OnPopupTextAnimation_Coroutine(string strText, Vector3 vecWorldPos)
        {
            pText.text = strText;
            transform.position = vecWorldPos;

            _listLogicCoroutine.Clear();
            for (int i = 0; i < _listPopupTextLogic_OnShow.Count; i++)
                _listLogicCoroutine.Add(StartCoroutine(_listPopupTextLogic_OnShow[i].OnAnimation(this, strText)));

            yield return _listLogicCoroutine.GetEnumerator_Safe();

            _listLogicCoroutine.Clear();
            for (int i = 0; i < _listPopupTextLogic_OnHide.Count; i++)
                _listLogicCoroutine.Add(StartCoroutine(_listPopupTextLogic_OnHide[i].OnAnimation(this, strText)));

            yield return _listLogicCoroutine.GetEnumerator_Safe();

            gameObject.SetActive(false);
            yield break;
        }

        #endregion Private
    }
}