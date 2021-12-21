using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameSummary : MonoBehaviour
{
    //
    public StatisticsWindow     MaterStat;

    // PlayerInfo
    public Image                portraitPlayer;
    public Image                portraitAI;
    public TextMeshProUGUI      playerName;
    public TextMeshProUGUI      campPlayer;
    public TextMeshProUGUI      campAI;

    // GameResult
    public TextMeshProUGUI      gameTime;
    public TextMeshProUGUI      resultPlayer;
    public TextMeshProUGUI      resultAI;

    // GameData
    public TextMeshProUGUI      foodPlayerNum;              // 총 식량
    public TextMeshProUGUI      foodAINum;
    public Image                foodPlayer;
    public Image                foodAI;
    public TextMeshProUGUI      armyPlayerNum;              // 최종 부대 가치
    public TextMeshProUGUI      armyAINum;
    public Image                armyPlayer;                 
    public Image                armyAI;
    public TextMeshProUGUI      damagePlayerNum;            // 피해량
    public TextMeshProUGUI      damageAINum;
    public Image                damagePlayer;
    public Image                damageAI;
    public TextMeshProUGUI      gristmillPlayer;            // 제분소 건설/파괴
    public TextMeshProUGUI      gristmillAI;
    public TextMeshProUGUI      gristmillDestroyPlayer;
    public TextMeshProUGUI      gristmillDestroyAI;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetUpSummary()
    {
        // PlayerInfo
        portraitPlayer.sprite   = SceneStarter.Instance.uIElements.UIComIconDic[SceneStarter.Instance.statisticElements.campPlayer];
        portraitAI.sprite       = SceneStarter.Instance.uIElements.UIComIconDic[SceneStarter.Instance.statisticElements.campAI];
        playerName.text         = SceneStarter.Instance.userElements.UserData.UserName;

        switch (SceneStarter.Instance.statisticElements.campPlayer)
        {
            case Camp.Bellafide:
                campPlayer.text = "벨라피드";
                break;
            case Camp.Hopper:
                campPlayer.text = "호퍼";
                break;
            case Camp.Quartermaster:
                campPlayer.text = "병참장교";
                break;
            case Camp.Archimedes:
                campPlayer.text = "아르키메데스";
                break;
        }
        switch (SceneStarter.Instance.statisticElements.campAI)
        {
            case Camp.Bellafide:
                campAI.text = "벨라피드";
                break;
            case Camp.Hopper:
                campAI.text = "호퍼";
                break;
            case Camp.Quartermaster:
                campAI.text = "병참장교";
                break;
            case Camp.Archimedes:
                campAI.text = "아르키메데스";
                break;
        }

        // GameResult
        int min = Mathf.RoundToInt(SceneStarter.Instance.statisticElements.gameTime) / 60;
        int sec = Mathf.RoundToInt(SceneStarter.Instance.statisticElements.gameTime) % 60;
        if (sec < 10)
            gameTime.text = min.ToString() + ":0" + sec.ToString();
        else
            gameTime.text = min.ToString() + ":" + sec.ToString();

        if (SceneStarter.Instance.statisticElements.winPlayer)
        {
            resultPlayer.text = "승리";
            resultAI.text = "패배";
        }
        else
        {
            resultPlayer.text = "패배";
            resultAI.text = "승리";
        }

        // GameData
        int tempCount = 0;

        foodPlayerNum.text      = SceneStarter.Instance.statisticElements.foodPlayer.ToString();
        foodAINum.text          = SceneStarter.Instance.statisticElements.foodAI.ToString();

        tempCount               = SceneStarter.Instance.statisticElements.foodPlayer > SceneStarter.Instance.statisticElements.foodAI
                                  ? SceneStarter.Instance.statisticElements.foodPlayer / 10 + 1
                                  : SceneStarter.Instance.statisticElements.foodAI / 10 + 1;
        foodPlayer.fillAmount   = SceneStarter.Instance.statisticElements.foodPlayer / (float)(10 * tempCount);
        foodAI.fillAmount       = SceneStarter.Instance.statisticElements.foodAI / (float)(10 * tempCount);


        armyPlayerNum.text      = SceneStarter.Instance.statisticElements.armyPlayer.ToString();
        armyAINum.text          = SceneStarter.Instance.statisticElements.armyAI.ToString();

        tempCount               = SceneStarter.Instance.statisticElements.armyPlayer > SceneStarter.Instance.statisticElements.armyAI
                                  ? SceneStarter.Instance.statisticElements.armyPlayer / 10 + 1
                                  : SceneStarter.Instance.statisticElements.armyAI / 10 + 1;
        armyPlayer.fillAmount   = SceneStarter.Instance.statisticElements.armyPlayer / (float)(10 * tempCount);
        armyAI.fillAmount       = SceneStarter.Instance.statisticElements.armyAI / (float)(10 * tempCount);


        damagePlayerNum.text    = Mathf.RoundToInt(SceneStarter.Instance.statisticElements.damagePlayer).ToString();
        damageAINum.text        = Mathf.RoundToInt(SceneStarter.Instance.statisticElements.damageAI).ToString();

        if (SceneStarter.Instance.statisticElements.damagePlayer > SceneStarter.Instance.statisticElements.damageAI)
        {
            tempCount               = Mathf.RoundToInt(SceneStarter.Instance.statisticElements.damagePlayer) / 10 + 1;
            damagePlayer.fillAmount = SceneStarter.Instance.statisticElements.damagePlayer / (float)(10 * tempCount);
            damageAI.fillAmount     = SceneStarter.Instance.statisticElements.damageAI / (float)(10 * tempCount);
        }
        else
        {
            tempCount               = Mathf.RoundToInt(SceneStarter.Instance.statisticElements.damageAI) / 10 + 1;
            damagePlayer.fillAmount = SceneStarter.Instance.statisticElements.damagePlayer / (float)(10 * tempCount);
            damageAI.fillAmount     = SceneStarter.Instance.statisticElements.damageAI / (float)(10 * tempCount);
        }
        
        gristmillPlayer.text        = SceneStarter.Instance.statisticElements.gristmillPlayer.ToString() + "개";
        gristmillAI.text            = SceneStarter.Instance.statisticElements.gristmillAI.ToString() + "개";
        gristmillDestroyPlayer.text = SceneStarter.Instance.statisticElements.gristmillDestroyPlayer.ToString() + "개";
        gristmillDestroyAI.text     = SceneStarter.Instance.statisticElements.gristmillDestroyAI.ToString() + "개";
    }
}
