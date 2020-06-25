#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-01-22
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class DragableObject : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public delegate void delOnDrag(DragEvent sMessage);
        public event delOnDrag OnDragEvent;

        public List<DragableObject> listDragObject_Clone { get; private set; } = new List<DragableObject>();

        /* protected & private - Field declaration  */

        Dictionary<EDragState, List<DragLogicBase>> _mapDragLogic = new Dictionary<EDragState, List<DragLogicBase>>();

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoAddLogic(EDragState eDragStateFlag, DragLogicBase pLogic)
        {
            if (_mapDragLogic.ContainsKey(eDragStateFlag) == false)
                _mapDragLogic.Add(eDragStateFlag, new List<DragLogicBase>());

            pLogic.OnInit(this);
            _mapDragLogic[eDragStateFlag].Add(pLogic);
        }

        public void DoClearAllLogic()
        {
            foreach (var pLogicList in _mapDragLogic.Values)
                pLogicList.Clear();
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */


        public void OnBeginDrag(PointerEventData eventData)
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

        public void OnDrag(PointerEventData eventData)
        {
            DragEvent sDragEvent = new DragEvent(this, EDragState.Stay, Input.mousePosition);
            foreach (var pDragLogic_KeyPair in _mapDragLogic)
            {
                if (ContainEnumFlag(pDragLogic_KeyPair.Key, EDragState.Stay) == false)
                    continue;

                foreach (var pLogic in pDragLogic_KeyPair.Value)
                    pLogic.OnDragEvent(sDragEvent);
            }

            OnDragEvent?.Invoke(sDragEvent);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            DragEvent sDragEvent = new DragEvent(this, EDragState.End, Input.mousePosition);
            foreach (var pDragLogic_KeyPair in _mapDragLogic)
            {
                if (ContainEnumFlag(pDragLogic_KeyPair.Key, EDragState.End) == false)
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
}