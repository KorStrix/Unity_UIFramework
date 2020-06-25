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
    public class UIObjectAnimatorComponent_UnManaged : UIWidgetObjectBase
    {
        /* const & readonly declaration             */
        
        readonly bool bIsDebug = false;

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
        public bool bIgnoreTimeScale = false;

        public List<UIAnimationLogic_ComponentBase> listAnimationLogic_Show = new List<UIAnimationLogic_ComponentBase>();
        public List<UIAnimationLogic_ComponentBase> listAnimationLogic_Hide = new List<UIAnimationLogic_ComponentBase>();


        /* protected & private - Field declaration         */


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

        protected override void OnAwake()
        {
            base.OnAwake();

            UIAnimationLogicFactory pLogicFactory_Show = new UIAnimationLogicFactory();
            UIAnimationLogicFactory pLogicFactory_Hide = new UIAnimationLogicFactory();

            // 로직이 이미 있으면 그 로직을 전부 사용하고,
            // 로직이 하나도 없으면 GetComponents를 이용해서 얻어오는데,
            // 리펙토링이 필요하다..
            if (listAnimationLogic_Show.Count != 0)
                pLogicFactory_Show.DoAdd_Animation_Collection(listAnimationLogic_Show);
            else
                pLogicFactory_Show.DoAdd_Animation_Collection(GetComponents<IUIAnimationLogic_Show>());

            if (listAnimationLogic_Hide.Count != 0)
                pLogicFactory_Hide.DoAdd_Animation_Collection(listAnimationLogic_Hide);
            else
                pLogicFactory_Hide.DoAdd_Animation_Collection(GetComponents<IUIAnimationLogic_Hide>());

            pAnimation_Show.DoInit(this, StartCoroutine, StopCoroutine);
            pAnimation_Show.DoInit_Animation(pLogicFactory_Show);

            pAnimation_Hide.DoInit(this, StartCoroutine, StopCoroutine);
            pAnimation_Hide.DoInit_Animation(pLogicFactory_Hide);
        }


        //IEnumerator OnEnableCoroutine()
        //{
        //    yield return null;

        //    if (bIsPlay_ShowAnimation_OnEnable)
        //        this.DoShowCoroutine();
        //}

        public override void IUIWidget_OnBeforeShow()
        {
            base.IUIWidget_OnBeforeShow();

            pAnimation_Show.Event_OnBeforeShow();
        }

        public override IEnumerator OnShowCoroutine()
        {
            if (bIsDebug)
                Debug.Log($"{name} - {nameof(OnShowCoroutine)} Start - eAnimationPlayHow : {eAnimationPlayHow}", this);


            if (gameObject.activeSelf == false)
                yield break;

            switch (eAnimationPlayHow)
            {
                case EAnimationPlayHow.Sequential: yield return pAnimation_Show.PlayAnimationCoroutine_Sequential(bIgnoreTimeScale); break;
                case EAnimationPlayHow.Concurrently: yield return pAnimation_Show.PlayAnimationCoroutine_Concurrently(bIgnoreTimeScale); break;

                default:
                    yield break;
            }


            if (bIsDebug)
                Debug.Log($"{name} - {nameof(OnShowCoroutine)} Finish - eAnimationPlayHow : {eAnimationPlayHow}", this);
        }

        public override IEnumerator OnHideCoroutine()
        {
            if (bIsDebug)
                Debug.Log($"{name} - {nameof(OnHideCoroutine)} Start - eAnimationPlayHow : {eAnimationPlayHow}", this);


            if (gameObject.activeSelf == false)
                yield break;

            switch (eAnimationPlayHow)
            {
                case EAnimationPlayHow.Sequential: yield return pAnimation_Hide.PlayAnimationCoroutine_Sequential(bIgnoreTimeScale); break;
                case EAnimationPlayHow.Concurrently: yield return pAnimation_Hide.PlayAnimationCoroutine_Concurrently(bIgnoreTimeScale); break;

                default:
                    yield break;
            }


            if (bIsDebug)
                Debug.Log($"{name} - {nameof(OnHideCoroutine)} Finish - eAnimationPlayHow : {eAnimationPlayHow}", this);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private

    }
}