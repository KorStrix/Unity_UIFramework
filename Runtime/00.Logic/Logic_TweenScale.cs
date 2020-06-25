#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-02-04
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
    /// 타겟의 크기를 특정 크기로 보간하는 로직입니다.
    /// </summary>
    [System.Serializable]
    public class Logic_TweenScale
    {
        public enum ETweenHow
        {
            CurrentValue_To_Dest,
            SettingValue_To_Dest,
        }

        public ETweenHow eTweenHow = ETweenHow.CurrentValue_To_Dest;

        public AnimationCurve pAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        public Vector3 vecScaleStart = Vector3.one;
        public Vector3 vecScaleDest = Vector3.one;
        public float fDurationSecond = 1f;
        public float fDelaySecond;


        public Logic_TweenScale(Vector3 vecScaleDest, float fDurationSecond, float fDelaySecond = 0f)
        {
            this.vecScaleDest = vecScaleDest;
            this.fDurationSecond = fDurationSecond;
            this.fDelaySecond = fDelaySecond;
        }

        public IEnumerator ExecuteLogic_Coroutine(Transform pTransform)
        {
            yield return new WaitForSeconds(fDelaySecond);

            Transform pTransform_Target = pTransform;

            switch (eTweenHow)
            {
                case ETweenHow.CurrentValue_To_Dest:
                    this.vecScaleStart = pTransform_Target.localScale;
                    break;

                case ETweenHow.SettingValue_To_Dest:
                    break;
            }

            float fProgress_0_1 = 0f;
            while (fProgress_0_1 < 1f)
            {
                pTransform_Target.localScale = Vector3.Lerp(vecScaleStart, vecScaleDest, pAnimationCurve.Evaluate(fProgress_0_1));

                fProgress_0_1 += Time.deltaTime / fDurationSecond;
                yield return null;
            }

            pTransform_Target.localScale = vecScaleDest;

            yield break;
        }
    }
}