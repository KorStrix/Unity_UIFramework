#region Header
/*	============================================
 *	Author   			    : Strix
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
using UIFramework.InventorySlotLogic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class InventorySlot : UIWidgetObjectBase, IPointerEnterHandler, IPointerClickHandler
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public enum EInventorySlot_StateEvent
        {
            None,

            OnFillSlot,
            OnEmptySlot,

            OnSwapSlot,
            OnSelectedSlot,
        }

        public enum EInventorySlot_CommandEvent
        {
            None,

            OnHover,
            OnClick,

            OnDragBegin,
            OnDrag,
            OnDragEnd,
        }

        public struct OnChangeSlotData_Msg
        {
            public InventorySlot pSlot { get; private set; }
            public IInventoryData pData_Prev { get; private set; }
            public IInventoryData pData_Current { get; private set; }
            public bool bSlot_IsEmpty { get; private set; }

            public OnChangeSlotData_Msg(InventorySlot pSlot, IInventoryData pData_Prev, IInventoryData pData_Current)
            {
                this.pSlot = pSlot; this.pData_Prev = pData_Prev; this.pData_Current = pData_Current;
                bSlot_IsEmpty = pData_Current == null;
            }
        }

        /* public - Field declaration               */

        public event System.Action<InventorySlot, PointerEventData> OnClickedSlot;
        public event System.Action<InventorySlot, PointerEventData> OnHoverSlot;

        public event System.Action<Inventory.OnSwapSlot_Msg> OnSwapSlot;

        public delegate void delOnChangeSlotData(OnChangeSlotData_Msg sMsg);
        public event delOnChangeSlotData OnChange_SlotData;
        public event System.Action<bool> OnChange_IsSelected;

        public bool bIsSelected { get; private set; }
        public bool bIsClone { get; private set; } = false;
        public IInventoryData pData { get; private set; }
        public Inventory pInventory { get; private set; }

        [Header("디버깅 유무")]
        public bool bIsDebug = false;

        [Header("선택 유무")]
        public bool bIsSelectAble = true;

        public int iSlotIndex;

        /* protected & private - Field declaration  */

        protected static List<RaycastResult> g_listHit = new List<RaycastResult>();

        Dictionary<EInventorySlot_StateEvent, List<InventorySlot_StateLogic>> _mapSlotLogic_State = new Dictionary<EInventorySlot_StateEvent, List<InventorySlot_StateLogic>>();
        Dictionary<EInventorySlot_StateEvent, List<InventorySlot_StateLogic>> _mapSlotLogic_State_Undo = new Dictionary<EInventorySlot_StateEvent, List<InventorySlot_StateLogic>>();

        Dictionary<EInventorySlot_CommandEvent, List<InventorySlot_CommandLogic>> _mapSlotLogic_Command = new Dictionary<EInventorySlot_CommandEvent, List<InventorySlot_CommandLogic>>();
        Dictionary<EInventorySlot_CommandEvent, List<InventorySlot_CommandLogic>> _mapSlotLogic_Command_Undo = new Dictionary<EInventorySlot_CommandEvent, List<InventorySlot_CommandLogic>>();

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoInit(Inventory pInventory)
        {
            this.pInventory = pInventory;

            Event_SetSelected(false);
        }

        public void DoUpdateIndex_IsSibling()
        {
            iSlotIndex = transform.GetSiblingIndex();
        }

        public void DoInit_SlotStateLogic(InventorySlot_StateLogic[] arrSlotStateLogic)
        {
            _mapSlotLogic_State = arrSlotStateLogic.
                GroupBy(p => p.eEvent).
                ToDictionary(p => p.Key, p => p.ToList());

            _mapSlotLogic_State_Undo = arrSlotStateLogic.
                Where(p => p.eEvent_Undo != EInventorySlot_StateEvent.None).
                GroupBy(p => p.eEvent_Undo).
                ToDictionary(p => p.Key, p => p.ToList());

            OnChange_SlotData -= ExecuteLogic_StateEvent_OnChangeData;
            OnChange_SlotData += ExecuteLogic_StateEvent_OnChangeData;
            OnChange_IsSelected -= ExecuteLogic_StateEvent_OnSelected;
            OnChange_IsSelected += ExecuteLogic_StateEvent_OnSelected;
            OnSwapSlot -= ExecuteLogic_StateEvent_OnSwap;
            OnSwapSlot += ExecuteLogic_StateEvent_OnSwap;
        }

        public virtual void DoInit_SlotCommandLogic(InventorySlot_CommandLogic[] arrSlotCommandLogic)
        {
            _mapSlotLogic_Command = arrSlotCommandLogic.
                GroupBy(p => p.eEvent).
                ToDictionary(p => p.Key, p => p.ToList());

            _mapSlotLogic_Command_Undo = arrSlotCommandLogic.
                Where(p => p.eEvent_Undo != EInventorySlot_CommandEvent.None).
                GroupBy(p => p.eEvent_Undo).
                ToDictionary(p => p.Key, p => p.ToList());

            OnHoverSlot -= ExecuteLogic_CommandEvent_OnHover;
            OnHoverSlot += ExecuteLogic_CommandEvent_OnHover;
            OnClickedSlot -= ExecuteLogic_CommandEvent_OnClick;
            OnClickedSlot += ExecuteLogic_CommandEvent_OnClick;
        }

        public void DoSwapSlot(InventorySlot pSlotOrigin)
        {
            var pMyData = this.pData;
            DoSetData(pSlotOrigin.pData);
            pSlotOrigin.DoSetData(pMyData);
        }

        public void DoSetData(IInventoryData pData)
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

        public void OnPointerClick(PointerEventData eventData) { OnClickedSlot?.Invoke(this, eventData); }
        public void OnPointerEnter(PointerEventData eventData) { OnHoverSlot?.Invoke(this, eventData); }

        public void Event_SetSelected(bool bIsSelected)
        {
            this.bIsSelected = bIsSelected;
            //if (bIsSelected && Inventory.g_setActiveInventory.Count >= 2)
            //{
            //    foreach (Inventory pInventoryOther in Inventory.g_setActiveInventory)
            //    {
            //        if (pInventoryOther == _pInventory)
            //            continue;

            //        if (pInventoryOther.pSlotSelected == null)
            //            continue;

            //        Event_OnSwapSlot(pInventoryOther, pInventoryOther.pSlotSelected, _pInventory, this);
            //    }
            //}

            if(bIsSelectAble)
                OnChange_IsSelected?.Invoke(bIsSelected);
        }

        protected void Event_OnSwapSlot(Inventory pInventory_OnDraging, InventorySlot pSlot_OnDraging, Inventory pInventory_Dest, InventorySlot pSlot_Dest)
        {
            if (bIsDebug)
                Debug.Log($"{nameof(Event_OnSwapSlot)} - pSlot_OnDraging : {pInventory_OnDraging.name}-{pSlot_OnDraging.name} => pSlot_Dest : {pInventory_Dest.name}-{pSlot_Dest.name}", pSlot_OnDraging);

            OnSwapSlot?.Invoke(new Inventory.OnSwapSlot_Msg(pInventory_OnDraging, pSlot_OnDraging, pInventory_Dest, pSlot_Dest));
        }

        public void Event_OnSetClone()
        {
            bIsClone = true;
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        void ExecuteLogic_StateEvent_OnChangeData(OnChangeSlotData_Msg sMsg) { ExecuteLogic_State(sMsg.bSlot_IsEmpty ? EInventorySlot_StateEvent.OnEmptySlot : EInventorySlot_StateEvent.OnFillSlot); }
        void ExecuteLogic_StateEvent_OnSelected(bool bSelected) { ExecuteLogic_State(EInventorySlot_StateEvent.OnSelectedSlot); }
        void ExecuteLogic_StateEvent_OnSwap(Inventory.OnSwapSlot_Msg sMsg) { ExecuteLogic_State(EInventorySlot_StateEvent.OnSwapSlot); }

        void ExecuteLogic_CommandEvent_OnHover(InventorySlot arg1, PointerEventData arg2) { ExecuteLogic_Command(EInventorySlot_CommandEvent.OnHover, arg2); }
        void ExecuteLogic_CommandEvent_OnClick(InventorySlot arg1, PointerEventData arg2) { ExecuteLogic_Command(EInventorySlot_CommandEvent.OnClick, arg2); }

        protected void ExecuteLogic_State(EInventorySlot_StateEvent eEvent)
        {
            if (_mapSlotLogic_State.TryGetValue(eEvent, out var listLogic))
            {
                for (int i = 0; i < listLogic.Count; i++)
                    listLogic[i].pLogic.IInventorySlot_StateLogic(this);
            }

            if (_mapSlotLogic_State_Undo.TryGetValue(eEvent, out listLogic))
            {
                for (int i = 0; i < listLogic.Count; i++)
                    listLogic[i].pLogic.IInventorySlot_StateLogic_Undo(this);
            }
        }

        protected void ExecuteLogic_Command(EInventorySlot_CommandEvent eEvent, PointerEventData pPointerEventData)
        {
            if (_mapSlotLogic_Command.TryGetValue(eEvent, out var listLogic))
            {
                for (int i = 0; i < listLogic.Count; i++)
                    listLogic[i].pLogic.IInventorySlot_CommandLogic(this, pPointerEventData);
            }

            if (_mapSlotLogic_Command_Undo.TryGetValue(eEvent, out listLogic))
            {
                for (int i = 0; i < listLogic.Count; i++)
                    listLogic[i].pLogic.IInventorySlot_CommandLogic_Undo(this, pPointerEventData);
            }
        }

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
                    pSlot.DoUpdateIndex_IsSibling();
            }
        }
    }
#endif
}
