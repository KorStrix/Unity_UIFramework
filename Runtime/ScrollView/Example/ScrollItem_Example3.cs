using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollItem_Example3 : MonoBehaviour, IScrollItem
{
    public Text text;
    public void IScrollItem_Update(int idx)
    {
        string name = "Cell " + idx.ToString();
        if (text != null)
        {
            text.text = name;
        }
    }
}
