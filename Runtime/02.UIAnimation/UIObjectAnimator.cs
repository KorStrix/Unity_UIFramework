#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-18 오후 5:20:18
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    /// <summary>
    /// UI Widget용 애니메이션 실행기
    /// </summary>
    public class UIObjectAnimator : MonoBehaviour, IUIWidget_Managed
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EAnimationPlayHow
        {
            /// <summary>
            /// 순차적으로 하나씩
            /// </summary>
            Sequential,

            /// <summary>
            /// 동시에
            /// </summary>
            Concurrently
        }

        /* public - Field declaration            */

        public IUIManager pUIManager { get; set; }
        public UIAnimation pAnimation_Show { get; private set; } = new UIAnimation();
        public UIAnimation pAnimation_Hide { get; private set; } = new UIAnimation();


        public EAnimationPlayHow eAnimationPlayHow = EAnimationPlayHow.Sequential;
        public bool bIsPlay_ShowAnimation_OnEnable = true;
        public bool bIgnoreTimeScale = false;

        public List<UIAnimationLogic_ComponentBase> listAnimationLogic_Show = new List<UIAnimationLogic_ComponentBase>();
        public List<UIAnimationLogic_ComponentBase> listAnimationLogic_Hide = new List<UIAnimationLogic_ComponentBase>();


        /* protected & private - Field declaration         */

        bool _bIsExecute_Awake = false;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoClear_AnimationLogic()
        {
            pAnimation_Show.DoClear_AnimationLogic();
            pAnimation_Hide.DoClear_AnimationLogic();
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        public void Awake()
        {
            if (_bIsExecute_Awake)
                return;
            _bIsExecute_Awake = true;

            pAnimation_Show.DoInit(this, StartCoroutine, StopCoroutine);

            if (listAnimationLogic_Show.Count != 0)
                pAnimation_Show.DoAdd_Animation_Collection(listAnimationLogic_Show);
            else
                pAnimation_Show.DoAdd_Animation_Collection(GetComponents<IUIAnimationLogic_Show>());

            pAnimation_Hide.DoInit(this, StartCoroutine, StopCoroutine);

            if (listAnimationLogic_Hide.Count != 0)
                pAnimation_Hide.DoAdd_Animation_Collection(listAnimationLogic_Hide);
            else
                pAnimation_Hide.DoAdd_Animation_Collection(GetComponents<IUIAnimationLogic_Hide>());
        }

        private void OnEnable()
        {
            if (bIsPlay_ShowAnimation_OnEnable)
                this.DoShow();
        }

        public IEnumerator OnShowCoroutine()
        {
            if (gameObject.activeSelf == false)
                yield break;

            switch (eAnimationPlayHow)
            {
                case EAnimationPlayHow.Sequential: yield return pAnimation_Show.CoPlayAnimation_Sequential(bIgnoreTimeScale); break;
                case EAnimationPlayHow.Concurrently: yield return pAnimation_Show.CoPlayAnimation_Concurrently(bIgnoreTimeScale); break;

                default:
                    yield break;
            }
        }

        public IEnumerator OnHideCoroutine()
        {
            if (gameObject.activeSelf == false)
                yield break;

            switch (eAnimationPlayHow)
            {
                case EAnimationPlayHow.Sequential: yield return pAnimation_Hide.CoPlayAnimation_Sequential(bIgnoreTimeScale); break;
                case EAnimationPlayHow.Concurrently: yield return pAnimation_Hide.CoPlayAnimation_Concurrently(bIgnoreTimeScale); break;

                default:
                    yield break;
            }
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private

    }
}