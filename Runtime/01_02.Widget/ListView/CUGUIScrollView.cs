#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-10 오후 12:20:59
 *	개요 : 
 *	
 *	원본 코드 링크 : https://github.com/qiankanglai/LoopScrollRect

 *  수정 내용
 *      클래스 하나로 Horizontal, Vertical을 세팅할 수 있게 수정하였습니다.
 *      스크롤 아이템 풀링 코드가 리소스 폴더에 종속되있는 것을 외부에서 세팅할 수 있게 수정하였습니다.
 *      스크롤 아이템의 갱신을 SendMessage가 아닌 Interface를 통해 메세지를 받게끔 수정하였습니다.
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using static ICollectionView;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CUGUIScrollView : UIBehaviour, ICollectionView, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IScrollHandler, ICanvasElement, ILayoutElement, ILayoutGroup
{
    public enum EScrollDirection
    {
        Horizontal,
        Vertical,
    }

    //==========LoopScrollRect==========

    public bool bPrintDebug = false;

    public EScrollDirection eScrollDirection;

    [Tooltip("Original Scroll Item")]
    public GameObject prefabSource;

    [Tooltip("Total count, negative means INFINITE mode")]
    int _iScrollItem_TotalCount;

    public int iInitPoolCount = 10;

    protected float threshold = 0;
    [Tooltip("Reverse direction for dragging")]
    public bool reverseDirection = false;
    [Tooltip("Rubber scale for outside")]
    public float rubberScale = 1;

    protected int itemTypeStart = 0;
    protected int itemTypeEnd = 0;


    protected int directionSign = 0;

    private float m_ContentSpacing = -1;
    protected GridLayoutGroup m_GridLayout = null;
    protected float contentSpacing
    {
        get
        {
            if (m_ContentSpacing >= 0)
            {
                return m_ContentSpacing;
            }
            m_ContentSpacing = 0;
            if (content != null)
            {
                HorizontalOrVerticalLayoutGroup layout1 = content.GetComponent<HorizontalOrVerticalLayoutGroup>();
                if (layout1 != null)
                {
                    m_ContentSpacing = layout1.spacing;
                }
                m_GridLayout = content.GetComponent<GridLayoutGroup>();
                if (m_GridLayout != null)
                {
                    m_ContentSpacing = Mathf.Abs(GetDimension(m_GridLayout.spacing));
                }
            }
            return m_ContentSpacing;
        }
    }

    private int m_ContentConstraintCount = 0;
    protected int contentConstraintCount
    {
        get
        {
            if (m_ContentConstraintCount > 0)
            {
                return m_ContentConstraintCount;
            }
            m_ContentConstraintCount = 1;
            if (content != null)
            {
                GridLayoutGroup layout2 = content.GetComponent<GridLayoutGroup>();
                if (layout2 != null)
                {
                    if (layout2.constraint == GridLayoutGroup.Constraint.Flexible)
                    {
                        Debug.LogWarning("[LoopScrollRect] Flexible not supported yet");
                    }
                    m_ContentConstraintCount = layout2.constraintCount;
                }
            }
            return m_ContentConstraintCount;
        }
    }

    // the first line
    int StartLine
    {
        get
        {
            return Mathf.CeilToInt((float)(itemTypeStart) / contentConstraintCount);
        }
    }

    // how many lines we have for now
    int CurrentLines
    {
        get
        {
            return Mathf.CeilToInt((float)(itemTypeEnd - itemTypeStart) / contentConstraintCount);
        }
    }

    // how many lines we have in total
    int TotalLines
    {
        get
        {
            return Mathf.CeilToInt((float)(_iScrollItem_TotalCount) / contentConstraintCount);
        }
    }

    //==========LoopScrollRect==========

    public enum MovementType
    {
        Unrestricted, // Unrestricted movement -- can scroll forever
        Elastic, // Restricted but flexible -- can go past the edges, but springs back in place
        Clamped, // Restricted movement where it's not possible to go past the edges
    }

    public enum ScrollbarVisibility
    {
        Permanent,
        AutoHide,
        AutoHideAndExpandViewport,
    }

    [Serializable]
    public class ScrollRectEvent : UnityEvent<Vector2> { }

    [SerializeField]
    private RectTransform m_Content;
    public RectTransform content { get { return m_Content; } set { m_Content = value; } }

    [SerializeField]
    private MovementType m_MovementType = MovementType.Elastic;
    public MovementType movementType { get { return m_MovementType; } set { m_MovementType = value; } }

    [SerializeField]
    private float m_Elasticity = 0.1f; // Only used for MovementType.Elastic
    public float elasticity { get { return m_Elasticity; } set { m_Elasticity = value; } }

    [SerializeField]
    private bool m_Inertia = true;
    public bool inertia { get { return m_Inertia; } set { m_Inertia = value; } }

    [SerializeField]
    private float m_DecelerationRate = 0.135f; // Only used when inertia is enabled
    public float decelerationRate { get { return m_DecelerationRate; } set { m_DecelerationRate = value; } }

    [SerializeField]
    private float m_ScrollSensitivity = 1.0f;
    public float scrollSensitivity { get { return m_ScrollSensitivity; } set { m_ScrollSensitivity = value; } }

    [Header("Horizontal ScrollBar")]
    [SerializeField]
    private Scrollbar m_HorizontalScrollbar;
    public Scrollbar horizontalScrollbar
    {
        get
        {
            return m_HorizontalScrollbar;
        }
        set
        {
            if (m_HorizontalScrollbar)
                m_HorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
            m_HorizontalScrollbar = value;
            if (m_HorizontalScrollbar)
                m_HorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
            SetDirtyCaching();
        }
    }
    [SerializeField]
    private ScrollbarVisibility m_HorizontalScrollbarVisibility;
    public ScrollbarVisibility horizontalScrollbarVisibility { get { return m_HorizontalScrollbarVisibility; } set { m_HorizontalScrollbarVisibility = value; SetDirtyCaching(); } }

    [SerializeField]
    private float m_HorizontalScrollbarSpacing;
    public float horizontalScrollbarSpacing { get { return m_HorizontalScrollbarSpacing; } set { m_HorizontalScrollbarSpacing = value; SetDirty(); } }

    [Header("Vertical ScrollBar")]
    [Space(10)]
    [SerializeField]
    private Scrollbar m_VerticalScrollbar;
    public Scrollbar verticalScrollbar
    {
        get
        {
            return m_VerticalScrollbar;
        }
        set
        {
            if (m_VerticalScrollbar)
                m_VerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);
            m_VerticalScrollbar = value;
            if (m_VerticalScrollbar)
                m_VerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);
            SetDirtyCaching();
        }
    }


    [SerializeField]
    private ScrollbarVisibility m_VerticalScrollbarVisibility;
    public ScrollbarVisibility verticalScrollbarVisibility { get { return m_VerticalScrollbarVisibility; } set { m_VerticalScrollbarVisibility = value; SetDirtyCaching(); } }

    [SerializeField]
    private float m_VerticalScrollbarSpacing;
    public float verticalScrollbarSpacing { get { return m_VerticalScrollbarSpacing; } set { m_VerticalScrollbarSpacing = value; SetDirty(); } }

    [SerializeField]
    private ScrollRectEvent m_OnValueChanged = new ScrollRectEvent();
    public ScrollRectEvent onValueChanged { get { return m_OnValueChanged; } set { m_OnValueChanged = value; } }

    // The offset from handle position to mouse down position
    private Vector2 m_PointerStartLocalCursor = Vector2.zero;
    private Vector2 m_ContentStartPosition = Vector2.zero;

    private RectTransform m_ViewRect;

    protected RectTransform viewRect
    {
        get
        {
            if (m_ViewRect == null)
                m_ViewRect = (RectTransform)transform;
            return m_ViewRect;
        }
    }

    private Bounds m_ContentBounds;
    private Bounds m_ViewBounds;

    private Vector2 m_Velocity;
    public Vector2 velocity { get { return m_Velocity; } set { m_Velocity = value; } }

    private bool m_Dragging;

    private Vector2 m_PrevPosition = Vector2.zero;
    private Bounds m_PrevContentBounds;
    private Bounds m_PrevViewBounds;
    [NonSerialized]
    private bool m_HasRebuiltLayout = false;

    private bool m_HSliderExpand;
    private bool m_VSliderExpand;
    private float m_HSliderHeight;
    private float m_VSliderWidth;

    [System.NonSerialized]
    private RectTransform m_Rect;
    private RectTransform rectTransform
    {
        get
        {
            if (m_Rect == null)
                m_Rect = GetComponent<RectTransform>();
            return m_Rect;
        }
    }

    private RectTransform m_HorizontalScrollbarRect;
    private RectTransform m_VerticalScrollbarRect;

    private DrivenRectTransformTracker m_Tracker;

    protected CUGUIScrollView()
    {
        flexibleWidth = -1;
    }

    //==========LoopScrollRect==========

    public event UIFramework.del_OnInstantiate_CollectionItem OnInstantiate_CollectionItem;
    public event UIFramework.del_OnUpdate_CollectionItem OnUpdate_CollectionItem;

    public delegate void del_OnAfterUpdateScroll();
    public event del_OnAfterUpdateScroll OnAfterUpdate_Scroll;

    protected delegate float del_GetSize(RectTransform item);
    del_GetSize GetSize;

    protected delegate float del_GetDimension(Vector2 vector);
    del_GetDimension GetDimension;

    protected delegate Vector2 del_GetVector(float value);
    del_GetVector GetVector;

    protected delegate bool del_UpdateItems(Bounds viewBounds, Bounds contentBounds);
    del_UpdateItems UpdateItems;


    public void ClearCells()
    {
        if (Application.isPlaying)
        {
            itemTypeStart = 0;
            itemTypeEnd = 0;
            _iScrollItem_TotalCount = 0;
            for (int i = content.childCount - 1; i >= 0; i--)
            {
                ReturnScrollItem(content.GetChild(i).gameObject);
            }
        }
    }

    public void SrollToCell(int index, float speed)
    {
        if(_iScrollItem_TotalCount >= 0 && (index < 0 || index >= _iScrollItem_TotalCount))
        {
            Debug.LogWarningFormat("invalid index {0}", index);
            return;
        }
        if(speed <= 0)
        {
            Debug.LogWarningFormat("invalid speed {0}", speed);
            return;
        }
        StopAllCoroutines();
        StartCoroutine(ScrollToCellCoroutine(index, speed));
    }

    IEnumerator ScrollToCellCoroutine(int index, float speed)
    {
        bool needMoving = true;
        while(needMoving)
        {
            yield return null;
            if(!m_Dragging)
            {
                float move = 0;
                if(index < itemTypeStart)
                {
                    move = -Time.deltaTime * speed;
                }
                else if(index >= itemTypeEnd)
                {
                    move = Time.deltaTime * speed;
                }
                else
                {
                    m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
                    var m_ItemBounds = GetBounds4Item(index);
                    var offset = 0.0f;
                    if (directionSign == -1)
                        offset = reverseDirection ? (m_ViewBounds.min.y - m_ItemBounds.min.y) : (m_ViewBounds.max.y - m_ItemBounds.max.y);
                    else if (directionSign == 1)
                        offset = reverseDirection ? (m_ItemBounds.max.x - m_ViewBounds.max.x) : (m_ItemBounds.min.x - m_ViewBounds.min.x);
                    // check if we cannot move on
                    if (_iScrollItem_TotalCount >= 0)
                    {
                        if (offset > 0 && itemTypeEnd == _iScrollItem_TotalCount && !reverseDirection)
                        {
                            m_ItemBounds = GetBounds4Item(_iScrollItem_TotalCount - 1);
                            // reach bottom
                            if ((directionSign == -1 && m_ItemBounds.min.y > m_ViewBounds.min.y) ||
                                (directionSign == 1 && m_ItemBounds.max.x < m_ViewBounds.max.x))
                            {
                                needMoving = false;
                                break;
                            }
                        }
                        else if (offset < 0 && itemTypeStart == 0 && reverseDirection)
                        {
                            m_ItemBounds = GetBounds4Item(0);
                            if ((directionSign == -1 && m_ItemBounds.max.y < m_ViewBounds.max.y) ||
                                (directionSign == 1 && m_ItemBounds.min.x > m_ViewBounds.min.x))
                            {
                                needMoving = false;
                                break;
                            }
                        }
                    }

                    float maxMove = Time.deltaTime * speed;
                    if(Mathf.Abs(offset) < maxMove)
                    {
                        needMoving = false;
                        move = offset;
                        }
                        else
                        move = Mathf.Sign(offset) * maxMove;
                }
                if (move != 0)
                {
                    Vector2 offset = GetVector(move);
                    content.anchoredPosition += offset;
                    m_PrevPosition += offset;
                    m_ContentStartPosition += offset;
                }
            }
        }
        StopMovement();
        UpdatePrevData();
    }

    public void RefreshCells()
    {
        if (Application.isPlaying && this.isActiveAndEnabled)
        {
            itemTypeEnd = itemTypeStart;
            // recycle items if we can
            for (int i = 0; i < content.childCount; i++)
            {
                if (itemTypeEnd < _iScrollItem_TotalCount)
                {
                    ProvideData(content.GetChild(i).gameObject, itemTypeEnd);
                    itemTypeEnd++;
                }
                else
                {
                    ReturnScrollItem(content.GetChild(i).gameObject);
                    i--;
                }
            }
        }
    }

    public void RefillCellsFromEnd(int offset = 0)
    {
        if (!Application.isPlaying || prefabSource == null)
            return;
            
        StopMovement();
        itemTypeEnd = reverseDirection ? offset : _iScrollItem_TotalCount - offset;
        itemTypeStart = itemTypeEnd;

        if (_iScrollItem_TotalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
            Debug.LogWarning("Grid will become strange since we can't fill items in the last line");

        for (int i = m_Content.childCount - 1; i >= 0; i--)
        {
            ReturnScrollItem(m_Content.GetChild(i).gameObject);
        }

        float sizeToFill = 0, sizeFilled = 0;
        if (directionSign == -1)
            sizeToFill = viewRect.rect.size.y;
        else
            sizeToFill = viewRect.rect.size.x;
            
        while(sizeToFill > sizeFilled)
        {
            float size = reverseDirection ? NewItemAtEnd() : NewItemAtStart();
            if(size <= 0) break;
            sizeFilled += size;
        }

        Vector2 pos = m_Content.anchoredPosition;
        float dist = Mathf.Max(0, sizeFilled - sizeToFill);
        if (reverseDirection)
            dist = -dist;
        if (directionSign == -1)
            pos.y = dist;
        else if (directionSign == 1)
            pos.x = -dist;
        m_Content.anchoredPosition = pos;
    }

    public void RefillCells(int offset = 0)
    {
        if (!Application.isPlaying || prefabSource == null)
            return;

        StopMovement();
        itemTypeStart = reverseDirection ? _iScrollItem_TotalCount - offset : offset;
        itemTypeEnd = itemTypeStart;

        if (_iScrollItem_TotalCount >= 0 && itemTypeStart % contentConstraintCount != 0)
            Debug.LogWarning("Grid will become strange since we can't fill items in the first line");

        // Don't `Canvas.ForceUpdateCanvases();` here, or it will new/delete cells to change itemTypeStart/End
        for (int i = m_Content.childCount - 1; i >= 0; i--)
        {
            ReturnScrollItem(m_Content.GetChild(i).gameObject);
        }

        Update_ScrollItemCount();
    }

    private void Update_ScrollItemCount()
    {
        float sizeToFill = 0, sizeFilled = 0;
        // m_ViewBounds may be not ready when RefillCells on Start
        if (directionSign == -1)
            sizeToFill = viewRect.rect.size.y;
        else
            sizeToFill = viewRect.rect.size.x;

        while (sizeToFill > sizeFilled)
        {
            float size = reverseDirection ? NewItemAtStart() : NewItemAtEnd();
            if (size <= 0) break;
            sizeFilled += size;
        }

        Vector2 pos = m_Content.anchoredPosition;
        if (directionSign == -1)
            pos.y = 0;
        else if (directionSign == 1)
            pos.x = 0;
        m_Content.anchoredPosition = pos;

        OnAfterUpdate_Scroll?.Invoke();
    }

    protected float NewItemAtStart()
    {
        if (_iScrollItem_TotalCount >= 0 && itemTypeStart - contentConstraintCount < 0)
        {
            return 0;
        }
        float size = 0;
        for (int i = 0; i < contentConstraintCount; i++)
        {
            itemTypeStart--;
            RectTransform newItem = Instantiate_NextItem(itemTypeStart);
            newItem.SetAsFirstSibling();
            size = Mathf.Max(GetSize(newItem), size);
        }
        threshold = Mathf.Max(threshold, size * 1.5f);

        if (!reverseDirection)
        {
            Vector2 offset = GetVector(size);
            content.anchoredPosition += offset;
            m_PrevPosition += offset;
            m_ContentStartPosition += offset;
        }
            
        return size;
    }

    protected float DeleteItemAtStart()
    {
        // special case: when moving or dragging, we cannot simply delete start when we've reached the end
        if (((m_Dragging || m_Velocity != Vector2.zero) && _iScrollItem_TotalCount >= 0 && itemTypeEnd >= _iScrollItem_TotalCount - 1) 
            || content.childCount == 0)
        {
            return 0;
        }

        float size = 0;
        for (int i = 0; i < contentConstraintCount; i++)
        {
            RectTransform oldItem = content.GetChild(0) as RectTransform;
            size = Mathf.Max(GetSize(oldItem), size);
            ReturnScrollItem(oldItem.gameObject);

            itemTypeStart++;

            if (content.childCount == 0)
            {
                break;
            }
        }

        if (!reverseDirection)
        {
            Vector2 offset = GetVector(size);
            content.anchoredPosition -= offset;
            m_PrevPosition -= offset;
            m_ContentStartPosition -= offset;
        }
        return size;
    }


    protected float NewItemAtEnd()
    {
        if (_iScrollItem_TotalCount >= 0 && itemTypeEnd >= _iScrollItem_TotalCount)
        {
            return 0;
        }
        float size = 0;
        // issue 4: fill lines to end first
        int count = contentConstraintCount - (content.childCount % contentConstraintCount);
        for (int i = 0; i < count; i++)
        {
            RectTransform newItem = Instantiate_NextItem(itemTypeEnd);
            size = Mathf.Max(GetSize(newItem), size);
            itemTypeEnd++;
            if (_iScrollItem_TotalCount >= 0 && itemTypeEnd >= _iScrollItem_TotalCount)
            {
                break;
            }
        }
        threshold = Mathf.Max(threshold, size * 1.5f);

        if (reverseDirection)
        {
            Vector2 offset = GetVector(size);
            content.anchoredPosition -= offset;
            m_PrevPosition -= offset;
            m_ContentStartPosition -= offset;
        }
            
        return size;
    }

    protected float DeleteItemAtEnd()
    {
        if (((m_Dragging || m_Velocity != Vector2.zero) && _iScrollItem_TotalCount >= 0 && itemTypeStart < contentConstraintCount) 
            || content.childCount == 0)
        {
            return 0;
        }

        float size = 0;
        for (int i = 0; i < contentConstraintCount; i++)
        {
            RectTransform oldItem = content.GetChild(content.childCount - 1) as RectTransform;
            size = Mathf.Max(GetSize(oldItem), size);
            ReturnScrollItem(oldItem.gameObject);

            itemTypeEnd--;
            if (itemTypeEnd % contentConstraintCount == 0 || content.childCount == 0)
            {
                break;  //just delete the whole row
            }
        }

        if (reverseDirection)
        {
            Vector2 offset = GetVector(size);
            content.anchoredPosition += offset;
            m_PrevPosition += offset;
            m_ContentStartPosition += offset;
        }
        return size;
    }

    private RectTransform Instantiate_NextItem(int itemIdx)
    {            
        RectTransform nextItem = GetScrollItem().transform as RectTransform;
        nextItem.transform.SetParent(content, false);

        GameObject pObjectNextItem = nextItem.gameObject;
        pObjectNextItem.SetActive(true);
        ProvideData(pObjectNextItem, itemIdx);

        return nextItem;
    }
    //==========LoopScrollRect==========

    public virtual void Rebuild(CanvasUpdate executing)
    {
        if (executing == CanvasUpdate.Prelayout)
        {
            UpdateCachedData();
        }

        if (executing == CanvasUpdate.PostLayout)
        {
            UpdateBounds();
            UpdateScrollbars(Vector2.zero);
            UpdatePrevData();

            m_HasRebuiltLayout = true;
        }
    }

    public virtual void LayoutComplete()
    { }

    public virtual void GraphicUpdateComplete()
    { }

    void UpdateCachedData()
    {
        Transform transform = this.transform;
        m_HorizontalScrollbarRect = m_HorizontalScrollbar == null ? null : m_HorizontalScrollbar.transform as RectTransform;
        m_VerticalScrollbarRect = m_VerticalScrollbar == null ? null : m_VerticalScrollbar.transform as RectTransform;

        // These are true if either the elements are children, or they don't exist at all.
        bool viewIsChild = (viewRect.parent == transform);
        bool hScrollbarIsChild = (!m_HorizontalScrollbarRect || m_HorizontalScrollbarRect.parent == transform);
        bool vScrollbarIsChild = (!m_VerticalScrollbarRect || m_VerticalScrollbarRect.parent == transform);
        bool allAreChildren = (viewIsChild && hScrollbarIsChild && vScrollbarIsChild);

        m_HSliderExpand = allAreChildren && m_HorizontalScrollbarRect && horizontalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
        m_VSliderExpand = allAreChildren && m_VerticalScrollbarRect && verticalScrollbarVisibility == ScrollbarVisibility.AutoHideAndExpandViewport;
        m_HSliderHeight = (m_HorizontalScrollbarRect == null ? 0 : m_HorizontalScrollbarRect.rect.height);
        m_VSliderWidth = (m_VerticalScrollbarRect == null ? 0 : m_VerticalScrollbarRect.rect.width);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        if (m_HorizontalScrollbar)
            m_HorizontalScrollbar.onValueChanged.AddListener(SetHorizontalNormalizedPosition);
        if (m_VerticalScrollbar)
            m_VerticalScrollbar.onValueChanged.AddListener(SetVerticalNormalizedPosition);

        CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
    }

    protected override void OnDisable()
    {
        CanvasUpdateRegistry.UnRegisterCanvasElementForRebuild(this);

        if (m_HorizontalScrollbar)
            m_HorizontalScrollbar.onValueChanged.RemoveListener(SetHorizontalNormalizedPosition);
        if (m_VerticalScrollbar)
            m_VerticalScrollbar.onValueChanged.RemoveListener(SetVerticalNormalizedPosition);

        m_HasRebuiltLayout = false;
        m_Tracker.Clear();
        m_Velocity = Vector2.zero;
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
        base.OnDisable();
    }

    public override bool IsActive()
    {
        return base.IsActive() && m_Content != null;
    }

    private void EnsureLayoutHasRebuilt()
    {
        if (!m_HasRebuiltLayout && !CanvasUpdateRegistry.IsRebuildingLayout())
            Canvas.ForceUpdateCanvases();
    }

    public virtual void StopMovement()
    {
        m_Velocity = Vector2.zero;
    }

    public virtual void OnScroll(PointerEventData data)
    {
        if (!IsActive())
            return;

        EnsureLayoutHasRebuilt();
        UpdateBounds();

        Vector2 delta = data.scrollDelta;
        // Down is positive for scroll events, while in UI system up is positive.
        delta.y *= -1;
        if (eScrollDirection == EScrollDirection.Vertical)
        {
            if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                delta.y = delta.x;
            delta.x = 0;
        }
        if (eScrollDirection == EScrollDirection.Horizontal)
        {
            if (Mathf.Abs(delta.y) > Mathf.Abs(delta.x))
                delta.x = delta.y;
            delta.y = 0;
        }

        Vector2 position = m_Content.anchoredPosition;
        position += delta * m_ScrollSensitivity;
        if (m_MovementType == MovementType.Clamped)
            position += CalculateOffset(position - m_Content.anchoredPosition);

        SetContentAnchoredPosition(position);
        UpdateBounds();
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        m_Velocity = Vector2.zero;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!IsActive())
            return;

        UpdateBounds();

        m_PointerStartLocalCursor = Vector2.zero;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out m_PointerStartLocalCursor);
        m_ContentStartPosition = m_Content.anchoredPosition;
        m_Dragging = true;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        m_Dragging = false;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (!IsActive())
            return;

        Vector2 localCursor;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(viewRect, eventData.position, eventData.pressEventCamera, out localCursor))
            return;

        UpdateBounds();

        var pointerDelta = localCursor - m_PointerStartLocalCursor;
        Vector2 position = m_ContentStartPosition + pointerDelta;

        // Offset to get content into place in the view.
        Vector2 offset = CalculateOffset(position - m_Content.anchoredPosition);
        position += offset;
        if (m_MovementType == MovementType.Elastic)
        {
            //==========LoopScrollRect==========
            if (offset.x != 0)
                position.x = position.x - RubberDelta(offset.x, m_ViewBounds.size.x) * rubberScale;
            if (offset.y != 0)
                position.y = position.y - RubberDelta(offset.y, m_ViewBounds.size.y) * rubberScale;
            //==========LoopScrollRect==========
        }

        SetContentAnchoredPosition(position);
    }

    protected virtual void SetContentAnchoredPosition(Vector2 position)
    {
        if (eScrollDirection == EScrollDirection.Vertical)
            position.x = m_Content.anchoredPosition.x;
        if (eScrollDirection == EScrollDirection.Horizontal)
            position.y = m_Content.anchoredPosition.y;

        if (position != m_Content.anchoredPosition)
        {
            m_Content.anchoredPosition = position;
            UpdateBounds(true);
        }
    }

    protected virtual void LateUpdate()
    {
        if (!m_Content)
            return;

        if (prefabSource == null)
            return;

        EnsureLayoutHasRebuilt();
        UpdateScrollbarVisibility();
        UpdateBounds();
        float deltaTime = Time.unscaledDeltaTime;
        Vector2 offset = CalculateOffset(Vector2.zero);
        if (!m_Dragging && (offset != Vector2.zero || m_Velocity != Vector2.zero))
        {
            Vector2 position = m_Content.anchoredPosition;
            for (int axis = 0; axis < 2; axis++)
            {
                // Apply spring physics if movement is elastic and content has an offset from the view.
                if (m_MovementType == MovementType.Elastic && offset[axis] != 0)
                {
                    float speed = m_Velocity[axis];
                    position[axis] = Mathf.SmoothDamp(m_Content.anchoredPosition[axis], m_Content.anchoredPosition[axis] + offset[axis], ref speed, m_Elasticity, Mathf.Infinity, deltaTime);
                    m_Velocity[axis] = speed;
                }
                // Else move content according to velocity with deceleration applied.
                else if (m_Inertia)
                {
                    m_Velocity[axis] *= Mathf.Pow(m_DecelerationRate, deltaTime);
                    if (Mathf.Abs(m_Velocity[axis]) < 1)
                        m_Velocity[axis] = 0;
                    position[axis] += m_Velocity[axis] * deltaTime;
                }
                // If we have neither elaticity or friction, there shouldn't be any velocity.
                else
                {
                    m_Velocity[axis] = 0;
                }
            }

            if (m_Velocity != Vector2.zero)
            {
                if (m_MovementType == MovementType.Clamped)
                {
                    offset = CalculateOffset(position - m_Content.anchoredPosition);
                    position += offset;
                }

                SetContentAnchoredPosition(position);
            }
        }

        if (m_Dragging && m_Inertia)
        {
            Vector3 newVelocity = (m_Content.anchoredPosition - m_PrevPosition) / deltaTime;
            m_Velocity = Vector3.Lerp(m_Velocity, newVelocity, deltaTime * 10);
        }

        if (m_ViewBounds != m_PrevViewBounds || m_ContentBounds != m_PrevContentBounds || m_Content.anchoredPosition != m_PrevPosition)
        {
            UpdateScrollbars(offset);
            m_OnValueChanged.Invoke(normalizedPosition);
            UpdatePrevData();
        }
    }

    private void UpdatePrevData()
    {
        if (m_Content == null)
            m_PrevPosition = Vector2.zero;
        else
            m_PrevPosition = m_Content.anchoredPosition;
        m_PrevViewBounds = m_ViewBounds;
        m_PrevContentBounds = m_ContentBounds;
    }

    private void UpdateScrollbars(Vector2 offset)
    {
        if (m_HorizontalScrollbar)
        {
            //==========LoopScrollRect==========
            if (m_ContentBounds.size.x > 0 && _iScrollItem_TotalCount > 0)
            {
                m_HorizontalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.x - Mathf.Abs(offset.x)) / m_ContentBounds.size.x * CurrentLines / TotalLines);
            }
            //==========LoopScrollRect==========
            else
                m_HorizontalScrollbar.size = 1;

            m_HorizontalScrollbar.value = horizontalNormalizedPosition;
        }

        if (m_VerticalScrollbar)
        {
            //==========LoopScrollRect==========
            if (m_ContentBounds.size.y > 0 && _iScrollItem_TotalCount > 0)
            {
                m_VerticalScrollbar.size = Mathf.Clamp01((m_ViewBounds.size.y - Mathf.Abs(offset.y)) / m_ContentBounds.size.y * CurrentLines / TotalLines);
            }
            //==========LoopScrollRect==========
            else
                m_VerticalScrollbar.size = 1;

            m_VerticalScrollbar.value = verticalNormalizedPosition;
        }
    }

    public Vector2 normalizedPosition
    {
        get
        {
            return new Vector2(horizontalNormalizedPosition, verticalNormalizedPosition);
        }
        set
        {
            SetNormalizedPosition(value.x, 0);
            SetNormalizedPosition(value.y, 1);
        }
    }

    public float horizontalNormalizedPosition
    {
        get
        {
            UpdateBounds();
            //==========LoopScrollRect==========
            if(_iScrollItem_TotalCount > 0 && itemTypeEnd > itemTypeStart)
            {
                //TODO: consider contentSpacing
                float elementSize = m_ContentBounds.size.x / CurrentLines;
                float totalSize = elementSize * TotalLines;
                float offset = m_ContentBounds.min.x - elementSize * StartLine;
                    
                if (totalSize <= m_ViewBounds.size.x)
                    return (m_ViewBounds.min.x > offset) ? 1 : 0;
                return (m_ViewBounds.min.x - offset) / (totalSize - m_ViewBounds.size.x);
            }
            else
                return 0.5f;
            //==========LoopScrollRect==========
        }
        set
        {
            SetNormalizedPosition(value, 0);
        }
    }

    public float verticalNormalizedPosition
    {
        get
        {
            UpdateBounds();
            //==========LoopScrollRect==========
            if(_iScrollItem_TotalCount > 0 && itemTypeEnd > itemTypeStart)
            {
                //TODO: consider contentSpacinge
                float elementSize = m_ContentBounds.size.y / CurrentLines;
                float totalSize = elementSize * TotalLines;
                float offset = m_ContentBounds.max.y + elementSize * StartLine;

                if (totalSize <= m_ViewBounds.size.y)
                    return (offset > m_ViewBounds.max.y) ? 1 : 0;
                return (offset - m_ViewBounds.max.y) / (totalSize - m_ViewBounds.size.y);
            }
            else
                return 0.5f;
            //==========LoopScrollRect==========
        }
        set
        {
            SetNormalizedPosition(value, 1);
        }
    }
        
    private void SetHorizontalNormalizedPosition(float value) { SetNormalizedPosition(value, 0); }
    private void SetVerticalNormalizedPosition(float value) { SetNormalizedPosition(value, 1); }

    private void SetNormalizedPosition(float value, int axis)
    {
        //==========LoopScrollRect==========
        if (_iScrollItem_TotalCount <= 0 || itemTypeEnd <= itemTypeStart)
            return;
        //==========LoopScrollRect==========

        EnsureLayoutHasRebuilt();
        UpdateBounds();

        //==========LoopScrollRect==========
        Vector3 localPosition = m_Content.localPosition;
        float newLocalPosition = localPosition[axis];
        if (axis == 0)
        {
            float elementSize = m_ContentBounds.size.x / CurrentLines;
            float totalSize = elementSize * TotalLines;
            float offset = m_ContentBounds.min.x - elementSize * StartLine;

            newLocalPosition += m_ViewBounds.min.x - value * (totalSize - m_ViewBounds.size[axis]) - offset;
        }
        else if(axis == 1)
        {
            float elementSize = m_ContentBounds.size.y / CurrentLines;
            float totalSize = elementSize * TotalLines;
            float offset = m_ContentBounds.max.y + elementSize * StartLine;

            newLocalPosition -= offset - value * (totalSize - m_ViewBounds.size.y) - m_ViewBounds.max.y;
        }
        //==========LoopScrollRect==========

        if (Mathf.Abs(localPosition[axis] - newLocalPosition) > 0.01f)
        {
            localPosition[axis] = newLocalPosition;
            m_Content.localPosition = localPosition;
            m_Velocity[axis] = 0;
            UpdateBounds(true);
        }
    }

    private static float RubberDelta(float overStretching, float viewSize)
    {
        return (1 - (1 / ((Mathf.Abs(overStretching) * 0.55f / viewSize) + 1))) * viewSize * Mathf.Sign(overStretching);
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }

    private bool hScrollingNeeded
    {
        get
        {
            if (Application.isPlaying)
                return m_ContentBounds.size.x > m_ViewBounds.size.x + 0.01f;
            return true;
        }
    }
    private bool vScrollingNeeded
    {
        get
        {
            if (Application.isPlaying)
                return m_ContentBounds.size.y > m_ViewBounds.size.y + 0.01f;
            return true;
        }
    }

    public virtual void CalculateLayoutInputHorizontal() { }
    public virtual void CalculateLayoutInputVertical() { }

    public virtual float minWidth { get { return -1; } }
    public virtual float preferredWidth { get { return -1; } }
    public virtual float flexibleWidth { get; private set; }

    public virtual float minHeight { get { return -1; } }
    public virtual float preferredHeight { get { return -1; } }
    public virtual float flexibleHeight { get { return -1; } }

    public virtual int layoutPriority { get { return -1; } }

    public virtual void SetLayoutHorizontal()
    {
        m_Tracker.Clear();

        if (m_HSliderExpand || m_VSliderExpand)
        {
            m_Tracker.Add(this, viewRect,
                DrivenTransformProperties.Anchors |
                DrivenTransformProperties.SizeDelta |
                DrivenTransformProperties.AnchoredPosition);

            // Make view full size to see if content fits.
            viewRect.anchorMin = Vector2.zero;
            viewRect.anchorMax = Vector2.one;
            viewRect.sizeDelta = Vector2.zero;
            viewRect.anchoredPosition = Vector2.zero;

            // Recalculate content layout with this size to see if it fits when there are no scrollbars.
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();
        }

        // If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
        if (m_VSliderExpand && vScrollingNeeded)
        {
            viewRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), viewRect.sizeDelta.y);

            // Recalculate content layout with this size to see if it fits vertically
            // when there is a vertical scrollbar (which may reflowed the content to make it taller).
            LayoutRebuilder.ForceRebuildLayoutImmediate(content);
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();
        }

        // If it doesn't fit horizontally, enable horizontal scrollbar and shrink view vertically to make room for it.
        if (m_HSliderExpand && hScrollingNeeded)
        {
            viewRect.sizeDelta = new Vector2(viewRect.sizeDelta.x, -(m_HSliderHeight + m_HorizontalScrollbarSpacing));
            m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
            m_ContentBounds = GetBounds();
        }

        // If the vertical slider didn't kick in the first time, and the horizontal one did,
        // we need to check again if the vertical slider now needs to kick in.
        // If it doesn't fit vertically, enable vertical scrollbar and shrink view horizontally to make room for it.
        if (m_VSliderExpand && vScrollingNeeded && viewRect.sizeDelta.x == 0 && viewRect.sizeDelta.y < 0)
        {
            viewRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), viewRect.sizeDelta.y);
        }
    }

    public virtual void SetLayoutVertical()
    {
        UpdateScrollbarLayout();
        m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
        m_ContentBounds = GetBounds();
    }

    void UpdateScrollbarVisibility()
    {
        if (m_VerticalScrollbar && m_VerticalScrollbarVisibility != ScrollbarVisibility.Permanent && m_VerticalScrollbar.gameObject.activeSelf != vScrollingNeeded)
            m_VerticalScrollbar.gameObject.SetActive(vScrollingNeeded);

        if (m_HorizontalScrollbar && m_HorizontalScrollbarVisibility != ScrollbarVisibility.Permanent && m_HorizontalScrollbar.gameObject.activeSelf != hScrollingNeeded)
            m_HorizontalScrollbar.gameObject.SetActive(hScrollingNeeded);
    }

    void UpdateScrollbarLayout()
    {
        if (m_VSliderExpand && m_HorizontalScrollbar)
        {
            m_Tracker.Add(this, m_HorizontalScrollbarRect,
                            DrivenTransformProperties.AnchorMinX |
                            DrivenTransformProperties.AnchorMaxX |
                            DrivenTransformProperties.SizeDeltaX |
                            DrivenTransformProperties.AnchoredPositionX);
            m_HorizontalScrollbarRect.anchorMin = new Vector2(0, m_HorizontalScrollbarRect.anchorMin.y);
            m_HorizontalScrollbarRect.anchorMax = new Vector2(1, m_HorizontalScrollbarRect.anchorMax.y);
            m_HorizontalScrollbarRect.anchoredPosition = new Vector2(0, m_HorizontalScrollbarRect.anchoredPosition.y);
            if (vScrollingNeeded)
                m_HorizontalScrollbarRect.sizeDelta = new Vector2(-(m_VSliderWidth + m_VerticalScrollbarSpacing), m_HorizontalScrollbarRect.sizeDelta.y);
            else
                m_HorizontalScrollbarRect.sizeDelta = new Vector2(0, m_HorizontalScrollbarRect.sizeDelta.y);
        }

        if (m_HSliderExpand && m_VerticalScrollbar)
        {
            m_Tracker.Add(this, m_VerticalScrollbarRect,
                            DrivenTransformProperties.AnchorMinY |
                            DrivenTransformProperties.AnchorMaxY |
                            DrivenTransformProperties.SizeDeltaY |
                            DrivenTransformProperties.AnchoredPositionY);
            m_VerticalScrollbarRect.anchorMin = new Vector2(m_VerticalScrollbarRect.anchorMin.x, 0);
            m_VerticalScrollbarRect.anchorMax = new Vector2(m_VerticalScrollbarRect.anchorMax.x, 1);
            m_VerticalScrollbarRect.anchoredPosition = new Vector2(m_VerticalScrollbarRect.anchoredPosition.x, 0);
            if (hScrollingNeeded)
                m_VerticalScrollbarRect.sizeDelta = new Vector2(m_VerticalScrollbarRect.sizeDelta.x, -(m_HSliderHeight + m_HorizontalScrollbarSpacing));
            else
                m_VerticalScrollbarRect.sizeDelta = new Vector2(m_VerticalScrollbarRect.sizeDelta.x, 0);
        }
    }

    private void UpdateBounds(bool updateItems = false)
    {
        m_ViewBounds = new Bounds(viewRect.rect.center, viewRect.rect.size);
        m_ContentBounds = GetBounds();

        if (m_Content == null)
            return;

        // ============LoopScrollRect============
        // Don't do this in Rebuild
        if (Application.isPlaying && updateItems && UpdateItems(m_ViewBounds, m_ContentBounds))
        {
            Canvas.ForceUpdateCanvases();
            m_ContentBounds = GetBounds();
        }
        // ============LoopScrollRect============

        // Make sure content bounds are at least as large as view by adding padding if not.
        // One might think at first that if the content is smaller than the view, scrolling should be allowed.
        // However, that's not how scroll views normally work.
        // Scrolling is *only* possible when content is *larger* than view.
        // We use the pivot of the content rect to decide in which directions the content bounds should be expanded.
        // E.g. if pivot is at top, bounds are expanded downwards.
        // This also works nicely when ContentSizeFitter is used on the content.
        Vector3 contentSize = m_ContentBounds.size;
        Vector3 contentPos = m_ContentBounds.center;
        Vector3 excess = m_ViewBounds.size - contentSize;
        if (excess.x > 0)
        {
            contentPos.x -= excess.x * (m_Content.pivot.x - 0.5f);
            contentSize.x = m_ViewBounds.size.x;
        }
        if (excess.y > 0)
        {
            contentPos.y -= excess.y * (m_Content.pivot.y - 0.5f);
            contentSize.y = m_ViewBounds.size.y;
        }

        m_ContentBounds.size = contentSize;
        m_ContentBounds.center = contentPos;
    }

    private readonly Vector3[] m_Corners = new Vector3[4];
    private Bounds GetBounds()
    {
        if (m_Content == null)
            return new Bounds();

        var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        var toLocal = viewRect.worldToLocalMatrix;
        m_Content.GetWorldCorners(m_Corners);
        for (int j = 0; j < 4; j++)
        {
            Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
            vMin = Vector3.Min(v, vMin);
            vMax = Vector3.Max(v, vMax);
        }

        var bounds = new Bounds(vMin, Vector3.zero);
        bounds.Encapsulate(vMax);
        return bounds;
    }

    private Bounds GetBounds4Item(int index)
    {
        if (m_Content == null)
            return new Bounds();

        var vMin = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var vMax = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        var toLocal = viewRect.worldToLocalMatrix;
        int offset = index - itemTypeStart;
        if (offset < 0 || offset >= m_Content.childCount)
            return new Bounds();
        var rt = m_Content.GetChild(offset) as RectTransform;
        if (rt == null)
            return new Bounds();
        rt.GetWorldCorners(m_Corners);
        for (int j = 0; j < 4; j++)
        {
            Vector3 v = toLocal.MultiplyPoint3x4(m_Corners[j]);
            vMin = Vector3.Min(v, vMin);
            vMax = Vector3.Max(v, vMax);
        }

        var bounds = new Bounds(vMin, Vector3.zero);
        bounds.Encapsulate(vMax);
        return bounds;
    }

    private Vector2 CalculateOffset(Vector2 delta)
    {
        Vector2 offset = Vector2.zero;
        if (m_MovementType == MovementType.Unrestricted)
            return offset;

        Vector2 min = m_ContentBounds.min;
        Vector2 max = m_ContentBounds.max;

        if (eScrollDirection == EScrollDirection.Horizontal)
        {
            min.x += delta.x;
            max.x += delta.x;
            if (min.x > m_ViewBounds.min.x)
                offset.x = m_ViewBounds.min.x - min.x;
            else if (max.x < m_ViewBounds.max.x)
                offset.x = m_ViewBounds.max.x - max.x;
        }

        if (eScrollDirection == EScrollDirection.Vertical)
        {
            min.y += delta.y;
            max.y += delta.y;
            if (max.y < m_ViewBounds.max.y)
                offset.y = m_ViewBounds.max.y - max.y;
            else if (min.y > m_ViewBounds.min.y)
                offset.y = m_ViewBounds.min.y - min.y;
        }

        return offset;
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

    protected void SetDirtyCaching()
    {
        if (!IsActive())
            return;

        CanvasUpdateRegistry.RegisterCanvasElementForLayoutRebuild(this);
        LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirtyCaching();
    }
#endif

    bool _bIsExecute_Awake = false;

    public void DoAwake()
    {
        Awake();
    }

    protected override void Awake()
    {
        if (_bIsExecute_Awake)
            return;
        _bIsExecute_Awake = true;

        GameObject pObjectPool = new GameObject("Pool");
        pObjectPool.transform.parent = transform;
        _pPoolParents = pObjectPool.AddComponent<RectTransform>();

        if (eScrollDirection == EScrollDirection.Horizontal)
        {
            GetSize = GetSize_Horizontal;
            GetDimension = GetDimension_Horizontal;
            GetVector = GetVector_Horizontal;
            UpdateItems = UpdateItems_Horizontal;

            Awake_Horizontal();
        }
        else if (eScrollDirection == EScrollDirection.Vertical)
        {
            GetSize = GetSize_Vertical;
            GetDimension = GetDimension_Vertical;
            GetVector = GetVector_Vertical;
            UpdateItems = UpdateItems_Vertical;

            Awake_Vertical();
        }
    }

    #region Horizontal

    protected float GetSize_Horizontal(RectTransform item)
    {
        float size = contentSpacing;
        if (m_GridLayout != null)
        {
            size += m_GridLayout.cellSize.x;
        }
        else
        {
            size += LayoutUtility.GetPreferredWidth(item);
        }
        return size;
    }

    protected float GetDimension_Horizontal(Vector2 vector)
    {
        return -vector.x;
    }

    protected Vector2 GetVector_Horizontal(float value)
    {
        return new Vector2(-value, 0);
    }

    protected void Awake_Horizontal()
    {
        base.Awake();
        directionSign = 1;

        GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
        if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedRowCount)
        {
            Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
        }
    }

    protected bool UpdateItems_Horizontal(Bounds viewBounds, Bounds contentBounds)
    {
        if (prefabSource == null)
            return false;

        bool changed = false;
        if (viewBounds.max.x > contentBounds.max.x)
        {
            float size = NewItemAtEnd(), totalSize = size;
            while (size > 0 && viewBounds.max.x > contentBounds.max.x + totalSize)
            {
                size = NewItemAtEnd();
                totalSize += size;
            }
            if (totalSize > 0)
                changed = true;
        }
        else if (viewBounds.max.x < contentBounds.max.x - threshold)
        {
            float size = DeleteItemAtEnd(), totalSize = size;
            while (size > 0 && viewBounds.max.x < contentBounds.max.x - threshold - totalSize)
            {
                size = DeleteItemAtEnd();
                totalSize += size;
            }
            if (totalSize > 0)
                changed = true;
        }

        if (viewBounds.min.x < contentBounds.min.x)
        {
            float size = NewItemAtStart(), totalSize = size;
            while (size > 0 && viewBounds.min.x < contentBounds.min.x - totalSize)
            {
                size = NewItemAtStart();
                totalSize += size;
            }
            if (totalSize > 0)
                changed = true;
        }
        else if (viewBounds.min.x > contentBounds.min.x + threshold)
        {
            float size = DeleteItemAtStart(), totalSize = size;
            while (size > 0 && viewBounds.min.x > contentBounds.min.x + threshold + totalSize)
            {
                size = DeleteItemAtStart();
                totalSize += size;
            }
            if (totalSize > 0)
                changed = true;
        }
        return changed;
    }

    #endregion

    #region Vertical

    protected float GetSize_Vertical(RectTransform item)
    {
        float size = contentSpacing;
        if (m_GridLayout != null)
        {
            size += m_GridLayout.cellSize.y;
        }
        else
        {
            size += LayoutUtility.GetPreferredHeight(item);
        }
        return size;
    }

    protected float GetDimension_Vertical(Vector2 vector)
    {
        return vector.y;
    }

    protected Vector2 GetVector_Vertical(float value)
    {
        return new Vector2(0, value);
    }

    protected void Awake_Vertical()
    {
        directionSign = -1;

        GridLayoutGroup layout = content.GetComponent<GridLayoutGroup>();
        if (layout != null && layout.constraint != GridLayoutGroup.Constraint.FixedColumnCount)
        {
            Debug.LogError("[LoopHorizontalScrollRect] unsupported GridLayoutGroup constraint");
        }
    }

    protected bool UpdateItems_Vertical(Bounds viewBounds, Bounds contentBounds)
    {
        if (prefabSource == null)
            return false;

        bool changed = false;
        if (viewBounds.min.y < contentBounds.min.y)
        {
            float size = NewItemAtEnd(), totalSize = size;
            while (size > 0 && viewBounds.min.y < contentBounds.min.y - totalSize)
            {
                size = NewItemAtEnd();
                totalSize += size;
            }
            if (totalSize > 0)
                changed = true;
        }
        else if (viewBounds.min.y > contentBounds.min.y + threshold)
        {
            float size = DeleteItemAtEnd(), totalSize = size;
            while (size > 0 && viewBounds.min.y > contentBounds.min.y + threshold + totalSize)
            {
                size = DeleteItemAtEnd();
                totalSize += size;
            }
            if (totalSize > 0)
                changed = true;
        }

        if (viewBounds.max.y > contentBounds.max.y)
        {
            float size = NewItemAtStart(), totalSize = size;
            while (size > 0 && viewBounds.max.y > contentBounds.max.y + totalSize)
            {
                size = NewItemAtStart();
                totalSize += size;
            }
            if (totalSize > 0)
                changed = true;
        }
        else if (viewBounds.max.y < contentBounds.max.y - threshold)
        {
            float size = DeleteItemAtStart(), totalSize = size;
            while (size > 0 && viewBounds.max.y < contentBounds.max.y - threshold - totalSize)
            {
                size = DeleteItemAtStart();
                totalSize += size;
            }
            if (totalSize > 0)
                changed = true;
        }
        return changed;
    }

    #endregion

    #region Pooling

    List<GameObject> _listScrollItem_Instance = new List<GameObject>();
    List<GameObject> _listScrollItem_Pool = new List<GameObject>();
    Transform _pPoolParents;

    void ReturnScrollItem(GameObject pScrollItem)
    {
        pScrollItem.transform.SetParent(_pPoolParents, false);
        pScrollItem.SetActive(false);
        _listScrollItem_Pool.Add(pScrollItem);
    }

    GameObject GetScrollItem()
    {
        if(_listScrollItem_Pool.Count < 1)
        {
            int iCurrentCount = _listScrollItem_Instance.Count;
            if(iCurrentCount < 1)
                iCurrentCount = iInitPoolCount;

            for (int i = 0; i < iCurrentCount; i++)
            {
                GameObject pObjectCopy = Instantiate(prefabSource);
                pObjectCopy.transform.SetParent(_pPoolParents, false);
                pObjectCopy.SetActive(false);

                _listScrollItem_Pool.Add(pObjectCopy);
                _listScrollItem_Instance.Add(pObjectCopy);

                OnInstantiate_CollectionItem?.Invoke(pObjectCopy);
            }

#if UNITY_EDITOR

            if(bPrintDebug)
                Debug.Log(name + " Extand Pool Capacity - Source Name : " + prefabSource.name + " Capacity : " + iCurrentCount + " => " + _listScrollItem_Instance.Count, this);
            _pPoolParents.name = "Pool_Size:" + _listScrollItem_Instance.Count;
#endif
        }

        GameObject pLast = _listScrollItem_Pool[_listScrollItem_Pool.Count - 1];
        _listScrollItem_Pool.RemoveAt(_listScrollItem_Pool.Count - 1);
        pLast.SetActive(true);

        return pLast;
    }

    #endregion

    #region ScrollItem

    void ProvideData(GameObject pScrollObject, int iIndex)
    {
        pScrollObject.SendMessage(nameof(ICollectionItem.ICollectionItem_Update), iIndex, SendMessageOptions.DontRequireReceiver);
        OnUpdate_CollectionItem?.Invoke(pScrollObject, iIndex);
    }

    #endregion

    #region ListView

    public void DoCreate_ScrollItem(int iScrollItemCount)
    {
        _bIsExecute_Awake = false;
        DoAwake();

        _iScrollItem_TotalCount = iScrollItemCount;
        Update_ScrollItemCount();
    }

    public void ICollectionView_UpdateItem(int iItemCount)
    {
        DoAwake();

        _iScrollItem_TotalCount = iItemCount;
        RefillCells();
    }


    public void DoAllClearEvent_OnUpdate_CollectionItem()
    {
        OnUpdate_CollectionItem = null;
    }

    public void DoAllClearEvent_OnInstantiate_CollectionItem()
    {
        OnInstantiate_CollectionItem = null;
    }

    #endregion
}

