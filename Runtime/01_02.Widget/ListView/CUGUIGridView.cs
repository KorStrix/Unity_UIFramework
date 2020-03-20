#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-10 오후 12:20:59
 *	개요 : 
 *	
 *	원본 코드 링크 : https://github.com/qiankanglai/LoopScrollRect

 *  수정 내용
 *      클래스 하나로 Horizontal, Vertical을 세팅할 수 있게 수정하였습니다.
 *      스크롤 아이템 풀링 코드가 리소스 폴더에 종속되있는 것을 외부에서 세팅할 수 있게 수정하였습니다.
 *      스크롤 아이템의 갱신을 SendMessage가 아닌 Interface를 통해 메세지를 받게끔 수정하였습니다.
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UIFramework;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
[RequireComponent(typeof(RectTransform))]
public class CUGUIGridView : UIBehaviour, ICollectionView /*, ICanvasElement, ILayoutElement, ILayoutGroup*/
{
    public event del_OnInstantiate_CollectionItem OnInstantiate_CollectionItem;
    public event del_OnUpdate_CollectionItem OnUpdate_CollectionItem;

    public GameObject PrefabResource;
    public Transform pTransformParents;

    List<GameObject> _listChildrenObject = new List<GameObject>();
    SimplePool<GameObject> _pPool;

    public void DoAllClearEvent_OnInstantiate_CollectionItem()
    {
        OnInstantiate_CollectionItem = null;
    }

    public void DoAllClearEvent_OnUpdate_CollectionItem()
    {
        OnUpdate_CollectionItem = null;;
    }

    public void ICollectionView_UpdateItem(int iItemCount)
    {
        if (_pPool == null)
            return;

        while (_listChildrenObject.Count < iItemCount)
        {
            GameObject pObjectCopy = _pPool.DoPop();
            pObjectCopy.SetActive(true);

            _listChildrenObject.Add(pObjectCopy);
        }

        while (_listChildrenObject.Count > iItemCount && _listChildrenObject.Count > 0)
        {
            int iRemoveIndex = _listChildrenObject.Count - 1;
            GameObject pObjectReturn = _listChildrenObject[iRemoveIndex];
            _listChildrenObject.RemoveAt(iRemoveIndex);

            _pPool.DoPush(pObjectReturn);
            pObjectReturn.SetActive(false);
        }

        if (OnUpdate_CollectionItem == null)
        {
            Debug.LogWarning(name + " OnUpdate_CollectionItem == null", this);
            return;
        }

        for (int i = 0; i < _listChildrenObject.Count; i++)
            OnUpdate_CollectionItem.Invoke(_listChildrenObject[i], i);
    }

    // =================================================================================

    protected override void Awake()
    {
        _pPool = new SimplePool<GameObject>(OnCreateItem, OnDestroyItem, 0);

        if (pTransformParents == null)
            pTransformParents = transform;
    }

    private GameObject OnCreateItem(int iInstanceCount)
    {
        GameObject pObjectCopy = Instantiate(PrefabResource, pTransformParents, true);
#if UNITY_EDITOR
        pObjectCopy.name = pObjectCopy.name.Replace("(Clone)", "");
        pObjectCopy.name += "_" + iInstanceCount;
#endif

        OnInstantiate_CollectionItem?.Invoke(pObjectCopy);
        return pObjectCopy;
    }

    private void OnDestroyItem(GameObject pItem)
    {
        Destroy(pItem);
    }

}