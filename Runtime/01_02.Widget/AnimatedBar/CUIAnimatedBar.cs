#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-18 오전 10:58:03
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIFramework.AnimatedBarLogic;

namespace UIFramework
{
    /// <summary>
    /// 체력바, 경험치바 등에 사용하는 애니메이션 바.
    /// <para>주의) 이 객체는 바로 사용할 수 없습니다.</para>
    /// <para><see cref="DoInit"/>를 통해 로직을 Add하여 사용해야 합니다.</para>
    /// </summary>
    public class CUIAnimatedBar : UIWidgetObjectBase
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EDirection
        {
            None,

            /// <summary>
            /// 증가했을 때
            /// </summary>
            Increase = 1 << 0,

            /// <summary>
            /// 감소했을 때
            /// </summary>
            Decrease = 1 << 1,

            /// <summary>
            /// 증가 및 감소 둘다 해당됐을 때
            /// </summary>
            Both = Increase + Decrease,
        }

        /* public - Field declaration            */

        public delegate void del_OnChange_FillAmount(float fFillAmount_0_1_Before, float fFillAmount_0_1_After, EDirection eDirection);

        /// <summary>
        /// 이 Bar의 FillAmount가 변경될 때
        /// </summary>
        public event del_OnChange_FillAmount OnChange_FillAmount;

        public IUIManager pUIManager { get; set; }

        [SerializeField]
        Image _pImage_Fill = null;

        /* protected & private - Field declaration         */

        List<IAnimatedBarLogic> _listLogic_OnIncrease = new List<IAnimatedBarLogic>();
        List<IAnimatedBarLogic> _listLogic_OnDecrease = new List<IAnimatedBarLogic>();

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        /// <summary>
        /// 동작할 Logic을 Add합니다. 로직은 namespace <see cref="IAnimatedBarLogic"/>를 참고바랍니다.
        /// </summary>
        public void DoInit(AnimatedBarLogicFactory pLogicFactory)
        {
            _listLogic_OnIncrease.Clear();
            _listLogic_OnDecrease.Clear();

            foreach (var pLogicPair in pLogicFactory.mapLogicContainer)
            {
                switch (pLogicPair.Key)
                {
                    case EDirection.Increase: _listLogic_OnIncrease.AddRange(pLogicPair.Value); break;
                    case EDirection.Decrease: _listLogic_OnDecrease.AddRange(pLogicPair.Value); break;

                    case EDirection.Both:
                        _listLogic_OnIncrease.AddRange(pLogicPair.Value);
                        _listLogic_OnDecrease.AddRange(pLogicPair.Value);
                        break;
                }
            }
        }

        /// <summary>
        /// Fill Image를 세팅합니다.
        /// </summary>
        /// <param name="pImage_Fill"></param>
        public void DoSet_FillImage(Image pImage_Fill)
        {
            this._pImage_Fill = pImage_Fill;
        }

        /// <summary>
        /// Fill Amount를 애니메이션 없이 세팅합니다.
        /// </summary>
        public void DoSet_BarFill(float fFill_0_1)
        {
            _pImage_Fill.fillAmount = fFill_0_1;
        }

        /// <summary>
        /// Fill Amount를 세팅과 동시에 등록된 효과를 실행합니다.
        /// </summary>
        public void DoSet_BarFill_And_PlayAnimation(float fFill_0_1)
        {
            if (_pImage_Fill.fillAmount < fFill_0_1)
                Set_FillAmount_OnIncrease(fFill_0_1);
            else if (_pImage_Fill.fillAmount > fFill_0_1)
                Set_FillAmount_OnDecrease(fFill_0_1);
        }

        /// <summary>
        /// 이 Bar의 FillAmount가 변경될 때 이벤트를 모두 지웁니다.
        /// </summary>
        public void Do_AllClear_Listener_OnChange_FillAmount()
        {
            OnChange_FillAmount = null;
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        void Update()
        {
            float fDeltaTime = Time.deltaTime;
            for (int i = 0; i < _listLogic_OnIncrease.Count; i++)
                _listLogic_OnIncrease[i].IAnimatedBarLogic_OnUpdate(fDeltaTime);

            for (int i = 0; i < _listLogic_OnDecrease.Count; i++)
                _listLogic_OnDecrease[i].IAnimatedBarLogic_OnUpdate(fDeltaTime);
        }


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        void Set_FillAmount_OnIncrease(float fFillAmount_0_1)
        {
            float fBeforeFill = _pImage_Fill.fillAmount;
            _pImage_Fill.fillAmount = fFillAmount_0_1;

            for (int i = 0; i < _listLogic_OnIncrease.Count; i++)
                _listLogic_OnIncrease[i].IAnimatedBarLogic_OnStartAnimation(fBeforeFill, fFillAmount_0_1, EDirection.Increase);

            OnChange_FillAmount?.Invoke(fBeforeFill, fFillAmount_0_1, EDirection.Increase);
        }

        void Set_FillAmount_OnDecrease(float fFillAmount_0_1)
        {
            float fBeforeFill = _pImage_Fill.fillAmount;
            _pImage_Fill.fillAmount = fFillAmount_0_1;

            for (int i = 0; i < _listLogic_OnDecrease.Count; i++)
                _listLogic_OnDecrease[i].IAnimatedBarLogic_OnStartAnimation(fBeforeFill, fFillAmount_0_1, EDirection.Decrease);

            OnChange_FillAmount?.Invoke(fBeforeFill, fFillAmount_0_1, EDirection.Decrease);
        }

        #endregion Private
    }
}