#region Header
/*	============================================
 *	Aurthor 			    : Strix
 *	Initial Creation Date 	: 2020-02-13
 *	Summary 		        : 
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class TextWrapper_Text : UIWidgetObjectBase, ITextWrapperComponent
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public string strText
        {
            get  => _pText.text;
            set => _pText.text = value; 
        }

        public Graphic[] arrGraphic => _arrGraphic;

        /* protected & private - Field declaration  */

        Graphic[] _arrGraphic;
        Text _pText;

        // ========================================================================== //

        /* public - [Do~Somthing] Function 	        */


        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _pText = GetComponentInChildren<Text>();
            _arrGraphic = new Graphic[1] { _pText };
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}