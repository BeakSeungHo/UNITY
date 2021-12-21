using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class MissionSlot : MonoBehaviour
{
    public MissionScroll        MasterScroll;   // 스크롤뷰 오브젝트
    public MissionType          missionType;
    public int                  missionIndex;

    public TextMeshProUGUI      missionTitle;
    public TextMeshProUGUI      missionDesc;
    public TextMeshProUGUI      progressTitle;
    public TextMeshProUGUI      progressNum;

    public Image                progressFill;
    public Image                btnInactive;

    public bool                 bIsComplete;

    public List<InvenSlot>      RewardList;     // 보상목록



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 완료된 미션의 음영을 조절하는 함수
    public void MakeComplete()
    {
        btnInactive.gameObject.SetActive(true);

        missionTitle.alpha = 0.6f;
        missionDesc.alpha = 0.6f;
        progressTitle.alpha = 0.6f;
        progressNum.alpha = 0.6f;

        foreach(var data in RewardList)
        {
            data.icon.transform.GetChild(0).gameObject.SetActive(true);
            data.countText.color = new Color(0.5f, 0.5f, 0.5f);
        }

        progressFill.color = new Color(progressFill.color.r, progressFill.color.g, progressFill.color.b, 0.6f);
    }

    // 완료한 미션의 보상을 수령하는 함수
    public void OnClickClaimBtn()
    {
        if (!btnInactive.gameObject.activeSelf)
        {
            if (SceneStarter.Instance.userElements.CompleteMission(missionType, missionIndex))
            {
                bIsComplete = true;

                switch(missionType)
                {
                    case MissionType.Daily:
                        SceneStarter.Instance.userElements.AddMissionCount(MissionType.Daily, 4, 1);
                        if (4 == missionIndex)
                        {
                            progressFill.fillAmount = (float)SceneStarter.Instance.userElements.MissionsDic[MissionType.Daily][4].CompleteCount
                                                        / (float)SceneStarter.Instance.userElements.MissionsDic[MissionType.Daily][4].MissionCount;
                        }
                        break;
                    case MissionType.Weekly:
                        SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 8, 1);

                        if (8 == missionIndex)
                        {
                            progressFill.fillAmount = (float)SceneStarter.Instance.userElements.MissionsDic[MissionType.Weekly][8].CompleteCount
                                                        / (float)SceneStarter.Instance.userElements.MissionsDic[MissionType.Weekly][8].MissionCount;
                        }
                        break;
                }
                GameManager.Instance.ReceivedMissionCount++;
            }

            MasterScroll.OrderChange();
        }
    }
}
