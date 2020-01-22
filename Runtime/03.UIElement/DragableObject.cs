#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-22
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public class DragableObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    public enum EDragState
    {
        None = 0,

        Begin = 1 << 0,
        Stay = 1 << 1,
        End = 1 << 2,
    }

    public struct DragEvent
    {
        public DragableObject pDragableObject { get; private set; }
        public EDragState eDragState { get; private set; }
        public Vector2 vecMousePos { get; private set; }

        public DragEvent(DragableObject pDragableObject, EDragState eDragState, Vector2 vecMousePos)
        {
            this.pDragableObject = pDragableObject; this.eDragState = eDragState; this.vecMousePos = vecMousePos;
        }

        public Vector3 GetWorldPos(Camera pCamera)
        {
            return pCamera.ScreenToWorldPoint(pCamera.ScreenToWorldPoint(new Vector3(vecMousePos.x, vecMousePos.y, pCamera.nearClipPlane)));
        }
    }

    public abstract class DragLogicBase
    {
        protected DragableObject _pDragableObject { get; private set; }
        public virtual void OnInit(DragableObject pDragableObject) { _pDragableObject = pDragableObject; }
        public abstract void OnDragEvent(DragEvent sDragEvent);
    }

    public class DragLogic_CloneObject : DragLogicBase
    {
        System.Func<DragableObject, DragableObject> _OnInstantiate;

        public DragLogic_CloneObject(System.Func<DragableObject, DragableObject> OnInstantiate)
        {
            _OnInstantiate = OnInstantiate;
        }

        public override void OnDragEvent(DragEvent sDragEvent)
        {
            _pDragableObject.listDragObject_Clone.Add(_OnInstantiate(_pDragableObject));
        }
    }

    public class DragLogic_DestroyAll_CloneObject : DragLogicBase
    {
        System.Action<DragableObject> _OnDestroy;

        public DragLogic_DestroyAll_CloneObject(System.Action<DragableObject> OnDestroy)
        {
            _OnDestroy = OnDestroy;
        }

        public override void OnDragEvent(DragEvent sDragEvent)
        {
            for(int i = 0; i < _pDragableObject.listDragObject_Clone.Count; i++)
            {
                _OnDestroy(_pDragableObject.listDragObject_Clone[i]);
                i--;
            }
        }
    }

    /* public - Field declaration               */

    public delegate void delOnDrag(DragEvent sMessage);
    public event delOnDrag OnDragEvent;

    public List<DragableObject> listDragObject_Clone { get; private set; } = new List<DragableObject>();

    /* protected & private - Field declaration  */

    Dictionary<EDragState, List<DragLogicBase>> _mapDragLogic = new Dictionary<EDragState, List<DragLogicBase>>();

    // ========================================================================== //

    /* public - [Do~Somthing] Function 	        */

    public void DoAddLogic(EDragState eDragStateFlag, DragLogicBase pLogic)
    {
        if (_mapDragLogic.ContainsKey(eDragStateFlag) == false)
            _mapDragLogic.Add(eDragStateFlag, new List<DragLogicBase>());

        pLogic.OnInit(this);
        _mapDragLogic[eDragStateFlag].Add(pLogic);
    }

    public void DoClearAllLogic()
    {
        foreach(var pLogicList in _mapDragLogic.Values)
            pLogicList.Clear();
    }

    // ========================================================================== //

    /* protected - [Override & Unity API]       */


    public void OnBeginDrag(PointerEventData eventData)
    {
        DragEvent sDragEvent = new DragEvent(this, EDragState.Begin, Input.mousePosition);
        foreach(var pDragLogic_KeyPair in _mapDragLogic)
        {
            if (ContainEnumFlag(pDragLogic_KeyPair.Key, EDragState.Begin) == false)
                continue;

            foreach(var pLogic in pDragLogic_KeyPair.Value)
                pLogic.OnDragEvent(sDragEvent); 
        }

        OnDragEvent?.Invoke(sDragEvent);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragEvent sDragEvent = new DragEvent(this, EDragState.Begin, Input.mousePosition);
        foreach (var pDragLogic_KeyPair in _mapDragLogic)
        {
            if (ContainEnumFlag(pDragLogic_KeyPair.Key, EDragState.Begin) == false)
                continue;

            foreach (var pLogic in pDragLogic_KeyPair.Value)
                pLogic.OnDragEvent(sDragEvent);
        }

        OnDragEvent?.Invoke(sDragEvent);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DragEvent sDragEvent = new DragEvent(this, EDragState.Begin, Input.mousePosition);
        foreach (var pDragLogic_KeyPair in _mapDragLogic)
        {
            if (ContainEnumFlag(pDragLogic_KeyPair.Key, EDragState.Begin) == false)
                continue;

            foreach (var pLogic in pDragLogic_KeyPair.Value)
                pLogic.OnDragEvent(sDragEvent);
        }

        OnDragEvent?.Invoke(sDragEvent);
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    bool ContainEnumFlag<T>(T eEnumFlag, params T[] arrEnum)
        where T : struct, System.IConvertible, System.IComparable, System.IFormattable
    {
        bool bIsContain = false;

        int iEnumFlag = eEnumFlag.GetHashCode();
        for (int i = 0; i < arrEnum.Length; i++)
        {
            int iEnum = arrEnum[i].GetHashCode();
            bIsContain = (iEnumFlag & iEnum) != 0;
            if (bIsContain)
                break;
        }

        return bIsContain;
    }

    #endregion Private
}