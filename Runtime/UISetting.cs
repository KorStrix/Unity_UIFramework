#region Header

/*	============================================
 *	Author 			    	: strix
 *	Initial Creation Date 	: 2020-08-10
 *	Summary 		        : 
 *  Template 		        : New Behaviour For ReSharper
   ============================================ */

#endregion Header

using UnityEngine;

namespace UIFramework
{
    /// <summary>
    /// 
    /// </summary>
    public class UISetting : ScriptableObject
    {
        public bool bIsCurrent;

        public string strExportCSPath = "";
        public string strManagerClassName = "UIManager";
        public string strPopupEnumName = "EPopupName";
    }
}
