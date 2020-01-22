using UnityEngine;
using System.Collections;

namespace SG
{
    [RequireComponent(typeof(CUGUIScrollView))]
    [DisallowMultipleComponent]
    public class CUGUIScrollView_Example : MonoBehaviour
    {
        public int totalCount = -1;
        void Start()
        {
            var ls = GetComponent<CUGUIScrollView>();
            ls.DoCreate_ScrollItem(totalCount);
        }
    }
}