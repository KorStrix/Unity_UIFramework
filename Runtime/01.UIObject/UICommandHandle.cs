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
using UIFramework;

public class UICommandHandle<T> : System.IDisposable
    where T : IUIObject
{
    #region Static
    public static int g_iInstanceCount { get { return g_setHandle.Count; } }

    static HashSet<UICommandHandle<T>> g_setHandle = new HashSet<UICommandHandle<T>>();
    static SimplePool<UICommandHandle<T>> g_pPool = null;

    public static UICommandHandle<T> GetInstance(T pObject_OrNull)
    {
        if (g_pPool == null)
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
    #endregion



    public event System.Action<T> OnBeforeShow;

    public delegate IEnumerator delOnChecking_IsShow(System.Action<bool> OnCheck_IsShow);
    public delOnChecking_IsShow OnChecking_IsShow;

    public event System.Action<T> OnShow_BeforeAnimation;
    public event System.Action<T> OnShow_AfterAnimation;
    public event System.Action<T> OnHide;

    /// <summary>
    /// 호출한 직후는 null이며 한프레임 기다려야 합니다.
    /// </summary>
    public T pUIObject { get; private set; }
    public int iID { get; private set; }

    public bool bIsExecute_BeforeShow { get; private set; }
    public bool bIsFinish_Animation { get; private set; }
    public bool bIsDisable { get; private set; }

    public UICommandHandle<T> Reset()
    {
        // Null Check를 안하기 위해 Default Func Setting
        OnBeforeShow = DefaultFunc;
        OnChecking_IsShow = DefaultCo;

        OnShow_BeforeAnimation = DefaultFunc;
        OnShow_AfterAnimation = DefaultFunc;
        OnHide = DefaultFunc;

        bIsExecute_BeforeShow = false;
        bIsFinish_Animation = false;
        bIsDisable = false;

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

    public void Event_OnBeforeShow() { bIsExecute_BeforeShow = true; OnBeforeShow(pUIObject); }
    public IEnumerator DoExecute_Check_IsShowCoroutine(System.Action<bool> OnCheck_IsShow) { yield return OnChecking_IsShow(OnCheck_IsShow); }

    public void Event_OnShow_BeforeAnimation() { OnShow_BeforeAnimation(pUIObject); }
    public void Event_OnShow_AfterAnimation() { bIsFinish_Animation = true; OnShow_AfterAnimation(pUIObject); }
    public void Event_OnHide(bool bExecute = true)
    {
        bIsFinish_Animation = true;

        if (bExecute)
            OnHide(pUIObject);

        ReturnInstance(this);
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
        while (bIsFinish_Animation == false)
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

public static class UICommandHandleHelper
{
    public static UICommandHandle<T> Set_OnCheck_IsShow<T>(this UICommandHandle<T> pHandle, UICommandHandle<T>.delOnChecking_IsShow OnChecking_IsShow)
        where T : IUIObject
    {
        pHandle.OnChecking_IsShow = OnChecking_IsShow;
        return pHandle;
    }

    public static UICommandHandle<T> Set_OnBeforeShow<T>(this UICommandHandle<T> pHandle, System.Action<T> OnBeforeShow)
        where T : IUIObject
    {
        if (pHandle.bIsExecute_BeforeShow)
            OnBeforeShow(pHandle.pUIObject);
        else
            pHandle.OnBeforeShow += OnBeforeShow;

        return pHandle;
    }

    public static UICommandHandle<T> Set_OnShow_BeforeAnimation<T>(this UICommandHandle<T> pHandle, System.Action<T> OnShow)
        where T : IUIObject
    {
        pHandle.OnShow_BeforeAnimation += OnShow;
        return pHandle;
    }

    public static UICommandHandle<T> Set_OnShow_AfterAnimation<T>(this UICommandHandle<T> pHandle, System.Action<T> OnShow)
        where T : IUIObject
    {
        pHandle.OnShow_AfterAnimation += OnShow;
        return pHandle;
    }

    public static UICommandHandle<T> Set_OnHide<T>(this UICommandHandle<T> pHandle, System.Action<T> OnHide)
        where T : IUIObject
    {
        pHandle.OnHide += OnHide;
        return pHandle;
    }
}
