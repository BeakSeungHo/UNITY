using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonTintColor : MonoBehaviour
{
    public Image buttonImage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null)
        {
            switch (GameManager.Instance.CommanderList[0])
            {
                case Camp.Hopper:
                    buttonImage.color = Global.CommanderButtonColorHopper;
                    break;
                case Camp.Bellafide:
                    buttonImage.color = Global.CommanderButtonColorBellafide;
                    break;
                case Camp.Quartermaster:
                    buttonImage.color = Global.CommanderButtonColorQuartermaster;
                    break;
                case Camp.Archimedes:
                    buttonImage.color = Global.CommanderButtonColorArchimedes;
                    break;
            }
        }
    }
}
