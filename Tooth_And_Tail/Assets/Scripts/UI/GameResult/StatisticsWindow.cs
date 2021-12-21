using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class StatisticsWindow : MonoBehaviour
{
    public MainLobby            lobby;
    public LevelUpPopUp         levelupPopup;

    // Toggle
    public Toggle               summaryTab;
    public Toggle               armyValueTab;
    public Toggle               foodIncomeTab;

    // UI Objects
    public TextMeshProUGUI      victoryMsg;

    // Toggle Contents
    public GraphCategory        category;
    public GraphAxis            axis;
    public GameSummary          gameSummary;
    public ArmyValueGraph       armyValue;
    public FoodIncomeGraph      foodIncome;

    // Graph Container
    public GraphContainer       graphValue;
    public GraphContainer       graphFood;

    // Reward Popup
    public RewardPopup          rewardPopup;


    // Start is called before the first frame update
    void Start()
    {
        gameSummary.MaterStat = this;
        gameSummary.gameObject.SetActive(true);

        armyValue.MaterStat = this;
        armyValue.gameObject.SetActive(false);

        foodIncome.MaterStat = this;
        foodIncome.gameObject.SetActive(false);

        axis.MaterStat = this;
        category.MaterStat = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        rewardPopup.gameObject.SetActive(false);
    }

    public void OnClickToggle()
    {
        // 게임 요약
        if (summaryTab.isOn)
        {
            summaryTab.transform.GetChild(1).gameObject.SetActive(false);
            gameSummary.gameObject.SetActive(true);

            armyValueTab.transform.GetChild(1).gameObject.SetActive(true);
            armyValue.gameObject.SetActive(false);

            foodIncomeTab.transform.GetChild(1).gameObject.SetActive(true);
            foodIncome.gameObject.SetActive(false);

            category.gameObject.SetActive(false);
            axis.gameObject.SetActive(false);
        }
        // 부대 가치
        else if (armyValueTab.isOn)
        {
            armyValueTab.transform.GetChild(1).gameObject.SetActive(false);
            armyValue.gameObject.SetActive(true);

            summaryTab.transform.GetChild(1).gameObject.SetActive(true);
            gameSummary.gameObject.SetActive(false);

            foodIncomeTab.transform.GetChild(1).gameObject.SetActive(true);
            foodIncome.gameObject.SetActive(false);

            category.gameObject.SetActive(true);
            axis.gameObject.SetActive(true);
            axis.ChangeVerAxis();
        }
        // 식량
        else if (foodIncomeTab.isOn)
        {
            foodIncomeTab.transform.GetChild(1).gameObject.SetActive(false);
            foodIncome.gameObject.SetActive(true);

            summaryTab.transform.GetChild(1).gameObject.SetActive(true);
            gameSummary.gameObject.SetActive(false);

            armyValueTab.transform.GetChild(1).gameObject.SetActive(true);
            armyValue.gameObject.SetActive(false);

            category.gameObject.SetActive(true);
            axis.gameObject.SetActive(true);
            axis.ChangeVerAxis();
        }
    }

    // 보상 확인창
    public void OnClickRewardButton()
    {
        rewardPopup.gameObject.SetActive(true);
    }

    // 로비 화면으로
    public void OnClickToButtonToNxt()
    {
        graphValue.ClearGraph();
        graphFood.ClearGraph();

        lobby.OnClickReturnToLobby();
        if (SceneStarter.Instance.userElements.bLevelUp)
        {
            levelupPopup.gameObject.SetActive(true);
            SceneStarter.Instance.userElements.bLevelUp = false;
        }
    }

    public void SetUpGraph()
    {
        switch (SceneStarter.Instance.statisticElements.winCamp)
        {
            case Camp.Bellafide:
                victoryMsg.text = "롱코트의 득세!";
                break;
            case Camp.Hopper:
                victoryMsg.text = "민중의 득세!";
                break;
            case Camp.Quartermaster:
                victoryMsg.text = "KSR의 득세!";
                break;
            case Camp.Archimedes:
                victoryMsg.text = "문명교의 득세!";
                break;
        }

        gameSummary.SetUpSummary();
        graphValue.SetUpGraph();
        graphFood.SetUpGraph();

        axis.SetUpAxis();
        axis.gameObject.SetActive(false);
        category.SetUpCategory();
        category.gameObject.SetActive(false);
    }
}
