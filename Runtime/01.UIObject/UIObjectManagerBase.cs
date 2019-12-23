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

/// <summary>
/// 
/// </summary>
abstract public class UIObjectManagerBase<CLASS_DRIVEN_MANAGER, UIOBJECT> : MonoBehaviour, IUIManager
    where CLASS_DRIVEN_MANAGER : UIObjectManagerBase<CLASS_DRIVEN_MANAGER, UIOBJECT>
    where UIOBJECT : IUIObject
{
    /* const & readonly declaration             */

    /* enum & struct declaration                */

    /* public - Field declaration            */

    public delegate void delOnException(System.Exception pException, Object pObject);
    static public event delOnException OnException;

    public static CLASS_DRIVEN_MANAGER instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CLASS_DRIVEN_MANAGER>();
                if (_instance == null)
                {
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

    static protected CLASS_DRIVEN_MANAGER _instance { get; private set; }

    // ========================================================================== //

    /* public - [Do] Function
     * 외부 객체가 호출(For External class call)*/

    static public void DoDestroy_Manager()
    {
        if(_instance.IsNull() == false)
        {
            _instance.OnDestroy_ManagerInstance();
            Destroy(_instance.gameObject);
        }

        _instance = null;
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


    /* protected - [abstract & virtual]         */

    virtual protected void OnAwake() { }

    /// <summary>
    /// 싱글톤으로 instance가 생성될 때 호출됩니다.
    /// </summary>
    virtual protected void OnCreate_ManagerInstance() { }
    virtual protected void OnDestroy_ManagerInstance() { }

    abstract public UICommandHandle<CLASS_UIOBJECT> IUIManager_Show<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject)
        where CLASS_UIOBJECT : IUIObject;

    abstract public UICommandHandle<CLASS_UIOBJECT> IUIManager_Hide<CLASS_UIOBJECT>(CLASS_UIOBJECT pUIObject, bool bPlayAnimation)
        where CLASS_UIOBJECT : IUIObject;

    // ========================================================================== //

    #region Private

    #endregion Private
}