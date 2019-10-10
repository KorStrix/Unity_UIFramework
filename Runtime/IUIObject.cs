#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-08 오후 2:21:29
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 
/// </summary>
public interface IUIObject
{
    IUIManager pUIManager { get; set; }
    Transform transform { get; }
}