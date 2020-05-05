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
using UnityEngine.UI;

/// <summary>
/// 
/// </summary>
public static class UIElement_Extension
{
    #region TextNumber

    delegate T GetLerp<T>(T iNumberStart, T iNumberDest, float fProgress_0_1);
    public delegate string ToString<T>(T iNumber, string strFormat_Or_Null);

  /// <summary>
    /// Text컴포넌트에서 어떤 텍스트 애니메이션을 실행시키기 용도
    /// <para>예시) 숫자 1에서 10까지 천천히 올리고 싶을 때 사용</para>
    /// </summary>
    /// <param name="pCoroutineExecuter">코루틴 실행자</param>
    /// <param name="iNumberStart">시작 숫자</param>
    /// <param name="iNumberDest"></param>
    /// <param name="fDuration"></param>
    /// <returns></returns>
    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fDuration)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Int, ToString_Int_NotUseFormat));
    }  

    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fDuration, string strNumberFormat)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, strNumberFormat, GetLerp_Int, ToString_Int_UseFormat));
    }

    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberStart, int iNumberDest, float fDuration, ToString<int> OnTextResult)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Int, OnTextResult));
    }
    
    
    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberDest, float fDuration)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        int.TryParse(pText.text, out int iNumberStart);
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Int, ToString_Int_UseFormat));
    }
    
    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberDest, float fDuration, string strNumberFormat)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        int.TryParse(pText.text, out int iNumberStart);
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, strNumberFormat, GetLerp_Int, ToString_Int_UseFormat));
    }
    
    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, int iNumberDest, float fDuration, ToString<int> OnTextResult)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        int.TryParse(pText.text, out int iNumberStart);
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Int, OnTextResult));
    }
    
    
    
    

    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, long iNumberStart, long iNumberDest, float fDuration)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Long, ToString_Long_NotUseFormat));
    }

    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, long iNumberStart, long iNumberDest, float fDuration, string strNumberFormat)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, strNumberFormat, GetLerp_Long, ToString_Long_UseFormat));
    }

    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, long iNumberStart, long iNumberDest, float fDuration, ToString<long> OnTextResult)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, iNumberStart, iNumberDest, fDuration, null, GetLerp_Long, OnTextResult));
    }

    
    
    
    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fDuration)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, null, GetLerp_Float, ToString_Float_NotUseFormat));
    }

    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fDuration, string strNumberFormat)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, strNumberFormat, GetLerp_Float, ToString_Float_UseFormat));
    }

    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberStart, float fNumberDest, float fDuration, ToString<float> OnTextResult)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, null, GetLerp_Float, OnTextResult));
    }

    
    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberDest, float fDuration)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        float.TryParse(pText.text, out float fNumberStart);
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, null, GetLerp_Float, ToString_Float_UseFormat));
    }
    
    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberDest, float fDuration, string strNumberFormat)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        float.TryParse(pText.text, out float fNumberStart);
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, strNumberFormat, GetLerp_Float, ToString_Float_UseFormat));
    }
    
    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, float fNumberDest, float fDuration, ToString<float> OnTextResult)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        float.TryParse(pText.text, out float fNumberStart);
        return pCoroutineExecuter.StartCoroutine(TweenText(pText, fNumberStart, fNumberDest, fDuration, null, GetLerp_Float, OnTextResult));
    }
    
    
    
    
    public static void DoSeekTween(this UnityEngine.UI.Text pText, int iNumberStart, int iNumberDest, float fSeekPos_0_1)
    {
        SeekTweenText(pText, iNumberStart, iNumberDest, fSeekPos_0_1, null, GetLerp_Int, ToString_Int_NotUseFormat);
    }

    public static void DoSeekTween(this UnityEngine.UI.Text pText, int iNumberStart, int iNumberDest, float fSeekPos_0_1, string strFormat)
    {
        SeekTweenText(pText, iNumberStart, iNumberDest, fSeekPos_0_1, strFormat, GetLerp_Int, ToString_Int_UseFormat);
    }

    public static void DoSeekTween(this UnityEngine.UI.Text pText, float fNumberStart, float fNumberDest, float fSeekPos_0_1)
    {
        SeekTweenText(pText, fNumberStart, fNumberDest, fSeekPos_0_1, null, GetLerp_Float, ToString_Float_NotUseFormat);
    }

    public static void DoSeekTween(this UnityEngine.UI.Text pText, float fNumberStart, float fNumberDest, float fSeekPos_0_1, string strFormat)
    {
        SeekTweenText(pText, fNumberStart, fNumberDest, fSeekPos_0_1, strFormat, GetLerp_Float, ToString_Float_UseFormat);
    }


    /// <summary>
    /// 텍스트 애니메이션
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pText">애니메이션 타겟</param>
    /// <param name="iNumberStart">시작 숫자</param>
    /// <param name="iNumberDest">끝 숫자</param>
    /// <param name="fDuration">애니메이션 총 걸리는 시간</param>
    /// <param name="strFormat">텍스트를 출력할 포멧</param>
    /// <param name="GetLerp">보간 로직</param>
    /// <param name="ToString">텍스트 출력 함수</param>
    private static IEnumerator TweenText<T>(UnityEngine.UI.Text pText, T iNumberStart, T iNumberDest, float fDuration, string strFormat, GetLerp<T> GetLerp, ToString<T> ToString)
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


    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="pText"></param>
    /// <param name="iNumberStart"></param>
    /// <param name="iNumberDest"></param>
    /// <param name="fSeekPos_0_1"></param>
    /// <param name="strFormat"></param>
    /// <param name="GetLerp"></param>
    /// <param name="ToString"></param>
    private static void SeekTweenText<T>(UnityEngine.UI.Text pText, T iNumberStart, T iNumberDest, float fSeekPos_0_1, string strFormat, GetLerp<T> GetLerp, ToString<T> ToString)
    {
        int iVAlue = 1000;
        iVAlue.ToString(); // =1000
        iVAlue.ToString("n0");  //=1,000

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

    public static Coroutine DoPlayTween(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, string strTextDest, float fDuration)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, strTextDest, fDuration, OnCalculateProgress_Duration));
    }

    public static Coroutine DoPlayTween_BySpeed(this UnityEngine.UI.Text pText, MonoBehaviour pCoroutineExecuter, string strTextDest, float fSpeed)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenText(pText, strTextDest, fSpeed, OnCalculateProgress_Speed));
    }

    private static IEnumerator TweenText(UnityEngine.UI.Text pText, string strTextDest, float fValue, System.Func<float, float, float> OnCalculateProgress)
    {
        int iLength = strTextDest.Length;
        int iCutLengthPrev = -1;
        pText.text = "";

        float fProgress_0_1 = 0f;
        while (fProgress_0_1 < 1f)
        {
            int iCutLength = (int)(iLength * fProgress_0_1);
            if (iCutLengthPrev != iCutLength)
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


    public static Coroutine DoPlayTween_Number(this UnityEngine.UI.Slider pSlider, MonoBehaviour pCoroutineExecuter, float fValueStart, float fValueDest, float fDuration)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenSlider((fValue) => pSlider.value = fValue, fValueStart, fValueDest, fDuration));
    }

    public static Coroutine DoPlayTween_FillAmount(this UnityEngine.UI.Image pImage, MonoBehaviour pCoroutineExecuter, float fValueStart, float fValueDest, float fDuration)
    {
        if (pCoroutineExecuter.gameObject.activeInHierarchy == false)
        {
            Debug.LogWarning($"{pCoroutineExecuter.name} - {nameof(DoPlayTween)} activeInHierarchy == false", pCoroutineExecuter);
            return null;
        }

        return pCoroutineExecuter.StartCoroutine(TweenSlider((fValue) => pImage.fillAmount = fValue, fValueStart, fValueDest, fDuration));
    }

    //static public Coroutine DoPlayTween(this UnityEngine.UI.Image pSprite, MonoBehaviour pCoroutineExecuter, float fValueStart, float fValueDest, float fDuration)
    //{
    //    return pCoroutineExecuter.StartCoroutine(TweenSlider(pSprite, fValueStart, fValueDest, fDuration));
    //}

    private static IEnumerator TweenSlider(System.Action<float> OnChangeValue, float fValueStart, float fValueDest, float fDuration)
    {
        // 진행도
        float fProgress_0_1 = 0f;
        while (fProgress_0_1 < 1f)
        {
            OnChangeValue(Mathf.Lerp(fValueStart, fValueDest, fProgress_0_1));
            fProgress_0_1 += Time.deltaTime / fDuration;

            yield return null;
        }

        // 마지막에 목적값?을 대입해줍니다.
        OnChangeValue(fValueDest);
    }


    // https://forum.unity.com/threads/scrollrect-scroll-to-a-gameobject-position.473214/
    public static void DoCenterToItem(this ScrollRect pScroll, RectTransform pScrollMask, RectTransform pCenterItem)
    {
        pScroll.normalizedPosition = CalculateScrollPosition(pScroll, pScrollMask, pCenterItem);
    }

    public static Coroutine DoCenterToItem_CoroutineAnimation(this ScrollRect pScroll, RectTransform pScrollMask, RectTransform pCenterItem, float fDuration)
    {
        return pScroll.StartCoroutine(Scroll_CenterToItem_Coroutine(pScroll, CalculateScrollPosition(pScroll, pScrollMask, pCenterItem), fDuration));
    }

    static IEnumerator Scroll_CenterToItem_Coroutine(ScrollRect pScroll, Vector2 vecDestNormalizePosition, float fDuration)
    {
        pScroll.enabled = false;

        Vector2 vecStartNoramlizePosition = pScroll.normalizedPosition;
        float fProgress_0_1 = 0f;
        while(fProgress_0_1 < 1f)
        {
            pScroll.normalizedPosition = Vector2.Lerp(vecStartNoramlizePosition, vecDestNormalizePosition, fProgress_0_1);
            fProgress_0_1 += Time.deltaTime / fDuration;

            yield return null;
        }

        pScroll.normalizedPosition = vecDestNormalizePosition;
        pScroll.enabled = true;
    }

    private static Vector2 CalculateScrollPosition(ScrollRect pScroll, RectTransform pScrollMask, RectTransform pCenterItem)
    {
        RectTransform mScrollTransform = pScroll.GetComponent<RectTransform>();
        RectTransform mContent = pScroll.content;

        Vector2 vecTargetItemPos = GetWorldPoint_InWidget(mScrollTransform, GetWorldPoint(pCenterItem));
        Vector2 vecScrollCenterPos = GetWorldPoint_InWidget(mScrollTransform, GetWorldPoint(pScrollMask));
        Vector2 fTargetDistance = vecScrollCenterPos - vecTargetItemPos;

        if (pScroll.horizontal == false)
            fTargetDistance.x = 0f;

        if (pScroll.vertical == false)
            fTargetDistance.y = 0f;

        Vector3 vecSizeOffset = mContent.rect.size - mScrollTransform.rect.size;

        var normalizedDifference = new Vector2(
            fTargetDistance.x / vecSizeOffset.x,
            fTargetDistance.y / vecSizeOffset.y);

        var newNormalizedPosition = pScroll.normalizedPosition - normalizedDifference;
        // if (pScroll.movementType != ScrollRect.MovementType.Unrestricted)
        {
            newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
            newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
        }

        return newNormalizedPosition;
    }

    private static Vector3 GetWorldPoint(RectTransform target)
    {
        //pivot position + item size has to be included
        var pivotOffset = new Vector3(
            (0.5f - target.pivot.x) * target.rect.size.x,
            (0.5f - target.pivot.y) * target.rect.size.y);

        var localPosition = target.localPosition + pivotOffset;
        return target.parent.TransformPoint(localPosition);
    }

    private static Vector3 GetWorldPoint_InWidget(RectTransform target, Vector3 worldPoint)
    {
        return target.InverseTransformPoint(worldPoint);
    }
}