#if UNITY_EDITOR

[CustomEditor(typeof(CUGUIScrollView), true)]
public class CUGUIScrollView_Inspector : Editor
{
    int index = 0;
    float speed = 1000;
    int iTestCount;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        EditorGUILayout.Space();

        CUGUIScrollView scroll = (CUGUIScrollView)target;

        //iTestCount = EditorGUILayout.IntField("Test Object Count : ", iTestCount);
        //if (GUILayout.Button("Create Scroll Item"))
        //{
        //    scroll.DoCreate_ScrollItem(iTestCount);
        //}

        GUI.enabled = Application.isPlaying;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Clear"))
        {
            scroll.ClearCells();
        }
        if (GUILayout.Button("Refresh"))
        {
            scroll.RefreshCells();
        }
        if (GUILayout.Button("Refill"))
        {
            scroll.RefillCells();
        }
        if (GUILayout.Button("RefillFromEnd"))
        {
            scroll.RefillCellsFromEnd();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUIUtility.labelWidth = 45;
        float w = (EditorGUIUtility.currentViewWidth - 100) / 2;
        EditorGUILayout.BeginHorizontal();
        index = EditorGUILayout.IntField("Index", index, GUILayout.Width(w));
        speed = EditorGUILayout.FloatField("Speed", speed, GUILayout.Width(w));
        if (GUILayout.Button("Scroll", GUILayout.Width(45)))
        {
            scroll.SrollToCell(index, speed);
        }
        EditorGUILayout.EndHorizontal();
    }
}


#endif