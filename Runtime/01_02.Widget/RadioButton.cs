#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2020-01-16 오전 10:56:12
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 그룹 내에 특정 버튼을 클릭하면 다른 그룹의 버튼들은 클릭이 해제되는 버튼.
/// 하이어라키 부모 기준으로 그룹이 형성됩니다
/// </summary>
public class RadioButton : Selectable, IPointerClickHandler, IUIWidget
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public struct RadioButtonEventMsg
    {
        public RadioButton pRadioButton { get; private set; }
        public bool bIsClick { get; private set; }

        public RadioButtonEventMsg(RadioButton pRadioButton, bool bIsClick)
        {
            this.pRadioButton = pRadioButton; this.bIsClick = bIsClick;
        }
    }

    public delegate void delOnRadioButtonEvent(RadioButtonEventMsg sMsg);
    public event delOnRadioButtonEvent OnRadioButtonEvent;


    static public Dictionary<Transform, HashSet<RadioButton>> g_mapRadioButtonAll = new Dictionary<Transform, HashSet<RadioButton>>();

    [Header("처음 상태를 클릭한 채로")]
    public bool bDefaultClick = true;

    public bool bIsClick { get; private set; }

    /* protected & private - Field declaration         */

    bool _bApplicationIsQuit = false;
    Transform _pTransformParents;

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    public void DoClick_RadioButton()
    {
        HashSet<RadioButton> setButton;
        if(g_mapRadioButtonAll.TryGetValue(_pTransformParents, out setButton) == false)
        {
            if (_bApplicationIsQuit == false)
                Debug.LogError($"{name} RadioButtonGroup.TryGetValue({_pTransformParents}) Fail", this);

            return;
        }

        foreach(var pRadioButton in setButton)
            pRadioButton.DoSetOnClick(false, true);

        DoSetOnClick(true, true);
    }

    public void DoSetOnClick(bool bClick, bool bNotify)
    {
        this.bIsClick = bClick;

        if (bClick)
            DoStateTransition(SelectionState.Pressed, true);
        else
            DoStateTransition(SelectionState.Normal, true);

        if (bNotify)
            OnRadioButtonEvent?.Invoke(new RadioButtonEventMsg(this, bClick));
    }

    // ========================================================================== //

    /* protected - Override & Unity API         */

    protected override void Awake()
    {
        _pTransformParents = transform.parent;
        if (g_mapRadioButtonAll.ContainsKey(_pTransformParents) == false)
            g_mapRadioButtonAll.Add(_pTransformParents, new HashSet<RadioButton>());
        g_mapRadioButtonAll[_pTransformParents].Add(this);

        if (bDefaultClick)
            DoClick_RadioButton();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        if (_bApplicationIsQuit)
            return;

        if (bIsClick)
            state = SelectionState.Pressed;

        base.DoStateTransition(state, instant);
    }

    public IEnumerator OnShowCoroutine()
    {
        yield break;
    }

    public IEnumerator OnHideCoroutine()
    {
        yield break;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DoClick_RadioButton();
    }

    private void OnApplicationQuit()
    {
        _bApplicationIsQuit = true;
    }

    /* protected - [abstract & virtual]         */


    // ========================================================================== //

    #region Private

    #endregion Private


#if UNITY_EDITOR
    [UnityEditor.MenuItem("GameObject/UI/Custom/" + nameof(RadioButton))]
    static public void CreateRadioButton(MenuCommand pCommand)
    {
        const float const_fPosX = 120f;

        GameObject pObjectParents = pCommand.context as GameObject;
        if (pObjectParents == null)
        {
            pObjectParents = new GameObject($"{nameof(RadioButton)}Group");

            // 생성된 오브젝트를 Undo 시스템에 등록한다.
            Undo.RegisterCreatedObjectUndo(pObjectParents, "Create " + pObjectParents.name);
        }


        GameObject pObjectButtonLast = null;
        for (int i = 0; i < 3; i++)
        {
            string strButtonName = $"{nameof(RadioButton)}_{i + 1}";
            pObjectButtonLast = new GameObject(strButtonName);
            GameObjectUtility.SetParentAndAlign(pObjectButtonLast, pObjectParents);
            pObjectButtonLast.transform.position += new Vector3(const_fPosX * i, 0f);

            // 생성된 오브젝트를 Undo 시스템에 등록한다.
            Undo.RegisterCreatedObjectUndo(pObjectButtonLast, "Create " + pObjectButtonLast.name);

            Image pButtonImage = pObjectButtonLast.AddComponent<Image>();
            pButtonImage.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");

            GameObject pObjectText = new GameObject($"Text_{strButtonName}");
            GameObjectUtility.SetParentAndAlign(pObjectText, pObjectButtonLast);

            Text pButtonText = pObjectText.AddComponent<Text>();
            pButtonText.rectTransform.SetAnchor(AnchorPresets.StretchAll);
            pButtonText.rectTransform.sizeDelta = Vector2.one * -20f;
            pButtonText.alignment = TextAnchor.MiddleCenter;
            pButtonText.color = Color.black;
            pButtonText.resizeTextForBestFit = true;
            pButtonText.text = strButtonName;

            RadioButton pRadioButton = pObjectButtonLast.AddComponent<RadioButton>();
        }

        Selection.activeObject = pObjectButtonLast;
    }
#endif

}

#if UNITY_EDITOR

[CustomEditor(typeof(RadioButton))]
public class RadioButton_Inspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
            return;

        RadioButton pTarget = target as RadioButton;

        if(pTarget.bDefaultClick)
        {
            Transform pTransformParent = pTarget.transform.parent;
            RadioButton[] arrButton = pTransformParent.GetComponentsInChildren<RadioButton>();
            for (int i = 0; i < arrButton.Length; i++)
            {
                RadioButton pRadioButton = arrButton[i];
                if(pRadioButton != pTarget)
                    pRadioButton.bDefaultClick = false;

                pRadioButton.DoSetOnClick(pRadioButton.bDefaultClick, false);
            }
        }
    }
}

#endif