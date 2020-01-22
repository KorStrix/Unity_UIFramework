using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class ScrollItem_Example2 : MonoBehaviour, ICollectionItem, IPointerClickHandler
{
    public Text text;
    public LayoutElement element;
    private static float[] randomWidths = new float[3] { 100, 150, 50 };
    public void ICollectionItem_Update(int idx)
    {
        string name = "Cell " + idx.ToString();
        if (text != null)
        {
            text.text = name;
        }
        element.preferredWidth = randomWidths[Mathf.Abs(idx) % 3];
        gameObject.name = name;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Click - " + text.text, this);
    }

}
