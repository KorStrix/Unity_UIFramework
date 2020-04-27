#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-17 오후 6:02:02
 *	개요 : 
   ============================================ */
#endregion Header
   
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    public static class CollectionExtension
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> arrTarget, System.Action<T> OnExecute)
        {
            foreach (var pTarget in arrTarget)
                OnExecute.Invoke(pTarget);

            return arrTarget;
        }
    }
}
