using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Checker : MonoBehaviour
{
    [Range(1, 100)]
    public int FontSize;
    [Range(0, 1)]
    public float Red, Green, Blue;

    float fps = 0;

    float deltaTime = 0f;

    void Start()
    {
        FontSize = FontSize == 0 ? 50 : FontSize;
    }

    // Update is called once per frame
    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    void OnGUI()
    {
        int w = Screen.width, h = Screen.height;

        GUIStyle style = new GUIStyle();

        Rect rect = new Rect(0, 0, w, h * 2 / 100);
        style.alignment = TextAnchor.UpperLeft;
        style.fontSize = h * 2 / FontSize;
        style.normal.textColor = new Color(Red, Green, Blue, 1.0f);
        float mSec = deltaTime * 1000f;
        fps = 1f / deltaTime;
        string text = string.Format("{0:0.0} ms ({1:0.} fps)", mSec, fps);
        if (TilemapSystem.Instance != null &&  TilemapSystem.Instance.IsLoadedVectorField())
        {
            text += "!!!!";
        }
        else
            text += "...";
        GUI.Label(rect, text, style);
    }
}
