#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-11-13 오후 12:31:39
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public enum EColor_ForRichText
{
    aqua, black, blue, brown, cyan, darkblue, fuchsia, green, grey, lightblue, lime, magenta, maroon, navy, olive, orange, purple, red, silver, teal, white, yellow,
}

public enum EFontStyle_ForRichText
{
    Bold, Italic
}

/// <summary>
/// 
/// https://docs.unity3d.com/kr/530/Manual/StyledText.html
/// </summary>
public static class String_ForRichText_Extension
{
    static public string SetFontStyle_ForRichText(this string strText, EFontStyle_ForRichText eStyle)
    {
        switch (eStyle)
        {
            case EFontStyle_ForRichText.Bold: return string.Format("<b>{0}</b>", strText);
            case EFontStyle_ForRichText.Italic: return string.Format("<i>{0}</i>", strText);
            default: return strText;
        }
    }

    static public string SetColor_ForRichText(this string strText, EColor_ForRichText eColor)
    {
        return "<color=" + eColor.ToString() + ">" + strText + "</color>";
    }

    static public string SetFontSize_ForRichText(this string strText, int iFontSize)
    {
        return string.Format("<size={0}>{1}</size>", iFontSize, strText);
    }

    static public string AddNextLine(this string strText)
    {
        return strText + System.Environment.NewLine;
    }
}