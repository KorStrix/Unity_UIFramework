#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-17 오전 11:23:33
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UIFramework
{
    public class UIAnimationLogicFactory
    { 
        public List<IUIAnimationLogic> listUIAnimationLogic { get; private set; } = new List<IUIAnimationLogic>();

        public void DoAdd_Animation_Collection<T>(IEnumerable<T> listAnimationLogic)
            where T : IUIAnimationLogic
        {
            if (listAnimationLogic == null)
                return;

            foreach (T pLogic in listAnimationLogic)
            {
                if (pLogic == null)
                    continue;

                listUIAnimationLogic.Add(pLogic);
            }
        }
    }


    /// <summary>
    /// 범용적인 UIAnimation 로직의 Base Interface
    /// <para><see cref="UIAnimation"/>에의해 관리되며 실행됩니다.</para>
    /// </summary>
    public interface IUIAnimationLogic
    {
        void IUIAnimationLogic_OnAwake(IUIObject pUIObject);
        void IUIAnimationLogic_OnBeforeShow(IUIObject pUIObject);
        IEnumerator IUIAnimationLogic_OnAnimation(IUIObject pUIObject, bool bIgnoreTimeScale);
        void IUIAnimationLogic_OnDestroy(IUIObject pUIObject);

    }

    /// <summary>
    /// UI Animation - Show 전용
    /// </summary>
    public interface IUIAnimationLogic_Show : IUIAnimationLogic { }

    /// <summary>
    /// UI Animation - Hide 전용
    /// </summary>
    public interface IUIAnimationLogic_Hide : IUIAnimationLogic { }

    [System.Serializable]
    public abstract class UIAnimationLogic_ComponentBase : MonoBehaviour, IUIAnimationLogic
    {
        public abstract void IUIAnimationLogic_OnAwake(IUIObject pUIObject);
        public abstract void IUIAnimationLogic_OnBeforeShow(IUIObject pUIObject);
        public abstract IEnumerator IUIAnimationLogic_OnAnimation(IUIObject pUIObject, bool bIgnoreTimeScale);
        public virtual void IUIAnimationLogic_OnDestroy(IUIObject pUIObject) { }

    }

    /// <summary>
    /// <see cref="IUIAnimationLogic"/> 컨테이너
    /// </summary>
    [System.Serializable]
    public class UIAnimation
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration            */

        public List<IUIAnimationLogic> listUIAnimationLogic { get; private set; } = new List<IUIAnimationLogic>();

        /* protected & private - Field declaration         */

        List<Coroutine> _listExecuteCoroutine = new List<Coroutine>();

        System.Func<IEnumerator, Coroutine> _OnStartCoroutine;
        System.Action<Coroutine> _OnStopCoroutine;

        IUIObject _pUIObject;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoInit(IUIObject pUIObject, System.Func<IEnumerator, Coroutine> OnStartCoroutine, System.Action<Coroutine> OnStopCoroutine, bool bClearLogicList = true)
        {
            _pUIObject = pUIObject;
            _OnStartCoroutine = OnStartCoroutine;
            _OnStopCoroutine = OnStopCoroutine;

            if (bClearLogicList)
                listUIAnimationLogic.Clear();
        }

        public void DoInit_Animation(UIAnimationLogicFactory pLogicFactory)
        {
            listUIAnimationLogic.AddRange(pLogicFactory.listUIAnimationLogic);
            pLogicFactory.listUIAnimationLogic.ForEach(p => p.IUIAnimationLogic_OnAwake(_pUIObject));
        }

        public void DoAdd_Animation(IUIAnimationLogic pLogic)
        {
            listUIAnimationLogic.Add(pLogic);
            pLogic.IUIAnimationLogic_OnAwake(_pUIObject);
        }

        public void DoClear_AnimationLogic()
        {
            listUIAnimationLogic.ForEach(p => p.IUIAnimationLogic_OnDestroy(_pUIObject));
            listUIAnimationLogic.Clear();
        }

        public void Event_OnBeforeShow()
        {
            listUIAnimationLogic.ForEach(p => p.IUIAnimationLogic_OnBeforeShow(_pUIObject));
        }


        /// <summary>
        /// 애니메이션들을 동시에 재생
        /// </summary>
        public IEnumerator PlayAnimationCoroutine_Concurrently(bool bIgnoreTimeScale)
        {
            for (int i = 0; i < _listExecuteCoroutine.Count; i++)
                _OnStopCoroutine(_listExecuteCoroutine[i]);
            _listExecuteCoroutine.Clear();

            for (int i = 0; i < listUIAnimationLogic.Count; i++)
                _listExecuteCoroutine.Add(_OnStartCoroutine(listUIAnimationLogic[i].IUIAnimationLogic_OnAnimation(_pUIObject, bIgnoreTimeScale)));
            yield return _listExecuteCoroutine.GetEnumerator_Safe();
        }

        /// <summary>
        /// 애니메이션을 하나씩 순차적으로 재생
        /// </summary>
        public IEnumerator PlayAnimationCoroutine_Sequential(bool bIgnoreTimeScale)
        {
            for (int i = 0; i < _listExecuteCoroutine.Count; i++)
                _OnStopCoroutine(_listExecuteCoroutine[i]);
            _listExecuteCoroutine.Clear();

            for (int i = 0; i < listUIAnimationLogic.Count; i++)
            {
                if (listUIAnimationLogic[i] != null)
                    yield return listUIAnimationLogic[i].IUIAnimationLogic_OnAnimation(_pUIObject, bIgnoreTimeScale);
            }
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}
