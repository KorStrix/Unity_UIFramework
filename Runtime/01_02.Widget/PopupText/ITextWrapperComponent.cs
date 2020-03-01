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
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public interface ITextWrapperComponent
    {
        GameObject gameObject { get; }
        Transform transform { get; }
        string strText { get; set; }

        IEnumerable<Graphic> arrGraphic { get; }
    }
}