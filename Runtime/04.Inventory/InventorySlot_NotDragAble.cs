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
    /// 주의) 드래그가 안될 수 있음
    /// </summary>
    public class InventorySlot_NotDragAble : InventorySlot, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public event System.Action<InventorySlot, PointerEventData> OnDragBeginSlot;
        public event System.Action<InventorySlot, PointerEventData> OnDragSlot;
        public event System.Action<InventorySlot, PointerEventData> OnDragEndSlot;

        [Header("드래그 유무")]
        public bool bIsDragAble = false;

        /* protected & private - Field declaration  */

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public override void DoInit_SlotCommandLogic(InventorySlot_CommandLogic[] arrSlotCommandLogic)
        {
            OnDragBeginSlot -= ExecuteLogic_CommandEvent_OnDragBegin;
            OnDragBeginSlot += ExecuteLogic_CommandEvent_OnDragBegin;
            OnDragSlot -= ExecuteLogic_CommandEvent_OnDrag;
            OnDragSlot += ExecuteLogic_CommandEvent_OnDrag;
            OnDragEndSlot -= ExecuteLogic_CommandEvent_OnDragEnd;
            OnDragEndSlot += ExecuteLogic_CommandEvent_OnDragEnd;
        }

        public void OnBeginDrag(PointerEventData eventData) { if (bIsDragAble) OnDragBeginSlot?.Invoke(this, eventData); }
        public void OnDrag(PointerEventData eventData) { if (bIsDragAble) OnDragSlot?.Invoke(this, eventData); }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (bIsDragAble == false)
                return;

            EventSystem.current.RaycastAll(eventData, g_listHit);
            for (int i = 0; i < g_listHit.Count; i++)
            {
                InventorySlot pSlot = g_listHit[i].gameObject.GetComponentInParent<InventorySlot>();
                if (pSlot == null || pSlot == this || pSlot.bIsClone)
                    continue;

                Event_OnSwapSlot(pInventory, this, pSlot.pInventory, pSlot);
                break;
            }

            OnDragEndSlot?.Invoke(this, eventData);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        void ExecuteLogic_CommandEvent_OnDragBegin(InventorySlot arg1, PointerEventData arg2) { ExecuteLogic_Command(EInventorySlot_CommandEvent.OnDragBegin, arg2); }
        void ExecuteLogic_CommandEvent_OnDrag(InventorySlot arg1, PointerEventData arg2) { ExecuteLogic_Command(EInventorySlot_CommandEvent.OnDrag, arg2); }
        void ExecuteLogic_CommandEvent_OnDragEnd(InventorySlot arg1, PointerEventData arg2) { ExecuteLogic_Command(EInventorySlot_CommandEvent.OnDragEnd, arg2); }

        #endregion Private
    }
}