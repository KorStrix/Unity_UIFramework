#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-04
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
    [System.Serializable]
    public class Logic_TweenColor
    {
        public enum ETweenHow
        {
            CurrentValue_To_Dest,
            SettingValue_To_Dest,
        }

        public ETweenHow eTweenHow = ETweenHow.CurrentValue_To_Dest;

        public AnimationCurve pAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        public Color pColorStart = new Color(1f, 1f, 1f);
        public Color pColorDest = new Color(1f, 1f, 1f);

        public float fDurationSecond = 1f;
        public float fDelaySecond;
        Graphic pTarget;

        public Logic_TweenColor(Color pColorStart, Color pColorDest, float fDurationSecond, float fDelaySecond = 0f)
        {
            this.eTweenHow = ETweenHow.SettingValue_To_Dest;

            this.pColorStart = pColorStart;
            this.pColorDest = pColorDest;

            this.fDurationSecond = fDurationSecond;
            this.fDelaySecond = fDelaySecond;
        }

        public IEnumerator ExecuteLogic_Coroutine(Graphic pTarget)
        {
            yield return new WaitForSeconds(fDelaySecond);

            switch (eTweenHow)
            {
                case ETweenHow.CurrentValue_To_Dest:
                    this.pColorStart = pTarget.color;
                    break;

                case ETweenHow.SettingValue_To_Dest:
                    break;
            }

            float fProgress_0_1 = 0f;
            while (fProgress_0_1 < 1f)
            {
                pTarget.color = Color.Lerp(pColorStart, pColorDest, pAnimationCurve.Evaluate(fProgress_0_1));
                fProgress_0_1 += Time.deltaTime / fDurationSecond;
                yield return null;
            }

            pTarget.color = pColorDest;

            yield break;
        }
    }
}