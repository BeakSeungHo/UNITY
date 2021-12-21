using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class OutLine : MonoBehaviour
{
    public Color color = Color.white;

    [Range(0, 16)]
    public int outlineSize = 0;

    private SpriteRenderer spriteRenderer;
    MaterialPropertyBlock MaterialProp;
    private void Start()
    {
        spriteRenderer = transform.parent.gameObject.GetComponent<SpriteRenderer>();
        MaterialProp = new MaterialPropertyBlock();
        UpdateOutline(true);
    }
    void OnEnable()
    {

    }

    void OnDisable()
    {
        UpdateOutline(false);
    }

    void Update()
    {
        UpdateOutline(true);
    }

    void UpdateOutline(bool outline)
    { 
        // spriteRenderer.sprite = parentSpriteRenderer.sprite;
        //MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        if (MaterialProp != null)
        {
            spriteRenderer.GetPropertyBlock(MaterialProp);
            MaterialProp.SetFloat("_Outline", outline ? 1f : 0);
            MaterialProp.SetColor("_OutlineColor", color);
            MaterialProp.SetFloat("_OutlineSize", outlineSize);
            spriteRenderer.SetPropertyBlock(MaterialProp);
        }
    }

}
