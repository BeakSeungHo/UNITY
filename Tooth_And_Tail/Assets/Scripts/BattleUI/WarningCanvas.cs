using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningCanvas : MonoBehaviour
{
    public Image WarningImg = null;
    
    float ChangeAlphaValue = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.transform.localPosition = new Vector3(0, 0.25f, 0);
        gameObject.transform.localScale = new Vector2((10f / Screen.width), (10f / Screen.width));
    }

    // Update is called once per frame
    void Update()
    {
        Blink();
    }

    public void Blink()
    {
        if(WarningImg.color.a >= 1 || WarningImg.color.a <= 0)
            ChangeAlphaValue *= -1;

        Color color = WarningImg.color;
        color.a += ChangeAlphaValue;
        WarningImg.color = color;
    }

    public void ChangeAlpha(float Alpha)
    {
        Color color = WarningImg.color;
        color.a = Alpha;
        WarningImg.color = color;
    }
}
