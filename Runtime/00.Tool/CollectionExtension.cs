#region Header
/*	============================================
 *	�ۼ��� : Strix
 *	�ۼ��� : 2019-10-17 ���� 6:02:02
 *	���� : 
   ============================================ */
#endregion Header
   
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    static public class CollectionExtension
    {
        static public IEnumerable<T> ForEach<T>(this IEnumerable<T> arrTarget, System.Action<T> OnExecute)
        {
            foreach (var pTarget in arrTarget)
                OnExecute.Invoke(pTarget);

            return arrTarget;
        }
    }
}
