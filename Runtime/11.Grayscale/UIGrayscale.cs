#region Header
/*	============================================
 *	Author 			    	: Strix
 *	Initial Creation Date 	: 2020-03-30
 *	Summary 		        : 
 *	
 *	
 *	√‚√≥ : https://forum.unity.com/threads/sprite-shader-with-greyscale.222693/
 *  Template 		        : For Unity Editor V1
   ============================================ */
#endregion Header

using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
[RequireComponent(typeof(Graphic))]
public class UIGrayscale : MonoBehaviour
{
    [Range(0, 1)]
    public float effect = 1;

    Material _pGrayMaterial;
    MaskableGraphic _pGraphic;

    void Start()
    {
        _pGraphic = GetComponent<MaskableGraphic>();
        _pGrayMaterial = _pGraphic.material;
    }

    void Update()
    {
        Image img = GetComponent<Image>(); // FIXED
        _pGrayMaterial = img.materialForRendering; // FIXED
        if (_pGrayMaterial != null)
            _pGrayMaterial.SetFloat("_EffectAmount", effect); //_EffectAmount  // _GrayscaleAmount
    }
}
