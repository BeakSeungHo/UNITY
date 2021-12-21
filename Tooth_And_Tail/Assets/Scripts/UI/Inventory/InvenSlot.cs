using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InvenSlot : MonoBehaviour
{
    public InventoryWindow      MasterInven;    // 인벤토리 오브젝트
    public TrainingWindow       MasterTrain;    // 훈련 오브젝트

    public int                  itemType;       // 아이템 타입
    public Image                bgImage;        // 배경 이미지
    public Image                icon;           // 아이템 아이콘 이미지
    public Image                iconTint;       // 아이템 아이콘 틴트
    public TextMeshProUGUI      countText;      // 아이템 개수


    public void OnClickIcon()
    {
        if (MasterInven != null)
            MasterInven.ChangeDesc(itemType);
    }
}
