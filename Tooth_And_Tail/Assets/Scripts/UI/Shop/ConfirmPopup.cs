using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


/// <summary>
/// 20.11.12
/// 
/// </summary>


public class ConfirmPopup : MonoBehaviour
{
    public ShopWindow           MasterShop;
    public GameObject           NameChangePopup;

    public Image                itemIcon;
    public Image                itemIconTint;
    public TextMeshProUGUI      itemName;
    public TextMeshProUGUI      popupDesc;

    public ShopType             curShop;            // 현재 선택된 카테고리, 아이템 번호
    public int                  curItemIdx;

    // Price
    public PriceType            priceType;          // 구입 재화 종류
    public Image                priceTypeIcon;
    public TextMeshProUGUI      priceNum;

    // Warning Message
    public TextMeshProUGUI      warningMsg;
    public bool                 bMsgPlayed;
    public float                msgAlpha;
    public float                fadeSpeed;


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

    // 경고 메세지 초기화
    private void OnEnable()
    {
        bMsgPlayed = false;
        msgAlpha = 1.0f;
        warningMsg.gameObject.SetActive(false);
    }

    // 선택된 아이템 정보 갱신
    public void ChangeDesc(ShopType shopT, PriceType priceT, int itemIdx)
    {
        ShopItemData curItem = SceneStarter.Instance.userElements.GetShopItem(shopT, itemIdx);

        curShop             = shopT;
        curItemIdx          = itemIdx;

        itemIcon.sprite     = curItem.ItemImg;
        if (null != curItem.ItemImgTint)
        {
            itemIconTint.gameObject.SetActive(true);
            itemIconTint.sprite = curItem.ItemImgTint;
            itemIconTint.color = Global.CommanderInGameColorBellafide;
        }
        else
            itemIconTint.gameObject.SetActive(false);
        itemName.text       = curItem.ShopTitle;
        popupDesc.text      = curItem.PopupDesc;

        priceType           = curItem.PriceType;
        switch (priceType)
        {
            case PriceType.Gold:
                priceTypeIcon.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Gold2];
                break;
            case PriceType.Jewel:
                priceTypeIcon.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Dia2];
                break;
        }
        priceNum.text       = curItem.Price.ToString();
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
            gameObject.SetActive(false);
            MasterShop.BuySuccessNormal();

            if (ShopType.BuffGoods ==  curShop && 2 == curItemIdx)
            {
                NameChangePopup.gameObject.SetActive(true);
            }
        }
    }

    // 구매 확인창 닫기
    public void OnClickCancelBtn()
    {
        gameObject.SetActive(false);
    }
}
