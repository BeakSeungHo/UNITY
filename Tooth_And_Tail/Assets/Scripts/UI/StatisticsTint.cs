using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatisticsTint : MonoBehaviour
{
    public Color TintColor = Color.white;
    public Image TintImage;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        switch (SceneStarter.Instance.statisticElements.winCamp)
        {
            case Camp.Bellafide:
                TintColor = Global.SummaryBgColorBellafide;
                break;
            case Camp.Hopper:
                TintColor = Global.SummaryBgColorHopper;
                break;
            case Camp.Quartermaster:
                TintColor = Global.SummaryBgColorQuartermaster;
                break;
            case Camp.Archimedes:
                TintColor = Global.SummaryBgColorArchimedes;
                break;
        }

        Material mat = Instantiate(TintImage.material);

        mat.SetColor("_Color", TintColor);

        TintImage.material = mat;
    }
}
