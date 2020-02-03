#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-01-31
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    public interface IPopupText_Logic
    {
        IEnumerator OnShowText(PopupText pTextOwner, string strText);
        IEnumerator OnHideText(PopupText pTextOwner);
    }
}