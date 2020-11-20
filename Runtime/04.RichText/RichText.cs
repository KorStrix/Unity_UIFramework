#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-11-19
 *	Summary 		        :
 *
 * Unity의 RichText Tag를 Cut할 때 Tag가 깨지는 현상을 막는 string입니다.
 *
 * 깨지는 예시로 "<b>볼드체<b>" 를 크기 1만큼 자르면
 * "<" 인데 이걸 쓰면
 * "<b>볼</b>"가 나오는 것이 이 객체의 역할
 *
 * 원본 코드 : https://github.com/majecty/Unity3dRichTextHelper/blob/master/Assets/RichTextHelper/RichTextHelper.cs
 *
 *  Template 		        : New Behaviour For Unity Editor V2
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections.Generic;


public static class StringExt
{
    public static string RichTextSubString(this string text, int length)
    {
        var m = new RichTextSubStringMaker(text);
        for (int i = 0; i < length; i++)
        {
            m.Consume();
        }
        return m.GetRichText();
    }

    public static int RichTextLength(this string text)
    {
        var m = new RichTextSubStringMaker(text);
        var length = 0;
        while (m.IsConsumable())
        {
            if (m.Consume())
            {
                length += 1;
            }
        }
        return length;
    }
}

/// <summary>
/// 
/// </summary>
public class RichTextSubStringMaker
{
    /* const & readonly declaration             */

    private static readonly char[] tagBrackets = new char[2] { '<', '>' };

    /* enum & struct declaration                */

    class RichTextTag
    {
        public string tagName;
        public string endTag =>$"</{tagName}>";
    }

    /* public - Field declaration               */

    /* protected & private - Field declaration  */

    private Stack<RichTextTag> tagStack;
    private string originalText;
    private string middleText;
    private int consumedLength;

    // ========================================================================== //

    /* public - [Do~Something] Function 	        */

    public RichTextSubStringMaker(string original)
    {
        this.originalText = original;
        middleText = "";
        tagStack = new Stack<RichTextTag>();
        consumedLength = 0;
    }

    // Return if real character is consumed
    // If line's last part is closing tag, this function return false when consumed last closing tag.
    public bool Consume()
    {
        var peekedOriginChar = PeekNextOriginChar();
        if (peekedOriginChar == null)
            return false;

        // Only consume the start tag if a tag closing bracket (>) was found.
        var isStartTag = peekedOriginChar == '<' && IsNextTagBracketClosing(consumedLength + 1);
        var isEndTag = peekedOriginChar == '<' && PeekNextNextOriginChar() == '/';
        if (isStartTag || isEndTag)
        {
            if (isEndTag)
            {
                ConsumeEndTag();
            }
            else if (isStartTag)
            {
                ConsumeStartTag();
            }

            if (IsConsumable())
            {
                return Consume();
            }
            else
            {
                return false;
            }
        }
        else
        {
            ConsumeRawChar();
            return true;
        }
    }

    public string GetRichText()
    {
        if (tagStack.Count == 0)
        {
            return middleText;
        }

        var ret = middleText;
        // Caution! we iterate tags from the last item of the stack.
        // copiedQueue's first item is tagStack's last item.
        var copiedQueue = new Queue<RichTextTag>(tagStack);
        while (copiedQueue.Count != 0)
        {
            ret += copiedQueue.Dequeue().endTag;
        }

        return ret;
    }

    public bool IsConsumable()
    {
        return consumedLength < originalText.Length;
    }

    // ========================================================================== //

    /* protected - [Override & Unity API]       */


    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private char? PeekNextNextOriginChar()
    {
        if (originalText.Length <= consumedLength + 1)
        {
            return null;
        }

        return originalText[consumedLength + 1];
    }

    private char? PeekNextOriginChar()
    {
        if (originalText.Length > consumedLength)
            return originalText[consumedLength];
        else
            return null;
    }

    // Returns if the first tag bracket found in the string is '>', as opposed to '<'
    private bool IsNextTagBracketClosing(int charIndexToStartSearch = 0)
    {
        int bracketIndex = originalText.IndexOfAny(tagBrackets, charIndexToStartSearch);

        return bracketIndex != -1 && originalText[bracketIndex] == '>';
    }

    private void ConsumeStartTag()
    {
        var tagName = "";
        bool tagNameComplete = false;

        ConsumeRawChar();

        while (true)
        {
            var consumedChar = ConsumeRawChar();
            if (consumedChar == null)
            {
                Debug.LogError("Cannot close start tag");
                return;
            }

            if (consumedChar == '>')
            {
                if (tagName == "")
                {
                    Debug.LogWarning("Empty tag name");
                }

                tagStack.Push(new RichTextTag
                {
                    tagName = tagName
                });
                return;
            }

            if (!tagNameComplete)
            {
                if (!char.IsLetterOrDigit(consumedChar.Value))
                {
                    tagNameComplete = true;
                }
                else
                {
                    tagName += consumedChar;
                }
            }
        }
    }

    private void ConsumeEndTag()
    {
        var tagName = "";
        bool tagNameComplete = false;

        ConsumeRawChar();
        ConsumeRawChar();

        while (true)
        {
            var consumedChar = ConsumeRawChar();
            if (consumedChar == null)
            {
                Debug.LogError("Cannot close start tag");
                return;
            }

            if (consumedChar == '>')
            {
                if (tagName == "")
                {
                    Debug.LogWarning("Empty tag name");
                }

                if (tagStack.Count == 0)
                {
                    Debug.LogError("Could not pop tag " + tagName);
                }

                if (tagStack.Peek().tagName != tagName)
                {
                    Debug.LogError("Could not pop tag " + tagName + " expeted " + tagStack.Peek().tagName);
                }

                tagStack.Pop();
                return;
            }

            if (!tagNameComplete)
            {
                if (!char.IsLetterOrDigit(consumedChar.Value))
                {
                    tagNameComplete = true;
                }
                else
                {
                    tagName += consumedChar;
                }
            }
        }
    }

    private char? ConsumeRawChar()
    {
        if (consumedLength > originalText.Length)
        {
            return null;
        }

        var peeked = PeekNextOriginChar();
        this.middleText += peeked;
        this.consumedLength += 1;

        return peeked;
    }

    #endregion Private
}