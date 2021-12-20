using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UnitSlot : MonoBehaviour
{
    public BattleReadyWindow    MasterBattle;   // 배틀 화면 오브젝트
    public EncyclopediaWindow   MasterEncy;     // 도감 오브젝트
    public TrainingWindow       MasterTrain;    // 훈련 오브젝트

    public int                  unitType;       // 유닛 타입
    public Image                icon;           // 유닛 아이콘
    public Image                tint;           // 유닛 틴트
    public Image                inactive;       // 비활성화 이미지
    public Image                overlay;        // 음영 오버레이

    public bool                 acquired;       // 획득 유무
    public bool                 selected;       // 선택된 유닛

    public TeamColor            TeamColor;      //애니메이션 쉐이더
    // 유닛 선택 화면의 아이콘 클릭
    public void OnClickBattleIcon()
    {
        if (SceneStarter.Instance.userElements.GetIsPossession(unitType))
        {
            // 선택 > 미선택
            if (selected)
            {
                if (GameManager.Instance.SetUnitType(unitType))
                {
                    selected = false;
                    overlay.gameObject.SetActive(true);
                    MasterBattle.ChangeDesc(unitType);
                    //  클릭 소리
                    SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.Deselect, 0, false);
                }
            }
            // 미선택 > 선택
            else
            {
                if (GameManager.Instance.SetUnitType(unitType))
                {
                    selected = true;
                    overlay.gameObject.SetActive(false);
                    MasterBattle.ChangeDesc(unitType);

                    //  클릭 소리
                    SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.Select, 0, false);
                }
            }
        }
    }

    // 도감 화면의 아이콘 클릭
    public void OnClickEncyIcon()
    {
        Debug.Log("도감 클릭 " + unitType.ToString());
        MasterEncy.ChangeDesc(unitType);
        TeamColor.ReLoad = true;

        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.Button, 0, false);
    }

    // 훈련 화면의 아이콘 클릭
    public void OnClickTrainIcon()
    {
        if (unitType < 15)
        {
            Debug.Log("훈련 클릭 " + unitType.ToString());
            MasterTrain.ChangeDesc(unitType);
            MasterTrain.ChangeReq(unitType);
            TeamColor.ReLoad = true;

            //  클릭 소리
            SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.Button, 0, false);
        }
    }
}
