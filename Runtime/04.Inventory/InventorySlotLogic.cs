#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-19
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static UIFramework.InventorySlot;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UIFramework.InventorySlotLogic;

namespace UIFramework
{
    public enum EInventory_StateLogicName
    {
    }

    public enum EInventory_CommandLogicName
    {
        Instantiate_CloneSlot,
    }

    public class InventoryLogicFactory
    {
        public List<InventorySlot_StateLogic> list_StateLogic = new List<InventorySlot_StateLogic>();
        public List<InventorySlot_CommandLogic> list_CommandLogic = new List<InventorySlot_CommandLogic>();

        public IInventorySlot_StateLogic DoCreate_LibraryLogic_State(EInventorySlot_StateEvent eEvent, EInventory_StateLogicName eLogic, EInventorySlot_StateEvent eEventUndo = EInventorySlot_StateEvent.None)
        {
            IInventorySlot_StateLogic pLogic = null;
            switch (eLogic)
            {
                default: Debug.LogError("Error - Not Found Logic"); return null;
            }

            if(pLogic != null)
                list_StateLogic.Add(new InventorySlot_StateLogic(eEvent, eEventUndo, pLogic));

            return pLogic;
        }

        public IInventorySlot_CommandLogic DoCreate_LibraryLogic_Command(EInventorySlot_CommandEvent eEvent, EInventory_CommandLogicName eLogic, EInventorySlot_CommandEvent eEventUndo = EInventorySlot_CommandEvent.None)
        {
            IInventorySlot_CommandLogic pLogic = null;
            switch (eLogic)
            {
                case EInventory_CommandLogicName.Instantiate_CloneSlot: pLogic = new Instantiate_CloneSlot(); break;

                default: Debug.LogError("Error - Not Found Logic"); return null;
            }

            if (pLogic != null)
                list_CommandLogic.Add(new InventorySlot_CommandLogic(eEvent, eEventUndo, pLogic));

            return pLogic;
        }
    }

    namespace InventorySlotLogic
    {
        /// <summary>
        /// 
        /// </summary>
        public interface IInventorySlot_StateLogic
        {
            void IInventorySlot_StateLogic(InventorySlot pSlot);
            void IInventorySlot_StateLogic_Undo(InventorySlot pSlot);
        }

        public class InventorySlot_StateLogic
        {
            public EInventorySlot_StateEvent eEvent { get; private set; }
            public EInventorySlot_StateEvent eEvent_Undo { get; private set; }

            public IInventorySlot_StateLogic pLogic { get; private set; }

            public InventorySlot_StateLogic(EInventorySlot_StateEvent eStateEvent, IInventorySlot_StateLogic pStateLogic)
            {
                this.eEvent = eStateEvent; this.pLogic = pStateLogic; this.eEvent_Undo = EInventorySlot_StateEvent.None;
            }

