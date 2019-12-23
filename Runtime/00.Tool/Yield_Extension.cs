#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-11-08 오후 2:25:54
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public static class Yield_Extension
{
    // TODO : 나중에 Pooling 해야함
    /// <summary>
    /// Coroutine list가 실행중 변동될 때 에러를 내기 때문에 백업
    /// </summary>
    /// <param name="listCoroutine"></param>
    /// <returns></returns>
    static public IEnumerator GetEnumerator_Safe(this List<Coroutine> listCoroutine)
    {
        if (listCoroutine.Count != 0)
            yield return new List<Coroutine>(listCoroutine).GetEnumerator();
        else
            yield break;
    }

}