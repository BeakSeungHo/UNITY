using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CampaignWindow : MonoBehaviour
{
    public MainLobby            MasterLobby;

    public List<CampaignSlot>   ChapterList;

    // ChapterInfo
    public GameObject           chapterInfo;
    public TextMeshProUGUI      staminaNeed;
    public TextMeshProUGUI      warningMsg;
    public bool                 bMsgPlayed;
    public float                msgAlpha;
    public float                fadeSpeed;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // 경고 메세지 출력
        if (bMsgPlayed)
        {
            warningMsg.alpha = msgAlpha;
            msgAlpha -= Time.deltaTime * fadeSpeed;

            // 투명도가 0이 되면 비활성화
            if (msgAlpha < 0.0f)
            {
                bMsgPlayed = false;
                msgAlpha = 1.0f;
                warningMsg.gameObject.SetActive(false);
            }
        }
    }

    private void OnEnable()
    {
        // 경고 메세지 초기화
        fadeSpeed = 0.75f;
        bMsgPlayed = false;
        msgAlpha = 1.0f;
        warningMsg.gameObject.SetActive(false);

        // 필요 스태미나 표시
        staminaNeed.text = "-" + Global.StaminaCampaign.ToString();


        var chTempList = GetComponentsInChildren<CampaignSlot>(true);

        ChapterList.Clear();
        int idx = 0;
        foreach (var data in chTempList)
        {
            data.MasterCampaign = this;
            data.chapter = (Camp)idx;
            data.isUnlocked = false;

            ChapterList.Add(data);
            idx++;
        }

        ChapterList[0].UnlockChapter();
        chapterInfo.gameObject.SetActive(false);
    }

    public void OnClickToSelection()
    {
        chapterInfo.gameObject.SetActive(false);
    }

    public void OnClickStart()
    {
        if (SceneStarter.Instance.userElements.UserData.UserCurStamina < Global.StaminaCampaign)
        {
            // 경고 메세지 활성화
            warningMsg.text = "탄환이 부족합니다";
            bMsgPlayed = true;
            msgAlpha = 1.0f;
            warningMsg.gameObject.SetActive(true);
        }
        else
        {
            SceneStarter.Instance.userElements.UserData.UserCurStamina -= Global.StaminaCampaign;
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Daily, 2, Global.StaminaCampaign);
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 4, Global.StaminaCampaign);
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 5, Global.StaminaCampaign);
            MasterLobby.OnClickPlay();
        }
    }
}
