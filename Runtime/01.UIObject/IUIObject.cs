#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-08 오후 2:21:29
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using UIFramework;

// ReSharper disable ConditionIsAlwaysTrueOrFalse
namespace UIFramework
{
    /// <summary>
    /// Canvas 상태
    /// </summary>
    public enum EUIObjectState
    {
        Error = -1,

        /// <summary>
        /// 인스턴스 생성중
        /// </summary>
        Creating,

        /// <summary>
        /// Show Coroutine 플레이 직전
        /// </summary>
        Process_Before_ShowCoroutine,

        /// <summary>
        /// Show Coroutine 플레이 직후
        /// </summary>
        Process_After_ShowCoroutine,

        /// <summary>
        /// Show Animation 절차가 끝나고 현재 Cavnas가 보여지는 중인 상태
        /// </summary>
        Showing,

        /// <summary>
        /// Hide Coroutine 플레이 직전
        /// </summary>
        Process_Before_HideCoroutine,

        /// <summary>
        /// Hide Coroutine 플레이 직후
        /// </summary>
        Process_After_HideCoroutine,

        /// <summary>
        /// Hide Coroutine 후 현재 Canvas를 안쓰는 중인 상태
        /// </summary>
        Disable,

        Destroyed,

        MAX,
    }

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

    EUIObjectState IUIManager_GetUIObjectState<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
        where CLASS_UIOBJECT : IUIObject;
}

public static class IUIObjectExtension
{
    /// <summary>
    /// 오브젝트가 파괴되었는지 null인지 확실하게 체크, 비용이 무겁습니다
    /// </summary>
    public static bool IsNull(this IUIObjectBase pObject)
    {
        MonoBehaviour pMono = pObject as MonoBehaviour;
        if (pObject is MonoBehaviour)
            return pMono == null || ReferenceEquals(pMono, null) || pMono.gameObject.IsNull();
        else
            return pObject == null || ReferenceEquals(pObject, null) || pObject.gameObject.IsNull();
    }

    /// <summary>
    /// 오브젝트가 파괴되었는지 null인지 확실하게 체크, 비용이 무겁습니다
    /// </summary>
    static bool IsNull(this GameObject gameObject)
    {
        return gameObject == null || ReferenceEquals(gameObject, null);
    }

    /// <summary>
    /// 오브젝트의 이름을 Null에 상관없이 얻어옵니다.
    /// </summary>
    public static string GetObjectName_Safe(this IUIObjectBase pObject)
    {
        if (pObject.IsNull())
            return "is null Object";

        return pObject.gameObject.name;
    }
}
