using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GraphCategory : MonoBehaviour
{
    public StatisticsWindow     MaterStat;

    public TextMeshProUGUI      playerName;
    public TextMeshProUGUI      cpuName;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpCategory()
    {
        playerName.text = SceneStarter.Instance.userElements.UserData.UserName;

        switch (SceneStarter.Instance.statisticElements.campPlayer)
        {
            case Camp.Bellafide:
                playerName.color = Global.GraphColorBellafide;
                break;
            case Camp.Hopper:
                playerName.color = Global.GraphColorHopper;
                break;
            case Camp.Quartermaster:
                playerName.color = Global.GraphColorQuartermaster;
                break;
            case Camp.Archimedes:
                playerName.color = Global.GraphColorArchimedes;
                break;
        }
        switch (SceneStarter.Instance.statisticElements.campAI)
        {
            case Camp.Bellafide:
                cpuName.color = Global.GraphColorBellafide;
                break;
            case Camp.Hopper:
                cpuName.color = Global.GraphColorHopper;
                break;
            case Camp.Quartermaster:
                cpuName.color = Global.GraphColorQuartermaster;
                break;
            case Camp.Archimedes:
                cpuName.color = Global.GraphColorArchimedes;
                break;
        }
    }
}
