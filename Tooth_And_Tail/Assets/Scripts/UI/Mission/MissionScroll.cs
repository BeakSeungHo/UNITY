using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionScroll : MonoBehaviour
{
    public MissionWindow        MasterMission;  // 미션 오브젝트
    public MissionType          missionType;

    public List<MissionSlot>    SlotList;

    private ScrollRect          scrollRect;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetUp()
    {
        scrollRect = GetComponent<ScrollRect>();

        var slotTempList = GetComponentsInChildren<MissionSlot>(true);
        List<MissionData> missList = SceneStarter.Instance.userElements.GetMissionsList(missionType);

        SlotList.Clear();
        int idx = 0;
        foreach (var data in slotTempList)
        {
            data.MasterScroll = this;

            data.missionType = missList[idx].missionType;
            data.missionIndex = idx;

            data.missionTitle.text = missList[idx].MissionName;
            data.missionDesc.text = missList[idx].Missiondesc;
            data.progressNum.text = missList[idx].MissionCount.ToString() + "/" + missList[idx].CompleteCount.ToString();

            data.progressFill.fillAmount = (float)missList[idx].MissionCount / (float)missList[idx].CompleteCount;
            if (missList[idx].MissionCount != missList[idx].CompleteCount)
                data.btnInactive.gameObject.SetActive(true);
            else
                data.btnInactive.gameObject.SetActive(false);
            data.bIsComplete = missList[idx].bIsComplete;

            if (data.bIsComplete)
                data.transform.SetAsLastSibling();

            // 보상
            for (int i = 0; i< missList[idx].RewardList.Count; i++)
            {
                data.RewardList[i].itemType = (int)missList[idx].RewardList[i].ItemType;
                data.RewardList[i].icon.sprite = SceneStarter.Instance.userElements.ItemDataList[data.RewardList[i].itemType].ItemImg;
                data.RewardList[i].countText.text = missList[idx].RewardList[i].ItemCount.ToString();
                data.RewardList[i].countText.color = new Color(1.0f, 1.0f, 1.0f);
                data.RewardList[i].gameObject.SetActive(true);
            }

            SlotList.Add(data);
            idx++;
        }

        OrderChange();
    }

    private void OnEnable()
    {
        if (null == scrollRect)
            scrollRect = GetComponent<ScrollRect>();

        scrollRect.verticalNormalizedPosition = 1.0f;

        SetUp();
    }

    public void OrderChange()
    {
        foreach(var data in SlotList)
        {
            if (data.bIsComplete)
            {
                data.MakeComplete();
                data.transform.SetAsLastSibling();
            }
        }
    }
}
