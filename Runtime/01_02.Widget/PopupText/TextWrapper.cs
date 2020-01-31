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

/// <summary>
/// 
/// </summary>
public class TextWrapper
{
    Text _pText;

    public string Text
    {
        get
        {
            return _pText.text;
        }
        set
        {
            _pText.text = value;
        }
    }

    public TextWrapper(Text pText)
    {
        _pText = pText;
    }
}