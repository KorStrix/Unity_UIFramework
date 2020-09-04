using System.IO;
using System.Text;
using System.Collections.Generic;

namespace UIFramework
{
    /// <summary>
    /// cs 파일을 만드는 클래스입니다.
    /// </summary>
    public class CustomCodedom
    {
        public const string const_strListFieldName = "list";

        private static string const_strPrefix = @"
/*	============================================
 *	Author   			    : Strix
 *	Summary 		        : 
 *
 *  툴로 자동으로 생성되는 코드입니다.
 *  이 파일을 직접 수정하시면 나중에 툴로 생성할 때 날아갑니다.
   ============================================= */

using UnityEngine;
using System.Collections.Generic;

public partial class {0} : CanvasManager<{0}, {0}.{1}>
{
    public enum {1}
    {

    }
}";

        enum ETextType
        {
            ManagerName,
            EnumName,
            ManagerLogic,

            MAX,
        }

        private Dictionary<ETextType, StringBuilder> mapBuilder = new Dictionary<ETextType, StringBuilder>();

        public CustomCodedom()
        {
            mapBuilder.Clear();
            for (int i = 0; i < (int)ETextType.MAX; i++)
                mapBuilder.Add((ETextType)i, new StringBuilder());
        }

        /// <summary>
        /// 
        /// </summary>
        public void DoExportCS(string strFilePath_Absolute)
        {
            // string.Format이 안됨;
            //string strFileContent = string.Format(const_strPrefix, 
            //    nameof(CustomLogType),
            //    _strBuilder_Class.ToString());

            string strFileContent = const_strPrefix.
                Replace("{0}", mapBuilder[ETextType.ManagerName].ToString()).
                Replace("{1}", mapBuilder[ETextType.EnumName].ToString());

            File.WriteAllText($"{strFilePath_Absolute}.cs", strFileContent, Encoding.UTF8);
        }
    }
}
