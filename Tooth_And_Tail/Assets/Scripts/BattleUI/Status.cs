using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Status : MonoBehaviour
{
    public HPCanvas         MasterHPCanvas   = null;

    public Image            HealImage        = null;
    public Image            StimImage        = null;
    public Image            PoisonImage      = null;

    public TextMeshProUGUI  HealText         = null;
    public TextMeshProUGUI  StimText         = null;
    public TextMeshProUGUI  PoisonText       = null;
    
    // 초기화 함수
    public void Ready()
    {
        if (HealImage == null || StimImage == null || PoisonImage == null)
            return;
        SetColor();
    }

    // Base의 camp에 따라 색깔 설정
    void SetColor()
    {
        switch (MasterHPCanvas.Base.MyCamp)
        {
            case Camp.Bellafide:
                HealImage.color = new Color32(23, 114, 196, 255);
                StimImage.color = new Color32(23, 114, 196, 255);
                PoisonImage.color = new Color32(23, 114, 196, 255);
                break;
            case Camp.Hopper:
                HealImage.color = new Color32(164, 31, 31, 255);
                StimImage.color = new Color32(164, 31, 31, 255);
                PoisonImage.color = new Color32(164, 31, 31, 255);
                break;
            case Camp.Quartermaster:
                HealImage.color = new Color32(42, 162, 57, 255);
                StimImage.color = new Color32(42, 162, 57, 255);
                PoisonImage.color = new Color32(42, 162, 57, 255);
                break;
            case Camp.Archimedes:
                HealImage.color = new Color32(198, 174, 8, 255);
                StimImage.color = new Color32(198, 174, 8, 255);
                PoisonImage.color = new Color32(198, 174, 8, 255);
                break;
        }
    }
    
    void Update()
    {
        SetUIBuffDebuff();
    }
    
    // 힐,포이즌,스팀 버프 유무에 따른 UI 작동
    void SetUIBuffDebuff()
    {
        // 힐 스택이 존재하면 활성화 / 존재하지않으면 비활성화하며 알파값 낮춰둠
        if (MasterHPCanvas.character.HealStack > 0)
            HealImage.gameObject.SetActive(true);
        else
        {
            Color color = HealImage.color;
            color.a = 0f;
            HealImage.color = color;
            HealImage.gameObject.SetActive(false);
        }

        // 독 스택이 존재하면 활성화 / 존재하지않으면 비활성화하며 알파값 낮춰둠
        if (MasterHPCanvas.character.PoisonStack > 0)
            PoisonImage.gameObject.SetActive(true);
        else
        {
            Color color = PoisonImage.color;
            color.a = 0f;
            PoisonImage.color = color;
            PoisonImage.gameObject.SetActive(false);
        }

        // 스팀 버프가 존재하면 활성화 / 존재하지않으면 비활성화하며 알파값 낮춰둠
        if (MasterHPCanvas.character.Stim)
            StimImage.gameObject.SetActive(true);
        else
        {
            Color color = StimImage.color;
            color.a = 0f;
            StimImage.color = color;
            StimImage.gameObject.SetActive(false);
        }

        // 처음 나올때 서서히 보이는 효과위함
        // 힐 이미지 알파값이 낮을때 작동
        if (HealImage.color.a < 1)
        {
            Color color = HealImage.color;
            color.a += 0.1f;
            HealImage.color = color;
            
        }

        // 독 이미지 알파값이 낮을때 작동
        if (PoisonImage.color.a < 1)
        {
            Color color = PoisonImage.color;
            color.a += 0.1f;
            PoisonImage.color = color;
        }

        // 스팀 이미지 알파값이 낮을때 작동
        if (StimImage.color.a < 1)
        {
            Color color = StimImage.color;
            color.a += 0.1f;
            StimImage.color = color;
        }

        // 커져잇던 이미지를 점점 작아지는 부분
        // 각 이미지의 알파값에 따라 스케일 변경
        HealImage.transform.localScale = new Vector2(5, 5) - new Vector2(4,4) * HealImage.color.a;
        PoisonImage.transform.localScale = new Vector2(5, 5) - new Vector2(4, 4) * PoisonImage.color.a;
        StimImage.transform.localScale = new Vector2(5, 5) - new Vector2(4, 4) * StimImage.color.a;

        // 힐 텍스트 활성/비활성
        if (MasterHPCanvas.character.HealStack > 1)
        {
            HealText.gameObject.SetActive(true);
            HealText.text = "x" + MasterHPCanvas.character.HealStack.ToString();
        }
        else
            HealText.gameObject.SetActive(false);

        // 독 텍스트 활성/비활성
        if (MasterHPCanvas.character.PoisonStack > 1)
        {
            PoisonText.gameObject.SetActive(true);
            PoisonText.text = "x" + MasterHPCanvas.character.PoisonStack.ToString();
        }
        else
            PoisonText.gameObject.SetActive(false);
        
    }
}
