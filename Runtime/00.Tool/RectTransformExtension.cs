#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-31 오후 2:30:11
 *	개요 : 
 *	
 *	원본 링크 : https://answers.unity.com/questions/1225118/solution-set-ui-recttransform-anchor-presets-from.html
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public enum AnchorPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottonCenter,
    BottomRight,
    BottomStretch,

    VertStretchLeft,
    VertStretchRight,
    VertStretchCenter,

    HorStretchTop,
    HorStretchMiddle,
    HorStretchBottom,

    StretchAll
}

public enum PivotPresets
{
    TopLeft,
    TopCenter,
    TopRight,

    MiddleLeft,
    MiddleCenter,
    MiddleRight,

    BottomLeft,
    BottomCenter,
    BottomRight,
}

/// <summary>
/// Rect Transform의 확장 함수 모음
/// </summary>
public static class RectTransformExtensions
{
    /// <summary>
    /// UI Object의 Anchor를 세팅합니다.
    /// </summary>
    public static void SetAnchor(this RectTransform source, AnchorPresets allign, int offsetX = 0, int offsetY = 0)
    {
        if (source == null)
            return;

        source.anchoredPosition = new Vector3(offsetX, offsetY, 0);

        switch (allign)
        {
            case (AnchorPresets.TopLeft):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.TopCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 1);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.TopRight):
                {
                    source.anchorMin = new Vector2(1, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.MiddleLeft):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(0, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0.5f);
                    source.anchorMax = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (AnchorPresets.MiddleRight):
                {
                    source.anchorMin = new Vector2(1, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }

            case (AnchorPresets.BottomLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 0);
                    break;
                }
            case (AnchorPresets.BottonCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 0);
                    break;
                }
            case (AnchorPresets.BottomRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.HorStretchTop):
                {
                    source.anchorMin = new Vector2(0, 1);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
            case (AnchorPresets.HorStretchMiddle):
                {
                    source.anchorMin = new Vector2(0, 0.5f);
                    source.anchorMax = new Vector2(1, 0.5f);
                    break;
                }
            case (AnchorPresets.HorStretchBottom):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 0);
                    break;
                }

            case (AnchorPresets.VertStretchLeft):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(0, 1);
                    break;
                }
            case (AnchorPresets.VertStretchCenter):
                {
                    source.anchorMin = new Vector2(0.5f, 0);
                    source.anchorMax = new Vector2(0.5f, 1);
                    break;
                }
            case (AnchorPresets.VertStretchRight):
                {
                    source.anchorMin = new Vector2(1, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }

            case (AnchorPresets.StretchAll):
                {
                    source.anchorMin = new Vector2(0, 0);
                    source.anchorMax = new Vector2(1, 1);
                    break;
                }
        }
    }

    /// <summary>
    /// UI Object의 Pivot을 세팅합니다.
    /// </summary>
    public static void SetPivot(this RectTransform source, PivotPresets preset)
    {
        if (source == null)
            return;

        switch (preset)
        {
            case (PivotPresets.TopLeft):
                {
                    source.pivot = new Vector2(0, 1);
                    break;
                }
            case (PivotPresets.TopCenter):
                {
                    source.pivot = new Vector2(0.5f, 1);
                    break;
                }
            case (PivotPresets.TopRight):
                {
                    source.pivot = new Vector2(1, 1);
                    break;
                }

            case (PivotPresets.MiddleLeft):
                {
                    source.pivot = new Vector2(0, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleCenter):
                {
                    source.pivot = new Vector2(0.5f, 0.5f);
                    break;
                }
            case (PivotPresets.MiddleRight):
                {
                    source.pivot = new Vector2(1, 0.5f);
                    break;
                }

            case (PivotPresets.BottomLeft):
                {
                    source.pivot = new Vector2(0, 0);
                    break;
                }
            case (PivotPresets.BottomCenter):
                {
                    source.pivot = new Vector2(0.5f, 0);
                    break;
                }
            case (PivotPresets.BottomRight):
                {
                    source.pivot = new Vector2(1, 0);
                    break;
                }
        }
    }
    
    /// <summary>
    /// UI 오브젝트의 중앙 위치를 리턴합니다.
    /// </summary>
    public static Vector3 GetUICenterPosition(this RectTransform pRectTransform)
    {
        if (pRectTransform == null)
            return Vector3.zero;

        return pRectTransform.TransformPoint(pRectTransform.rect.center);
    }

    /// <summary>
    /// 스크린 기준의 Rect를 구합니다
    /// https://answers.unity.com/questions/1013011/convert-recttransform-rect-to-screen-space.html
    /// </summary>
    /// <param name="pRectTransform"></param>
    public static Rect GetScreenRect(this RectTransform pRectTransform)
    {
        Vector2 size = Vector2.Scale(pRectTransform.rect.size, pRectTransform.lossyScale);
        return new Rect((Vector2)pRectTransform.position - (size * 0.5f), size);
    }

    /// <summary>
    /// UI에 있는 오브젝트를 월드 위치로 변환합니다.
    /// </summary>
    /// <param name="pTarget">변환할 UI 오브젝트</param>
    /// <param name="pUICamera">UI오브젝트를 그리는 카메라</param>
    /// <param name="pWorldCamera">변환할 기준이 되는 월드 카메라</param>
    /// <returns></returns>
    static public Vector3 Convert_UI_To_WorldScreenPos(this RectTransform pTarget, Camera pUICamera, Camera pWorldCamera)
    {
        Vector3 vecScreenPoint = pUICamera.WorldToScreenPoint(pTarget.position);
        return pWorldCamera.ScreenToWorldPoint(vecScreenPoint);
    }
}