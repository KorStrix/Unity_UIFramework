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
            if (pCamera == null)
            {
                Debug.LogError("GetWorldPos pCamera == null");
                return Vector3.zero;
            }

            return pCamera.ScreenToWorldPoint(new Vector3(vecMousePos.x, vecMousePos.y, pCamera.nearClipPlane));
        }
    }

    public abstract class DragLogicBase
    {
        public DragableObject pDragableObject { get; private set; }
        public virtual void OnInit(DragableObject pDragableObject) { this.pDragableObject = pDragableObject; }
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
            pDragableObject.listDragObject_Clone.Add(_OnInstantiate(pDragableObject));
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
            for (int i = 0; i < pDragableObject.listDragObject_Clone.Count; i++)
            {
                _OnDestroy(pDragableObject.listDragObject_Clone[i]);
                i--;
            }
        }
    }
}