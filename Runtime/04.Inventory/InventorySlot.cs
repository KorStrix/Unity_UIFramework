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

/// <summary>
/// 
/// </summary>
public class InventorySlot : UIFramework.UIWidgetObjectBase, IPointerClickHandler
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public struct OnChangeSlotData_Msg
    {
        public InventorySlot pSlot { get; private set; }
        public object pData_Prev { get; private set; }
        public object pData_Current { get; private set; }

        public OnChangeSlotData_Msg(InventorySlot pSlot, object pData_Prev, object pData_Current)
        {
            this.pSlot = pSlot; this.pData_Prev = pData_Prev; this.pData_Current = pData_Current;
        }
    }

    /* public - Field declaration               */

    public event System.Action<InventorySlot, PointerEventData> OnClickedSlot;
    public event System.Action<OnChangeSlotData_Msg> OnChangeSlotData;

    public object pData { get; private set; }

    [Header("드래그 유무")]
    public bool bIsDragAble = false;

    [Header("데이터가 없을 때 슬롯 그리기 유무")]
    public bool bIsDraw_OnEmptyData = true;

    public int iSlotIndex;

    /* protected & private - Field declaration  */


    // ========================================================================== //

    /* public - [Do~Somthing] Function 	        */

    public void DoSetData(object pData)
    {
        OnChangeSlotData?.Invoke(new OnChangeSlotData_Msg(this, this.pData, pData));

        this.pData = pData;
    }

    public void DoClear()
    {
        OnChangeSlotData?.Invoke(new OnChangeSlotData_Msg(this, pData, null));

        pData = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        OnClickedSlot.Invoke(this, eventData);
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
            foreach(var pSlot in arrTarget)
                pSlot.iSlotIndex = pSlot.transform.GetSiblingIndex();
        }
    }
}


#endif