            public InventorySlot_StateLogic(EInventorySlot_StateEvent eStateEvent, EInventorySlot_StateEvent eStateEvent_Undo, IInventorySlot_StateLogic pStateLogic)
            {
                this.eEvent = eStateEvent; this.pLogic = pStateLogic; this.eEvent_Undo = eStateEvent_Undo;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public interface IInventorySlot_CommandLogic
        {
            void IInventorySlot_CommandLogic(InventorySlot pSlot, PointerEventData pPointerEventData);
            void IInventorySlot_CommandLogic_Undo(InventorySlot pSlot, PointerEventData pPointerEventData);
        }

        public class InventorySlot_CommandLogic
        {
            public EInventorySlot_CommandEvent eEvent { get; private set; }
            public EInventorySlot_CommandEvent eEvent_Undo { get; private set; }

            public IInventorySlot_CommandLogic pLogic { get; private set; }

            public InventorySlot_CommandLogic(EInventorySlot_CommandEvent eEvent, IInventorySlot_CommandLogic pLogic)
            {
                this.eEvent = eEvent; this.pLogic = pLogic; this.eEvent_Undo = EInventorySlot_CommandEvent.None;
            }

            public InventorySlot_CommandLogic(EInventorySlot_CommandEvent eEvent, EInventorySlot_CommandEvent eEvent_Undo, IInventorySlot_CommandLogic pLogic)
            {
                this.eEvent = eEvent; this.pLogic = pLogic; this.eEvent_Undo = eEvent_Undo;
            }
        }

        public class Instantiate_CloneSlot : IInventorySlot_CommandLogic
        {
            InventorySlot _pSlotClone;
            RectTransform _pTransform_Parents;
            Camera _pCamera;

            System.Func<InventorySlot, InventorySlot> _OnInstantiate_CloneSlot;
            System.Action<GameObject> _OnDestroy_CloneSlot;

            System.Action<InventorySlot, PointerEventData> _OnSetSlotPos;
            System.Action<InventorySlot> _OnCloneSlot;
            System.Func<InventorySlot, Transform> _GetCloneSlotParents;

            public void DoInit(System.Action<InventorySlot> OnCloneSlot_OrNull, System.Func<InventorySlot, Transform> GetCloneSlotParents_OrNull)
            {
                DoSetCallBack(GameObject.Instantiate, GameObject.Destroy);
                _OnCloneSlot = OnCloneSlot_OrNull;

                if (GetCloneSlotParents_OrNull == null)
                    _GetCloneSlotParents = p => p.transform.parent;
                else
                    _GetCloneSlotParents = GetCloneSlotParents_OrNull;
            }

            public void DoSetCallBack(System.Func<InventorySlot, InventorySlot> OnInstantiate_CloneSlot, System.Action<GameObject> OnDestroy_CloneSlot)
            {
                _OnInstantiate_CloneSlot = OnInstantiate_CloneSlot; _OnDestroy_CloneSlot = OnDestroy_CloneSlot;
            }

            public void IInventorySlot_CommandLogic(InventorySlot pSlot, PointerEventData pPointerEventData)
            {
                _pSlotClone = _OnInstantiate_CloneSlot(pSlot);
                _pSlotClone.DoSetData(pSlot.pData);
                _pSlotClone.Event_OnSetClone();

                Canvas pCanvas = pSlot.GetComponentInParent<Canvas>();

                if (pCanvas.renderMode != RenderMode.ScreenSpaceOverlay)
                {
                    _pCamera = pCanvas.rootCanvas.worldCamera;
                    _OnSetSlotPos = GetWorld_To_CameraScreenPoint;
                }
                else
                {
                    _pCamera = Camera.main;
                    _OnSetSlotPos = GetWorld_To_ScreenPoint;
                }
                pSlot.OnDragSlot += _OnSetSlotPos;

                RectTransform pTransformOrigin = pSlot.transform as RectTransform;
                RectTransform pTransformClone = _pSlotClone.transform as RectTransform;

                _pTransform_Parents = pSlot.transform.parent as RectTransform;
                pTransformClone.SetParent(_GetCloneSlotParents(pSlot));
                pTransformClone.position = pSlot.transform.position;
                pTransformClone.localScale = pSlot.transform.localScale;
                pTransformClone.sizeDelta = pTransformOrigin.sizeDelta;

                Graphic[] arrGraphic = pTransformClone.GetComponentsInChildren<Graphic>();
                for (int i = 0; i < arrGraphic.Length; i++)
                    arrGraphic[i].raycastTarget = false;

                _OnCloneSlot?.Invoke(_pSlotClone);
            }

            public void IInventorySlot_CommandLogic_Undo(InventorySlot pSlot, PointerEventData pPointerEventData)
            {
                pSlot.OnDragSlot -= _OnSetSlotPos;
                _OnDestroy_CloneSlot(_pSlotClone.gameObject);
            }

            void GetWorld_To_CameraScreenPoint(InventorySlot pSlot, PointerEventData pPointerEventData)
            {
                _pSlotClone.transform.position = _pCamera.ScreenToWorldPoint(new Vector3(pPointerEventData.position.x, pPointerEventData.position.y, _pCamera.nearClipPlane));
            }

            void GetWorld_To_ScreenPoint(InventorySlot pSlot, PointerEventData pPointerEventData)
            {
                Vector3 vecOutPos;
                RectTransformUtility.ScreenPointToWorldPointInRectangle(_pTransform_Parents, pPointerEventData.position, null, out vecOutPos);
                _pSlotClone.transform.position = vecOutPos;
            }
        }
    }
}
