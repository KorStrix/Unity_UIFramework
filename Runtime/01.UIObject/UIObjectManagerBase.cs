#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-21 오후 6:42:42
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class UIObjectManagerBase<CLASS_DRIVEN_MANAGER, UIOBJECT> : MonoBehaviour, IUIManager
        where CLASS_DRIVEN_MANAGER : UIObjectManagerBase<CLASS_DRIVEN_MANAGER, UIOBJECT>
        where UIOBJECT : IUIObject
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        [System.Flags]
        public enum ELogTypeFlag : ulong
        {
            None = 0,

            Debug_1 = 1 << 0,
            Debug_2 = 1 << 1,

            Warning = 1 << 2,
            Error = 1 << 3,

            Test = 1 << 4,
        }

        /* public - Field declaration            */

        public delegate void delOnPrintLog(ELogTypeFlag eLogTypeFlag, string strLog, Object pContextObject);
        public event delOnPrintLog OnPrintLog = DefaultPrintLog;

        public delegate void delOnException(System.Exception pException, Object pObject);
        public static event delOnException OnException;

        public static CLASS_DRIVEN_MANAGER instance
        {
            get
            {
                if (_instance.IsNull())
                {
                    _instance = FindObjectOfType<CLASS_DRIVEN_MANAGER>();
                    if (_instance.IsNull())
                    {
                        if (_bApplication_IsQuit)
                            return null;

                        GameObject pObjectInstance = new GameObject(typeof(CLASS_DRIVEN_MANAGER).Name);
                        _instance = pObjectInstance.AddComponent<CLASS_DRIVEN_MANAGER>();
                    }

                    _instance.OnCreate_ManagerInstance();
                }

                return _instance;
            }
        }
        public bool bIsExecute_Awake { get; private set; } = false;

        /* protected & private - Field declaration         */

        protected static CLASS_DRIVEN_MANAGER _instance { get; private set; }
        protected static bool _bIsDestroying { get; private set; } = false;
        protected static bool _bApplication_IsQuit { get; private set; } = false;

        // ========================================================================== //

        /* public - [Do] Function
         * 외부 객체가 호출(For External class call)*/

        public static void DefaultPrintLog(ELogTypeFlag eLogTypeFlag, string strLog, Object pContextObject)
        {
            if ((eLogTypeFlag & ELogTypeFlag.Error) == ELogTypeFlag.Error)
            {
                Debug.LogError(eLogTypeFlag.ToString() + strLog, pContextObject);
            }
            else if ((eLogTypeFlag & ELogTypeFlag.Warning) == ELogTypeFlag.Warning)
            {
                Debug.LogWarning(eLogTypeFlag.ToString() + strLog, pContextObject);
            }
            else
            {
                Debug.Log(eLogTypeFlag.ToString() + strLog, pContextObject);
            }
        }


        /// <summary>
        /// 싱글톤을 파괴합니다
        /// </summary>
        public static void DoDestroy_Manager(bool bDeleteObject_Immediately = false)
        {
            if (_bIsDestroying)
                return;
            _bIsDestroying = true;

            if (_instance.IsNull() == false)
            {
                _instance.OnDestroy_ManagerInstance();

                if (bDeleteObject_Immediately)
                    DestroyImmediate(_instance.gameObject);
                else
                    Destroy(_instance.gameObject);
            }

            _instance = null;
            _bIsDestroying = false;
        }

        // ========================================================================== //

        /* protected - Override & Unity API         */

        protected void Awake()
        {
            if (bIsExecute_Awake)
                return;
            bIsExecute_Awake = true;

            try
            {
                OnAwake();
            }
            catch (System.Exception e)
            {
                if (OnException != null)
                    OnException.Invoke(e, this);
                else
                    throw;
            }
        }

        private void OnDestroy()
        {
            if (_bApplication_IsQuit)
                return;

            try
            {
                DoDestroy_Manager();
            }
            catch (System.Exception e)
            {
                if (OnException != null)
                    OnException.Invoke(e, this);
                else
                    throw;
            }

            OnException = null;
        }

        private void OnApplicationQuit()
        {
            _bApplication_IsQuit = true;
        }

        /* protected - [abstract & virtual]         */

        protected virtual void OnAwake() { }

        protected virtual void OnCreate_ManagerInstance() { }
        protected virtual void OnDestroy_ManagerInstance() { }

        public abstract UICommandHandle<CLASS_UIOBJECT> IUIManager_Show<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
            where CLASS_UIOBJECT : IUIObject;

        public abstract UICommandHandle<CLASS_UIOBJECT> IUIManager_Hide<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject, bool bPlayAnimation)
            where CLASS_UIOBJECT : IUIObject;
        public abstract EUIObjectState IUIManager_GetUIObjectState<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
            where CLASS_UIOBJECT : IUIObject;

        // ========================================================================== //

        #region Private

        #endregion Private
    }
}