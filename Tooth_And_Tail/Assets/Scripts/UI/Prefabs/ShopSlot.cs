using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopSlot : MonoBehaviour
{
    public ShopScroll       MasterScroll;
    public ShopType         shopType;           // 상점 종류
    public int              itemIndex;

    // Desc
    public ItemType         itemType;
    public Image            itemIcon;
    public Image            itemIconTint;
    public TextMeshProUGUI  shopTitle;
    public TextMeshProUGUI  shopDesc;

    // Price
    public PriceType        priceType;          // 구입 재화 종류
    public Image            priceTypeIcon;
    public TextMeshProUGUI  priceNum;

    // Limit
    public PurchaseType     purchaseType;       // 구매제한
    public TextMeshProUGUI  dailyLimitText;
    public TextMeshProUGUI  weeklyLimitText;
    public TextMeshProUGUI  leftCount;
    public TextMeshProUGUI  maxCount;
    public Image            limitIcon;

    public Image            btnInactive;
    public bool             bIsSoldout;
    public bool             bIsRandom;



    // Start is called before the first frame update
    void Start()
    {
        switch (priceType)
        {
            case PriceType.Gold:
                priceTypeIcon.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Gold2];
                break;
            case PriceType.Jewel:
                priceTypeIcon.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Dia2];
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 매진된 상품의 음영을 조절
    public void MakeSoldout()
    {
        if (!btnInactive.gameObject.activeSelf)
        {
            bIsSoldout = true;
            btnInactive.gameObject.SetActive(true);

            leftCount.text = "0";
            maxCount.text = "/" + SceneStarter.Instance.userElements.GetShopItem(shopType, itemIndex).LimitCount.ToString();
        }
    }

    // 상품을 선택했을 때 구매 확인창 오픈
    public void OnClickShopSlot()
    {
        if (bIsSoldout)
            return;

        // 랜덤 상자 상품
        if (bIsRandom)
        {
            MasterScroll.MaterShop.RandomPopUpOpen(shopType, priceType, itemIndex);
        }
        else
            MasterScroll.MaterShop.ConfirmPopUpOpen(shopType, priceType, itemIndex);
    }
}
