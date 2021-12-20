using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EncyPopup : MonoBehaviour
{
    public EncyclopediaWindow   Master;         // 도감 오브젝트
    public int                  curType;        // 현재 유닛 타입

    public Image                portrait;
    public TextMeshProUGUI      unitName;           // 유닛 이름
    public TextMeshProUGUI      nameShadow;     // 판매 금액


    // 팝업창 활성
    public void Open(int _type)
    {
        curType = _type;

        portrait.sprite = SceneStarter.Instance.uIElements.UIPortraitDic_S[(CommonType)curType];
        unitName.text = SceneStarter.Instance.commonElements.CommonDataList[curType].Name;
        nameShadow.text = unitName.text;
    }


    // 팝업창 비활성화
    public void IllustPopUpClose()
    {
        Master.SpriteRender.color = new Color(1, 1, 1, 1);
        Master.SpriteAni.enabled = true;
        gameObject.SetActive(false);
    }
}
