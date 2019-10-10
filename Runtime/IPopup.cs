#region Header
/*	============================================
 *	작성자 : Strix
 *	작성일 : 2019-10-07 오전 11:07:10
 *	개요 : 
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 팝업 매니져에게 관리받는 팝업 상속용.
/// <para> <seealso cref="CPopupManager{CLASS_DRIVEN, ENUM_POPUP_NAME}"/>에게 관리받습니다. </para>
/// </summary>
public interface IPopup : IUIObject
{
    /// <summary>
    /// 매니져를 통해 팝업을 켤 때 - Active(True) -> <see cref="IPopup_OnShowCoroutine"/>
    /// <para> 팝업이 아닌 매니져에 종속되있기 때문에 Popup의 Active유무와 관계없습니다. </para>
    /// </summary>
    IEnumerator IPopup_OnShowCoroutine();

    /// <summary>
    /// 매니져를 통해 팝업을 끌 때 - Active 유무와 상관없음 -> <see cref="IPopup_OnHideCoroutine"/> -> Active(False)
    /// <para> 팝업이 아닌 매니져에 종속되있기 때문에 Popup의 Active유무와 관계없습니다. </para>
    /// </summary>
    IEnumerator IPopup_OnHideCoroutine();
}
