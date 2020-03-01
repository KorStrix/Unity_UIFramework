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
    [System.Serializable]
    public class Logic_TextTyping
    {
        public float fDurationSecond = 1f;
        public float fDelaySecond;
        public bool bUseRealTime = false;
        public Logic_TextTyping(float fDurationSecond, float fDelaySecond = 0f)
        {
            this.fDurationSecond = fDurationSecond;
            this.fDelaySecond = fDelaySecond;
        }

        public IEnumerator ExecuteLogic_Coroutine(ITextWrapperComponent pText, string strText)
        {
            if(bUseRealTime)
                yield return new WaitForSecondsRealtime(fDelaySecond);
            else
                yield return new WaitForSeconds(fDelaySecond);

            pText.strText = "";
            float fProgress_0_1 = 0f;
            while (fProgress_0_1 < 1f)
            {
                int iTextLength = (int)(strText.Length * fProgress_0_1);
                pText.strText = strText.Substring(0, iTextLength);

                if (bUseRealTime)
                    fProgress_0_1 += Time.unscaledDeltaTime / fDurationSecond;
                else
                    fProgress_0_1 += Time.deltaTime / fDurationSecond;

                yield return null;
            }

            pText.strText = strText;

            yield break;
        }
    }
}