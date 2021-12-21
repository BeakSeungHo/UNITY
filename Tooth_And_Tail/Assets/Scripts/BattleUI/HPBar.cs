using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPBar : MonoBehaviour
{
    public HPCanvas MasterHPCanvas = null;
    public TextMeshProUGUI HPText = null;
    public Image ProductImage = null;
    public BuildingBase buildingBase = null;
    public RectTransform rectTransform = null;
    public Image BackImage = null;
    public Image FillImage = null;

    public void Ready()
    {
        if (rectTransform == null || BackImage == null || FillImage == null)
            return;
        SetColor();
        SetSize();
    }

    public void SetCompleteFarmHP()
    {
        rectTransform.sizeDelta = new Vector2(30, 30);
        ProductImage.gameObject.SetActive(false);
    }

    // Type, 티어에 따라 HP바 크기 변경 및 Text 크기 변경
    void SetSize()
    {
        switch (MasterHPCanvas.Base.Type)
        {
            case CommonType.Squirrel:
            case CommonType.Lizard:
            case CommonType.Toad:
            case CommonType.Pigeon:
            case CommonType.Mole:
                rectTransform.sizeDelta = new Vector2(10, 10);
                HPText.gameObject.SetActive(false);
                break;
            case CommonType.Ferret:
            case CommonType.Falcon:
            case CommonType.Skunk:
            case CommonType.Chameleon:
            case CommonType.Snake:
                rectTransform.sizeDelta = new Vector2(30, 30);
                HPText.gameObject.SetActive(false);
                HPText.fontSize = 24;
                break;
            case CommonType.Boar:
            case CommonType.Badger:
            case CommonType.Owl:
            case CommonType.Wolf:
            case CommonType.Fox:
                rectTransform.sizeDelta = new Vector2(40, 40);
                HPText.gameObject.SetActive(false);
                HPText.fontSize = 28;
                break;
            case CommonType.Gristmill:
            case CommonType.Cabin:
            case CommonType.CampFire:
            case CommonType.Turret:
            case CommonType.Wire:
            case CommonType.Mine:
            case CommonType.Cannon:
                rectTransform.sizeDelta = new Vector2(50, 50);
                HPText.gameObject.SetActive(false);
                break;
            case CommonType.Balloon:
                rectTransform.sizeDelta = new Vector2(30, 30);
                HPText.gameObject.SetActive(false);
                break;
            case CommonType.WarrenT1:
            case CommonType.WarrenT2:
            case CommonType.WarrenT3:
            case CommonType.MoleeMerge:
                rectTransform.sizeDelta = new Vector2(100, 100);
                HPText.gameObject.SetActive(false);
                if (GameManager.Instance.CommanderList[0] == MasterHPCanvas.Base.MyCamp)
                {
                    ProductImage.gameObject.SetActive(true);
                    ProductImage.sprite = SceneStarter.Instance.uIElements.UIIconDic[buildingBase.Product_Type];
                    Color color = FillImage.color;
                    color.a = 0.5f;
                    FillImage.color = color;
                }
                break;
            case CommonType.Farm:
                rectTransform.sizeDelta = new Vector2(50, 50);
                HPText.gameObject.SetActive(false);
                if (GameManager.Instance.CommanderList[0] == MasterHPCanvas.Base.MyCamp)
                {
                    ProductImage.gameObject.SetActive(true);
                    ProductImage.sprite = SceneStarter.Instance.uIElements.UIIconDic[buildingBase.Product_Type];
                    ProductImage.gameObject.transform.localScale *= 0.8f;
                    Color color = FillImage.color;
                    color.a = 0.5f;
                    FillImage.color = color;
                }
                break;
        }
    }

    // 진영에 따라 HP 채워지는 Color값 변경
    void SetColor()
    {
        switch (MasterHPCanvas.Base.MyCamp)
        {
            case Camp.Bellafide:
                FillImage.color = new Color32(0, 69, 238, 255);
                break;
            case Camp.Hopper:
                FillImage.color = new Color32(204, 0, 0, 255);
                break;
            case Camp.Quartermaster:
                FillImage.color = new Color32(37, 123, 14, 255);
                break;
            case Camp.Archimedes:
                FillImage.color = new Color32(255, 180, 0, 255);
                break;
        }
    }

    void Update()
    {
        NonFlip();
        UpdateHp();
    }

    void NonFlip()
    {
        if (MasterHPCanvas.character.transform.localScale.x < 0)
        {
            Vector3 localScale = transform.parent.localScale;
            localScale.x = -Mathf.Abs(localScale.x);
            transform.parent.localScale = localScale;
        }
        else
        {
            Vector3 localScale = transform.parent.localScale;
            localScale.x = Mathf.Abs(localScale.x);
            transform.parent.localScale = localScale;
        }
    }

    void UpdateHp()
    {
        FillImage.fillAmount = MasterHPCanvas.character.HP / MasterHPCanvas.Base.GetData(MasterHPCanvas.Base.Type).MaxHp;
        if (FillImage.fillAmount == 1)
            FillImage.gameObject.SetActive(false);
        else
            FillImage.gameObject.SetActive(true);
        HPText.text = ((int)MasterHPCanvas.character.HP).ToString();
    }
}
