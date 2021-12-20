using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UITest : MonoBehaviour
{
    // Start is called before the first frame update

    // Image는 using UnityEngine.UI 필요함
    Image test = null;

    void Start()
    {
        test = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //// UIIconDic = 유닛아이콘을 모아둔 Dictionary
        //SceneStarter.Instance.uIElements.UIIconDic[CommonType.캐릭터명]

        //// UIPortraitDic_C = 유닛초상화(Cropped)를 모아둔 Dictionary
        //SceneStarter.Instance.uIElements.UIPortraitDic_C[CommonType.캐릭터명]

        //// UIPortraitDic_S = 유닛초상화(Small)를 모아둔 Dictionary
        //SceneStarter.Instance.uIElements.UIPortraitDic_S[CommonType.캐릭터명]

        //// UIComIconDic = 커맨더아이콘을 모아둔 Dictionary
        //SceneStarter.Instance.uIElements.UIComIconDic[Camp.커맨더]

        //// UIComPortraitDic = 커맨더초상화를 모아둔 Dictionary
        //SceneStarter.Instance.uIElements.UIComPortraitDic[Camp.커맨더]

        //// UIComPortraitDic_L = 커맨더초상화(Lock)를 모아둔 Dictionary
        //SceneStarter.Instance.uIElements.UIComPortraitDic_L[Camp.커맨더]

        //// UIComPortraitDic_S = 커맨더초상화(Small)를 모아둔 Dictionary
        //SceneStarter.Instance.uIElements.UIComPortraitDic_S[Camp.커맨더]

        // 이미지파일의 sprite에 SceneStarter.Instance.uIElements.해당 디렉토리[키값] 으로 Sprite을 넣음
        test.sprite = SceneStarter.Instance.uIElements.UIIconDic[CommonType.Badger];
    }
}
