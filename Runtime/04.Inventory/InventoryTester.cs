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

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(Inventory))]
    public class InventoryTester : MonoBehaviour
    {
        /* const & readonly declaration             */


        /* enum & struct declaration                */

        [System.Serializable]
        public class SomthingData
        {
            public string strName;
            public Sprite pSpriteIcon;
            public Color pColor;
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

            _pInventory.DoClear();
            _pInventory.DoAddRange(listSomthingData.ToArray());
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