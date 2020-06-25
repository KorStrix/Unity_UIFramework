#region Header
/*	============================================
 *	Author   			    : Strix
 *	Initial Creation Date 	: 2020-02-04
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    /// <summary>
    /// 숫자를 char array로 리턴하는 로직입니다.
    /// </summary>
    [System.Serializable]
    public class Logic_SplitText
    {
        static char[] const_ZeroChar = new char[0];

        List<char> listTemp = new List<char>();

        public bool DoSplit_Number(string strNumber, out char[] arrResult)
        {
            arrResult = const_ZeroChar;
            listTemp.Clear();
            for (int i = 0; i < strNumber.Length; i++)
            {
                try
                {
                    double dNumber = char.GetNumericValue(strNumber[i]);
                    listTemp.Add((char)((int)dNumber));
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"SplitNumber Parsing Fail {strNumber} - Error : {e}");
                    return false;
                }
            }

            arrResult = listTemp.ToArray();
            return true;
        }
    }
}