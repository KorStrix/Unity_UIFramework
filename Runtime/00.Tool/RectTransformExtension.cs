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
using System.Linq;
using UnityEngine.UI;

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
    /// UI에 있는 오브젝트의 위치를 다른 카메라의 위치로 변환합니다.
    /// </summary>
    /// <param name="pTarget">변환할 UI 오브젝트</param>
    /// <param name="pUICamera">UI오브젝트를 그리는 카메라</param>
    /// <param name="pOtherCamera">변환할 기준이 되는 타겟 카메라</param>
    /// <returns></returns>
    public static Vector3 Convert_UI_To_OtherScreenPos(this RectTransform pTarget, Camera pUICamera, Camera pOtherCamera)
    {
        Vector3 vecScreenPoint = pUICamera.WorldToScreenPoint(pTarget.position);
        return pOtherCamera.ScreenToWorldPoint(vecScreenPoint);
    }

    /// <summary>
    /// UI에 있는 오브젝트의 Rect를 다른 카메라 기준의 SizeDelta로 변환합니다.
    /// </summary>
    /// <param name="pTarget">변환할 UI 오브젝트</param>
    /// <param name="pUICamera">UI오브젝트를 그리는 카메라</param>
    /// <param name="pOtherCam">변환할 기준이 되는 타겟 카메라</param>
    /// <returns></returns>
    public static Vector2 Convert_UI_To_OtherSizeDelta(this RectTransform pTarget, Camera pUICamera, Camera pOtherCam)
    {
        Rect sRect_Current = pTarget.rect;

        Vector2 vecMin = new Vector2(sRect_Current.xMin, sRect_Current.yMin);
        Vector2 vecMax = new Vector2(sRect_Current.xMax, sRect_Current.yMax);

        //vecMin = RectTransformUtility.WorldToScreenPoint(pUICamera, vecMin);
        //vecMax = RectTransformUtility.WorldToScreenPoint(pUICamera, vecMax);
        Vector2 vecUI_Min = pUICamera.WorldToScreenPoint(vecMin);
        Vector2 vecUI_Max = pUICamera.WorldToScreenPoint(vecMax);

        Vector2 vecWorld_Min = pOtherCam.ScreenToWorldPoint(vecUI_Min);
        Vector2 vecWorld_Max = pOtherCam.ScreenToWorldPoint(vecUI_Max);

        return new Vector2(Mathf.Abs(vecWorld_Max.x - vecWorld_Min.x), Mathf.Abs(vecWorld_Max.y - vecWorld_Min.y));
    }

    enum EWorldCornersIndex
    {
        LeftDown,
        LeftUp,
        RightUp,
        RightDown,
    }

    /// <summary>
    /// UI에 있는 오브젝트의 Rect를 다른 카메라 기준의 SizeDelta로 변환합니다.
    /// </summary>
    /// <param name="pTarget">변환할 UI 오브젝트</param>
    /// <param name="pOtherCam">변환할 기준이 되는 타겟 카메라</param>
    /// <returns></returns>
    public static Vector2 Convert_World_To_OtherSizeDelta(this RectTransform pTarget, Camera pOtherCam)
    {
        Vector3[] arrCorner = new Vector3[4];
        pTarget.GetWorldCorners(arrCorner);

        Vector3 vecLeftDown = pOtherCam.WorldToScreenPoint(arrCorner[(int)EWorldCornersIndex.LeftDown]);
        Vector3 vecRightUp = pOtherCam.WorldToScreenPoint(arrCorner[(int)EWorldCornersIndex.RightUp]);

        return new Vector2(Mathf.Abs(vecRightUp.x - vecLeftDown.x), Mathf.Abs(vecRightUp.y - vecLeftDown.y));
    }

    /// <summary>
    /// 이 <see cref="RectTransform.anchoredPosition"/>을 인자로 넣는 다른 오브젝트의 위치로 설정합니다.
    /// </summary>
    public static void SetAnchorPos_FromOtherWorld(this RectTransform pTarget, Vector3 vecOtherObject_WorldPos, Camera pOtherObjectDrawCamera)
    {
        pTarget.anchoredPosition = pTarget.ConvertUIPos_FromOtherWorld(vecOtherObject_WorldPos, pOtherObjectDrawCamera);
    }

    public static Vector2 ConvertUIPos_FromOtherWorld(this RectTransform pTarget, Vector3 vecOtherObject_WorldPos, Camera pOtherObjectDrawCamera)
    {
        Canvas pCanvas = pTarget.GetComponentInParent<Canvas>();
        RectTransform pCanvasRect = pCanvas.rootCanvas.GetComponent<RectTransform>();

        return Convert_World_To_ViewportPoint(vecOtherObject_WorldPos, pOtherObjectDrawCamera, pCanvasRect.sizeDelta);
    }

    /// <summary>
    /// 이 <see cref="RectTransform.anchoredPosition"/>을 인자로 넣는 다른 오브젝트의 위치로 설정합니다.
    /// </summary>
    public static Bounds ConvertBound_FromOtherWorld(this RectTransform pTarget, Bounds sBound, Camera pOtherObjectDrawCamera)
    {
        Canvas pCanvas = pTarget.GetComponentInParent<Canvas>();
        Canvas pRootCanvas = pCanvas.rootCanvas;
        RectTransform pCanvasRect = pRootCanvas.GetComponent<RectTransform>();

        Vector2 vecCenter = Convert_World_To_ViewportPoint(sBound.center, pOtherObjectDrawCamera, pCanvasRect.sizeDelta);


        // Center는 구하기 쉬운데,
        // Rect(3D)를 2D로 바꾸려면 모든 정점을 구해서
        // 일일이 ViewPort로 컨버팅 후 (World기준으로 Bounds의 min, max가 Screen상에서 min, max가 아님, 로테이션된 오브젝트 기준)
        // 최소 x,y 최대 x,y를 기준으로 새로 Rect를 만듦
        Vector3[] arrVertices = GetVertices_FromBound(sBound);
        Vector3[] arrVertices_ViewPortPoint = arrVertices.Select(p => Convert_World_To_ViewportPoint(p, pOtherObjectDrawCamera, pCanvasRect.sizeDelta)).ToArray();

        float fMinX = arrVertices_ViewPortPoint.OrderBy(p => p.x).First().x;
        float fMinY = arrVertices_ViewPortPoint.OrderBy(p => p.y).First().y;
        float fMaxX = arrVertices_ViewPortPoint.OrderByDescending(p => p.x).First().x;
        float fMaxY = arrVertices_ViewPortPoint.OrderByDescending(p => p.y).First().y;

        Vector2 vecMin = new Vector2(fMinX, fMinY);
        Vector2 vecMax = new Vector2(fMaxX, fMaxY);

        Vector2 vecOffset = vecMax - vecMin;
        return new Bounds(vecCenter, new Vector2(Mathf.Abs(vecOffset.x), Mathf.Abs(vecOffset.y)));
    }

    private static Vector3[] GetVertices_FromBound(Bounds sBound)
    {
        Vector3 boundPoint1 = sBound.min;
        Vector3 boundPoint2 = sBound.max;

        return new Vector3[]
        {
            boundPoint1,
            boundPoint2,
            new Vector3(boundPoint1.x, boundPoint1.y, boundPoint2.z),
            new Vector3(boundPoint1.x, boundPoint2.y, boundPoint1.z),
            new Vector3(boundPoint2.x, boundPoint1.y, boundPoint1.z),
            new Vector3(boundPoint1.x, boundPoint2.y, boundPoint2.z),
            new Vector3(boundPoint2.x, boundPoint1.y, boundPoint2.z),
            new Vector3(boundPoint2.x, boundPoint2.y, boundPoint1.z)
        };
    }


    private static Vector3 Convert_World_To_ViewportPoint(Vector3 vecWorldPos, Camera pOtherObjectDrawCamera, Vector2 vecSizeDelta)
    {
        Vector3 ViewportPosition = pOtherObjectDrawCamera.WorldToViewportPoint(vecWorldPos);

        return new Vector3(
            ((ViewportPosition.x * vecSizeDelta.x) - (vecSizeDelta.x * 0.5f)),
            ((ViewportPosition.y * vecSizeDelta.y) - (vecSizeDelta.y * 0.5f)),
                ViewportPosition.z
            );
    }
}
