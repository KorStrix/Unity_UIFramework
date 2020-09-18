#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-21 오후 12:31:30
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;
using UIFramework;

#if UNITY_EDITOR
using UnityEditor;
#endif



/// <summary>
/// 관리받는(관리자(<see cref="IUIManager"/>가 있는) <see cref="IUIObject"/> 
/// </summary>
public interface IUIObject_Managed
{
    IUIManager pUIManager { get; set; }
}

/// <summary>
/// <see cref="Canvas"/>와 <see cref="IUIWidget"/> 자식들을 가지고 있는 UI의 한 장면을 담당하는 객체
/// </summary>
public interface ICanvas : IUIObject, IUIObject_Managed
{
    // UIWidgetContainer pWidgetContainer { get; }
}

public static class ICanvasHelper
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Custom/" + "PopupBase")]
    public static void CreatePopup(MenuCommand pCommand)
    {
        GameObject pObjectParents = pCommand.context as GameObject;

        GameObject pObjectPopup = new GameObject("SomthingPopup");
        GameObjectUtility.SetParentAndAlign(pObjectPopup, pObjectParents);

        RectTransform pRectTransform_Popup = pObjectPopup.AddComponent<RectTransform>();
        pRectTransform_Popup.SetAnchor(AnchorPresets.StretchAll);
        pRectTransform_Popup.sizeDelta = Vector2.zero;
        pObjectPopup.AddComponent<Canvas>();
        pObjectPopup.AddComponent<GraphicRaycaster>();

        GameObject pObjectBG = new GameObject("Image_BG");
        GameObjectUtility.SetParentAndAlign(pObjectBG, pObjectPopup);

        Image pImageBG = pObjectBG.AddComponent<Image>();
        pImageBG.color = Color.white;
        pImageBG.rectTransform.SetAnchor(AnchorPresets.StretchAll);
        pImageBG.rectTransform.sizeDelta = new Vector2(-600f, -300f);

        GameObject pObjectTitleBG = new GameObject("Image_TitleBG");
        GameObjectUtility.SetParentAndAlign(pObjectTitleBG, pObjectBG);

        Image pImageTitleBG = pObjectTitleBG.AddComponent<Image>();
        pImageTitleBG.color = Color.gray;
        pImageTitleBG.rectTransform.SetAnchor(AnchorPresets.TopCenter);
        pImageTitleBG.rectTransform.sizeDelta = new Vector2(300f, 100f);

        GameObject pObjectTitleText = new GameObject("Text_Title");
        GameObjectUtility.SetParentAndAlign(pObjectTitleText, pObjectTitleBG);

        Text pTextTitle = pObjectTitleText.AddComponent<Text>();
        pTextTitle.color = Color.black;
        pTextTitle.text = "Title";



        GameObject pObjectButtonClose = new GameObject("Button_Close");
        GameObjectUtility.SetParentAndAlign(pObjectButtonClose, pObjectBG);

        Image pImageButton = pObjectButtonClose.AddComponent<Image>();
        pImageButton.rectTransform.sizeDelta = new Vector2(150f, 50f);

        pObjectButtonClose.AddComponent<Button>();

        GameObject pObjectButtonCloseText = new GameObject("Text_Close");
        GameObjectUtility.SetParentAndAlign(pObjectButtonCloseText, pObjectButtonClose);

        Text pTextCloseButton = pObjectButtonCloseText.AddComponent<Text>();
        pTextCloseButton.rectTransform.SetAnchor(AnchorPresets.StretchAll);
        pTextCloseButton.text = "Close";

        // 생성된 오브젝트를 Undo 시스템에 등록.
        Undo.RegisterCreatedObjectUndo(pObjectPopup, "Create " + pObjectPopup.name);
        Selection.activeObject = pObjectPopup;
    }
#endif


    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 켭니다.
    /// </summary>
    public static UICommandHandle<T> DoShowCoroutine<T>(this T pObject)
        where T : MonoBehaviour, ICanvas
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager", pObject.gameObject.name, nameof(DoShowCoroutine), pObject);
            pObject.gameObject.SetActive(true);

            return null;
        }

        return pObject.pUIManager.IUIManager_Show(pObject);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// <para>Delegate 핸들링용</para>
    /// </summary>
    public static void DoHide_Coroutine_ReturnVoid<T>(this T pObject)
        where T : ICanvas
    {
        DoHide_Coroutine(pObject);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// </summary>
    public static UICommandHandle<T> DoHide_Coroutine<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject == null)
            return null;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager", pObject.gameObject.name, nameof(DoHide_Coroutine), pObject);
            pObject.gameObject.SetActive(false);

            return null;
        }

        return pObject.pUIManager.IUIManager_Hide(pObject, true);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// <para>코루틴을 실행하지 않습니다.</para>
    /// <para>Delegate 핸들링용</para>
    /// </summary>
    public static void DoHide_NotPlayHideCoroutine_ReturnVoid<T>(this T pObject)
        where T : ICanvas
    {
        DoHide_NotPlayHideCoroutine(pObject);
    }

    /// <summary>
    /// 이 오브젝트를 관리하는 매니져를 찾아 매니져를 통해 오브젝트를 끕니다.
    /// <para>코루틴을 실행하지 않습니다.</para>
    /// </summary>
    public static UICommandHandle<T> DoHide_NotPlayHideCoroutine<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject.IsNull())
            return null;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager", pObject.gameObject.name, nameof(DoHide_NotPlayHideCoroutine), pObject);
            pObject.gameObject.SetActive(false);

            return null;
        }

        return pObject.pUIManager.IUIManager_Hide(pObject, false);
    }

    /// <summary>
    /// 이 오브젝트의 UI 상태를 얻습니다.
    /// </summary>
    public static EUIObjectState GetUIObjectState<T>(this T pObject)
        where T : ICanvas
    {
        if (pObject.IsNull())
            return EUIObjectState.Disable;

        if (pObject.pUIManager.IsNull())
        {
            Debug.LogWarningFormat("{0} {1} - Not Found Manager", pObject.gameObject.name, nameof(GetUIObjectState), pObject);
            return EUIObjectState.Disable;
        }

        return pObject.pUIManager.IUIManager_GetUIObjectState(pObject);
    }
}
