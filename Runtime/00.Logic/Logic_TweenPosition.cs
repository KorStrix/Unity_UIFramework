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
    /// 타겟의 위치를 특정 위치로 보간하는 로직입니다.
    /// </summary>
    [System.Serializable]
    public class Logic_TweenPosition
    {
        public enum ESpace
        {
            World,
            Local,
        }

        public ESpace eSpace = ESpace.Local;

        public AnimationCurve pAnimationCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));

        public Vector3 vecMovePos = new Vector3(0f, 5f);
        public float fDurationSecond = 1f;
        public float fDelaySecond;

        public Logic_TweenPosition(Vector3 vecMovePos, float fDurationSecond, float fDelaySecond = 0f, ESpace eSpace = ESpace.Local)
        {
            this.eSpace = eSpace;
            this.vecMovePos = vecMovePos;
            this.fDurationSecond = fDurationSecond;
            this.fDelaySecond = fDelaySecond;
        }

        public IEnumerator ExecuteLogic_Coroutine(Transform pTransform)
        {
            yield return new WaitForSeconds(fDelaySecond);

            Transform pTransform_Target = pTransform;

            Vector3 vecStartPos = pTransform_Target.position;
            Vector3 vecDestPos = vecMovePos;

            if (eSpace == ESpace.Local)
            {
                vecStartPos = pTransform_Target.localPosition;
                vecDestPos = pTransform_Target.localPosition + vecMovePos;
            }

            float fProgress_0_1 = 0f;
            while (fProgress_0_1 < 1f)
            {
                switch (eSpace)
                {
                    case ESpace.World: pTransform_Target.position = Vector3.Lerp(vecStartPos, vecDestPos, pAnimationCurve.Evaluate(fProgress_0_1)); break;
                    case ESpace.Local: pTransform_Target.localPosition = Vector3.Lerp(vecStartPos, vecDestPos, pAnimationCurve.Evaluate(fProgress_0_1)); break;
                }

                fProgress_0_1 += Time.deltaTime / fDurationSecond;
                yield return null;
            }

            switch (eSpace)
            {
                case ESpace.World: pTransform_Target.position = vecDestPos; break;
                case ESpace.Local: pTransform_Target.localPosition = vecDestPos; break;
            }

            yield break;
        }
    }
}