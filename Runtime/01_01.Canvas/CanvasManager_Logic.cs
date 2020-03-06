#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-30 오후 5:29:29
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UIFramework.UIWidgetContainerManager_Logic;

namespace UIFramework
{
    public enum ECanvasManagerLogicName
    {
        Print_CanvasState,
        SetTransform_LastSibling,
        Show_Object,
        Lock_AllInput,
    }

    public class CanvasManagerLogicFactory
    {
        public Dictionary<EUIObjectState, List<ICanvasManager_Logic>> mapLogicContainer = new Dictionary<EUIObjectState, List<ICanvasManager_Logic>>();

        public ICanvasManager_Logic DoCreate_LibraryLogic(EUIObjectState eState, ECanvasManagerLogicName eLogic)
        {
            ICanvasManager_Logic pLogic = null;
            switch (eLogic)
            {
                case ECanvasManagerLogicName.Print_CanvasState: pLogic = new Print_CanvasState(eState); break;
                case ECanvasManagerLogicName.SetTransform_LastSibling: pLogic = new SetTransform_LastSibling(); break;
                case ECanvasManagerLogicName.Show_Object: pLogic = new Show_Object(); break;
                case ECanvasManagerLogicName.Lock_AllInput: pLogic = new Lock_AllInput(); break;

                default: Debug.LogError("Error - Not Found Logic"); return null;
            }

            if (mapLogicContainer.ContainsKey(eState) == false)
                mapLogicContainer.Add(eState, new List<ICanvasManager_Logic>());
            mapLogicContainer[eState].Add(pLogic);

            return pLogic;
        }

        public void DoAddLogic_PossibleUndo(EUIObjectState eState, EUIObjectState eUndoState, ICanvasManager_Logic_IsPossible_Undo pLogic)
        {
            if (mapLogicContainer.ContainsKey(eState) == false)
                mapLogicContainer.Add(eState, new List<ICanvasManager_Logic>());

            mapLogicContainer[eState].Add(new CanvasManager_LogicUndo_Wrapper(pLogic, eUndoState));
        }

        public void DoAddLogic(EUIObjectState eState, params ICanvasManager_Logic[] arrLogic)
        {
            if (mapLogicContainer.ContainsKey(eState) == false)
                mapLogicContainer.Add(eState, new List<ICanvasManager_Logic>());
            mapLogicContainer[eState].AddRange(arrLogic);
        }
    }

    namespace UIWidgetContainerManager_Logic
    {
        #region ICanvasManager_Logic

        /// <summary>
        /// 
        /// </summary>
        public interface ICanvasManager_Logic
        {
            IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug);
        }

