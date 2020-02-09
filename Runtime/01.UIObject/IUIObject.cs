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
/// 이 프레임워크의 모든 오브젝트 베이스
/// </summary>
public interface IUIObjectBase
{
    GameObject gameObject { get; }
    Transform transform { get; }
}

/// <summary>
///<see cref="ICanvas"/>, <see cref="IUIWidget"/> 등 모든 UI Object의 Base
/// </summary>
public interface IUIObject : IUIObjectBase
{
    /// <summary>
    /// 매니져를 통해 오브젝트를 켤 때 - Active(True) -> <see cref="OnShowCoroutine"/>
    /// <para> 코루틴이 해당 오브젝트가 아닌 매니져가 실행하기 때문에 <see cref="IUIObject"/>의 Active유무와 관계없습니다. </para>
    /// </summary>
    IEnumerator OnShowCoroutine();

    /// <summary>
    /// 매니져를 통해 오브젝트를 끌 때 -> <see cref="OnHideCoroutine"/> -> Active(False)
    /// <para> 코루틴이 해당 오브젝트가 아닌 매니져가 실행하기 때문에 <see cref="IUIObject"/>의 Active유무와 관계없습니다. </para>
    /// </summary>
    IEnumerator OnHideCoroutine();
}

public class UICommandHandle<T> : System.IDisposable
    where T : IUIObject
{
    static public int iInstanceCount { get { return g_setHandle.Count; } }

    static HashSet<UICommandHandle<T>> g_setHandle = new HashSet<UICommandHandle<T>>();
    static SimplePool<UICommandHandle<T>> g_pPool = null;

    public static UICommandHandle<T> GetInstance(T pObject_OrNull)
    {
        if(g_pPool == null)
            g_pPool = new SimplePool<UICommandHandle<T>>(iCount => new UICommandHandle<T>(iCount), null, 1);

        UICommandHandle<T> pHandle = g_pPool.DoPop();
        if (pObject_OrNull != null)
            g_setHandle.Add(pHandle);

        return pHandle.Reset();
    }

    public static void ReturnInstance(UICommandHandle<T> pHandle)
    {
        g_setHandle.Remove(pHandle);
        g_pPool.DoPush(pHandle);
    }

    /// <summary>
    /// 호출한 직후는 null이며 한프레임 기다려야 합니다.
    /// </summary>
    public T pUIObject { get; private set; }
    public int iID { get; private set; }

    public delegate IEnumerator delOnChecking_IsShow(System.Action<bool> OnCheck_IsShow);

    System.Action<T> _OnBeforeShow;
    delOnChecking_IsShow _OnChecking_IsShow;

    System.Action<T> _OnShow_BeforeAnimation;
    System.Action<T> _OnShow_AfterAnimation;
    System.Action<T> _OnHide;

    bool _bIsExecute_BeforeShow;
    bool _bIsFinish_Animation;

    public UICommandHandle<T> Reset()
    {
        // Null Check를 안하기 위해 Default Func Setting
        _OnBeforeShow = DefaultFunc;
        _OnChecking_IsShow = DefaultCo;

        _OnShow_BeforeAnimation = DefaultFunc;
        _OnShow_AfterAnimation = DefaultFunc;
        _OnHide = DefaultFunc;

        _bIsExecute_BeforeShow = false;
        _bIsFinish_Animation = false;
        pUIObject = default(T);

        return this;
    }


    /// <summary>
    /// 싱글톤 패턴을 위한 외부 접근가능한 생성자 제한
    /// </summary>
    protected UICommandHandle(int iID)
    {
        this.iID = iID;
    }

    /// <summary>
    /// GC에 의해 소멸될 때 호출
    /// </summary>
    ~UICommandHandle()
    {
        ReturnInstance(this);
    }

    public void Dispose()
    {
        ReturnInstance(this);
    }

    public void Set_UIObject(T pUIObject)
    {
        this.pUIObject = pUIObject;
    }

    public void Event_OnBeforeShow() { _bIsExecute_BeforeShow = true; _OnBeforeShow(pUIObject); }
    public IEnumerator DoExecute_Check_IsShowCoroutine(System.Action<bool> OnCheck_IsShow)  { yield return _OnChecking_IsShow(OnCheck_IsShow);  }

    public void Event_OnShow_BeforeAnimation() { _OnShow_BeforeAnimation(pUIObject); }
    public void Event_OnShow_AfterAnimation() { _bIsFinish_Animation = true; _OnShow_AfterAnimation(pUIObject); }
    public void Event_OnHide(bool bExecute = true)  
    {
        _bIsFinish_Animation = true;

        if (bExecute)
            _OnHide(pUIObject); 

        ReturnInstance(this); 
    }

    public UICommandHandle<T> Set_OnBeforeShow(System.Action<T> OnBeforeShow)
    {
        if (_bIsExecute_BeforeShow)
            OnBeforeShow(pUIObject);
        else
            this._OnBeforeShow = OnBeforeShow;
        return this;
    }

    public UICommandHandle<T> Set_OnCheck_IsShow(delOnChecking_IsShow OnChecking_IsShow)
    {
        this._OnChecking_IsShow = OnChecking_IsShow;

        return this;
    }

    public UICommandHandle<T> Set_OnShow_BeforeAnimation(System.Action<T> OnShow)
    {
        this._OnShow_BeforeAnimation = OnShow;
        return this;
    }

    public UICommandHandle<T> Set_OnShow_AfterAnimation(System.Action<T> OnShow)
    {
        this._OnShow_AfterAnimation = OnShow;
        return this;
    }

    public UICommandHandle<T> Set_OnHide(System.Action<T> OnHide)
    {
        this._OnHide = OnHide;
        return this;
    }

    public IEnumerator Yield_WaitForSetUIObject()
    {
        while (pUIObject == null)
        {
            yield return null;
        }
    }

    public IEnumerator Yield_WaitForAnimation()
    {
        while(_bIsFinish_Animation == false)
        {
            yield return null;
        }
    }

    public override string ToString()
    {
        return base.ToString() + "_" + iID;
    }

    void DefaultFunc(T pObject) { }
    IEnumerator DefaultCo(System.Action<bool> OnCheckFinish) { OnCheckFinish(true); yield break; }

}

/// <summary>
/// 
/// </summary>
public interface IUIManager : IUIObjectBase
{
    UICommandHandle<CLASS_UIOBJECT> IUIManager_Show<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
        where CLASS_UIOBJECT : IUIObject;

    UICommandHandle<CLASS_UIOBJECT> IUIManager_Hide<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject, bool bPlayHideCoroutine)
        where CLASS_UIOBJECT : IUIObject;
}