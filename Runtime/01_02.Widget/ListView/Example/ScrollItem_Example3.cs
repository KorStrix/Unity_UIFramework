using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScrollItem_Example3 : MonoBehaviour, ICollectionItem , IPointerClickHandler
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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click - " + text.text, this);
    }
}
