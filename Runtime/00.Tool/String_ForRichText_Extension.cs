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
using System.Text.RegularExpressions;

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
    public static string ConvertRichText_SetStyle(this string strText, EFontStyle_ForRichText eStyle)
    {
        switch (eStyle)
        {
            case EFontStyle_ForRichText.Bold: return string.Format("<b>{0}</b>", strText);
            case EFontStyle_ForRichText.Italic: return string.Format("<i>{0}</i>", strText);
            default: return strText;
        }
    }

    /// <summary>
    /// Rich Text를 리턴합니다
    /// </summary>
    /// <param name="strColor">예시) Red = FF0000, Green = 00FF00, Blue = 0000FF </param>
    /// <returns></returns>
    public static string ConvertRichText_SetColor(this string strText, string strColorHex)
    {
        return "<color=#" + strColorHex + ">" + strText + "</color>";
    }

    public static string ConvertRichText_SetColor(this string strText, Color sColorHex)
    {
        return "<color=#" + ColorUtility.ToHtmlStringRGB(sColorHex) + ">" + strText + "</color>";
    }

    public static string ConvertRichText_SetColor(this string strText, EColor_ForRichText eColor)
    {
        return "<color=" + eColor.ToString() + ">" + strText + "</color>";
    }

    public static string ConvertRichText_SetFontSize(this string strText, int iFontSize)
    {
        return string.Format("<size={0}>{1}</size>", iFontSize, strText);
    }

    public static string Regex_Repace_SetRichText(this string strText, string strPattern, RegexOptions eRegexOption, MatchEvaluator OnMatchEvaluator)
    {
        try
        {
            return Regex.Replace(strText, strPattern, OnMatchEvaluator, eRegexOption);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Regex_Repace_SetRichText - strPattern : {strPattern}" + e);
        }

        return strText;
    }

    public static string AddNextLine(this string strText)
    {
        return strText + System.Environment.NewLine;
    }
}