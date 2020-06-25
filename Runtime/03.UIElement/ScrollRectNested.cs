#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-02-18
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Math = System.Math;

/// <summary>
/// ScrollRect that supports being nested inside another ScrollRect.
/// BASED ON: https://forum.unity3d.com/threads/nested-scrollrect.268551/#post-1906953
/// </summary>
public class ScrollRectNested : ScrollRect
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration               */

    [Header("Additional Fields")]
    [SerializeField]
    ScrollRect parentScrollRect;

    /* protected & private - Field declaration  */

    bool routeToParent = false;

    // ========================================================================== //

    /* public - [Do~Something] Function 	        */


    // ========================================================================== //

    /* protected - [Override & Unity API]       */
    public override void OnInitializePotentialDrag(PointerEventData eventData)
    {
        // Always route initialize potential drag event to parent
        if (parentScrollRect != null)
        {
            ((IInitializePotentialDragHandler)parentScrollRect).OnInitializePotentialDrag(eventData);
        }
        base.OnInitializePotentialDrag(eventData);
    }

    public override void OnDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (routeToParent)
        {
            if (parentScrollRect != null)
            {
                ((IDragHandler)parentScrollRect).OnDrag(eventData);
            }
        }
        else
        {
            base.OnDrag(eventData);
        }
    }

    public override void OnBeginDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (!horizontal && Math.Abs(eventData.delta.x) > Math.Abs(eventData.delta.y))
        {
            routeToParent = true;
        }
        else if (!vertical && Math.Abs(eventData.delta.x) < Math.Abs(eventData.delta.y))
        {
            routeToParent = true;
        }
        else
        {
            routeToParent = false;
        }

        if (routeToParent)
        {
            if (parentScrollRect != null)
            {
                ((IBeginDragHandler)parentScrollRect).OnBeginDrag(eventData);
            }
        }
        else
        {
            base.OnBeginDrag(eventData);
        }
    }

    public override void OnEndDrag(UnityEngine.EventSystems.PointerEventData eventData)
    {
        if (routeToParent)
        {
            if (parentScrollRect != null)
            {
                ((IEndDragHandler)parentScrollRect).OnEndDrag(eventData);
            }
        }
        else
        {
            base.OnEndDrag(eventData);
        }
        routeToParent = false;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private
}