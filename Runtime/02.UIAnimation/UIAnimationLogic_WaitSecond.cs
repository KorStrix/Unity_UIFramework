#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-17 오후 12:57:13
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using UIFramework;

/// <summary>
/// 
/// </summary>
public class UIAnimationLogic_WaitSecond : UIAnimationLogic_ComponentBase
{
    public float fWaitSecond = 0f;

    public override void IUIAnimationLogic_OnAwake(IUIObject pUIObject)
    {
    }

    public override void IUIAnimationLogic_OnBeforeShow(IUIObject pUIObject)
    {
    }

    public override IEnumerator IUIAnimationLogic_OnAnimation(IUIObject pUIObject, bool bIgnoreTimeScale)
    {
        yield return new WaitForSeconds(fWaitSecond);
    }

}