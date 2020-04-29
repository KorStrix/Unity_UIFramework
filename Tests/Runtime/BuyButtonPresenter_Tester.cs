#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-04-29
 *	Summary 		        : 
 *	테스트 지침 링크
 *	https://github.com/KorStrix/Unity_DevelopmentDocs/Test
 *
 *  Template 		        : Test For Unity Editor V2
   ============================================ */
#endregion Header

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UIFramework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{

    public class BuyButtonPresenter_Tester
    {
	    public enum EAssetType
	    {
		    Gold,
		    Gem,
	    }

	    public class AssetOwner_ForTest : IAssetOwner<EAssetType>
	    {
		    public int iHasGold;
		    public int iHasGem;

		    public AssetOwner_ForTest(int iGold, int iGem)
		    {
			    iHasGold = iGold;
			    iHasGem = iGem;
		    }
		    
		    public AssetOwner_ForTest DoAddAsset(EAssetType eAssetType, int iAmount)
		    {
			    switch (eAssetType)
			    {
				    case EAssetType.Gold: iHasGold += iAmount; break;
				    case EAssetType.Gem:  iHasGem += iAmount; break;
			    }

			    return this;
		    }
		    
		    public bool IAssetOwner_Check_IsEnoughAsset(EAssetType eAssetType, int iAmount)
		    {
			    switch (eAssetType)
			    {
				    case EAssetType.Gold: return iHasGold >= iAmount;
				    case EAssetType.Gem:  return iHasGem >= iAmount;
			    }

			    return false;
		    }

		    public void IAssetOwner_DecreaseAsset(EAssetType eAssetType, int iAmount)
		    {
			    switch (eAssetType)
			    {
				    case EAssetType.Gold: iHasGold -= iAmount; break;
				    case EAssetType.Gem:  iHasGem -= iAmount; break;
			    }
		    }
	    }
	    
        [Test]
        public void 이클래스는_이것을실행하면_이것이여야만합니다()
        {
			// Arrange (데이터 정렬)
			AssetOwner_ForTest pOwner = new AssetOwner_ForTest(100, 200);
			
			BuyButtonPresenter<EAssetType>.SetCallBack_AssetOwner(pOwner);
			BuyButtonPresenter<EAssetType> pButton = new BuyButtonPresenter<EAssetType>();

			bool bIsSuccess = false;
			pButton.OnSuccess += () => bIsSuccess = true;
			pButton.OnNotEnoughAsset += () => bIsSuccess = false;
			
			pButton.DoInit(null, EAssetType.Gold, 50);

			
			
			// Act (기능 실행) & Assert (맞는지 체크)
			pButton.Event_OnClick();
			Assert.IsTrue(bIsSuccess);

			pButton.Event_OnClick();
			Assert.IsTrue(bIsSuccess);

			pButton.Event_OnClick();
			Assert.IsFalse(bIsSuccess);
        }
    }
}
