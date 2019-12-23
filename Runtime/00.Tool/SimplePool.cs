#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-11-12 오전 10:07:06
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 간단하게 사용하는 풀링 매니져.
/// <para>사용전 꼭 <see cref="SimplePool{}.DoInit(SimplePool{}.OnCreateItem, SimplePool{}.OnDestroyItem, int)"/>를 호출하기 바랍니다.</para>
/// </summary>
public class SimplePool<T>
    where T : class
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public int iAllInstanceCount { get { return _list_AllInstance.Count; } }
    public int iUnUsedInstanceCount { get { return _list_Unused.Count; } }
    public int iUsedInstanceCount { get { return _list_AllInstance.Count - _list_Unused.Count; } }

    /* protected & private - Field declaration         */

    public delegate T OnCreateItem(int iInstanceCount);
    public OnCreateItem _OnCreateItem;

    public delegate void OnDestroyItem(T pItem);
    public OnDestroyItem _OnDestoryItem;

    List<T> _list_AllInstance = new List<T>();
    List<T> _list_Unused = new List<T>();
    HashSet<T> _set_Unused_Checker = new HashSet<T>();

    bool _bIsInit = false;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public SimplePool(OnCreateItem OnCreateItem, OnDestroyItem OnDestroyItem, int iPrePoolCount)
    {
        DoDestroyPool(false);

        _bIsInit = true;
        _OnCreateItem = OnCreateItem;
        _OnDestoryItem = OnDestroyItem;

        for (int i = 0; i < iPrePoolCount; i++)
            _list_Unused.Add(CreateItem());
    }

    /// <summary>
    /// 풀을 비웁니다.
    /// </summary>
    public void DoDestroyPool(bool bReset_InitFlag = false)
    {
        _list_Unused.Clear();

        T[] arrInstanceCopy = new T[_list_AllInstance.Count];
        _list_AllInstance.CopyTo(arrInstanceCopy);
        _list_AllInstance.Clear();

        if(_OnDestoryItem != null)
        {
            for (int i = 0; i < arrInstanceCopy.Length; i++)
                _OnDestoryItem(arrInstanceCopy[i]);
        }

        _bIsInit = !bReset_InitFlag;
    }

    /// <summary>
    /// 사용했던 풀 오브젝트를 모두 반환합니다.
    /// </summary>
    public void DoPushAll()
    {
        for(int i = 0; i < _list_AllInstance.Count; i++)
            DoPush(_list_AllInstance[i]);
    }

    /// <summary>
    /// 풀에서 사용하지 않는 오브젝트를 요청합니다.
    /// </summary>
    /// <returns></returns>
    public T DoPop()
    {
        if(_bIsInit == false)
        {
            Debug.LogError(GetName() + " Pop - _bIsInit == false");
            return null;
        }

        T pItem = null;
        if (_list_Unused.Count > 0)
        {
            pItem = _list_Unused[_list_Unused.Count - 1];
            _list_Unused.RemoveAt(_list_Unused.Count - 1);
            _set_Unused_Checker.Remove(pItem);
        }
        else
        {
            pItem = CreateItem();
        }

        return pItem;
    }

    /// <summary>
    /// 사용한 오브젝트를 반환합니다. 리턴값은 반환 성공 유무입니다.
    /// </summary>
    /// <param name="pItem"></param>
    public bool DoPush(T pItem)
    {
        if (_bIsInit == false)
            return false;

        bool bIsSuccessReturn = _set_Unused_Checker.Add(pItem);
        if (bIsSuccessReturn)
            _list_Unused.Add(pItem);

        return bIsSuccessReturn;
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    private T CreateItem()
    {
        T pItem = _OnCreateItem(_list_AllInstance.Count);
        _list_AllInstance.Add(pItem);

        return pItem;
    }

    static string GetName()
    {
        return string.Format("{0}<{1}> ", nameof(SimplePool<T>), typeof(T).ToString());
    }

    #endregion Private
}