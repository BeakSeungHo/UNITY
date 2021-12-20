using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RewardPopup : MonoBehaviour
{
    public List<RandomPopupSlot>    slotList;
    public TextMeshProUGUI          rewardText;

    [SerializeField]
    private ScrollRect              scrollRect;     // 스크롤 위치 조정을 위한 스크롤렉트


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        // 스크롤 위치 초기화
        scrollRect.verticalNormalizedPosition = 1.0f;


        // 보상 목록 로드
        List<ShopItemData> rewardList = GameManager.Instance.EndGameRewardList;
        var slotTempList = GetComponentsInChildren<RandomPopupSlot>(true);

        int idx = 0;
        slotList.Clear();
        foreach (var data in slotTempList)
        {
            // 보상 목록 데이터 로드
            if (idx < rewardList.Count)
            {
                data.itemIcon.sprite = rewardList[idx].ItemImg;
                if (null != rewardList[idx].ItemImgTint)
                {
                    data.itemIconTint.gameObject.SetActive(true);
                    data.itemIconTint.sprite = rewardList[idx].ItemImgTint;
                    data.itemIconTint.color = Global.CommanderInGameColorBellafide;
                }
                else
                    data.itemIconTint.gameObject.SetActive(false);

                data.shopTitle.text = rewardList[idx].ShopTitle;
                data.popupDesc.text = rewardList[idx].PopupDesc;

                if (1 == rewardList[idx].ItemCount)
                    data.itemCount.gameObject.SetActive(false);
                else
                    data.itemCount.text = rewardList[idx].ItemCount.ToString();

                slotList.Add(data);
                data.gameObject.SetActive(true);
            }
            // 남는 슬롯 비활성화
            else
                data.gameObject.SetActive(false);
            idx++;
        }

        rewardText.text = "경험치 " + GameManager.Instance.EndGameRewardExp.ToString()
                            + ", " + GameManager.Instance.EndGameRewardGold.ToString() + "G 와 위 보상을 획득했습니다.";
    }
}
