#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-30
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UIFramework.InventorySlotLogic;

namespace UIFramework
{

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Inventory))]
    public class InventoryExample : MonoBehaviour
    {
        /* const & readonly declaration             */


        /* enum & struct declaration                */

        [System.Serializable]
        public class SomthingData : IInventoryData
        {
            public string strName;
            public Sprite pSpriteIcon;
            public Color pColor;

            public string IInventoryData_Key => strName;

            public int IInventoryData_Count => 1;

            public IInventoryData IInventoryData_AddOrMinusCount(int iCount)
            {
                return this;
            }
        }

        /* public - Field declaration               */

        public List<SomthingData> listSomthingData = new List<SomthingData>();

        /* protected & private - Field declaration  */

        Inventory _pInventory;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        private void Awake()
        {
            _pInventory = GetComponent<Inventory>();
        }

        private void OnEnable()
        {
            if(_pInventory.bIsExecute_Awake == false)
                _pInventory.EventAwake();

            _pInventory.DoClearData();
            _pInventory.DoAddRangeData(listSomthingData.ToArray());

            InventoryLogicFactory pLogicFactory = new InventoryLogicFactory();
            var pLogic = pLogicFactory.DoCreate_LibraryLogic_Command(InventorySlot.EInventorySlot_CommandEvent.OnDragBegin, EInventory_CommandLogicName.Instantiate_CloneSlot, InventorySlot.EInventorySlot_CommandEvent.OnDragEnd);
            Instantiate_CloneSlot pInstantiateLogic = pLogic as Instantiate_CloneSlot;
            pInstantiateLogic.DoInit((p => p.GetComponent<Image>().enabled = false), null);

            _pInventory.DoInit_SlotLogic(pLogicFactory);
            _pInventory.OnSwap_Slot += _pInventory_OnSwap_Slot;
        }

        private void _pInventory_OnSwap_Slot(InventorySlot pStart, InventorySlot pDest)
        {
            pStart.DoSwapSlot(pDest);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}