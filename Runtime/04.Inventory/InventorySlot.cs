#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-30
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.EventSystems;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class InventorySlot : UIWidgetObjectBase, IPointerEnterHandler, IPointerClickHandler, IDragHandler, IEndDragHandler
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public struct OnChangeSlotData_Msg
        {
            public InventorySlot pSlot { get; private set; }
            public object pData_Prev { get; private set; }
            public object pData_Current { get; private set; }
            public bool bSlot_IsEmpty { get; private set; }

            public OnChangeSlotData_Msg(InventorySlot pSlot, object pData_Prev, object pData_Current)
            {
                this.pSlot = pSlot; this.pData_Prev = pData_Prev; this.pData_Current = pData_Current;
                bSlot_IsEmpty = pData_Current == null;
            }
        }

        public struct OnSwapSlot_Msg
        {
            public Inventory pInventory_OnDraging;
            public InventorySlot pSlot_OnDraging;

            public Inventory pInventory_Dest;
            public InventorySlot pSlot_Dest;

            public OnSwapSlot_Msg(Inventory pInventory_OnDraging, InventorySlot pSlot_OnDraging, Inventory pInventory_Dest, InventorySlot pSlot_Dest)
            {
                this.pInventory_OnDraging = pInventory_OnDraging; this.pSlot_OnDraging = pSlot_OnDraging; this.pInventory_Dest = pInventory_Dest; this.pSlot_Dest = pSlot_Dest;
            }
        }

        /* public - Field declaration               */

        public event System.Action<InventorySlot, PointerEventData> OnClickedSlot;
        public event System.Action<InventorySlot, PointerEventData> OnHoverSlot;

        public event System.Action<InventorySlot, PointerEventData> OnDragSlot;
        public event System.Action<InventorySlot, PointerEventData> OnDragEndSlot;

        public event System.Action<OnSwapSlot_Msg> OnSwapSlot;

        public delegate void delOnChangeSlotData(OnChangeSlotData_Msg sMsg);
        public event delOnChangeSlotData OnChange_SlotData;
        public event System.Action<bool> OnChange_IsSelected;

        public bool bIsSelected { get; private set; }
        public object pData { get; private set; }

        [Header("디버깅 유무")]
        public bool bIsDebug = false;

        [Header("드래그 유무")]
        public bool bIsDragAble = false;

        public int iSlotIndex;

        /* protected & private - Field declaration  */

        static List<RaycastResult> g_listHit = new List<RaycastResult>();

        Inventory _pInventory;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoInit(Inventory pInventory)
        {
            _pInventory = pInventory;
            Event_SetSelected(false);
        }

        public void DoSetData(object pData)
        {
            if (bIsDebug)
                Debug.Log($"{name}-{iSlotIndex} {nameof(DoSetData)} - {pData.ToString()}", this);

            OnChange_SlotData?.Invoke(new OnChangeSlotData_Msg(this, this.pData, pData));

            this.pData = pData;
        }

        public void DoClear()
        {
            if (bIsDebug)
                Debug.Log($"{name}-{iSlotIndex} {nameof(DoClear)}", this);

            OnChange_SlotData?.Invoke(new OnChangeSlotData_Msg(this, pData, null));

            pData = null;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (bIsDebug)
                Debug.Log($"{name}-{iSlotIndex} {nameof(OnPointerClick)}", this);

            OnClickedSlot?.Invoke(this, eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (bIsDebug)
                Debug.Log($"{name}-{iSlotIndex} {nameof(OnPointerEnter)}", this);

            OnHoverSlot?.Invoke(this, eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (bIsDebug)
                Debug.Log($"{name}-{iSlotIndex} {nameof(OnDrag)}", this);

            if (pData == null)
                return;

            OnDragSlot?.Invoke(this, eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (bIsDebug)
                Debug.Log($"{name}-{iSlotIndex} {nameof(OnEndDrag)}", this);

            EventSystem.current.RaycastAll(eventData, g_listHit);
            for(int i = 0; i < g_listHit.Count; i++)
            {
                InventorySlot pSlot = g_listHit[i].gameObject.GetComponent<InventorySlot>();
                if (pSlot == null)
                    continue;

                Event_OnSwapSlot(_pInventory, this, pSlot._pInventory, pSlot);
                break;
            }

            OnDragEndSlot?.Invoke(this, eventData);
        }

        public void Event_SetSelected(bool bIsSelected)
        {
            this.bIsSelected = bIsSelected;
            OnChange_IsSelected?.Invoke(bIsSelected);
        }

        protected void Event_OnSwapSlot(Inventory pInventory_OnDraging, InventorySlot pSlot_OnDraging, Inventory pInventory_Dest, InventorySlot pSlot_Dest)
        {
            if(bIsDebug)
                Debug.Log($"{nameof(Event_OnSwapSlot)} - pSlot_OnDraging : {pInventory_OnDraging.name}-{pSlot_OnDraging.name} => pSlot_Dest : {pInventory_Dest.name}-{pSlot_Dest.name}", pSlot_OnDraging);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }

#if UNITY_EDITOR

    [CanEditMultipleObjects]
    [CustomEditor(typeof(InventorySlot))]
    public class InventorySlot_Inspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            InventorySlot pTargetSlot = target as InventorySlot;
            IEnumerable<InventorySlot> arrTarget = targets.Cast<InventorySlot>();

            if (GUILayout.Button("Update Slot Index - by Sibling"))
            {
                foreach (var pSlot in arrTarget)
                    pSlot.iSlotIndex = pSlot.transform.GetSiblingIndex();
            }
        }
    }
#endif
}