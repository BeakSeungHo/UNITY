using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class RandomPopup : MonoBehaviour
{
    public ShopWindow               MasterShop;
    public ShopType                 curShop;        // 현재 선택된 카테고리, 아이템 번호
    public int                      curItemIdx;

    public List<RandomPopupSlot>    slotList;

    // Price
    public PriceType                priceType;          // 구입 재화 종류
    public Image                    priceTypeIcon;
    public TextMeshProUGUI          priceNum;

    // Warning Message
    public TextMeshProUGUI          warningMsg;
    public bool                     bMsgPlayed;
    public float                    msgAlpha;
    public float                    fadeSpeed;

    [SerializeField]
    private ScrollRect              scrollRect;     // 스크롤 위치 조정을 위한 스크롤렉트


    // Start is called before the first frame update
    void Start()
    {
        fadeSpeed = 0.75f;
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
        bMsgPlayed = false;
        msgAlpha = 1.0f;
        warningMsg.gameObject.SetActive(false);

        // 스크롤 위치 초기화
        scrollRect.verticalNormalizedPosition = 1.0f;


        // 보상 목록 로드
        List<ShopItemData> boxReward;
        var slotTempList = GetComponentsInChildren<RandomPopupSlot>(true);

        int idx = 0;
        switch (curItemIdx)
        {
            // 랜덤 박스1
            case 0:
                boxReward = SceneStarter.Instance.userElements.GetRandomBox1RewardList();

                slotList.Clear();
                foreach (var data in slotTempList)
                {
                    // 보상 목록 데이터 로드
                    if (idx < boxReward.Count)
                    {
                        data.itemIcon.sprite    = boxReward[idx].ItemImg;
                        if (null != boxReward[idx].ItemImgTint)
                        {
                            data.itemIconTint.sprite = boxReward[idx].ItemImgTint;
                            data.itemIconTint.color = Global.CommanderInGameColorBellafide;
                        }
                        else
                            data.itemIconTint.gameObject.SetActive(false);
                        data.shopTitle.text     = boxReward[idx].ShopTitle;
                        data.popupDesc.text     = boxReward[idx].PopupDesc;
                        data.itemCount.gameObject.SetActive(false);

                        slotList.Add(data);
                        data.gameObject.SetActive(true);
                    }
                    // 남는 슬롯 비활성화
                    else
                        data.gameObject.SetActive(false);
                    idx++;
                }
                break;
            // 랜덤 박스2
            case 1:
                boxReward = SceneStarter.Instance.userElements.GetRandomBox2RewardList();

                slotList.Clear();
                foreach (var data in slotTempList)
                {
                    // 보상 목록 데이터 로드
                    if (idx < boxReward.Count)
                    {
                        data.itemIcon.sprite    = boxReward[idx].ItemImg;
                        if (null != boxReward[idx].ItemImgTint)
                        {
                            data.itemIconTint.sprite = boxReward[idx].ItemImgTint;
                            data.itemIconTint.color = Global.CommanderInGameColorBellafide;
                        }
                        else
                            data.itemIconTint.gameObject.SetActive(false);
                        data.shopTitle.text     = boxReward[idx].ShopTitle;
                        data.popupDesc.text     = boxReward[idx].PopupDesc;
                        data.itemCount.gameObject.SetActive(false);

                        slotList.Add(data);
                        data.gameObject.SetActive(true);
                    }
                    // 남는 슬롯 비활성화
                    else
                        data.gameObject.SetActive(false);
                    idx++;
                }
                break;
        }

        // 가격 표시
        switch(SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx).PriceType)
        {
            case PriceType.Gold:
                priceTypeIcon.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Gold2];
                break;
            case PriceType.Jewel:
                priceTypeIcon.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Dia2];
                break;
        }
        priceNum.text = SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx).Price.ToString();
    }

    // 아이템 구매 확정
    public void OnClickBuyBtn()
    {
        // 구입 실패할 경우 경고 메세지 활성화
        if (!SceneStarter.Instance.userElements.BuyShopItem(curShop, curItemIdx))
        {
            bMsgPlayed = true;
            msgAlpha = 1.0f;
            warningMsg.gameObject.SetActive(true);
        }
        // 구입 성공
        else
        {
            if (SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx).bIsRandomGoods)
            {
                MasterShop.resultPopup.gameObject.SetActive(true);
                gameObject.SetActive(false);
            }
            else
            {
                MasterShop.BuySuccessNormal();
                gameObject.SetActive(false);
            }
        }
    }

    // 구매 확인창 닫기
    public void OnClickCancelBtn()
    {
        gameObject.SetActive(false);
    }
}
