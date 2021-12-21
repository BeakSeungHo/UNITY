using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class InventoryWindow : MonoBehaviour
{
    public List<InvenSlot>      SlotList;

    // ItemDesc
    public Image                BgImage;    
    public Image                ItemIcon;
    public Image                ItemIconTint;
    public TextMeshProUGUI      ItemName;
    public TextMeshProUGUI      ItemPrice;
    public TextMeshProUGUI      ItemSentence;   // 한줄 설명
    public TextMeshProUGUI      ItemDesc;       // 아이템 설명

    public InvenPopup           SellPopUp;      // 판매 팝업창
    public Button               ButtonUse;
    public Button               ButtonSell;

    public int                  curType = 0;

    [SerializeField]
    private ScrollRect          scrollRect;
    [SerializeField]
    private Transform           content;


    [ContextMenu("LinkSlot")]
    public void LinkSlot()
    {
        var slotTempList = GetComponentsInChildren<InvenSlot>(true);

        SlotList.Clear();
        int idx = 0;
        foreach (var data in slotTempList)
        {
            data.MasterInven = this;
            data.itemType = idx;

            SlotList.Add(data);
            idx++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        var slotTempList = GetComponentsInChildren<InvenSlot>(true);

        SlotList.Clear();
        int idx = 0;
        foreach (var data in slotTempList)
        {
            data.MasterInven = this;
            data.itemType = idx;
            data.icon.sprite = SceneStarter.Instance.userElements.ItemDataList[idx].ItemImg;
            if (null != SceneStarter.Instance.userElements.ItemDataList[idx].ItemImgTint)
            {
                data.iconTint.gameObject.SetActive(true);
                data.iconTint.sprite = SceneStarter.Instance.userElements.ItemDataList[idx].ItemImgTint;
                data.iconTint.color = Global.CommanderInGameColorBellafide;
            }
            else
                data.iconTint.gameObject.SetActive(false);
            data.countText.text = SceneStarter.Instance.userElements.GetItemCount((ItemType)idx).ToString();

            SlotList.Add(data);
            idx++;
        }
    }

    private void OnEnable()
    {
        scrollRect.verticalNormalizedPosition = 1.0f;
        SellPopUp.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInven();
    }

    // 인벤 데이터 갱신
    public void UpdateInven()
    {
        int idx = 0;
        foreach (var data in SlotList)
        {
            data.countText.text = SceneStarter.Instance.userElements.GetItemCount(idx).ToString();

            // 오브젝트 활성/비활성
            if (SceneStarter.Instance.userElements.IsItemEmpty(idx))
                content.GetChild(idx).gameObject.SetActive(false);
            else
                content.GetChild(idx).gameObject.SetActive(true);


            // 현재 아이템이 사라질 때
            if (SceneStarter.Instance.userElements.IsItemEmpty(curType))
                ResetDesc();

            idx++;
        }
    }

    // 정보창 갱신
    public void ChangeDesc(int _type)
    {
        curType = _type;

        BgImage.gameObject.SetActive(true);
        ItemIcon.gameObject.SetActive(true);
        ItemIcon.sprite = SceneStarter.Instance.userElements.ItemDataList[_type].ItemImg;
        if (null != SceneStarter.Instance.userElements.ItemDataList[_type].ItemImgTint)
        {
            ItemIconTint.gameObject.SetActive(true);
            ItemIconTint.sprite = SceneStarter.Instance.userElements.ItemDataList[_type].ItemImgTint;
            ItemIconTint.color = Global.CommanderInGameColorBellafide;
        }
        else
            ItemIconTint.gameObject.SetActive(false);

        ItemName.text = SceneStarter.Instance.userElements.ItemDataList[_type].ItemName;
        ItemPrice.text = SceneStarter.Instance.userElements.ItemDataList[_type].price.ToString() + "G";
        ItemSentence.text = SceneStarter.Instance.userElements.ItemDataList[_type].ItemSentence;
        ItemDesc.text = SceneStarter.Instance.userElements.ItemDataList[_type].ItemDesc;

        ButtonUse.gameObject.SetActive(true);
        ButtonSell.gameObject.SetActive(true);
    }

    // 정보창 초기화
    public void ResetDesc()
    {
        BgImage.gameObject.SetActive(false);
        ItemIcon.gameObject.SetActive(false);
        ItemIconTint.gameObject.SetActive(false);
        ItemName.text = "";
        ItemPrice.text = "";
        ItemSentence.text = "";
        ItemDesc.text = "";

        ButtonUse.gameObject.SetActive(false);
        ButtonSell.gameObject.SetActive(false);
    }

    // 아이템 사용
    public void OnClickUseBtn()
    {
        SceneStarter.Instance.userElements.UseItem(curType, 1);
    }


    // 팝업창 활성화
    public void SellPopUpOpen()
    {
        SellPopUp.gameObject.SetActive(true);
        SellPopUp.Open(curType);
    }
}
