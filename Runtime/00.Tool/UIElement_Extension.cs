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
    delegate string GetNumberLerpString<T>(T iNumberStart, T iNumberDest, float fProgress_0_1, string strFormat);

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fDuration)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetNumberLerp_Int_HasNotFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fDuration, string strFormat)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, strFormat, GetNumberLerp_Int_HasFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fDuration)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, null, GetNumberLerp_Float_HasNotFormat));
    }

    static public Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fDuration, string strFormat)
    {
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, strFormat, GetNumberLerp_Float_HasFormat));
    }

    static public void DoSeekTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fSeekPos_0_1)
    {
        SeekTweenText(pText, iNumberStart, iNumberDest, fSeekPos_0_1, null, GetNumberLerp_Int_HasNotFormat);
    }

    static public void DoSeekTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fSeekPos_0_1, string strFormat)
    {
        SeekTweenText(pText, iNumberStart, iNumberDest, fSeekPos_0_1, strFormat, GetNumberLerp_Int_HasFormat);
    }

    static public void DoSeekTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fSeekPos_0_1)
    {
        SeekTweenText(pText, fNumberStart, fNumberDest, fSeekPos_0_1, null, GetNumberLerp_Float_HasNotFormat);
    }

    static public void DoSeekTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fSeekPos_0_1, string strFormat)
    {
        SeekTweenText(pText, fNumberStart, fNumberDest, fSeekPos_0_1, strFormat, GetNumberLerp_Float_HasFormat);
    }


    static private IEnumerator TweenText<T>(UnityEngine.UI.Text pText, T iNumberStart, T iNumberDest, float fDuration, string strFormat, GetNumberLerpString<T> NumberFormat)
    {
        float fProgress_0_1 = 0f;
        while (fProgress_0_1 < 1f)
        {
            SeekTweenText(pText, iNumberStart, iNumberDest, fProgress_0_1, strFormat, NumberFormat);
            fProgress_0_1 += Time.deltaTime / fDuration;

            yield return null;
        }

        SeekTweenText(pText, iNumberStart, iNumberDest, 1f, strFormat, NumberFormat);
        yield return null;
    }

    static private void SeekTweenText<T>(UnityEngine.UI.Text pText, T iNumberStart, T iNumberDest, float fSeekPos_0_1, string strFormat, GetNumberLerpString<T> NumberFormat)
    {
        pText.text = NumberFormat(iNumberStart, iNumberDest, fSeekPos_0_1, strFormat);
    }

    private static string GetNumberLerp_Int_HasFormat(int iNumberStart, int iNumberDest, float fProgress_0_1, string strFormat)
    {
		return Mathf.RoundToInt(Mathf.Lerp(iNumberStart, iNumberDest, fProgress_0_1)).ToString(strFormat);
    }
    private static string GetNumberLerp_Int_HasNotFormat(int iNumberStart, int iNumberDest, float fProgress_0_1, string strFormat)
    {
		return Mathf.RoundToInt(Mathf.Lerp(iNumberStart, iNumberDest, fProgress_0_1)).ToString();
    }
    private static string GetNumberLerp_Float_HasFormat(float fNumberStart, float fNumberDest, float fProgress_0_1, string strFormat)
    {
        return Mathf.Lerp(fNumberStart, fNumberDest, fProgress_0_1).ToString(strFormat);
	} 
    private static string GetNumberLerp_Float_HasNotFormat(float fNumberStart, float fNumberDest, float fProgress_0_1, string strFormat)
    {
        return Mathf.Lerp(fNumberStart, fNumberDest, fProgress_0_1).ToString();
    }
}