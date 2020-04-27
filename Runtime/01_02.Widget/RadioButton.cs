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
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.UI;
#endif

namespace UIFramework
{
    /// <summary>
    /// 그룹 내에 특정 버튼을 클릭하면 다른 그룹의 버튼들은 클릭이 해제되는 버튼.
    /// 하이어라키 부모 기준으로 그룹이 형성됩니다.
    /// </summary>
    public class RadioButton : ButtonExtension, IPointerClickHandler, IUIWidget
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        public struct RadioButtonEventMsg
        {
            public IEnumerable<RadioButton> arrButtonGroup { get; private set; }
            public RadioButton pRadioButton { get; private set; }
            public bool bIsClick { get; private set; }

            public RadioButtonEventMsg(IEnumerable<RadioButton> arrButtonGroup, RadioButton pRadioButton, bool bIsClick)
            {
                this.arrButtonGroup = arrButtonGroup; this.pRadioButton = pRadioButton; this.bIsClick = bIsClick;
            }
        }

        [System.Serializable]
        public class StateActiveObject
        {
            public bool bPressed;
            public GameObject pObject;
        }

        /* public - Field declaration            */

        public delegate void delOnRadioButtonEvent(RadioButtonEventMsg sMsg);

        /// <summary>
        /// 그룹 내 라디오 버튼이 클릭되었을 경우
        /// </summary>
        public event delOnRadioButtonEvent OnRadioButtonEvent;

        public static Dictionary<Transform, HashSet<RadioButton>> g_mapRadioButtonAll = new Dictionary<Transform, HashSet<RadioButton>>();


        [Header("처음 상태를 클릭한 채로")]
        public bool bDefaultClick = true;

        [SerializeField]
        public List<StateActiveObject> listActiveObject_OnClicked = new List<StateActiveObject>();

        /// <summary>
        /// 현재 클릭되있는 상태인지
        /// </summary>
        public bool bIsClick { get; private set; }

        /* protected & private - Field declaration         */

        Dictionary<bool, List<GameObject>> _mapOnStateActive = new Dictionary<bool, List<GameObject>>();
        bool _bApplicationIsQuit = false;
        Transform _pTransformParents;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public void DoClick_RadioButton()
        {
            if (g_mapRadioButtonAll.TryGetValue(_pTransformParents, out var setButton) == false)
            {
                if (_bApplicationIsQuit == false)
                    Debug.LogError($"{name} RadioButtonGroup.TryGetValue({_pTransformParents}) Fail", this);

                return;
            }

            foreach (var pRadioButton in setButton)
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
            {
                g_mapRadioButtonAll.TryGetValue(_pTransformParents, out var setButton);
                OnRadioButtonEvent?.Invoke(new RadioButtonEventMsg(setButton, this, bClick));
            }
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected override void Awake()
        {
            _pTransformParents = transform.parent;
            if (g_mapRadioButtonAll.ContainsKey(_pTransformParents) == false)
                g_mapRadioButtonAll.Add(_pTransformParents, new HashSet<RadioButton>());
            g_mapRadioButtonAll[_pTransformParents].Add(this);

            _mapOnStateActive = listActiveObject_OnClicked.
                GroupBy(p => p.bPressed).
                ToDictionary(x => x.Key, y => y.Where(z => z.pObject != null).Select(z => z.pObject).ToList());
        }

        protected override void Start()
        {
            base.Start();
            
            if (bDefaultClick)
            {
                DoClick_RadioButton();
                DoStateTransition(SelectionState.Pressed, true);
            }
        }

        protected override void DoStateTransition(SelectionState state, bool instant)
        {
#if UNITY_EDITOR
            if (Application.isPlaying == false)
                return;
#endif

            if (_bApplicationIsQuit)
                return;

            if (bIsClick)
                state = SelectionState.Pressed;

            base.DoStateTransition(state, instant);

            bool bIsPressed = state == SelectionState.Pressed;
            foreach (var pOnStateActive in _mapOnStateActive)
            {
                bool bActive = (pOnStateActive.Key == bIsPressed);
                pOnStateActive.Value.ForEach(p => p.SetActive(bActive));
            }
        }

        public IEnumerator OnShowCoroutine()
        {
            yield break;
        }

        public IEnumerator OnHideCoroutine()
        {
            yield break;
        }

        public void IUIWidget_OnBeforeShow()
        {
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

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
        public static void CreateRadioButton(MenuCommand pCommand)
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

    [CanEditMultipleObjects]
    [CustomEditor(typeof(RadioButton))]
    public class RadioButton_Inspector : ButtonExtensionInspector
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("bDefaultClick"), true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("listActiveObject_OnClicked"), true);
            serializedObject.ApplyModifiedProperties();

            if (Application.isPlaying)
                return;

            RadioButton pTarget = target as RadioButton;
            if (pTarget.bDefaultClick)
            {
                Transform pTransformParent = pTarget.transform.parent;
                RadioButton[] arrButton = pTransformParent.GetComponentsInChildren<RadioButton>();
                for (int i = 0; i < arrButton.Length; i++)
                {
                    RadioButton pRadioButton = arrButton[i];
                    if (pRadioButton != pTarget)
                        pRadioButton.bDefaultClick = false;

                    pRadioButton.DoSetOnClick(pRadioButton.bDefaultClick, false);
                }
            }
        }
    }

#endif
}