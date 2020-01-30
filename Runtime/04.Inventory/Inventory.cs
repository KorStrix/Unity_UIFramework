#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-30
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    public enum EDataCalculateResult
    {
        None,

        Empty,
        OverFlow,
    }

    public interface IData
    {
        public int GetUniqueID { get; }
    }

    public interface IData_IsPossible_Add_And_Subtract : IData
    {
        EDataCalculateResult OnAdd(object pObjectX, object pObjectY, out object pObjectSumResult);
        EDataCalculateResult OnSubtract(object pObjectX, object pObjectY, out object pObjectSumResult);
    }


    /// <summary>
    /// 
    /// </summary>
    public class Inventory : UIFramework.WidgetObjectBase, IUIWidget_Managed
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public struct Inventory_OnSelectChangeIndex_Msg
        {
            public int iSelectIndex_Prev { get; private set; }
            public int iSelectIndex_Current { get; private set; }

            public object pObjectSelected_Prev { get; private set; }
            public object pObjectSelected_Current { get; private set; }

            public Inventory_OnSelectChangeIndex_Msg(int iSelectIndex_Prev, object pObjectSelected_Prev, int iSelectIndex_Current, object pObjectSelected_Current)
            {
                this.iSelectIndex_Prev = iSelectIndex_Prev; this.iSelectIndex_Current = iSelectIndex_Current;
                this.pObjectSelected_Prev = pObjectSelected_Prev; this.pObjectSelected_Current = pObjectSelected_Current;
            }
        }

        public event System.Action<Inventory_OnSelectChangeIndex_Msg> OnSelectChangeIndex;

        public IUIManager pUIManager { get; set; }

        /* protected & private - Field declaration  */

        Dictionary<int, InventorySlot> _mapSlot_ByData = new Dictionary<int, InventorySlot>();
        List<InventorySlot> _listSlot = new List<InventorySlot>();
        int _iSelectedIndex;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

        public void DoAddRange<T>(params T[] arrData)
            where T : IData
        {
            for (int i = 0; i < arrData.Length; i++)
            {
                IData pData = arrData[i];
                InventorySlot pSlot;
                if (_mapSlot_ByData.TryGetValue(pData.GetUniqueID, out pSlot) == false)
                    return;

                IData_IsPossible_Add_And_Subtract pDataIsFill = pData as IData_IsPossible_Add_And_Subtract;
                if (pDataIsFill != null)
                {
                }
                else
                {

                }
            }
        }

        public void DoAddRange(params object[] arrData)
        {
            int iDataIndex = 0;
            for (int i = 0; i < _listSlot.Count; i++)
            {
                InventorySlot pSlot = _listSlot[i];
                if (pSlot.pData == null)
                {
                    if (iDataIndex < arrData.Length)
                    {
                        object pData = arrData[iDataIndex++];
                        Slot_SetData(pSlot, pData, pData.GetHashCode());
                    }
                    else
                        break;
                }
            }
        }

        public void DoAdd<T>(T pData)
            where T : IData
        {
            DoAddRange(pData);
        }

        public void DoAdd(object pData)
        {
            DoAddRange(pData);
        }


        public void DoRemove<T>(T pData)
            where T : IData
        {
            InventorySlot pSlot;
            if (_mapSlot_ByData.TryGetValue(pData.GetUniqueID, out pSlot) == false)
                return;

            Slot_ClearData(pSlot, pData, pData.GetUniqueID);
        }

        public void DoRemove(object pData)
        {
            InventorySlot pSlot;
            if (_mapSlot_ByData.TryGetValue(pData.GetHashCode(), out pSlot) == false)
                return;

            Slot_ClearData(pSlot, pData, pData.GetHashCode());
        }

        public void DoClear()
        {
            for (int i = 0; i < _listSlot.Count; i++)
            {
                Slot_ClearData(_listSlot[i], _listSlot[i].pData, _listSlot[i].pData.GetHashCode());
            }

            _iSelectedIndex = -1;
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            GetComponentsInChildren(_listSlot);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void Slot_SetData(InventorySlot pSlot, object pData, int iDataKey)
        {
            pSlot.DoSetData(pData);
            _mapSlot_ByData.Add(iDataKey, pSlot);
        }

        private void Slot_ClearData(InventorySlot pSlot, object pData, int iDataKey)
        {
            pSlot.DoClear();
            _mapSlot_ByData.Remove(iDataKey);
        }

        #endregion Private
    }
}