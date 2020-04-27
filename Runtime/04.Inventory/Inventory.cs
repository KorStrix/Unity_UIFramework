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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using UIFramework.InventorySlotLogic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIFramework
{
    public interface IInventoryData
    {
        string IInventoryData_Key { get; }
        int IInventoryData_Count { get; }
        IInventoryData IInventoryData_AddOrMinusCount(int iCount);
    }

    /// <summary>
    /// 
    /// </summary>
    public class Inventory : UIWidgetObjectBase, IUIWidget_Managed
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public struct Inventory_OnChangeSelectSlot_Msg
        {
            public InventorySlot pSlot_Prev { get; private set; }
            public InventorySlot pSlot_Current { get; private set; }

            public Inventory_OnChangeSelectSlot_Msg(InventorySlot pSlot_Prev, InventorySlot pSlot_Current)
            {
                this.pSlot_Prev = pSlot_Prev; this.pSlot_Current = pSlot_Current;
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

        public static HashSet<Inventory> g_setActiveInventory = new HashSet<Inventory>();


        public event System.Func<InventorySlot, InventorySlot> OnEmptySlot;
        public event System.Action<Inventory_OnChangeSelectSlot_Msg> OnSelected_Slot;

        public delegate void delOnSwap_Slot(InventorySlot pStart, InventorySlot pDest);
        public event delOnSwap_Slot OnSwap_Slot;

        public event System.Action<OnSwapSlot_Msg> OnSwap_Slot_OtherInventory;
        public event System.Action<InventorySlot, PointerEventData> OnClick_Slot;
        public event System.Action<InventorySlot, PointerEventData, List<RaycastResult>> OnDragEnd_Slot;

        public IUIManager pUIManager { get; set; }
        public InventorySlot pSlotSelected { get; private set; }

        public IEnumerable<InventorySlot> arrSlot => _listSlotInstance;
        public IEnumerable<IInventoryData> arrData => _listSlotInstance.Where(p => p.pData != null).Select(p => p.pData);

        [Header("선택한 슬롯을 또 선택하면 선택 해제할지")]
        public bool bPossible_SelectedSlotRelease_OnClick = true;

        [Header("디버깅 유무")]
        public bool bIsDebug = false;

        /* protected & private - Field declaration  */

        static List<RaycastResult> g_listHit = new List<RaycastResult>();


        Dictionary<string, InventorySlot> _mapSlot_ByDataKey = new Dictionary<string, InventorySlot>();
        List<InventorySlot> _listSlotInstance = new List<InventorySlot>();

        InventorySlot_StateLogic[] _arrSlotLogic_State = new InventorySlot_StateLogic[0];
        InventorySlot_CommandLogic[] _arrSlotLogic_Command = new InventorySlot_CommandLogic[0];

        InventorySlot _pSlot_Origin;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoInit_ChildrenSlot(bool bInclude_Deactive = true)
        {
            for(int i = 0; i < _listSlotInstance.Count; i++)
            {
                InventorySlot pInventorySlot = _listSlotInstance[i];
                pInventorySlot.OnClickedSlot -= OnClickedSlot;
                //pInventorySlot.OnDragBeginSlot -= OnDragBeginSlot;
                //pInventorySlot.OnDragEndSlot -= OnDragEndSlot;
                pInventorySlot.OnSwapSlot -= OnSwapSlot;
                // pInventorySlot.OnChange_SlotData -= OnChange_SlotData;
            }

            GetComponentsInChildren(bInclude_Deactive, _listSlotInstance);

            for (int i = 0; i < _listSlotInstance.Count; i++)
                InitSlot(_listSlotInstance[i]);

            DoInitSlot_Origin(_listSlotInstance.FirstOrDefault());
        }

        public void DoInitSlot_Origin(InventorySlot pSlot_Origin)
        {
            _pSlot_Origin = pSlot_Origin;
        }

        public void DoInit_SlotLogic(InventoryLogicFactory pLogicFactory)
        {
            _arrSlotLogic_State = pLogicFactory.list_StateLogic.ToArray();
            _listSlotInstance.ForEach(p => p.DoInit_SlotStateLogic(_arrSlotLogic_State));

            _arrSlotLogic_Command = pLogicFactory.list_CommandLogic.ToArray();
            _listSlotInstance.ForEach(p => p.DoInit_SlotCommandLogic(_arrSlotLogic_Command));
        }


        public void DoAddRangeData(params IInventoryData[] arrData)
        {
            for(int i = 0; i < arrData.Length; i++)
            {
                IInventoryData pData = arrData[i];
                if (_mapSlot_ByDataKey.TryGetValue(pData.IInventoryData_Key, out InventorySlot pSlot))
                {
                    pSlot.DoSetData(pSlot.pData.IInventoryData_AddOrMinusCount(pData.IInventoryData_Count));
                }
                else
                {
                    pSlot = _listSlotInstance.FirstOrDefault(p => p.pData == null);
                    if(pSlot != null)
                    {
                        Slot_Set_NewData(pSlot, pData);
                        continue;
                    }

                    if(OnEmptySlot != null)
                    {
                        pSlot = OnEmptySlot(_pSlot_Origin);
                        if(pSlot != null)
                        {
                            _listSlotInstance.Add(pSlot);
                            InitSlot(pSlot);
                            Slot_Set_NewData(pSlot, pData);
                        }
                    }
                }
            }
        }

        public void DoAddData(IInventoryData pData)
        {
            DoAddRangeData(pData);
        }

        public void DoRemoveData(IInventoryData pData)
        {
            if (_mapSlot_ByDataKey.TryGetValue(pData.IInventoryData_Key, out var pSlot) == false)
                return;

            Slot_ClearData(pSlot, pData.IInventoryData_Key);
        }

        public void DoClearData(bool bClear_OnSelected = true)
        {
            for (int i = 0; i < _listSlotInstance.Count; i++)
            {
                InventorySlot pSlot = _listSlotInstance[i];
                if(pSlot.pData != null)
                    Slot_ClearData(pSlot, pSlot.pData.IInventoryData_Key);
                else
                    Slot_ClearData(pSlot);
            }

            if(bClear_OnSelected && pSlotSelected != null)
            {
                pSlotSelected.Event_SetSelected(false);
                pSlotSelected = null;
            }

            _mapSlot_ByDataKey.Clear();
        }

        public void DoClearEvent_OnClickSlot()
        {
            OnClick_Slot = null;
        }

        public void DoClearEvent_OnDragEndSlot()
        {
            OnDragEnd_Slot = null;
        }


        public void Event_Notify_OnClickSlot(InventorySlot pSlot, PointerEventData pPointerEvent)
        {
            OnClickedSlot(pSlot, pPointerEvent);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            DoInit_ChildrenSlot();
            DoClearData();
        }

        private void OnEnable()
        {
            g_setActiveInventory.Add(this);
        }

        protected override void OnDisableObject(bool bIsQuit_Application)
        {
            base.OnDisableObject(bIsQuit_Application);

            g_setActiveInventory.Remove(this);
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        private void InitSlot(InventorySlot pInventorySlot)
        {
            pInventorySlot.OnClickedSlot += OnClickedSlot;
            //pInventorySlot.OnDragBeginSlot += OnDragBeginSlot;
            //pInventorySlot.OnDragEndSlot += OnDragEndSlot;
            pInventorySlot.OnSwapSlot += OnSwapSlot;
            // pInventorySlot.OnChange_SlotData += OnChange_SlotData;

            pInventorySlot.DoInit_SlotStateLogic(_arrSlotLogic_State);
            pInventorySlot.DoInit_SlotCommandLogic(_arrSlotLogic_Command);

            pInventorySlot.DoInit(this);
        }

        private void OnSwapSlot(OnSwapSlot_Msg obj)
        {
            if (obj.pInventory_OnDraging == obj.pInventory_Dest)
            {
                OnSwap_Slot?.Invoke(obj.pSlot_OnDraging, obj.pSlot_Dest);
                OnClickedSlot(obj.pSlot_Dest, null);
            }
            else
            {
                OnSwap_Slot_OtherInventory?.Invoke(obj);
            }
        }

        private void OnDragBeginSlot(InventorySlot pSlotSelectedNew, PointerEventData pPointerData)
        {
            if (pSlotSelected == false)
                OnClickedSlot(pSlotSelectedNew, pPointerData);
        }

        private void OnDragEndSlot(InventorySlot pSlot, PointerEventData pPointerData)
        {
            EventSystem.current.RaycastAll(pPointerData, g_listHit);
            OnDragEnd_Slot?.Invoke(pSlot, pPointerData, g_listHit);
        }

        private void OnClickedSlot(InventorySlot pSlotSelectedNew, PointerEventData pPointerData)
        {
            if (pSlotSelected == pSlotSelectedNew && bPossible_SelectedSlotRelease_OnClick)
            {
                pSlotSelected?.Event_SetSelected(false);

                OnSelected_Slot?.Invoke(new Inventory_OnChangeSelectSlot_Msg(pSlotSelected, null));
                pSlotSelected = null;
            }
            else if (pSlotSelected != pSlotSelectedNew)
            {
                pSlotSelected?.Event_SetSelected(false);
                pSlotSelectedNew?.Event_SetSelected(true);

                OnSelected_Slot?.Invoke(new Inventory_OnChangeSelectSlot_Msg(pSlotSelected, pSlotSelectedNew));
                pSlotSelected = pSlotSelectedNew;
            }

            if (bIsDebug)
            {
                if (pSlotSelectedNew != null)
                    Debug.Log($"{name}-{nameof(OnClickedSlot)} Slot : {pSlotSelectedNew.name}", this);
                else
                    Debug.Log($"{name}-{nameof(OnClickedSlot)} Slot : Null", this);
            }

            OnClick_Slot?.Invoke(pSlotSelectedNew, pPointerData);
        }


        private void OnChange_SlotData(InventorySlot.OnChangeSlotData_Msg sMsg)
        {
            if (sMsg.pData_Prev != null)
                Slot_ClearData(sMsg.pSlot, sMsg.pData_Prev.IInventoryData_Key);

            if (sMsg.bSlot_IsEmpty)
                Slot_ClearData(sMsg.pSlot);
            else
                Slot_Set_NewData(sMsg.pSlot, sMsg.pData_Current);
        }

        private void Slot_Set_NewData(InventorySlot pSlot, IInventoryData pData)
        {
            pSlot.DoSetData(pData);
            _mapSlot_ByDataKey.Add(pData.IInventoryData_Key, pSlot);
        }

        private void Slot_ClearData(InventorySlot pSlot)
        {
            pSlot.DoClear();
        }

        private void Slot_ClearData(InventorySlot pSlot, string strDataKey)
        {
            pSlot.DoClear();
            _mapSlot_ByDataKey.Remove(strDataKey);
        }


#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Custom/" + nameof(Inventory))]
        public static void CreateInventory(MenuCommand pCommand)
        {
            const float const_fPosX = 120f;

            GameObject pObjectParents = pCommand.context as GameObject;
            if (pObjectParents == null)
            {
                pObjectParents = new GameObject($"{nameof(Inventory)}");

                // 생성된 오브젝트를 Undo 시스템에 등록한다.
                Undo.RegisterCreatedObjectUndo(pObjectParents, "Create " + pObjectParents.name);
            }

            if (pObjectParents.GetComponent<Inventory>() == null)
                Undo.AddComponent<Inventory>(pObjectParents);

            var pLayoutGroup = pObjectParents.GetComponent<GridLayoutGroup>();
            if (pLayoutGroup == null)
                pLayoutGroup = Undo.AddComponent<GridLayoutGroup>(pObjectParents);


            int iSlotCount = 5;
            GameObject pObjectSlotLast = null;
            for (int i = 0; i < iSlotCount; i++)
            {
                string strSlotName = $"Slot ({i + 1})";
                pObjectSlotLast = new GameObject(strSlotName);
                // 생성된 오브젝트를 Undo 시스템에 등록한다.
                Undo.RegisterCreatedObjectUndo(pObjectSlotLast, "Create " + strSlotName);

                GameObjectUtility.SetParentAndAlign(pObjectSlotLast, pObjectParents);
                pObjectSlotLast.transform.position += new Vector3(const_fPosX * i, 0f);

                Image pSlotIcon = pObjectSlotLast.AddComponent<Image>();
                pSlotIcon.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");


                GameObject pObjectSlotFrame = new GameObject($"Image_ItemFrame");
                GameObjectUtility.SetParentAndAlign(pObjectSlotFrame, pObjectSlotLast);
                Image pItemFrame = pObjectSlotFrame.AddComponent<Image>();
                pItemFrame.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                pItemFrame.rectTransform.SetAnchor(AnchorPresets.StretchAll);
                pItemFrame.rectTransform.sizeDelta = Vector2.one * -20f;



                GameObject pObjectItemIcon = new GameObject($"Image_ItemIcon");
                GameObjectUtility.SetParentAndAlign(pObjectItemIcon, pObjectSlotLast);
                Image pItemIcon = pObjectItemIcon.AddComponent<Image>();
                pItemIcon.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
                pItemIcon.rectTransform.SetAnchor(AnchorPresets.StretchAll);
                pItemIcon.rectTransform.sizeDelta = Vector2.one * -30f;



                InventorySlot pSlot = pObjectSlotLast.AddComponent<InventorySlot>();
                pSlot.iSlotIndex = i;
            }

            Selection.activeObject = pObjectSlotLast;
        }
#endif

        #endregion Private
    }
}