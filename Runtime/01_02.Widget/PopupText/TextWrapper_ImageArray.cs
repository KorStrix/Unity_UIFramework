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
    /// char를 Image로 변환, 사용하려면 <see cref="DoInit_Converter"/> 필요합니다.
    /// </summary>
    public class TextWrapper_ImageArray : UIWidgetObjectBase, ITextWrapperComponent
    {
        /* const & readonly declaration             */

        /* enum & struct declaration                */

        /* public - Field declaration               */

        public string strText
        {
            get => _strText_LastValue;
            set
            {
                if(_OnConvert_Char_To_Sprite == null)
                {
                    Debug.LogError($"{name} - Require Init", this);
                    return;
                }

                for (int i = 0; i < _arrImage.Length; i++)
                    _arrImage[i].gameObject.SetActive(false);

                if (_arrImage.Length < value.Length)
                {
                    Debug.LogError($"{name} - _arrImage.Length({_arrImage.Length}) < value.Length({value.Length})", this);
                    return;
                }

                for (int i = 0; i < value.Length; i++)
                {
                    Image pImage = _arrImage[i];
                    pImage.gameObject.SetActive(true);
                    pImage.sprite = _OnConvert_Char_To_Sprite(value[i]);
                }

                _strText_LastValue = value;
            }
        }

        public IEnumerable<Graphic> arrGraphic => _arrImage;

        public Image[] _arrImage;

        /* protected & private - Field declaration  */

        string _strText_LastValue;
        System.Func<char, Sprite> _OnConvert_Char_To_Sprite;

        // ========================================================================== //

        /* public - [Do~Something] Function 	        */

        public void DoInit_Converter(System.Func<char, Sprite> OnConvert_Char_To_Sprite)
        {
            _OnConvert_Char_To_Sprite = OnConvert_Char_To_Sprite;
        }

        // ========================================================================== //

        /* protected - [Override & Unity API]       */

        protected override void OnAwake()
        {
            base.OnAwake();

            _arrImage = GetComponentsInChildren<Image>();
            _strText_LastValue = "";
        }

        /* protected - [abstract & virtual]         */


        // ========================================================================== //

        #region Private

        #endregion Private
    }
}