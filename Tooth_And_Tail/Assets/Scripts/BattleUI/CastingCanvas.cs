using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CastingCanvas : MonoBehaviour
{
    public TextMeshProUGUI CastingText = null;
    public Image           CastingImage = null;

    void Update()
    {
        
    }

    public void ChangeText(string NewText)
    {
        CastingText.text = NewText;
    }
    


}
