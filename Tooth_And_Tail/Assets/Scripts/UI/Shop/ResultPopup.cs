using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class ResultPopup : MonoBehaviour
{
    public ShopWindow               MasterShop;

    public List<RandomPopupSlot>    slotList;
    public TextMeshProUGUI          confirmText;

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

    // 구매 결과 창
    private void OnEnable()
    {
        scrollRect.verticalNormalizedPosition = 1.0f;


        // 결과 목록 로드
        List<ShopItemData> boxResult = SceneStarter.Instance.userElements.GetRandomBoxItemList();
        var slotTempList = GetComponentsInChildren<RandomPopupSlot>(true);

        int idx = 0;
        int countSum = 0;
        slotList.Clear();
        foreach (var data in slotTempList)
        {
            // 보상 목록 데이터 로드
            if (idx < boxResult.Count)
            {
                data.itemIcon.sprite    = boxResult[idx].ItemImg;
                if (null != boxResult[idx].ItemImgTint)
                {
                    data.itemIconTint.sprite = boxResult[idx].ItemImgTint;
                    data.itemIconTint.color = Global.CommanderInGameColorBellafide;
                }
                else
                    data.itemIconTint.gameObject.SetActive(false);
                data.shopTitle.text     = boxResult[idx].ShopTitle;
                data.popupDesc.text     = boxResult[idx].PopupDesc;
                if (1 == boxResult[idx].ItemCount)
                    data.itemCount.gameObject.SetActive(false);
                else
                    data.itemCount.text = boxResult[idx].ItemCount.ToString();
                countSum += boxResult[idx].ItemCount;

                slotList.Add(data);
                data.gameObject.SetActive(true);
            }
            // 남는 슬롯 비활성화
            else
                data.gameObject.SetActive(false);
            idx++;
        }

        confirmText.text = "총 " + countSum.ToString() + "개의 강화재료를 얻었습니다.";
    }

    // 구매 결과창 닫기
    public void OnClickConfirmBtn()
    {
        MasterShop.BuySuccessRandom();
        gameObject.SetActive(false);
    }
}
