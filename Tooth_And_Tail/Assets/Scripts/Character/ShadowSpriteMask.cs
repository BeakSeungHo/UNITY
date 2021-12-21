using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowSpriteMask : MonoBehaviour
{
    public SpriteMask SpriteMask;
    public SpriteRenderer SpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        SpriteMask.sprite = SpriteRenderer.sprite;
    }
}
