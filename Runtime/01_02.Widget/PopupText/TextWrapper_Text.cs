#region Header
/*	============================================
 *	Author   			    : Strix
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
            get  => pText.text;
            set => pText.text = value; 
        }

        public IEnumerable<Graphic> arrGraphic => _arrGraphic;

        public Text pText { get; private set; }

        /* protected & private - Field declaration  */

        Graphic[] _arrGraphic;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoSetText(Text pText)
        {
            this.pText = pText;
            _arrGraphic = new Graphic[1] { pText };
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            if(pText == null)
                DoSetText(GetComponentInChildren<Text>());
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}