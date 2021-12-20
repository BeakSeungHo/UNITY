using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

/// <summary>
/// 
/// Name : 백승호
///  Date : 2020-09-23
///  
///  Desc.
///     UI 아이콘,초상화 등의 UI Element들을 관리하는 스크립트
///     
/// </summary>

public enum UIType { None, Bullet, Dia2, Gold2, Rallyall_BR, Rallyall_GY, Rallygroup_BR, Rallygroup_GY }

public class UIElements : ScriptableObject
{
    //아이콘과 초상화를 저장할 Dictionary
    public Dictionary<Camp, Sprite> UIComIconDic             = new Dictionary<Camp, Sprite>();
    public Dictionary<Camp, Sprite> UIComPortraitDic         = new Dictionary<Camp, Sprite>();
    public Dictionary<Camp, Sprite> UIComPortraitDic_L       = new Dictionary<Camp, Sprite>();
    public Dictionary<Camp, Sprite> UIComPortraitDic_S       = new Dictionary<Camp, Sprite>();
    public Dictionary<Camp, Sprite> UIWinImageDic            = new Dictionary<Camp, Sprite>();
    public Dictionary<Camp, Sprite> UILoseImageDic           = new Dictionary<Camp, Sprite>();
    public Dictionary<CommonType, Sprite> UIIconDic          = new Dictionary<CommonType, Sprite>();
    public Dictionary<CommonType, Sprite> UIPortraitDic_C    = new Dictionary<CommonType, Sprite>();
    public Dictionary<CommonType, Sprite> UIPortraitDic_S    = new Dictionary<CommonType, Sprite>();
    public Dictionary<UIType, Sprite> UITypeIconDic          = new Dictionary<UIType, Sprite>();