        /// <summary>
        /// 
        /// </summary>
        public interface ICanvasManager_Logic_IsPossible_Undo : ICanvasManager_Logic
        {
            IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug);
            void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug);
        }

        public class CanvasManager_LogicUndo_Wrapper : ICanvasManager_Logic_IsPossible_Undo
        {
            public ICanvasManager_Logic_IsPossible_Undo pLogic { get; private set; }
            public EUIObjectState eWhenUndo { get; private set; }


            public CanvasManager_LogicUndo_Wrapper(ICanvasManager_Logic_IsPossible_Undo pLogic, EUIObjectState eWhenUndo)
            {
                this.pLogic = pLogic; this.eWhenUndo = eWhenUndo;
            }

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                yield return pLogic.Execute_LogicCoroutine(pManager, pCanvas, bIsDebug);
            }

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                yield return pLogic.Execute_UndoLogic_Coroutine(pManager, pCanvas, bIsDebug);
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                pLogic.Execute_UndoLogic_NotCoroutine(pManager, pCanvas, bIsDebug);
            }
        }
        #endregion


        public class Print_CanvasState : ICanvasManager_Logic
        {
            EUIObjectState _eState;

            public Print_CanvasState(EUIObjectState eState)
            {
                _eState = eState;
            }

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if(pCanvas.IsNull())
                    Debug.Log(nameof(Print_CanvasState) + "Object is Null /// ECavnasState : " + _eState);
                else
                    Debug.Log(nameof(Print_CanvasState) + " " + pCanvas.gameObject.name + "// ECavnasState : " + _eState, pCanvas.gameObject);

                yield break;
            }
        }

        public class SetTransform_LastSibling : ICanvasManager_Logic
        {
            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (pCanvas.IsNull())
                    yield break;

                pCanvas.transform.SetAsLastSibling();

                yield break;
            }
        }

        public class Show_Object : ICanvasManager_Logic_IsPossible_Undo
        {
            Dictionary<ICanvas, GameObject> mapShowObject = new Dictionary<ICanvas, GameObject>();

            GameObject pObject;
            System.Func<GameObject, ICanvas, GameObject> OnRequireObject;
            System.Action<GameObject> OnHideObject;

            public void DoInit(GameObject pObject, System.Func<GameObject, ICanvas, GameObject> OnRequireObject, System.Action<GameObject> OnHideObject)
            {
                this.pObject = pObject; this.OnRequireObject = OnRequireObject; this.OnHideObject = OnHideObject;
            }

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (OnRequireObject == null || pObject == null)
                {
                    Debug.LogError("Error");
                    yield break;
                }

                if (mapShowObject.ContainsKey(pCanvas))
                    yield break;


                GameObject pObjectCopy = OnRequireObject(pObject, pCanvas);
                pObjectCopy.SetActive(true);

                mapShowObject.Add(pCanvas, pObjectCopy);

                if (bIsDebug)
                    Debug.Log(nameof(Show_Object) + " Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " Finish");
            }

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (mapShowObject.ContainsKey(pCanvas) == false)
                    yield break;

                GameObject pObject = mapShowObject[pCanvas];
                mapShowObject.Remove(pCanvas);

                OnHideObject(pObject);
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (mapShowObject.ContainsKey(pCanvas) == false)
                    return;

                GameObject pObject = mapShowObject[pCanvas];
                mapShowObject.Remove(pCanvas);

                OnHideObject(pObject);
            }
        }

        public class Show_UIWidget_Managed<T> : ICanvasManager_Logic_IsPossible_Undo
            where T : class, IUIWidget_Managed
        {
            public enum EOption
            {
                WaitAnimation,
                NotWaitAnimation,
            }

            Dictionary<ICanvas, T> mapShowObject = new Dictionary<ICanvas, T>();

            T pObject;
            System.Func<T, ICanvas, T> OnRequireObject;
            System.Action<T> OnHideObject;
            EOption _eOption;
            bool _bWaitAnimation = true;

            public Show_UIWidget_Managed(T pObject, System.Func<T, ICanvas, T> OnRequireObject, System.Action<T> OnHideObject, EOption eOption = EOption.WaitAnimation)
            {
                this.pObject = pObject; this.OnRequireObject = OnRequireObject; this.OnHideObject = OnHideObject; this._eOption = eOption;
            }

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (OnRequireObject == null || pObject == null)
                {
                    Debug.LogError("Error");
                    yield break;
                }

                if (mapShowObject.ContainsKey(pCanvas))
                    yield break;


                T pObjectCopy = OnRequireObject(pObject, pCanvas);
                if (pObjectCopy.IsNull())
                    yield break;

                mapShowObject.Add(pCanvas, pObjectCopy);

                _bWaitAnimation = _eOption == EOption.WaitAnimation;
                var pHandle = pObjectCopy.DoShow();
                if (pHandle == null)
                    yield break;
                pHandle.Set_OnShow_AfterAnimation(OnFinishAnimation);

                if (bIsDebug)
                    Debug.Log(nameof(Show_Object) + " Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " Start");

                while (_bWaitAnimation)
                {
                    yield return null;
                }

                if (bIsDebug)
                    Debug.Log(nameof(Show_Object) + " Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " Finish");
            }

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (mapShowObject.ContainsKey(pCanvas) == false)
                    yield break;

                T pObject = mapShowObject[pCanvas];
                mapShowObject.Remove(pCanvas);
                if (pObject.IsNull())
                    yield break;

                var pHandle = pObject.DoHide();
                if (pHandle == null)
                {
                    Debug.LogWarning("Execute_UndoLogicCoroutine - pHandle == null");
                    yield break;
                }
                pHandle.Set_OnHide(OnFinishAnimation);

                if (bIsDebug)
                    Debug.Log(nameof(Show_Object) + " Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " Start");

                _bWaitAnimation = _eOption == EOption.WaitAnimation;
                while (_bWaitAnimation)
                {
                    yield return null;
                }

                if (bIsDebug)
                    Debug.Log(nameof(Show_Object) + " Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " Finish");

                OnHideObject(pObject);
            }


            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (mapShowObject.ContainsKey(pCanvas) == false)
                    return;

                T pObject = mapShowObject[pCanvas];
                mapShowObject.Remove(pCanvas);
                if (pObject.IsNull())
                    return;

                var pHandle = pObject.DoHide();
                if (pHandle == null)
                {
                    Debug.LogWarning("Execute_UndoLogicCoroutine - pHandle == null");
                    return;
                }
                pHandle.Set_OnHide(OnFinishAnimation);

                if (bIsDebug)
                    Debug.Log(nameof(Show_Object) + " Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " Start");

                if (bIsDebug)
                    Debug.Log(nameof(Show_Object) + " Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " Finish");

                OnHideObject(pObject);
            }

            public T GetLinkedInstance(ICanvas pCanvas)
            {
                T pObject = null;
                mapShowObject.TryGetValue(pCanvas, out pObject);

                return pObject;
            }

            private void OnFinishAnimation(T pObject)
            {
                _bWaitAnimation = false;
            }
        }

        public class Lock_AllInput : ICanvasManager_Logic_IsPossible_Undo
        {
            public struct SelectAble_OriginState
            {
                public Selectable pSelectAble;
                public bool bEnableOrigin;

                public SelectAble_OriginState(Selectable pSelectAble)
                {
                    this.pSelectAble = pSelectAble; this.bEnableOrigin = pSelectAble.enabled;
                }
            }

            Dictionary<ICanvas, List<SelectAble_OriginState>> _mapSelectAble = new Dictionary<ICanvas, List<SelectAble_OriginState>>();

            System.Func<ICanvas, bool> _OnCheck_IsExecute = (p) => p.IsNull() == false;

            public Lock_AllInput DoSet_Check_IsLockCanvas(System.Func<ICanvas, bool> OnCheck_IsExecute)
            {
                this._OnCheck_IsExecute = OnCheck_IsExecute;
                return this;
            }

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (bIsDebug)
                    Debug.Log(nameof(Lock_AllInput) + " Start Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " 1");

                if (_OnCheck_IsExecute(pCanvas) == false)
                    yield break;

                if (pCanvas.IsNull())
                    yield break;

                if (_mapSelectAble.ContainsKey(pCanvas) == false)
                {
                    List<SelectAble_OriginState> listSelectAbleNew = new List<SelectAble_OriginState>();
                    _mapSelectAble.Add(pCanvas, listSelectAbleNew);

                    Selectable[] arrSelectAble = pCanvas.gameObject.GetComponentsInChildren<Selectable>(true);
                    for (int i = 0; i < arrSelectAble.Length; i++)
                        listSelectAbleNew.Add(new SelectAble_OriginState(arrSelectAble[i]));
                }


                List<SelectAble_OriginState> listSelectAble = _mapSelectAble[pCanvas];
                if (bIsDebug)
                {
                    for (int i = 0; i < listSelectAble.Count; i++)
                    {
                        if (listSelectAble[i].pSelectAble != null)
                        {
                            listSelectAble[i].pSelectAble.enabled = false;
                            Debug.Log(nameof(Lock_AllInput) + " Canvas : " + pCanvas.gameObject.name + " - " + listSelectAble[i].pSelectAble.name + " Enable False", listSelectAble[i].pSelectAble);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < listSelectAble.Count; i++)
                    {
                        if (listSelectAble[i].pSelectAble != null)
                            listSelectAble[i].pSelectAble.enabled = false;
                    }
                }

                if (bIsDebug)
                    Debug.Log(nameof(Lock_AllInput) + " Finish Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_LogicCoroutine) + " 2");

                yield break;
            }

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (bIsDebug)
                    Debug.Log(nameof(Lock_AllInput) + " Start Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_UndoLogic_Coroutine) + " 1");

                if (_OnCheck_IsExecute(pCanvas) == false)
                    yield break;

                if (pCanvas.IsNull())
                    yield break;

                if (_mapSelectAble.ContainsKey(pCanvas) == false)
                    yield break;

                List<SelectAble_OriginState> listSelectAble = _mapSelectAble[pCanvas];
                _mapSelectAble.Remove(pCanvas);
                if (listSelectAble == null)
                    yield break;

                if (bIsDebug)
                {
                    for (int i = 0; i < listSelectAble.Count; i++)
                    {
                        if (listSelectAble[i].pSelectAble != null)
                        {
                            Debug.Log(nameof(Lock_AllInput) + " Canvas : " + pCanvas.gameObject.name + " - " + listSelectAble[i].pSelectAble.name + " Enable : " + listSelectAble[i].bEnableOrigin, listSelectAble[i].pSelectAble);
                            listSelectAble[i].pSelectAble.enabled = listSelectAble[i].bEnableOrigin;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < listSelectAble.Count; i++)
                    {
                        if (listSelectAble[i].pSelectAble != null)
                            listSelectAble[i].pSelectAble.enabled = listSelectAble[i].bEnableOrigin;
                    }
                }

                if (bIsDebug)
                    Debug.Log(nameof(Lock_AllInput) + " Finish Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_UndoLogic_Coroutine) + " 2");

                yield break;
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, bool bIsDebug)
            {
                if (bIsDebug)
                    Debug.Log(nameof(Lock_AllInput) + " " + nameof(Execute_UndoLogic_Coroutine) + " 1");

                if (_OnCheck_IsExecute(pCanvas) == false)
                    return;

                if (pCanvas.IsNull())
                    return;

                if (_mapSelectAble.ContainsKey(pCanvas) == false)
                    return;

                List<SelectAble_OriginState> listSelectAble = _mapSelectAble[pCanvas];
                _mapSelectAble.Remove(pCanvas);
                if (listSelectAble == null)
                    return;

                if (bIsDebug)
                {
                    for (int i = 0; i < listSelectAble.Count; i++)
                    {
                        if (listSelectAble[i].pSelectAble != null)
                        {
                            Debug.Log(nameof(Lock_AllInput) + " Canvas : " + pCanvas.gameObject.name + " - " + listSelectAble[i].pSelectAble.name + " Enable : " + listSelectAble[i].bEnableOrigin, listSelectAble[i].pSelectAble);
                            listSelectAble[i].pSelectAble.enabled = listSelectAble[i].bEnableOrigin;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < listSelectAble.Count; i++)
                    {
                        if (listSelectAble[i].pSelectAble != null)
                            listSelectAble[i].pSelectAble.enabled = listSelectAble[i].bEnableOrigin;
                    }
                }

                if (bIsDebug)
                    Debug.Log(nameof(Lock_AllInput) + " " + nameof(Execute_UndoLogic_Coroutine) + " 2");
            }
        }



    }
}