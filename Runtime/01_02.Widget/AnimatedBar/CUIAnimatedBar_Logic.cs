#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-18 오후 12:59:10
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

using static UIFramework.CUIAnimatedBar;
using UIFramework.AnimatedBarLogic;

namespace UIFramework
{
    public enum EAnimatedBarLogicName
    {
        /// <summary>
        /// 바가 깜빡이는 로직
        /// </summary>
        AnimatedBarLogic_Blink_Image,

        /// <summary>
        /// 증가, 감소를 스무스하게 연출해주는 로직
        /// </summary>
        AnimatedBarLogic_Shirink,
    }

    public class AnimatedBarLogicFactory
    {
        public Dictionary<EDirection, List<IAnimatedBarLogic>> mapLogicContainer = new Dictionary<EDirection, List<IAnimatedBarLogic>>();

        public IAnimatedBarLogic DoCreate_LibraryLogic(EDirection eDirectionWhen, EAnimatedBarLogicName eLogic, Image pTargetImage)
        {
            IAnimatedBarLogic pLogic = null;
            switch (eLogic)
            {
                case EAnimatedBarLogicName.AnimatedBarLogic_Blink_Image: pLogic = new AnimatedBarLogic_Blink_Image(); break;
                case EAnimatedBarLogicName.AnimatedBarLogic_Shirink: pLogic = new AnimatedBarLogic_Shirink(); break;

                default: Debug.LogError("Error - Not Found Logic"); return null;
            }
            pLogic.IAnimatedBarLogic_OnAwake(pTargetImage);

            if (mapLogicContainer.ContainsKey(eDirectionWhen) == false)
                mapLogicContainer.Add(eDirectionWhen, new List<IAnimatedBarLogic>());
            mapLogicContainer[eDirectionWhen].Add(pLogic);

            return pLogic;
        }
    }


    namespace AnimatedBarLogic
    {
        /// <summary>
        /// <see cref="CUIAnimatedBar"/>의 애니메이션 로직.
        /// </summary>
        public interface IAnimatedBarLogic
        {
            void IAnimatedBarLogic_OnAwake(Image pImage);
            void IAnimatedBarLogic_OnStartAnimation(float fFillAmount_0_1_Before, float fFillAmount_0_1_After, CUIAnimatedBar.EDirection eDirection);
            void IAnimatedBarLogic_OnUpdate(float fDeltaTime);
        }


        /// <summary>
        /// 타겟 이미지가 붉은색으로 깜빡입니다.
        /// </summary>
        [System.Serializable]
        public class AnimatedBarLogic_Blink_Image : IAnimatedBarLogic
        {
            public Color pAnimateColor = Color.red;
            public float fDuration = 1f;

            Image _pImage;
            Color _pCurrentColor;
            float _fRemainTime;

            public void IAnimatedBarLogic_OnAwake(Image pImage)
            {
                _pImage = pImage;
                _pImage.gameObject.SetActive(false);
            }

            public void IAnimatedBarLogic_OnStartAnimation(float fFillAmount_0_1_Before, float fFillAmount_0_1_After, CUIAnimatedBar.EDirection eDirection)
            {
                if (eDirection == CUIAnimatedBar.EDirection.Increase)
                    return;

                if (_fRemainTime <= 0f)
                    _pImage.fillAmount = fFillAmount_0_1_Before;

                _fRemainTime = fDuration;

                _pCurrentColor = pAnimateColor;

                _pImage.gameObject.SetActive(true);
                _pImage.color = _pCurrentColor;
            }

            public void IAnimatedBarLogic_OnUpdate(float fDeltaTime)
            {
                if (_fRemainTime <= 0f)
                    return;
                _fRemainTime -= fDeltaTime;

                if (_fRemainTime > 0f)
                {
                    _pCurrentColor.a -= fDeltaTime / _fRemainTime;
                    _pImage.color = _pCurrentColor;
                }
                else
                {
                    _pImage.gameObject.SetActive(false);
                }
            }
        }

        [System.Serializable]
        public class AnimatedBarLogic_Shirink : IAnimatedBarLogic
        {
            public float fDuration = 1f;

            Image _pImage;
            float _fOffsetAmount;
            float _fRemainTime;

            public void IAnimatedBarLogic_OnAwake(Image pImage)
            {
                _pImage = pImage;
            }

            public void IAnimatedBarLogic_OnStartAnimation(float fFillAmount_0_1_Before, float fFillAmount_0_1_After, CUIAnimatedBar.EDirection eDirection)
            {
                _fRemainTime = fDuration;
                _fOffsetAmount = fFillAmount_0_1_After - fFillAmount_0_1_Before;

                _pImage.enabled = true;
                _pImage.fillAmount = fFillAmount_0_1_Before;
            }

            public void IAnimatedBarLogic_OnUpdate(float fDeltaTime)
            {
                if (_fRemainTime <= 0f)
                    return;
                _fRemainTime -= fDeltaTime;

                if (_fRemainTime > 0f)
                {
                    _pImage.fillAmount += _fOffsetAmount * (fDeltaTime / _fRemainTime);
                }
                else
                {
                    _pImage.enabled = false;
                }
            }
        }
    }
}