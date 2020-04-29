#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-04-29
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V2
   ============================================ */
#endregion Header

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;


namespace UIFramework
{
	public interface IAssetOwner<ASSET_TYPE>
	{
		bool IAssetOwner_Check_IsEnoughAsset(ASSET_TYPE eAssetType, int iAmount);
		void IAssetOwner_DecreaseAsset(ASSET_TYPE eAssetType, int iAmount);
	}
	
	/// <summary>
	/// 
	/// </summary>
	[System.Serializable]
	public class BuyButtonPresenter<ASSET_TYPE>
		where ASSET_TYPE : struct
	{
		/* const & readonly declaration             */

		/* enum & struct declaration                */

		[System.Serializable]
		public struct AssetPrice
		{
			public ASSET_TYPE eAssetType;
			public int iAmount;

			public AssetPrice(ASSET_TYPE eAssetType, int iAmount)
			{
				this.eAssetType = eAssetType;
				this.iAmount = iAmount;
			}
		}

		/* public - Field declaration               */

		public static event Action<AssetPrice> OnNotEnoughAsset_Global;
		
		public event Action OnSuccess;
		public event Action OnNotEnoughAsset;
		
		[SerializeField]
		AssetPrice sAssetPrice;

		/* protected & private - Field declaration  */
		
		private static IAssetOwner<ASSET_TYPE> g_pAssetOwner;

		private System.Action _OnClickEvent;

		private MonoBehaviour _pBehaviourOwner;
		
		// ========================================================================== //

		/* public - [Do~Something] Function 	        */

		public static void SetCallBack_AssetOwner(IAssetOwner<ASSET_TYPE> pAssetOwner)
		{
			g_pAssetOwner = pAssetOwner;
		}

		public static void DoClear_GlobalCallback()
		{
			OnNotEnoughAsset_Global = null;
		}
		
		
		public void DoUpdateUI()
		{
			
		}
		
		public void DoInit(MonoBehaviour pBehaviourOwner, ASSET_TYPE eAssetType, int iAmount)
		{
			_pBehaviourOwner = pBehaviourOwner;
			sAssetPrice = new AssetPrice(eAssetType, iAmount);
		}

		public void Event_OnClick()
		{
			if (g_pAssetOwner.IAssetOwner_Check_IsEnoughAsset(sAssetPrice.eAssetType, sAssetPrice.iAmount))
			{
				g_pAssetOwner.IAssetOwner_DecreaseAsset(sAssetPrice.eAssetType, sAssetPrice.iAmount);
				OnSuccess?.Invoke();
			}
			else
			{
				OnNotEnoughAsset?.Invoke();
			}
		}

		public void DoClearEvent()
		{
			OnSuccess = null;
			OnNotEnoughAsset = null;
		}
		
		// public void DoInit(EAssetType eAssetType, int iCost, System.Action OnClickEvent, bool bEnableIcon = true)
		// {
		// 	_OnClickEvent = OnClickEvent;
		// 	DoUpdateUI( eAssetType, iCost );
		// }
		//
		// public void DoUpdateUI( EAssetType eAssetType, int iCost, bool bEnableIcon = true )
		// {
		// 	_pImageIcon.enabled = bEnableIcon;
		// 	if( bEnableIcon ) 
		// 	{
		// 		switch( eAssetType )
		// 		{
		// 		case EAssetType.Gem:
		// 			_pImageIcon.sprite = DataManager.GetSprite_InAtlas( "Common", "icon_diamond" );
		// 			break;
		//
		// 		case EAssetType.Gold:
		// 			_pImageIcon.sprite = DataManager.GetSprite_InAtlas( "Common", "icon_gold" );
		// 			break;
		// 		}
		// 		_pText.transform.position = _pTransform_Pos_Right.position;
		// 		_pText.alignment = TextAnchor.MiddleLeft;
		// 	}
		// 	else
		// 	{
		// 		_pText.transform.position = _pTransform_Pos_Center.position;
		// 		_pText.alignment = TextAnchor.MiddleCenter;
		// 	}
		//
		// 	if( 0 == iCost )
		// 	{
		// 		_pText.text = DataManager.GetLocalText( ELanguageKey.Free ); //"����" �ؽ�Ʈ ���
		// 	}
		// 	else
		// 	{
		// 		_pText.text = iCost.ToString();
		// 	}
		// }

		// ========================================================================== //

		/* protected - [Override & Unity API]       */


		/* protected - [abstract & virtual]         */


		// ========================================================================== //

		#region Private

		#endregion Private

	}
}