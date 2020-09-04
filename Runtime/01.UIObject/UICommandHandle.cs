#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-08 오후 2:21:29
 *	개요 : 
   ============================================ */
#endregion Header

using System;
using System.Collections;
using System.Collections.Generic;
using UIFramework;
using UnityEngine;

// ReSharper disable PossibleNullReferenceException
public class UICommandHandle<TUIObject> : System.IDisposable
    where TUIObject : IUIObject
{
    #region Static

    public static bool g_bIsDebug = false;

    public static int g_iInstanceCount => g_mapHandle.Count;

    // 이걸 map 으로 바꿔야하나? key를 TUIObject로 해서..?
    static Dictionary<TUIObject, UICommandHandle<TUIObject>> g_mapHandle = new Dictionary<TUIObject, UICommandHandle<TUIObject>>();
    static SimplePool<UICommandHandle<TUIObject>> g_pPool;

    public static UICommandHandle<TUIObject> GetInstance(TUIObject pObject_OrNull)
    {
        if (pObject_OrNull != null && g_mapHandle.TryGetValue(pObject_OrNull, out UICommandHandle<TUIObject> pHandle))
            return pHandle;


        if (g_pPool == null)
            g_pPool = new SimplePool<UICommandHandle<TUIObject>>(iCount => new UICommandHandle<TUIObject>(iCount), null, 1);

        pHandle = g_pPool.DoPop();
        if (pObject_OrNull != null)
        {
            if (g_mapHandle.ContainsKey(pObject_OrNull) == false)
                g_mapHandle.Add(pObject_OrNull, pHandle);

            pHandle.Set_UIObject(pObject_OrNull);
        }

        return pHandle.DoReset();
    }

    public static UICommandHandle<TUIObject> GetInstance_Used_OrNull(TUIObject pObject_OrNull)
    {
        g_mapHandle.TryGetValue(pObject_OrNull, out var pHandle);

        return pHandle;
    }

    public static void ReturnInstance(UICommandHandle<TUIObject> pHandle)
    {
        pHandle.Set_UIState(EUIObjectState.Destroyed);

        if (pHandle.pUIObject != null)
            g_mapHandle.Remove(pHandle.pUIObject);
        g_pPool.DoPush(pHandle);
    }

    #endregion



    public event System.Action<TUIObject> OnBeforeShow;

    public System.Func<System.Action<bool>, IEnumerator> OnChecking_IsShow;

    public event System.Action<TUIObject> OnShow_BeforeAnimation;
    public event System.Action<TUIObject> OnShow_AfterAnimation;
    public event System.Action<TUIObject> OnHide;

    /// <summary>
    /// 호출한 직후는 null이며 한프레임 기다려야 합니다.
    /// </summary>
    public TUIObject pUIObject { get; private set; }
    public EUIObjectState eState { get; private set; }

    public int iID { get; private set; }

    public bool bIsExecute_BeforeShow { get; private set; }
    public bool bIsFinish_Animation { get; private set; }

    public UICommandHandle<TUIObject> DoReset()
    {
        pUIObject = default;
        Set_UIState(EUIObjectState.Creating);

        // Null Check를 안하기 위해 Default Func Setting
        OnBeforeShow = DefaultFunc;
        OnChecking_IsShow = DefaultCo;

        OnShow_BeforeAnimation = DefaultFunc;
        OnShow_AfterAnimation = DefaultFunc;
        OnHide = DefaultFunc;

        return DoResetFlag();
    }

    public UICommandHandle<TUIObject> DoResetFlag()
    {
        bIsExecute_BeforeShow = false;
        bIsFinish_Animation = false;

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

    public void Set_UIObject(TUIObject pUIObject)
    {
        this.pUIObject = pUIObject;

        if (g_mapHandle.ContainsKey(pUIObject) == false)
            g_mapHandle.Add(pUIObject, this);
    }

    /// <summary>
    /// 무한 Animation이 떠서 어쩔수 없이 캔슬되는 경우
    /// 직후 Event를 호출해야 하기 때문에 CommandHandle도 State를 저장
    /// </summary>
    /// <param name="eState"></param>
    public void Set_UIState(EUIObjectState eState)
    {
        this.eState = eState;
    }

    public UICommandHandle<T> Cast<T>()
        where T : class, IUIObject
    {
        T pObjectCast = pUIObject as T;
        if (pUIObject != null && pObjectCast == null)
            Debug.LogError($"Type : {typeof(T).Name} - Casting Fail Name : {pUIObject?.gameObject.name}", pUIObject?.gameObject);

        UICommandHandle<T> pCast = UICommandHandle<T>.GetInstance(pObjectCast);
        pCast.bIsExecute_BeforeShow = this.bIsExecute_BeforeShow;
        pCast.bIsFinish_Animation = this.bIsFinish_Animation;

        ReturnInstance(this);

        return pCast;
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

        if (g_bIsDebug)
            Debug.Log($"{pUIObject.gameObject.name} - {nameof(Event_OnHide)} - bExecute : {bExecute}", pUIObject.gameObject);

        ReturnInstance(this);
    }

    public IEnumerator Yield_WaitForSetUIObject()
    {
        while (pUIObject == null)
        {
            yield return null;
        }
    }

    /// <summary>
    /// 애니메이션이 끝날때 까지 기다립니다.
    /// </summary>
    /// <returns></returns>
    public IEnumerator Yield_WaitForAnimation()
    {
        while (bIsFinish_Animation == false)
        {
            yield return null;
        }
    }

    /// <summary>
    /// 애니메이션이 끝날때 까지 기다립니다. 타임아웃 시간이 지나면 에러와 함께 Yield Break합니다.
    /// </summary>
    /// <param name="fTimeOutSec">기다릴 시간</param>
    public IEnumerator Yield_WaitForAnimation_TimeOut(float fTimeOutSec = 10f)
    {
        float fRemainTime = fTimeOutSec;

        while (bIsFinish_Animation == false || fRemainTime > 0f)
        {
            fRemainTime -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (fRemainTime <= 0f)
        {
            bIsFinish_Animation = true;
            Debug.LogError($"{pUIObject.GetObjectName_Safe()} is {nameof(Yield_WaitForAnimation_TimeOut)} Timeout Wait Time : {fTimeOutSec}");

            OnForceCancel();
        }
    }

    private void OnForceCancel()
    {
        switch (eState)
        {
            case EUIObjectState.Process_Before_ShowCoroutine:
                Event_OnShow_BeforeAnimation();
                break;

            case EUIObjectState.Process_After_ShowCoroutine:
                Event_OnShow_AfterAnimation();
                break;

            case EUIObjectState.Process_Before_HideCoroutine:
            case EUIObjectState.Process_After_HideCoroutine:
                Event_OnHide();
                break;
        }
    }

    public override string ToString()
    {
        return base.ToString() + "_" + iID;
    }

    void DefaultFunc(TUIObject pObject) { }
    IEnumerator DefaultCo(System.Action<bool> OnCheckFinish) { OnCheckFinish(true); yield break; }
}

public static class UICommandHandleHelper
{
    public static UICommandHandle<T> Set_OnCheck_IsShow<T>(this UICommandHandle<T> pHandle, System.Func<System.Action<bool>, IEnumerator> OnChecking_IsShow)
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

    public static UICommandHandle<T> GetHandle<T>(this T pCanvas)
        where T : IUIObject
    {
        return UICommandHandle<T>.GetInstance_Used_OrNull(pCanvas);
    }
}
