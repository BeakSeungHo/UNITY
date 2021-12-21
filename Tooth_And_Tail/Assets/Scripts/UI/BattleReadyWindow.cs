using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class BattleReadyWindow : MonoBehaviour
{
    public List<CmdSlot>        CmdList;
    public List<UnitSlot>       UnitList;

    // UnitDesc
    public TextMeshProUGUI      BuildCost;
    public TextMeshProUGUI      BuildTime;

    public TextMeshProUGUI      HireCost;
    public TextMeshProUGUI      UnitApply;
    public TextMeshProUGUI      Attack;
    public TextMeshProUGUI      HP;
    public TextMeshProUGUI      Sight;
    public TextMeshProUGUI      Range;

    // Portrait
    public TextMeshProUGUI      UnitLevel;
    public TextMeshProUGUI      UnitName;
    public SpriteRenderer       Portrait;           // 초상화
    public Image                PortraitTint;       // 초상화 틴트

    public int                  curCmdType = 0;     // 현재 커맨더
    public int                  curUnitType = 0;    // 현재 유닛
    public int                  curLevel;           // 현재 유닛 레벨

    // Stamina / Warning
    public TextMeshProUGUI      staminaNeed;
    public TextMeshProUGUI      warningMsg;
    public bool                 bMsgPlayed;
    public float                msgAlpha;
    public float                fadeSpeed;


    public GameMode             curGameMode;
    public TextMeshProUGUI      textGameMode;


    private void Update()
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

    void OnEnable()
    {
        // 경고 메세지 초기화
        fadeSpeed = 0.75f;
        bMsgPlayed = false;
        msgAlpha = 1.0f;
        warningMsg.gameObject.SetActive(false);


        var cmdTempList     = GetComponentsInChildren<CmdSlot>(true);
        var unitTempList    = GetComponentsInChildren<UnitSlot>(true);

        curGameMode         = GameMode.Multi;
        textGameMode.text   = "일반대전";

        staminaNeed.text    = "-" + Global.StaminaMulti.ToString();

        //유닛,커맨더 타입 리셋
        GameManager.Instance.ResetUnitList();
      
        // 커맨드 입력
        CmdList.Clear();
        int idx = 0;
        foreach (var data in cmdTempList)
        {
            data.MasterBattleReady = this;
            data.cmdType = idx;
            data.icon.sprite = SceneStarter.Instance.uIElements.UIComIconDic[(Camp)idx];

            CmdList.Add(data);
            idx++;
        }
        OnClickCmdIcon(0);
        // 유닛 입력
        UnitList.Clear();
        idx = 0;
        foreach (var data in unitTempList)
        {
            data.MasterBattle = this;
            data.unitType = idx;
            data.selected = false;
            data.icon.sprite = SceneStarter.Instance.uIElements.UIIconDic[(CommonType)idx];
            data.tint.sprite = SceneStarter.Instance.tintElements.TintDic[data.icon.sprite.name][0];
            data.tint.color = Global.CommanderInGameColorBellafide; ;   // 초기 색상

            // 획득 유닛
            if (SceneStarter.Instance.userElements.GetIsPossession(idx))
            //if (true)
            {
                data.acquired = true;

                data.icon.gameObject.SetActive(true);
                data.tint.gameObject.SetActive(true);

                data.inactive.gameObject.SetActive(false);
                data.overlay.gameObject.SetActive(true);
            }
            // 미획득 유닛
            else
            {
                data.acquired = false;

                data.icon.gameObject.SetActive(false);
                data.tint.gameObject.SetActive(false);

                data.inactive.gameObject.SetActive(true);
                data.overlay.gameObject.SetActive(true);
            }

            UnitList.Add(data);
            idx++;
        }

        // 초기 정보창
        ChangeDesc(0);
    }

    // 정보창 갱신
    public void ChangeDesc(int _type)
    {
        curUnitType = _type;

        // 동물 유닛
        if (curUnitType < 14)
        {
            curLevel = SceneStarter.Instance.userElements.GetLevel(curUnitType);
            ReinforceData curData = SceneStarter.Instance.reinforceElements.CompleteReinforceCurData(curUnitType, curLevel);

            // 초상화
            UnitLevel.text = "Lv." + curLevel.ToString();
            UnitName.text = SceneStarter.Instance.commonElements.CommonDataList[_type].Name;
            Portrait.sprite = SceneStarter.Instance.uIElements.UIPortraitDic_C[(CommonType)_type];
            PortraitTint.sprite = SceneStarter.Instance.tintElements.TintDic[Portrait.sprite.name][0];

            // 현재 레벨, 스탯
            BuildCost.text = curData.BuildCost.ToString();
            BuildTime.text = curData.GenTime.ToString();

            HireCost.text = curData.Cost.ToString();
            UnitApply.text = curData.UnitPerBuliding.ToString();
            Attack.text = curData.Damage.ToString();
            HP.text = curData.MaxHp.ToString();
            Sight.text = curData.Sight.ToString();
            Range.text = curData.Range.ToString();
        }
        // 건물 유닛
        else
        {
            // 초상화
            UnitLevel.text = "Lv.-";
            UnitName.text = SceneStarter.Instance.commonElements.CommonDataList[_type].Name;
            Portrait.sprite = SceneStarter.Instance.uIElements.UIPortraitDic_C[(CommonType)curUnitType];
            PortraitTint.sprite = SceneStarter.Instance.tintElements.TintDic[Portrait.sprite.name][0];

            // 현재 스탯
            BuildCost.text = SceneStarter.Instance.commonElements.CommonDataList[curUnitType].BuildCost.ToString();
            BuildTime.text = SceneStarter.Instance.commonElements.CommonDataList[curUnitType].GenTime.ToString();

            HireCost.text = SceneStarter.Instance.commonElements.CommonDataList[curUnitType].Cost.ToString();
            UnitApply.text = SceneStarter.Instance.commonElements.CommonDataList[curUnitType].UnitPerBuliding.ToString();
            Attack.text = SceneStarter.Instance.commonElements.CommonDataList[curUnitType].Damage.ToString();
            HP.text = SceneStarter.Instance.commonElements.CommonDataList[curUnitType].MaxHp.ToString();
            Sight.text = SceneStarter.Instance.commonElements.CommonDataList[curUnitType].Sight.ToString();
            Range.text = SceneStarter.Instance.commonElements.CommonDataList[curUnitType].Range.ToString();
        }
    }

    // 유닛 색상 변화
    public void ChangeColor()
    {
        switch (curCmdType)
        {
            case 0: // Bellafide
                GameManager.Instance.CommanderType = Camp.Bellafide;
                PortraitTint.color = Global.CommanderUIColorBellafide;    // 틴트 색상 변경

                foreach (var data in UnitList)
                    data.tint.color = Global.CommanderInGameColorBellafide;
                break;
            case 1: // Hoper
                GameManager.Instance.CommanderType = Camp.Hopper;
                PortraitTint.color = Global.CommanderUIColorHopper;    // 틴트 색상 변경

                foreach (var data in UnitList)
                    data.tint.color = Global.CommanderInGameColorHopper;
                break;
            case 2: // Quartermaster
                GameManager.Instance.CommanderType = Camp.Quartermaster;
                PortraitTint.color = Global.CommanderUIColorQuartermaster;    // 틴트 색상 변경

                foreach (var data in UnitList)
                    data.tint.color = Global.CommanderInGameColorQuartermaster;
                break;
            case 3: // Archimedes
                GameManager.Instance.CommanderType = Camp.Archimedes;
                PortraitTint.color = Global.CommanderUIColorArchimedes;    // 틴트 색상 변경

                foreach (var data in UnitList)
                    data.tint.color = Global.CommanderInGameColorArchimedes;
                break;
        }
    }


    // 커맨더 버튼 클릭
    public void OnClickCmdIcon(int _cmdType)
    {
        curCmdType = _cmdType;

        foreach (var data in CmdList)
            data.overlay.gameObject.SetActive(true);

        CmdList[curCmdType].overlay.gameObject.SetActive(false);
        ChangeColor();
    }

    // 게임 모드 변경
    public void OnClickChangeMode()
    {
        switch (curGameMode)
        {
            case GameMode.Multi:
                curGameMode = GameMode.TimeAttack;
                textGameMode.text = "빠른대전";
                break;

            case GameMode.TimeAttack:
                curGameMode = GameMode.Multi;
                textGameMode.text = "일반대전";
                break;
        }
    }

    // 게임 시작
    public void OnClickGameStart()
    {
        if (GameManager.Instance.CompleteList())
        {
            // 스태미나 부족
            if (SceneStarter.Instance.userElements.UserData.UserCurStamina < Global.StaminaMulti)
            {
                // 경고 메세지 활성화
                warningMsg.text = "탄환이 부족합니다";
                bMsgPlayed = true;
                msgAlpha = 1.0f;
                warningMsg.gameObject.SetActive(true);
            }
            else
            {
                SceneStarter.Instance.userElements.UserData.UserCurStamina -= Global.StaminaMulti;
                SceneStarter.Instance.userElements.AddMissionCount(MissionType.Daily, 2, Global.StaminaMulti);
                SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 4, Global.StaminaMulti);
                SceneStarter.Instance.userElements.AddMissionCount(MissionType.Weekly, 5, Global.StaminaMulti);
                GameManager.Instance.CurGameMode = curGameMode;
                SceneManager.LoadScene("BattleScene");
                SceneManager.LoadScene("BattleScene_UI", LoadSceneMode.Additive);
            }

            //  클릭 소리
            SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.Enter, 0, false);
        }
        else
        {
            // 경고 메세지 활성화
            warningMsg.text = "6마리의 유닛을 선택해주세요";
            bMsgPlayed = true;
            msgAlpha = 1.0f;
            warningMsg.gameObject.SetActive(true);
        }
    }
}
