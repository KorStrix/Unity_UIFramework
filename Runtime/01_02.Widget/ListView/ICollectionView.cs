#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-12-09 오후 9:36:47
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    public delegate void del_OnInstantiate_CollectionItem(GameObject pObjectScrollItem);
    public delegate void del_OnUpdate_CollectionItem(GameObject pObjectScrollItem, int iIndex);
}


/// <summary>
/// 
/// </summary>
public interface ICollectionView
{
    GameObject gameObject { get; }

    void ICollectionView_UpdateItem(int iItemCount);
    void DoAllClearEvent_OnInstantiate_CollectionItem();
    void DoAllClearEvent_OnUpdate_CollectionItem();

    event UIFramework.del_OnInstantiate_CollectionItem OnInstantiate_CollectionItem;

    event UIFramework.del_OnUpdate_CollectionItem OnUpdate_CollectionItem;
}

public interface ICollectionItem
{
    void ICollectionItem_Update(int iIndex);
}
