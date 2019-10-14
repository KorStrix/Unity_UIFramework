using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollItem_Example2 : MonoBehaviour, IScrollItem
{
    public Text text;
    public LayoutElement element;
    private static float[] randomWidths = new float[3] { 100, 150, 50 };
    public void IScrollItem_Update(int idx)
    {
        string name = "Cell " + idx.ToString();
        if (text != null)
        {
            text.text = name;
        }
        element.preferredWidth = randomWidths[Mathf.Abs(idx) % 3];
        gameObject.name = name;
    }
}
