#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-01-31
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UIFramework.PopupText_Logic;

namespace UIFramework
{
    /// <summary>
    /// 인게임 데미지 표기 등에 사용하는 팝업 텍스트.
    /// <para>주의) 이 객체는 바로 사용할 수 없습니다.</para>
    /// <para><see cref="DoAddLogic"/>를 통해 로직을 Add하여 사용해야 합니다.</para>
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

        [Header("팝업 텍스트가 조작하는 TextWrapper 컴포넌트")]
        public ITextWrapperComponent pTextWrapper;

        [Header("로직을 다 끝낸 뒤 사라질때까지 기다리는 시간")]
        public float fWaitDisableSec = 0f;

        [Space(10)]
        public List<PopupupTextLogic> listLogic_Insert_OnAwake = new List<PopupupTextLogic>();

        /* protected & private - Field declaration  */

        List<IPopupText_Logic> _listPopupTextLogic_OnShow = new List<IPopupText_Logic>();
        List<IPopupText_Logic> _listPopupTextLogic_OnHide = new List<IPopupText_Logic>();
        List<Coroutine> _listLogicCoroutine = new List<Coroutine>();
        Coroutine _pCoroutine;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        /// <summary>
        /// 이 팝업 Text를 출력합니다. 출력과 동시에 등록된 <see cref="IPopupText_Logic"/>을 실행합니다.
        /// </summary>
        public void DoShow(string strText)
        {
            DoShow(strText, transform.position);
        }


        /// <summary>
        /// 이 팝업 Text를 출력합니다. 출력과 동시에 등록된 <see cref="IPopupText_Logic"/>을 실행합니다.
        /// </summary>
        public void DoShow(string strText, Vector3 vecWorldPos)
        {
            if (gameObject.activeSelf == false)
                gameObject.SetActive(true);

            if (_pCoroutine != null)
                StopCoroutine(_pCoroutine);
            _pCoroutine = StartCoroutine(OnPopupTextAnimation_Coroutine(strText, vecWorldPos));
        }

        /// <summary>
        /// 이 팝업 Text를 출력합니다. 출력과 동시에 등록된 <see cref="IPopupText_Logic"/>을 실행합니다.
        /// </summary>
        public void DoShow(Canvas pCanvasParents, string strText, Vector3 vecWorldPos)
        {
            if(pCanvasParents != null)
                transform.SetParent(pCanvasParents.transform);

            DoShow(strText, vecWorldPos);
        }

        /// <summary>
        /// 동작할 Logic을 Add합니다. 로직은 namespace UIFramework.AnimatedBarLogic를 참고바랍니다.
        /// </summary>
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

            if(pTextWrapper == null)
                pTextWrapper = GetComponentInChildren<ITextWrapperComponent>();

            for(int i = 0; i < listLogic_Insert_OnAwake.Count; i++)
                DoAddLogic(listLogic_Insert_OnAwake[i].eEventWhen, listLogic_Insert_OnAwake[i].pPopupTextLogic);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        IEnumerator OnPopupTextAnimation_Coroutine(string strText, Vector3 vecWorldPos)
        {
            pTextWrapper.strText = strText;
            transform.position = vecWorldPos;

            _listLogicCoroutine.Clear();
            for (int i = 0; i < _listPopupTextLogic_OnShow.Count; i++)
                _listLogicCoroutine.Add(StartCoroutine(_listPopupTextLogic_OnShow[i].OnAnimation(this, strText)));

            yield return _listLogicCoroutine.GetEnumerator_Safe();

            _listLogicCoroutine.Clear();
            for (int i = 0; i < _listPopupTextLogic_OnHide.Count; i++)
                _listLogicCoroutine.Add(StartCoroutine(_listPopupTextLogic_OnHide[i].OnAnimation(this, strText)));

            yield return _listLogicCoroutine.GetEnumerator_Safe();

            yield return new WaitForSeconds(fWaitDisableSec);

            gameObject.SetActive(false);
            yield break;
        }

        #endregion Private
    }
}