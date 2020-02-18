#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-18
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Math = System.Math;

/// <summary>
/// 기존 ScrollRect의 기능을 확장한 클래스입니다.
/// 
/// 다른 방향의 스크롤 이벤트를 구하고 싶을때 사용
/// </summary>
public class ScrollRectExtension : ScrollRect
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EDrageDirection
    {
        None,

        Horizontal,
        Vertical,
    }

    public struct DragEventMsg
    {
        public PointerEventData pPointerEventData { get; private set; }
        public float fScrollDelta { get; private set; }
        public float fScrollDelta_Nomalize_By_ScrollRectSize { get; private set; }

        public DragEventMsg(PointerEventData pPointerEventData, float fScrollDelta, float fScrollDelta_Nomalize_By_ScrollRectSize)
        {
            this.pPointerEventData = pPointerEventData; this.fScrollDelta = fScrollDelta; this.fScrollDelta_Nomalize_By_ScrollRectSize = fScrollDelta_Nomalize_By_ScrollRectSize;
        }
    }

    /* public - Field declaration               */

    public event System.Action<DragEventMsg> OnOtherSide_Drag;
    public event System.Action<DragEventMsg> OnOtherSide_DragBegin;
    public event System.Action<DragEventMsg> OnOtherSide_DragEnd;

    /* protected & private - Field declaration  */

    RectTransform _pRectTransform;
    EDrageDirection _eDragDirection = EDrageDirection.None;
    float _fScrollDelta;

    // ========================================================================== //

    /* public - [Do~Somthing] Function 	        */


    // ========================================================================== //

    /* protected - [Override & Unity API]       */
    //public override void OnInitializePotentialDrag(PointerEventData eventData)
    //{
    //    // Always route initialize potential drag event to parent
    //    if (parentScrollRect != null)
    //    {
    //        ((IInitializePotentialDragHandler)parentScrollRect).OnInitializePotentialDrag(eventData);
    //    }
    //    base.OnInitializePotentialDrag(eventData);
    //}

    protected override void Awake()
    {
        _pRectTransform = GetComponent<RectTransform>();
    }

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (_eDragDirection != EDrageDirection.None)
        {
            if (_eDragDirection == EDrageDirection.Horizontal)
                _fScrollDelta += eventData.delta.x;

            if (_eDragDirection == EDrageDirection.Vertical)
                _fScrollDelta += eventData.delta.y;



            OnOtherSide_Drag?.Invoke(new DragEventMsg(eventData, _fScrollDelta, Calculate_NormalizeDelta()));
        }
        else
        {
            base.OnDrag(eventData);
        }
    }

    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
        {
            _eDragDirection = EDrageDirection.Horizontal;
            _fScrollDelta = 0f;
        }
        else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
        {
            _eDragDirection = EDrageDirection.Vertical;
            _fScrollDelta = 0f;
        }
        else
        {
            _eDragDirection = EDrageDirection.None;
        }

        if (_eDragDirection != EDrageDirection.None)
        {
            OnOtherSide_DragBegin?.Invoke(new DragEventMsg(eventData, _fScrollDelta, Calculate_NormalizeDelta()));
        }
        else
        {
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (_eDragDirection != EDrageDirection.None)
        {
            OnOtherSide_DragEnd?.Invoke(new DragEventMsg(eventData, _fScrollDelta, Calculate_NormalizeDelta()));
        }
        else
        {
            base.OnEndDrag(eventData);
        }
        _eDragDirection = EDrageDirection.None;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private float Calculate_NormalizeDelta()
    {
        return _fScrollDelta / _pRectTransform.rect.height;
    }
    #endregion Private
}