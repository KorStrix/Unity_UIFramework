#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-18
 *	Summary 		        : 
 *	
 *	기존 ScrollRect의 기능을 확장한 클래스입니다.
 *	
 *	기능 1. 다른 방향의 스크롤 이벤트를 구하고 싶을때 사용
 *	
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Math = System.Math;

namespace UIFramework
{
    public interface IScrollItem
    {
        RectTransform rectTransform { get; }
        void IScrollItem_OnChangeScrollIndex(float fCenterDistance_0_1);
    }

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

        public event System.Action<DragEventMsg> OnDrag_OtherSide;
        public event System.Action<DragEventMsg> OnDragBegin_OtherSide;
        public event System.Action<DragEventMsg> OnDragEnd_OtherSide;

        public RectTransform pMaskTransform { get; private set; }

        /* protected & private - Field declaration  */

        List<IScrollItem> _listScrollItem = new List<IScrollItem>();
        List<IScrollItem> _listScrollItem_Temp = new List<IScrollItem>();

        RectTransform _pRectTransform;

        EDrageDirection _eDragDirection = EDrageDirection.None;
        float _fScrollDelta;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoUpdateScrollItemList()
        {
            _listScrollItem.Clear();
            _listScrollItem.AddRange(GetComponentsInChildren<IScrollItem>());
        }

        public void DoUpdateMaskTransform()
        {
            var pMask = GetComponentInChildren<Mask>();
            pMaskTransform = pMask?.rectTransform;
            if (pMaskTransform == null || ReferenceEquals(pMaskTransform, null))
                pMaskTransform = content.parent as RectTransform;
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void Awake()
        {
            _pRectTransform = GetComponent<RectTransform>();

            DoUpdateMaskTransform();
            DoUpdateScrollItemList();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (_eDragDirection != EDrageDirection.None)
            {
                if (_eDragDirection == EDrageDirection.Horizontal)
                    _fScrollDelta += eventData.delta.x;

                if (_eDragDirection == EDrageDirection.Vertical)
                    _fScrollDelta += eventData.delta.y;


                OnDrag_OtherSide?.Invoke(new DragEventMsg(eventData, _fScrollDelta, Calculate_NormalizeDelta()));
            }
            else
            {
                base.OnDrag(eventData);
            }

            if (_listScrollItem.Count == 0)
                return;

            float fMinX = _pRectTransform.rect.xMin;
            float fMaxX = _pRectTransform.rect.xMax;
            float fMinY = _pRectTransform.rect.yMin;
            float fMaxY = _pRectTransform.rect.yMax;

            _listScrollItem_Temp.Clear();
            for (int i = 0; i < _listScrollItem.Count; i++)
            {
                IScrollItem pScrollItemCurrent = _listScrollItem[i];
                RectTransform pTransformChild = pScrollItemCurrent.rectTransform;
                Vector3 vecPos = GetWorldPointInWidget(_pRectTransform, GetWidgetWorldPoint(pTransformChild));

                bool bIsShow = false;
                if (horizontal && fMinX < vecPos.x && vecPos.x < fMaxX)
                {
                    bIsShow = true;
                    pScrollItemCurrent.IScrollItem_OnChangeScrollIndex(1 - CalculateScrollPosition_0_1(pTransformChild).x);
                }

                if (bIsShow == false &&
                    vertical && fMinY < vecPos.y && vecPos.y < fMaxY)
                {
                    bIsShow = true;
                    pScrollItemCurrent.IScrollItem_OnChangeScrollIndex(1 - CalculateScrollPosition_0_1(pTransformChild).y);
                }

                if (bIsShow == false)
                    continue;

                _listScrollItem_Temp.Add(pScrollItemCurrent);
            }
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (base.horizontal == false && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
            {
                _eDragDirection = EDrageDirection.Horizontal;
                _fScrollDelta = 0f;
            }
            else if (base.vertical == false && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
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
                OnDragBegin_OtherSide?.Invoke(new DragEventMsg(eventData, _fScrollDelta, Calculate_NormalizeDelta()));
            }
            else
            {
                base.OnBeginDrag(eventData);
            }
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (_eDragDirection != EDrageDirection.None)
            {
                OnDragEnd_OtherSide?.Invoke(new DragEventMsg(eventData, _fScrollDelta, Calculate_NormalizeDelta()));
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

        private Vector2 CalculateScrollPosition_0_1(RectTransform pItemItem)
        {
            // Item is here
            var itemCenterPositionInScroll = GetWorldPointInWidget(_pRectTransform, GetWidgetWorldPoint(pItemItem));
            // But must be here
            var targetPositionInScroll = GetWorldPointInWidget(_pRectTransform, GetWidgetWorldPoint(pMaskTransform));
            // So it has to move this distance
            var difference = targetPositionInScroll - itemCenterPositionInScroll;
            difference.z = 0f;

            //clear axis data that is not enabled in the scrollrect
            if (!horizontal)
            {
                difference.x = 0f;
            }
            if (!vertical)
            {
                difference.y = 0f;
            }

            var normalizedDifference = new Vector2(
                difference.x / (content.rect.size.x - _pRectTransform.rect.size.x),
                difference.y / (content.rect.size.y - _pRectTransform.rect.size.y));

            var newNormalizedPosition = normalizedPosition - normalizedDifference;
            if (movementType != ScrollRect.MovementType.Unrestricted)
            {
                newNormalizedPosition.x = Mathf.Clamp01(newNormalizedPosition.x);
                newNormalizedPosition.y = Mathf.Clamp01(newNormalizedPosition.y);
            }

            return newNormalizedPosition;
        }

        private static Vector3 GetWidgetWorldPoint(RectTransform target)
        {
            //pivot position + item size has to be included
            var pivotOffset = new Vector3(
                (0.5f - target.pivot.x) * target.rect.size.x,
                (0.5f - target.pivot.y) * target.rect.size.y,
                0f);
            var localPosition = target.localPosition + pivotOffset;
            return target.parent.TransformPoint(localPosition);
        }

        private static Vector3 GetWorldPointInWidget(RectTransform target, Vector3 worldPoint)
        {
            return target.InverseTransformPoint(worldPoint);
        }

        private float Calculate_NormalizeDelta()
        {
            return _fScrollDelta / _pRectTransform.rect.height;
        }

        #endregion Private
    }
}