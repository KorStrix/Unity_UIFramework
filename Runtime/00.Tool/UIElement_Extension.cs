#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-17 오후 6:02:02
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public static class UIElement_Extension
{
    #region TextNumber

    delegate T GetLerp<T>(T iNumberStart, T iNumberDest, float fProgress_0_1);
    public delegate string ToString<T>(T iNumber, string strFormat_Or_Null);

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fDuration)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Int, ToString_Int_NotUseFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fDuration, string strNumberFormat)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, strNumberFormat, GetLerp_Int, ToString_Int_UseFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fDuration, ToString<int> OnTextResult)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Int, OnTextResult));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, long iNumberStart, long iNumberDest, float fDuration)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Long, ToString_Long_NotUseFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, long iNumberStart, long iNumberDest, float fDuration, string strNumberFormat)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, strNumberFormat, GetLerp_Long, ToString_Long_UseFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, long iNumberStart, long iNumberDest, float fDuration, ToString<long> OnTextResult)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Long, OnTextResult));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fDuration)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, null, GetLerp_Float, ToString_Float_NotUseFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fDuration, string strNumberFormat)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, strNumberFormat, GetLerp_Float, ToString_Float_UseFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fDuration, ToString<float> OnTextResult)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, null, GetLerp_Float, OnTextResult));
    }

    static public void DoSeekTween(this UnityEngine.UI.Text pText, int iNumberStart, int iNumberDest, float fSeekPos_0_1)
    {
        SeekTweenText(pText, iNumberStart, iNumberDest, fSeekPos_0_1, null, GetLerp_Int, ToString_Int_NotUseFormat);
    }

    static public void DoSeekTween(this UnityEngine.UI.Text pText, int iNumberStart, int iNumberDest, float fSeekPos_0_1, string strFormat)
    {
        SeekTweenText(pText, iNumberStart, iNumberDest, fSeekPos_0_1, strFormat, GetLerp_Int, ToString_Int_UseFormat);
    }

    static public void DoSeekTween(this UnityEngine.UI.Text pText, float fNumberStart, float fNumberDest, float fSeekPos_0_1)
    {
        SeekTweenText(pText, fNumberStart, fNumberDest, fSeekPos_0_1, null, GetLerp_Float, ToString_Float_NotUseFormat);
    }

    static public void DoSeekTween(this UnityEngine.UI.Text pText, float fNumberStart, float fNumberDest, float fSeekPos_0_1, string strFormat)
    {
        SeekTweenText(pText, fNumberStart, fNumberDest, fSeekPos_0_1, strFormat, GetLerp_Float, ToString_Float_UseFormat);
    }


    static private IEnumerator TweenText<T>(UnityEngine.UI.Text pText, T iNumberStart, T iNumberDest, float fDuration, string strFormat, GetLerp<T> GetLerp, ToString<T> ToString)
    {
        float fProgress_0_1 = 0f;
        while (fProgress_0_1 < 1f)
        {
            SeekTweenText(pText, iNumberStart, iNumberDest, fProgress_0_1, strFormat, GetLerp, ToString);
            fProgress_0_1 += Time.deltaTime / fDuration;

            yield return null;
        }

        pText.text = ToString(iNumberDest, strFormat);
    }
    

    static private void SeekTweenText<T>(UnityEngine.UI.Text pText, T iNumberStart, T iNumberDest, float fSeekPos_0_1, string strFormat, GetLerp<T> GetLerp, ToString<T> ToString)
    {
        pText.text = ToString(GetLerp(iNumberStart, iNumberDest, fSeekPos_0_1), strFormat);
    }

    private static int GetLerp_Int(int iNumberStart, int iNumberDest, float fProgress_0_1)
    {
        return Mathf.RoundToInt(Mathf.Lerp(iNumberStart, iNumberDest, fProgress_0_1));
    }
    
    private static long GetLerp_Long(long iNumberStart, long iNumberDest, float fProgress_0_1)
    {
        return Mathf.RoundToInt(Mathf.Lerp(iNumberStart, iNumberDest, fProgress_0_1));
    }

    private static float GetLerp_Float(float fNumberStart, float fNumberDest, float fProgress_0_1)
    {
        return Mathf.Lerp(fNumberStart, fNumberDest, fProgress_0_1);
	}

    private static string ToString_Int_UseFormat(int iNumber, string strFormat) { return iNumber.ToString(strFormat); }
    private static string ToString_Int_NotUseFormat(int iNumber, string strFormat) { return iNumber.ToString(); }

    private static string ToString_Long_UseFormat(long iNumber, string strFormat) { return iNumber.ToString(strFormat); }
    private static string ToString_Long_NotUseFormat(long iNumber, string strFormat) { return iNumber.ToString(); }

    private static string ToString_Float_UseFormat(float fNumber, string strFormat) { return fNumber.ToString(strFormat); }
    private static string ToString_Float_NotUseFormat(float fNumber, string strFormat) { return fNumber.ToString(); }

    #endregion TextNumber

    //static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, string strTextStart, string strTextDest, float fDuration)
    //{
    //    return pCoroutineExecuter.StartCoroutine(TweenText(pText, strTextStart, strTextDest, fDuration));
    //}

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, string strTextDest, float fDuration)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, strTextDest, fDuration,  OnCalculateProgress_Duration));
    }

    static public Coroutine DoPlayTween_BySpeed(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, string strTextDest, float fSpeed)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, strTextDest, fSpeed, OnCalculateProgress_Speed));
    }

    static private IEnumerator TweenText(UnityEngine.UI.Text pText, string strTextDest, float fValue, System.Func<float ,float ,float> OnCalculateProgress)
    {
        int iLength = strTextDest.Length;
        int iCutLengthPrev = -1;
        pText.text = "";

        float fProgress_0_1 = 0f;
        while (fProgress_0_1 < 1f)
        {
            int iCutLength = (int)(iLength * fProgress_0_1);
            if(iCutLengthPrev != iCutLength)
            {
                iCutLengthPrev = iCutLength;
                pText.text = strTextDest.Substring(0, iCutLength);
            }

            fProgress_0_1 += OnCalculateProgress(fValue, fProgress_0_1);
            yield return null;
        }

        pText.text = strTextDest;
    }

    private static float OnCalculateProgress_Duration(float fDuration, float fProgress_0_1)
    {
        return Time.deltaTime / fDuration;
    }

    private static float OnCalculateProgress_Speed(float fSpeed, float fProgress_0_1)
    {
        return Time.deltaTime * fSpeed;
    }


    static public Coroutine DoPlayTween(this UnityEngine.UI.Slider pSlider, MonoBehaviour pCoroutineExecuter, float fValueStart, float fValueDest, float fDuration)
    {
        return pCoroutineExecuter.StartCoroutine(TweenSlider((fValue) => pSlider.value = fValue, fValueStart, fValueDest, fDuration));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Image pImage, MonoBehaviour pCoroutineExecuter, float fValueStart, float fValueDest, float fDuration)
    {
        return pCoroutineExecuter.StartCoroutine(TweenSlider((fValue) => pImage.fillAmount = fValue, fValueStart, fValueDest, fDuration));
    }

    //static public Coroutine DoPlayTween(this UnityEngine.UI.Image pSprite, MonoBehaviour pCoroutineExecuter, float fValueStart, float fValueDest, float fDuration)
    //{
    //    return pCoroutineExecuter.StartCoroutine(TweenSlider(pSprite, fValueStart, fValueDest, fDuration));
    //}

    static private IEnumerator TweenSlider(System.Action<float> OnChangeValue, float fValueStart, float fValueDest, float fDuration)
    {
        float fProgress_0_1 = 0f;
        while (fProgress_0_1 < 1f)
        {
            OnChangeValue(Mathf.Lerp(fValueStart, fValueDest, fProgress_0_1));
            fProgress_0_1 += Time.deltaTime / fDuration;

            yield return null;
        }

        OnChangeValue(fValueDest);
    }
}