using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopScroll : MonoBehaviour
{
    public ShopWindow       MaterShop;      // 상점 오브젝트
    public ShopType         shopType;

    public List<ShopSlot>   slotList;

    private ScrollRect      scrollRect;     // 스크롤 위치 조정을 위한 스크롤렉트


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
        if (null == scrollRect)
            scrollRect = transform.GetComponent<ScrollRect>();

        scrollRect.verticalNormalizedPosition = 1.0f;
    }

    // 스크롤 내부에 아이템 슬롯을 채우는 함수
    public void SetUp()
    {
        scrollRect = GetComponent<ScrollRect>();


        // 상점 아이템 데이터를 저장
        var slotTempList = GetComponentsInChildren<ShopSlot>(true);
        List<ShopItemData> itemList = SceneStarter.Instance.userElements.GetShopItemList(shopType);

        slotList.Clear();
        int idx = 0;
        foreach (var data in slotTempList)
        {
            data.MasterScroll = this;
            data.shopType = shopType;
            data.itemIndex = idx;

            // Desc
            data.itemType               = itemList[idx].ItemType;
            data.itemIcon.sprite        = itemList[idx].ItemImg;
            if (null != itemList[idx].ItemImgTint)
            {
                data.itemIconTint.sprite = itemList[idx].ItemImgTint;
                data.itemIconTint.color = Global.CommanderInGameColorBellafide;
            }
            else
                data.itemIconTint.gameObject.SetActive(false);
            data.shopTitle.text         = itemList[idx].ShopTitle;
            data.shopDesc.text          = itemList[idx].ShopDesc;

            // Price
            data.priceType              = itemList[idx].PriceType;
            data.priceNum.text          = itemList[idx].Price.ToString();

            // Limit
            data.purchaseType           = itemList[idx].PurchaseType;
            data.bIsSoldout             = itemList[idx].bIsSoldout;
            data.bIsRandom              = itemList[idx].bIsRandomGoods;

            switch (data.purchaseType)
            {
                case PurchaseType.Weekly:
                    data.weeklyLimitText.gameObject.SetActive(true);
                    data.dailyLimitText.gameObject.SetActive(false);
                    data.limitIcon.gameObject.SetActive(true);

                    data.leftCount.text     = itemList[idx].LeftCount.ToString();
                    data.maxCount.text      = "/" + itemList[idx].LimitCount.ToString();
                    break;

                case PurchaseType.Daily:
                    data.weeklyLimitText.gameObject.SetActive(false);
                    data.dailyLimitText.gameObject.SetActive(true);
                    data.limitIcon.gameObject.SetActive(true);

                    data.leftCount.text     = itemList[idx].LeftCount.ToString();
                    data.maxCount.text      = "/" + itemList[idx].LimitCount.ToString();
                    break;

                case PurchaseType.Normal:
                    data.weeklyLimitText.gameObject.SetActive(false);
                    data.dailyLimitText.gameObject.SetActive(false);
                    data.leftCount.gameObject.SetActive(false);
                    data.maxCount.gameObject.SetActive(false);
                    data.limitIcon.gameObject.SetActive(false);
                    break;
            }

            slotList.Add(data);
            idx++;
        }

        OrderChange();
    }

    // 판매 완료된 아이템의 순서를 뒤로 보내는 함수
    public void OrderChange()
    {
        foreach (var data in slotList)
        {
            if (PurchaseType.Normal == data.purchaseType)
                break;
            else
            {
                if (data.bIsSoldout)
                {
                    data.MakeSoldout();
                    data.transform.SetAsLastSibling();
                }
            }
        }
    }
}
