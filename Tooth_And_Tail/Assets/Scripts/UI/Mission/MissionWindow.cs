using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionWindow : MonoBehaviour
{
    public Toggle           dailyTab;
    public Toggle           weeklyTab;
    public Toggle           achieveTab;

    public MissionScroll    dailyScroll;
    public MissionScroll    weeklyScroll;
    public MissionScroll    achieveScroll;




    // Start is called before the first frame update
    void Start()
    {
        //dailyScroll.MasterMission = this;
        //dailyScroll.missionType = MissionType.Daily;
        //dailyScroll.gameObject.SetActive(true);

        //weeklyScroll.MasterMission = this;
        //weeklyScroll.missionType = MissionType.Weekly;
        //weeklyScroll.gameObject.SetActive(false);

        //achieveScroll.MasterMission = this;
        //achieveScroll.missionType = MissionType.Achievements;
        //achieveScroll.gameObject.SetActive(false);

        //dailyScroll.SetUp();
        //weeklyScroll.SetUp();
        //achieveScroll.SetUp();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        dailyScroll.MasterMission = this;
        dailyScroll.missionType = MissionType.Daily;
        dailyScroll.gameObject.SetActive(true);

        weeklyScroll.MasterMission = this;
        weeklyScroll.missionType = MissionType.Weekly;
        weeklyScroll.gameObject.SetActive(false);

        achieveScroll.MasterMission = this;
        achieveScroll.missionType = MissionType.Achievements;
        achieveScroll.gameObject.SetActive(false);


        if (weeklyTab.isOn || achieveTab.isOn)
        {
            dailyTab.isOn = true;
            weeklyTab.isOn = false;
            achieveTab.isOn = false;
        }

        dailyTab.transform.GetChild(2).gameObject.SetActive(false);
        dailyScroll.gameObject.SetActive(true);

        weeklyTab.transform.GetChild(2).gameObject.SetActive(true);
        weeklyScroll.gameObject.SetActive(false);

        achieveTab.transform.GetChild(2).gameObject.SetActive(true);
        achieveScroll.gameObject.SetActive(false);
    }

    public void OnClickToggle()
    {
        if (dailyTab.isOn)
        {
            dailyTab.transform.GetChild(2).gameObject.SetActive(false);
            dailyScroll.gameObject.SetActive(true);

            weeklyTab.transform.GetChild(2).gameObject.SetActive(true);
            weeklyScroll.gameObject.SetActive(false);

            achieveTab.transform.GetChild(2).gameObject.SetActive(true);
            achieveScroll.gameObject.SetActive(false);
        }
        else if (weeklyTab.isOn)
        {
            weeklyTab.transform.GetChild(2).gameObject.SetActive(false);
            weeklyScroll.gameObject.SetActive(true);

            dailyTab.transform.GetChild(2).gameObject.SetActive(true);
            dailyScroll.gameObject.SetActive(false);

            achieveTab.transform.GetChild(2).gameObject.SetActive(true);
            achieveScroll.gameObject.SetActive(false);
        }
        else if (achieveTab.isOn)
        {
            achieveTab.transform.GetChild(2).gameObject.SetActive(false);
            achieveScroll.gameObject.SetActive(true);

            dailyTab.transform.GetChild(2).gameObject.SetActive(true);
            dailyScroll.gameObject.SetActive(false);

            weeklyTab.transform.GetChild(2).gameObject.SetActive(true);
            weeklyScroll.gameObject.SetActive(false);
        }
    }
}
