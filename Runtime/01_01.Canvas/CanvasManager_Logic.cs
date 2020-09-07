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
            IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags);
        }

        /// <summary>
        /// 
        /// </summary>
        public interface ICanvasManager_Logic_IsPossible_Undo : ICanvasManager_Logic
        {
            IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags);
            void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags);
        }

        public class CanvasManager_LogicUndo_Wrapper : ICanvasManager_Logic_IsPossible_Undo
        {
            public ICanvasManager_Logic_IsPossible_Undo pLogic { get; private set; }
            public EUIObjectState eWhenUndo { get; private set; }


            public CanvasManager_LogicUndo_Wrapper(ICanvasManager_Logic_IsPossible_Undo pLogic, EUIObjectState eWhenUndo)
            {
                this.pLogic = pLogic; this.eWhenUndo = eWhenUndo;
            }

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                yield return pLogic.Execute_LogicCoroutine(pManager, pCanvas, eDebugFlags);
            }

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                yield return pLogic.Execute_UndoLogic_Coroutine(pManager, pCanvas, eDebugFlags);
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                pLogic.Execute_UndoLogic_NotCoroutine(pManager, pCanvas, eDebugFlags);
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

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                if(pCanvas.IsNull())
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager_Core)}{nameof(Print_CanvasState)} Object is Null /// CanvasState : {_eState}");
                else
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.Manager_Core)}{nameof(Print_CanvasState)} {pCanvas.gameObject.name} /// CanvasState : {_eState}", pCanvas.gameObject);

                yield break;
            }
        }

        public class SetTransform_LastSibling : ICanvasManager_Logic
        {
            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
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

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
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

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}{nameof(Show_Object)} Canvas {pCanvas.gameObject.name} {nameof(Execute_LogicCoroutine)} Finish");
            }

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                if (mapShowObject.ContainsKey(pCanvas) == false)
                    yield break;

                GameObject pObject = mapShowObject[pCanvas];
                mapShowObject.Remove(pCanvas);

                OnHideObject(pObject);
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
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
                /// <summary>
                /// 애니메이션을 전부 기다리고 다음 코드를 실행할지
                /// </summary>
                WaitAnimation,

                /// <summary>
                /// 애니메이션을 안기다리고 바로 다음 코드를 실행할지
                /// </summary>
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

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
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

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + nameof(Show_UIWidget_Managed<T>) + " Canvas : " + pCanvas.GetObjectName_Safe() + " " + nameof(Execute_LogicCoroutine) + " Start");

                mapShowObject.Add(pCanvas, pObjectCopy);

                _bWaitAnimation = _eOption == EOption.WaitAnimation;
                var pHandle = pObjectCopy.DoShow();
                if (pHandle == null)
                    yield break;
                pHandle.Set_OnShow_AfterAnimation((pPopup) => _bWaitAnimation = false);

                while (_bWaitAnimation)
                {
                    yield return null;
                }

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + nameof(Show_UIWidget_Managed<T>) + " Canvas : " + pCanvas.GetObjectName_Safe() + " " + nameof(Execute_LogicCoroutine) + " Finish");
            }

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                if (mapShowObject.ContainsKey(pCanvas) == false)
                    yield break;

                T pObject = mapShowObject[pCanvas];
                mapShowObject.Remove(pCanvas);
                if (pObject.IsNull())
                    yield break;

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}{nameof(Show_UIWidget_Managed<T>)}/{nameof(Execute_UndoLogic_Coroutine)} // Canvas : {pCanvas.GetObjectName_Safe()} Start");

                var pHandle = pObject.DoHide();
                if (pHandle == null)
                {
                    Debug.LogWarning($"{nameof(Execute_UndoLogic_Coroutine)} // Canvas : {pCanvas.GetObjectName_Safe()} - pHandle == null");
                    yield break;
                }
                pHandle.Set_OnHide((pPopup) => _bWaitAnimation = false);

                _bWaitAnimation = _eOption == EOption.WaitAnimation;
                while (_bWaitAnimation)
                {
                    yield return null;
                }

                OnHideObject(pObject);

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}{nameof(Show_UIWidget_Managed<T>)}/{nameof(Execute_UndoLogic_Coroutine)} // Canvas : {pCanvas.GetObjectName_Safe()} Finish");
            }


            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                if (mapShowObject.ContainsKey(pCanvas) == false)
                    return;

                T pObject = mapShowObject[pCanvas];
                mapShowObject.Remove(pCanvas);
                if (pObject.IsNull())
                    return;

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + nameof(Show_UIWidget_Managed<T>) + " Canvas : " + pCanvas.GetObjectName_Safe() + " " + nameof(Execute_UndoLogic_NotCoroutine) + " Start");

                var pHandle = pObject.DoHide();
                if (pHandle == null)
                {
                    Debug.LogWarning($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + "Execute_UndoLogicCoroutine - pHandle == null");
                    return;
                }
                pHandle.Set_OnHide((pPopup) => _bWaitAnimation = false);
                OnHideObject(pObject);

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + nameof(Show_UIWidget_Managed<T>) + " Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_UndoLogic_NotCoroutine) + " Finish");
            }

            public T GetLinkedInstance(ICanvas pCanvas)
            {
                mapShowObject.TryGetValue(pCanvas, out var pObject);

                return pObject;
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

            public IEnumerator Execute_LogicCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}{nameof(Lock_AllInput)} Prepare Canvas : {pCanvas.GetObjectName_Safe()} + {nameof(Execute_LogicCoroutine)}", pCanvas.gameObject);

                if (_OnCheck_IsExecute(pCanvas) == false)
                    yield break;

                if (pCanvas.IsNull())
                    yield break;


                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}{nameof(Lock_AllInput)} Start Canvas : {pCanvas.GetObjectName_Safe()} {nameof(Execute_LogicCoroutine)}");

                if (_mapSelectAble.ContainsKey(pCanvas) == false)
                {
                    List<SelectAble_OriginState> listSelectAbleNew = new List<SelectAble_OriginState>();
                    _mapSelectAble.Add(pCanvas, listSelectAbleNew);

                    Selectable[] arrSelectAble = pCanvas.gameObject.GetComponentsInChildren<Selectable>(true);
                    for (int i = 0; i < arrSelectAble.Length; i++)
                        listSelectAbleNew.Add(new SelectAble_OriginState(arrSelectAble[i]));
                }


                List<SelectAble_OriginState> listSelectAble = _mapSelectAble[pCanvas];
                if ((eDebugFlags & EDebugLevelFlags.Detail) != 0)
                {
                    for (int i = 0; i < listSelectAble.Count; i++)
                    {
                        if (listSelectAble[i].pSelectAble != null)
                        {
                            listSelectAble[i].pSelectAble.enabled = false;
                            Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + nameof(Lock_AllInput) + " Canvas : " + pCanvas.gameObject.name + " - " + listSelectAble[i].pSelectAble.name + " Enable False", listSelectAble[i].pSelectAble);
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

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}{nameof(Lock_AllInput)} Finish Canvas : {pCanvas.GetObjectName_Safe()} + {nameof(Execute_LogicCoroutine)}");

                yield break;
            }

            public IEnumerator Execute_UndoLogic_Coroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)} {nameof(Lock_AllInput)} {nameof(Execute_UndoLogic_Coroutine)} Prepare Canvas : {pCanvas.GetObjectName_Safe()}", pCanvas?.gameObject);

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

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + nameof(Lock_AllInput) + " Start Canvas : " + pCanvas.GetObjectName_Safe() + " " + nameof(Execute_UndoLogic_Coroutine) + " 1");

                if ((eDebugFlags & EDebugLevelFlags.Detail) != 0)
                {
                    for (int i = 0; i < listSelectAble.Count; i++)
                    {
                        if (listSelectAble[i].pSelectAble != null)
                        {
                            Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + nameof(Lock_AllInput) + " Canvas : " + pCanvas.gameObject.name + " - " + listSelectAble[i].pSelectAble.name + " Enable : " + listSelectAble[i].bEnableOrigin, listSelectAble[i].pSelectAble);
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

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log($"{UIDebug.LogFormat(EDebugLevelFlags.ManagerLogic)}" + nameof(Lock_AllInput) + " Finish Canvas : " + pCanvas.gameObject.name + " " + nameof(Execute_UndoLogic_Coroutine) + " 2");

                yield break;
            }

            public void Execute_UndoLogic_NotCoroutine(MonoBehaviour pManager, ICanvas pCanvas, EDebugLevelFlags eDebugFlags)
            {
                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
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

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
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

                if ((eDebugFlags & EDebugLevelFlags.ManagerLogic) != 0)
                    Debug.Log(nameof(Lock_AllInput) + " " + nameof(Execute_UndoLogic_Coroutine) + " 2");
            }
        }



    }
}
