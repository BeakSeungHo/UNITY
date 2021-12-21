using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopWindow : MonoBehaviour
{
    public MainLobby        MasterLobby;        // 재화갱신을 위한 로비

    public ShopScroll       wealthScroll;
    public ShopScroll       growthScroll;
    public ShopScroll       buffScroll;

    public GameObject       wealthInactive;     // 각 메뉴의 비활성화 오버레이
    public GameObject       growthInactive;     
    public GameObject       buffInactive;

    public ShopType         curShop;            // 현재 선택된 카테고리, 아이템 번호
    public int              curItemIdx;
    public ConfirmPopup     confirmPopup;
    public RandomPopup      randomPopup;
    public ResultPopup      resultPopup;


    // Start is called before the first frame update
    void Start()
    {
        wealthScroll.MaterShop = this;
        wealthScroll.shopType = ShopType.Goods;
        wealthScroll.gameObject.SetActive(true);

        growthScroll.MaterShop = this;
        growthScroll.shopType = ShopType.Growth;
        growthScroll.gameObject.SetActive(false);

        buffScroll.MaterShop = this;
        buffScroll.shopType = ShopType.BuffGoods;
        buffScroll.gameObject.SetActive(false);


        // 스크롤 초기화
        curShop = ShopType.Goods;

        wealthScroll.SetUp();
        growthScroll.SetUp();
        buffScroll.SetUp();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnEnable()
    {
        // 첫번째 카테고리로 설정
        curShop = ShopType.Goods;
        wealthInactive.SetActive(false);
        growthInactive.SetActive(true);
        buffInactive.SetActive(true);

        wealthScroll.gameObject.SetActive(true);
        growthScroll.gameObject.SetActive(false);
        buffScroll.gameObject.SetActive(false);

        // 팝업창 비활성화
        confirmPopup.gameObject.SetActive(false);
        randomPopup.gameObject.SetActive(false);
        resultPopup.gameObject.SetActive(false);
    }

    // 재화 카테고리 클릭 함수
    public void OnClickWealthCategory()
    {
        if (ShopType.Goods == curShop)
            return;
        else
        {
            curShop = ShopType.Goods;

            wealthInactive.SetActive(false);
            growthInactive.SetActive(true);
            buffInactive.SetActive(true);

            wealthScroll.gameObject.SetActive(true);
            growthScroll.gameObject.SetActive(false);
            buffScroll.gameObject.SetActive(false);
        }
    }
    // 성장 카테고리 클릭 함수
    public void OnClickGrowthCategory()
    {
        if (ShopType.Growth == curShop)
            return;
        else
        {
            curShop = ShopType.Growth;

            wealthInactive.SetActive(true);
            growthInactive.SetActive(false);
            buffInactive.SetActive(true);

            wealthScroll.gameObject.SetActive(false);
            growthScroll.gameObject.SetActive(true);
            buffScroll.gameObject.SetActive(false);
        }
    }
    // 버프 카테고리 클릭 함수
    public void OnClickBuffCategory()
    {
        if (ShopType.BuffGoods == curShop)
            return;
        else
        {
            curShop = ShopType.BuffGoods;

            wealthInactive.SetActive(true);
            growthInactive.SetActive(true);
            buffInactive.SetActive(false);

            wealthScroll.gameObject.SetActive(false);
            growthScroll.gameObject.SetActive(false);
            buffScroll.gameObject.SetActive(true);
        }
    }

    // 아이템 버튼이 눌렸을 때 팝업
    public void ConfirmPopUpOpen(ShopType shopT, PriceType priceT, int itemIdx)
    {
        curShop     = shopT;
        curItemIdx  = itemIdx;

        confirmPopup.gameObject.SetActive(true);
        confirmPopup.ChangeDesc(curShop, priceT, curItemIdx);
    }

    // 랜덤 상자 버튼이 눌렸을 때 팝업
    public void RandomPopUpOpen(ShopType shopT, PriceType priceT, int itemIdx)
    {
        curShop = shopT;
        curItemIdx = itemIdx;

        randomPopup.curShop = shopT;
        randomPopup.priceType = priceT;
        randomPopup.curItemIdx = itemIdx;
        randomPopup.gameObject.SetActive(true);
    }

    // 확정 상품 구입 성공했을 경우 상품 목록 갱신
    public void BuySuccessNormal()
    {
        MasterLobby.UpdateWealth();

        switch (curShop)
        {
            case ShopType.Goods:
                if (PurchaseType.Normal != wealthScroll.slotList[curItemIdx].purchaseType)
                {
                    ShopItemData temp = SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx);

                    wealthScroll.slotList[curItemIdx].leftCount.text = temp.LeftCount.ToString();

                    if (temp.bIsSoldout)
                    {
                        wealthScroll.slotList[curItemIdx].bIsSoldout = true;
                        wealthScroll.OrderChange();
                    }
                }
                break;
            case ShopType.Growth:
                if (PurchaseType.Normal != growthScroll.slotList[curItemIdx].purchaseType)
                {
                    ShopItemData temp = SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx);

                    growthScroll.slotList[curItemIdx].leftCount.text = temp.LeftCount.ToString();

                    if (temp.bIsSoldout)
                    {
                        growthScroll.slotList[curItemIdx].bIsSoldout = true;
                        growthScroll.OrderChange();
                    }
                }
                break;
            case ShopType.BuffGoods:
                if (PurchaseType.Normal != buffScroll.slotList[curItemIdx].purchaseType)
                {
                    ShopItemData temp = SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx);

                    buffScroll.slotList[curItemIdx].leftCount.text = temp.LeftCount.ToString();

                    if (temp.bIsSoldout)
                    {
                        buffScroll.slotList[curItemIdx].bIsSoldout = true;
                        buffScroll.OrderChange();
                    }
                }
                break;
        }
    }

    // 랜덤 상품 구입 성공했을 경우
    public void BuySuccessRandom()
    {
        switch (curShop)
        {
            case ShopType.Goods:
                if (PurchaseType.Normal != wealthScroll.slotList[curItemIdx].purchaseType)
                {
                    ShopItemData temp = SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx);

                    wealthScroll.slotList[curItemIdx].leftCount.text = temp.LeftCount.ToString();

                    if (temp.bIsSoldout)
                    {
                        wealthScroll.slotList[curItemIdx].bIsSoldout = true;
                        wealthScroll.OrderChange();
                    }
                }
                break;
            case ShopType.Growth:
                if (PurchaseType.Normal != growthScroll.slotList[curItemIdx].purchaseType)
                {
                    ShopItemData temp = SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx);

                    growthScroll.slotList[curItemIdx].leftCount.text = temp.LeftCount.ToString();

                    if (temp.bIsSoldout)
                    {
                        growthScroll.slotList[curItemIdx].bIsSoldout = true;
                        growthScroll.OrderChange();
                    }
                }
                break;
            case ShopType.BuffGoods:
                if (PurchaseType.Normal != buffScroll.slotList[curItemIdx].purchaseType)
                {
                    ShopItemData temp = SceneStarter.Instance.userElements.GetShopItem(curShop, curItemIdx);

                    buffScroll.slotList[curItemIdx].leftCount.text = temp.LeftCount.ToString();

                    if (temp.bIsSoldout)
                    {
                        buffScroll.slotList[curItemIdx].bIsSoldout = true;
                        buffScroll.OrderChange();
                    }
                }
                break;
        }
    }
}