    public void InitializeElement()
    {
        // 아이콘과 초상화 로드
        Sprite[] UIIconSprite = Resources.LoadAll<Sprite>("Textures/UI/Icon");
        Sprite[] UIBattleSprite = Resources.LoadAll<Sprite>("Textures/UI/Battle");
        Sprite[] UIPortraitSprite = Resources.LoadAll<Sprite>("Textures/UI/Portrait/Com");
        Sprite[] UIPortraitSprite_C = Resources.LoadAll<Sprite>("Textures/UI/Portrait/Cropped");
        Sprite[] UIPortraitSprite_S = Resources.LoadAll<Sprite>("Textures/UI/Portrait/Small");
        Sprite[] UIWinImageSprite = Resources.LoadAll<Sprite>("Textures/UI/Battle/win");
        Sprite[] UILoseImageSprite = Resources.LoadAll<Sprite>("Textures/UI/Battle/lose");


        // string의 앞부분을 대문자로 바꾸기위한 TextInfo
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

        string strkey;
        // 아이콘 스프라이트를 Dictionary에 저장
        for (int i = 0; i < UIIconSprite.Length; ++i)
        {
            strkey = UIIconSprite[i].name.Substring(UIIconSprite[i].name.LastIndexOf('_')+1, UIIconSprite[i].name.Length - UIIconSprite[i].name.LastIndexOf('_') -1);
            strkey = textInfo.ToTitleCase(strkey);
            
            if (!UIIconDic.ContainsKey((CommonType)strkey.ToEnum<CommonType>()) && 
                !strkey.ToEnum<CommonType>().Equals((int)CommonType.Error))
                UIIconDic.Add((CommonType)strkey.ToEnum<CommonType>(), UIIconSprite[i]);
            else if (!UITypeIconDic.ContainsKey((UIType)strkey.ToEnum<UIType>()) &&
                !strkey.ToEnum<UIType>().Equals(-1))
                UITypeIconDic.Add((UIType)strkey.ToEnum<UIType>(), UIIconSprite[i]);
            else if(!UIComIconDic.ContainsKey((Camp)strkey.ToEnum<Camp>()) && !strkey.ToEnum<Camp>().Equals((int)Camp.Error))
                UIComIconDic.Add((Camp)strkey.ToEnum<Camp>(), UIIconSprite[i]);
        }

        for (int i = 0; i < UIBattleSprite.Length; ++i)
        {
            strkey = UIBattleSprite[i].name.Substring(UIBattleSprite[i].name.IndexOf('_') + 1, UIBattleSprite[i].name.Length - UIBattleSprite[i].name.IndexOf('_') - 1);
            strkey = textInfo.ToTitleCase(strkey);
            
            if (!UITypeIconDic.ContainsKey((UIType)strkey.ToEnum<UIType>()) &&
                !strkey.ToEnum<UIType>().Equals(-1))
                UITypeIconDic.Add((UIType)strkey.ToEnum<UIType>(), UIBattleSprite[i]);
        }

        // 초상화 스프라이트를 Dictionary에 저장
        for (int i = 0; i < UIPortraitSprite.Length; ++i)
        {
            strkey = UIPortraitSprite[i].name.Substring(UIPortraitSprite[i].name.LastIndexOf('_')+1, UIPortraitSprite[i].name.Length - UIPortraitSprite[i].name.LastIndexOf('_') -1);
            strkey = textInfo.ToTitleCase(strkey);

            // 초상화(ver.Lock)스프라이트를 Dictionary에 저장
            if ("Lock" == strkey)
            {
                strkey = UIPortraitSprite[i].name.Substring(UIPortraitSprite[i].name.IndexOf('_')+1, UIPortraitSprite[i].name.LastIndexOf('_') - UIPortraitSprite[i].name.IndexOf('_') -1);
                strkey = textInfo.ToTitleCase(strkey);
                
                if (!UIComPortraitDic_L.ContainsKey((Camp)strkey.ToEnum<Camp>()) && !strkey.ToEnum<Camp>().Equals((int)Camp.Error))
                    UIComPortraitDic_L.Add((Camp)strkey.ToEnum<Camp>(), UIPortraitSprite[i]);
            }
            //// 초상화(ver.Small) 스프라이트를 Dictionary에 저장
            else if ("Small" == strkey)
            {
                strkey = UIPortraitSprite[i].name.Substring(UIPortraitSprite[i].name.IndexOf('_')+1, UIPortraitSprite[i].name.LastIndexOf('_') - UIPortraitSprite[i].name.IndexOf('_') -1);
                strkey = textInfo.ToTitleCase(strkey);
                
                if (!UIComPortraitDic_S.ContainsKey((Camp)strkey.ToEnum<Camp>()) && !strkey.ToEnum<Camp>().Equals((int)Camp.Error))
                    UIComPortraitDic_S.Add((Camp)strkey.ToEnum<Camp>(), UIPortraitSprite[i]);
            }
            else
            {
               if (!UIComPortraitDic.ContainsKey((Camp)strkey.ToEnum<Camp>()) && !strkey.ToEnum<Camp>().Equals((int)Camp.Error))
                    UIComPortraitDic.Add((Camp)strkey.ToEnum<Camp>(), UIPortraitSprite[i]);
            }
        }

        // 초상화(ver.Cropped) 스프라이트를 Dictionary에 저장
        for (int i = 0; i < UIPortraitSprite_C.Length; ++i)
        {
            strkey = UIPortraitSprite_C[i].name.Substring(0, UIPortraitSprite_S[i].name.LastIndexOf('_'));
            strkey = textInfo.ToTitleCase(strkey);
            
           if (!UIPortraitDic_C.ContainsKey((CommonType)strkey.ToEnum<CommonType>()) && !strkey.ToEnum<CommonType>().Equals((int)CommonType.Error))
                UIPortraitDic_C.Add((CommonType)strkey.ToEnum<CommonType>(), UIPortraitSprite_C[i]);
            
        }

        // 초상화(ver.Small) 스프라이트를 Dictionary에 저장
        for (int i = 0; i < UIPortraitSprite_S.Length; ++i)
        {
            strkey = UIPortraitSprite_S[i].name.Substring(0, UIPortraitSprite_S[i].name.LastIndexOf('_'));
            strkey = textInfo.ToTitleCase(strkey);
            
            if (!UIPortraitDic_S.ContainsKey((CommonType)strkey.ToEnum<CommonType>()) && !strkey.ToEnum<CommonType>().Equals((int)CommonType.Error))
                UIPortraitDic_S.Add((CommonType)strkey.ToEnum<CommonType>(), UIPortraitSprite_S[i]);
        }

        // 승리 이미지
        for(int i = 0; i < UIWinImageSprite.Length; ++i)
        {
            strkey = UIWinImageSprite[i].name.Substring(UIWinImageSprite[i].name.IndexOf('_') + 1, UIWinImageSprite[i].name.Length - UIWinImageSprite[i].name.IndexOf('_')-1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!UIWinImageDic.ContainsKey((Camp)strkey.ToEnum<Camp>()) && !strkey.ToEnum<Camp>().Equals((int)Camp.Error))
                UIWinImageDic.Add((Camp)strkey.ToEnum<Camp>(), UIWinImageSprite[i]);
        }

        // 패배 이미지
        for (int i = 0; i < UILoseImageSprite.Length; ++i)
        {
            strkey = UILoseImageSprite[i].name.Substring(UILoseImageSprite[i].name.IndexOf('_') + 1, UILoseImageSprite[i].name.Length - UILoseImageSprite[i].name.IndexOf('_')-1);
            strkey = textInfo.ToTitleCase(strkey);

            if (!UILoseImageDic.ContainsKey((Camp)strkey.ToEnum<Camp>()) && !strkey.ToEnum<Camp>().Equals((int)Camp.Error))
                UILoseImageDic.Add((Camp)strkey.ToEnum<Camp>(), UILoseImageSprite[i]);
        }
    }
}
