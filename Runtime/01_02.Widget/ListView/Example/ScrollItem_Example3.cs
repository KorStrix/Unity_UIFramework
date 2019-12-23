using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollItem_Example3 : MonoBehaviour, ICollectionItem
{
    public Text text;
    public void ICollectionItem_Update(int idx)
    {
        string name = "Cell " + idx.ToString();
        if (text != null)
        {
            text.text = name;
        }
    }
}
