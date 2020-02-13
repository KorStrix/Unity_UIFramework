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

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UIFramework
{
    public enum EDataCalculateResult
    {
        None,

        Empty,
        OverFlow,
    }

    /// <summary>
    /// 
    /// </summary>
    public class Inventory : UIFramework.UIWidgetObjectBase, IUIWidget_Managed
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

        public event System.Action<Inventory_OnChangeSelectSlot_Msg> OnSelected_Slot;

        public delegate void delOnSwap_Slot(InventorySlot pStart, InventorySlot pDest);
        public event delOnSwap_Slot OnSwap_Slot;
        public event System.Action<OnSwapSlot_Msg> OnSwap_Slot_OtherInventory;
        public event System.Action<InventorySlot, UnityEngine.EventSystems.PointerEventData> OnClick_Slot;

        public IUIManager pUIManager { get; set; }
        public InventorySlot pSlotSelected { get; private set; }


        [Header("선택한 슬롯을 또 선택하면 선택 해제할지")]
        public bool bPossible_SelectedSlotRelease_OnClick = true;

        [Header("디버깅 유무")]
        public bool bIsDebug = false;

        /* protected & private - Field declaration  */

        Dictionary<int, InventorySlot> _mapSlot_ByData = new Dictionary<int, InventorySlot>();
        List<InventorySlot> _listSlot = new List<InventorySlot>();

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */

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

        public void DoAdd(object pData)
        {
            DoAddRange(pData);
        }

        public void DoRemove(object pData)
        {
            InventorySlot pSlot;
            if (_mapSlot_ByData.TryGetValue(pData.GetHashCode(), out pSlot) == false)
                return;

            Slot_ClearData(pSlot, pData.GetHashCode());
        }

        public void DoClear(bool bClear_OnSelected = true)
        {
            for (int i = 0; i < _listSlot.Count; i++)
            {
                InventorySlot pSlot = _listSlot[i];
                if(pSlot.pData != null)
                    Slot_ClearData(pSlot, pSlot.pData.GetHashCode());
                else
                    Slot_ClearData(pSlot);
            }

            if(bClear_OnSelected && pSlotSelected != null)
            {
                pSlotSelected.Event_SetSelected(false);
                pSlotSelected = null;
            }
        }

        public void Event_Set_SelectedSlot_IsNull(UnityEngine.EventSystems.PointerEventData pPointerEvent)
        {
            OnClickedSlot(null, pPointerEvent);
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            GetComponentsInChildren(_listSlot);

            for(int i = 0; i < _listSlot.Count; i++)
            {
                InventorySlot pInventorySlot = _listSlot[i];
                pInventorySlot.OnClickedSlot += OnClickedSlot;
                pInventorySlot.OnSwapSlot += OnSwapSlot;

                pInventorySlot.OnDragSlot += Inventory_OnDragSlot;
                pInventorySlot.OnDragEndSlot += Inventory_OnDragEndSlot;

                pInventorySlot.DoInit(this);
            }
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

        private void OnSwapSlot(OnSwapSlot_Msg obj)
        {
            if (obj.pInventory_OnDraging == obj.pInventory_Dest)
                OnSwap_Slot?.Invoke(obj.pSlot_OnDraging, obj.pSlot_Dest);
            else
                OnSwap_Slot_OtherInventory?.Invoke(obj);
        }
        private void OnClickedSlot(InventorySlot pSlotSelectedNew, UnityEngine.EventSystems.PointerEventData pPointerData)
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

        private void Inventory_OnDragSlot(InventorySlot arg1, UnityEngine.EventSystems.PointerEventData arg2)
        {
        }

        private void Inventory_OnDragEndSlot(InventorySlot arg1, UnityEngine.EventSystems.PointerEventData arg2)
        {
        }

        private void Slot_SetData(InventorySlot pSlot, object pData, int iDataKey)
        {
            pSlot.DoSetData(pData);
            _mapSlot_ByData.Add(iDataKey, pSlot);
        }

        private void Slot_ClearData(InventorySlot pSlot)
        {
            pSlot.DoClear();
        }

        private void Slot_ClearData(InventorySlot pSlot, int iDataKey)
        {
            pSlot.DoClear();
            _mapSlot_ByData.Remove(iDataKey);
        }


#if UNITY_EDITOR
        [UnityEditor.MenuItem("GameObject/UI/Custom/" + nameof(Inventory))]
        static public void CreateRadioButton(MenuCommand pCommand)
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