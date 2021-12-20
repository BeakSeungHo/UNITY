using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InvenPopup : MonoBehaviour
{
    public InventoryWindow  Master;         // 인벤토리 오브젝트
    public int              curType;        // 현재 아이템 타입

    // ItemDesc
    public Image            BgImage;
    public Image            ItemIcon;
    public TextMeshProUGUI  ItemName;
    public TextMeshProUGUI  ItemPrice;

    public int              sellAmount;
    public int              sellPrice;
    public TextMeshProUGUI  TextAmount;     // 판매개수
    public TextMeshProUGUI  TextPrice;      // 판매 금액


    // 팝업창 활성
    public void Open(int _type)
    {
        curType = _type;
        ItemIcon.sprite = SceneStarter.Instance.userElements.ItemDataList[_type].ItemImg;
        ItemName.text = SceneStarter.Instance.userElements.ItemDataList[_type].ItemName;
        ItemPrice.text = SceneStarter.Instance.userElements.ItemDataList[_type].price.ToString() + "G";
        sellAmount = 0;
        sellPrice = 0;
        TextAmount.text = "0";
        TextPrice.text = "0 G";
    }

    // 판매개수 증가
    public void ItemIncrease()
    {
        if (sellAmount == SceneStarter.Instance.userElements.GetItemCount(curType))
        {
            sellAmount = 0;
            sellPrice = 0;
            TextAmount.text = "0";
            TextPrice.text = "0 G";
        }
        else if (sellAmount < SceneStarter.Instance.userElements.GetItemCount(curType))
        {
            sellAmount++;
            TextAmount.text = sellAmount.ToString();

            sellPrice = sellAmount * SceneStarter.Instance.userElements.ItemDataList[curType].price;
            TextPrice.text = sellPrice.ToString() + " G";
        }
    }

    // 판매개수 감소
    public void ItemDecrease()
    {
        if (0 == sellAmount)
        {
            sellAmount = SceneStarter.Instance.userElements.GetItemCount(curType);
            TextAmount.text = sellAmount.ToString();

            sellPrice = sellAmount * SceneStarter.Instance.userElements.ItemDataList[curType].price;
            TextPrice.text = sellPrice.ToString() + " G";
        }
        else
        {
            sellAmount--;
            TextAmount.text = sellAmount.ToString();

            sellPrice = sellAmount * SceneStarter.Instance.userElements.ItemDataList[curType].price;
            TextPrice.text = sellPrice.ToString() + " G";
        }
    }

    // 아이템 판매
    public void OnClickSellBtn()
    {
        SceneStarter.Instance.userElements.SellItem(curType, sellAmount);
        gameObject.SetActive(false);
    }

    // 팝업창 비활성화
    public void OnClickCancelBtn()
    {
        gameObject.SetActive(false);
    }
}
