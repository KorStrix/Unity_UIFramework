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
    namespace PopupText_Logic
    {
        public interface IPopupText_Logic
        {
            IEnumerator OnAnimation(PopupText pTextOwner, string strText);
        }

        public abstract class PopupText_Logic_ComponentBase : MonoBehaviour, IPopupText_Logic
        {
            public abstract IEnumerator OnAnimation(PopupText pTextOwner, string strText);
        }
    }
}