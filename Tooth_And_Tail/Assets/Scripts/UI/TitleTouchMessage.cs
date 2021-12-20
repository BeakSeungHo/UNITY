using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TitleTouchMessage : MonoBehaviour
{
    public TextMeshProUGUI      text;
    private float               msgAlpha;
    private float               fadeSpeed;
    private bool                fadeOut;


    // Start is called before the first frame update
    void Start()
    {
        text = this.GetComponent<TextMeshProUGUI>();
        fadeSpeed = 0.6f;
        msgAlpha = 1f;
        fadeOut = true;
    }

    // Update is called once per frame
    void Update()
    {
        text.alpha = msgAlpha;

        if (fadeOut)
        {
            msgAlpha -= Time.deltaTime * fadeSpeed;

            if (msgAlpha < 0.4f)
                fadeOut = false;
        }
        else
        {
            msgAlpha += Time.deltaTime * fadeSpeed;
            if (msgAlpha > 1.1f)
                fadeOut = true;
        }
    }
}
