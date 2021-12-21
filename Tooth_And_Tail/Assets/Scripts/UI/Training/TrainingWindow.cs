using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TrainingWindow : MonoBehaviour
{
    public List<UnitSlot>       unitList;       // 유닛 리스트
    public List<InvenSlot>      invenList;      // 인벤토리 리스트
    public List<TrainReqSlot>   reqList;        // 강화 필요재료 리스트


    public TextMeshProUGUI      goldNeed;

    // UnitDesc
    public TextMeshProUGUI      UnitName;
    public TextMeshProUGUI      UnitLevel;
    public TextMeshProUGUI      BuildCost;
    public TextMeshProUGUI      BuildTime;

    public TrainStatDesc        HireCost;
    public TrainStatDesc        UnitApply;
    public TrainStatDesc        Attack;
    public TrainStatDesc        HP;
    public TrainStatDesc        Sight;
    public TrainStatDesc        Range;

    // LevelUp Message
    public Image                LevelUpMsg;
    [SerializeField]
    private TextMeshProUGUI     msgText;
    private bool                bMsgOn;
    private float               msgAlpha;
    public float                fadeSpeed;
    public Vector3              moveStep;

    public Vector3 pos;

    public int                  curType = 0;    // 현재 유닛
    public int                  curLevel = 0;   // 현재 유닛 레벨
    public int                  curAni = 0;     // 현재 애니메이션

    [SerializeField]
    private ScrollRect          scrollRect;
    [SerializeField]
    private Transform           content;

    public Animator             SpriteAni;
    public RectTransform        SpriteRect;
    private Vector2             SpriteOffset = Vector2.zero;


    // Start is called before the first frame update
    void Start()
    {
        var unitTempList = GetComponentsInChildren<UnitSlot>(true);
        var invenTempList = GetComponentsInChildren<InvenSlot>(true);
        var reqTempList = GetComponentsInChildren<TrainReqSlot>(true);

        // 유닛 로드
        unitList.Clear();
        int idx = 0;
        foreach (var data in unitTempList)
        {
            data.MasterTrain = this;
            data.unitType = idx;
            data.icon.sprite = SceneStarter.Instance.uIElements.UIIconDic[(CommonType)idx];
            data.tint.sprite = SceneStarter.Instance.tintElements.TintDic[data.icon.sprite.name][0];
            data.tint.color = Global.CommanderInGameColorBellafide;

            // 획득 유닛
            //if (SceneStarter.Instance.userElements.GetIsPossession(idx))
            if (true)
            {
                data.acquired = true;

                data.icon.gameObject.SetActive(true);
                data.tint.gameObject.SetActive(true);

                data.inactive.gameObject.SetActive(false);
                data.overlay.gameObject.SetActive(false);
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

            // 건물 유닛은 강화 불가
            if (15 <= idx)
                data.overlay.gameObject.SetActive(true);

            unitList.Add(data);
            idx++;
        }

        // 인벤 로드
        invenList.Clear();
        idx = 0;
        foreach (var data in invenTempList)
        {
            data.MasterTrain = this;
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

            invenList.Add(data);
            idx++;
        }

        // 강화재료
        reqList.Clear();
        idx = 0;
        foreach (var data in reqTempList)
        {
            data.Master = this;
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
            data.number.text = SceneStarter.Instance.userElements.GetItemCount((ItemType)idx).ToString();

            data.gameObject.SetActive(false);

            reqList.Add(data);
            idx++;
        }
    }

    private void OnEnable()
    {
        scrollRect.verticalNormalizedPosition = 1.0f;

        bMsgOn = false;
        LevelUpMsg.gameObject.SetActive(false);

        LevelUpMsg.GetComponent<RectTransform>().localPosition = new Vector2(0, -430);
        LevelUpMsg.color = new Color(1f, 1f, 1f, 0.8f);
        msgText.color = new Color(0.934f, 0.689f, 0.0749f, 1f);

        msgAlpha = 1f;
        fadeSpeed = 0.5f;
        moveStep = new Vector3(0f, fadeSpeed, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        pos = LevelUpMsg.GetComponent<RectTransform>().localPosition;
        UpdateInven();

        ChangeDesc(curType);
        ChangeReq(curType);

        if (bMsgOn)
        {
            msgAlpha -= Time.deltaTime * fadeSpeed;

            LevelUpMsg.color = new Color(1f, 1f, 1f, msgAlpha);
            msgText.color = new Color(0.934f, 0.689f, 0.0749f, msgAlpha);

            if (-425 > LevelUpMsg.GetComponent<RectTransform>().localPosition.y)
                LevelUpMsg.GetComponent<RectTransform>().localPosition += moveStep;
            else
                fadeSpeed += 0.2f;

            if (0 > msgAlpha)
            {
                bMsgOn = false;
                msgAlpha = 1f;
                fadeSpeed = 0.5f;

                LevelUpMsg.gameObject.SetActive(false);
                LevelUpMsg.GetComponent<RectTransform>().localPosition = new Vector2(0, -430);
                LevelUpMsg.color = new Color(1f, 1f, 1f, 0.8f);
                msgText.color = new Color(0.934f, 0.689f, 0.0749f, 1f);
            }
        }
    }

    // 인벤 데이터 갱신
    public void UpdateInven()
    {
        int idx = 0;
        foreach (var data in invenList)
        {
            data.countText.text = SceneStarter.Instance.userElements.GetItemCount(idx).ToString();

            // 오브젝트 활성/비활성
            // TrainingWindow-IngredientArea-Inventory-ScrollView-Viewport-Content
            if (SceneStarter.Instance.userElements.IsItemEmpty(idx))
                content.GetChild(idx).gameObject.SetActive(false);
            else
                content.GetChild(idx).gameObject.SetActive(true);

            idx++;
        }
    }

    // 정보창 갱신
    public void ChangeDesc(int _type)
    {
        // 획득 유닛만 변화
        //if (SceneStarter.Instance.userElements.GetIsPossession(_type))
        if (true)
        {
            curType = _type;
            UpdateAnimator(_type);
            if (curType != _type)
                curAni = 0;

            
            UnitName.text = SceneStarter.Instance.commonElements.CommonDataList[curType].Name;
            curLevel = SceneStarter.Instance.userElements.GetLevel(curType);
            if (10 == curLevel)
                UnitLevel.text = "Lv.10 (Max)";
            else
                UnitLevel.text = "Lv." + curLevel.ToString();

            ReinforceData curData = SceneStarter.Instance.reinforceElements.CompleteReinforceCurData(curType, curLevel);
            ReinforceData nxtData = SceneStarter.Instance.reinforceElements.GetReinforceNextData(curType, curLevel);
            bool upCheck = SceneStarter.Instance.reinforceElements.LevelUpCheck(curLevel, curType);  // 레벨업 가능 여부

            // 현재 레벨 스탯
            HireCost.number.text    = curData.Cost.ToString();
            UnitApply.number.text   = curData.UnitPerBuliding.ToString();
            Attack.number.text      = curData.Damage.ToString();
            HP.number.text          = curData.MaxHp.ToString();
            Sight.number.text       = curData.Sight.ToString();
            Range.number.text       = curData.Range.ToString();

            // 다음 레벨 상승폭 (레벨업 가능할 때)
            if (upCheck)
            {
                // HireCost
                if (0 != nxtData.Cost)
                    HireCost.change.text = "(-" + nxtData.Cost.ToString() + ")";
                else
                    HireCost.change.text = "";
                HireCost.change.color = new Color32(255, 226, 0, 255);

                // UnitApply
                if (0 != nxtData.UnitPerBuliding)
                    UnitApply.change.text = "(+" + nxtData.UnitPerBuliding.ToString() + ")";
                else
                    UnitApply.change.text = "";
                UnitApply.change.color = new Color32(255, 226, 0, 255);

                // Attack
                if (0 != nxtData.Damage)
                    Attack.change.text = "(+" + nxtData.Damage.ToString() + ")";
                else
                    Attack.change.text = "";
                Attack.change.color = new Color32(255, 226, 0, 255);

                // HP
                if (0 != nxtData.MaxHp)
                    HP.change.text = "(+" + nxtData.MaxHp.ToString() + ")";
                else
                    HP.change.text = "";
                HP.change.color = new Color32(255, 226, 0, 255);

                // Sight
                if (0 != nxtData.Sight)
                    Sight.change.text = "(+" + nxtData.Sight.ToString() + ")";
                else
                    Sight.change.text = "";
                Sight.change.color = new Color32(255, 226, 0, 255);

                // Range
                if (0 != nxtData.Range)
                    Range.change.text = "(+" + nxtData.Range.ToString() + ")";
                else
                    Range.change.text = "";
                Range.change.color = new Color32(255, 226, 0, 255);
            }
            else
            {
                // HireCost
                if (0 != nxtData.Cost)
                    HireCost.change.text = "(+" + nxtData.Cost.ToString() + ")";
                else
                    HireCost.change.text = "";
                HireCost.change.color = new Color32(223, 53, 0, 255);

                // UnitApply
                if (0 != nxtData.UnitPerBuliding)
                    UnitApply.change.text = "(+" + nxtData.UnitPerBuliding.ToString() + ")";
                else
                    UnitApply.change.text = "";
                UnitApply.change.color = new Color32(223, 53, 0, 255);

                // Attack
                if (0 != nxtData.Damage)
                    Attack.change.text = "(+" + nxtData.Damage.ToString() + ")";
                else
                    Attack.change.text = "";
                Attack.change.color = new Color32(223, 53, 0, 255);

                // HP
                if (0 != nxtData.MaxHp)
                    HP.change.text = "(+" + nxtData.MaxHp.ToString() + ")";
                else
                    HP.change.text = "";
                HP.change.color = new Color32(223, 53, 0, 255);

                // Sight
                if (0 != nxtData.Sight)
                    Sight.change.text = "(+" + nxtData.Sight.ToString() + ")";
                else
                    Sight.change.text = "";
                Sight.change.color = new Color32(223, 53, 0, 255);

                // Range
                if (0 != nxtData.Range)
                    Range.change.text = "(+" + nxtData.Range.ToString() + ")";
                else
                    Range.change.text = "";
                Range.change.color = new Color32(223, 53, 0, 255);
            }
        }
    }

    // 재료창 갱신
    public void ChangeReq(int _type)
    {
        // 획득 유닛
        //if (SceneStarter.Instance.userElements.GetIsPossession(_type))
        if (true)
        {
            curType = _type;

            ReinforceData nxtData = SceneStarter.Instance.reinforceElements.GetReinforceNextData(curType, curLevel);


            // Gold
            goldNeed.text = nxtData.Gold.ToString();

            // CheeseLow
            if (0 != nxtData.CheeseLowCount)
            {
                reqList[1].gameObject.SetActive(true);
                reqList[1].number.text = SceneStarter.Instance.userElements.GetItemCount(1).ToString() + "/" + nxtData.CheeseLowCount.ToString();
                reqList[1].number.color = new Color(1, 1, 1);

                if (nxtData.CheeseLowCount > SceneStarter.Instance.userElements.GetItemCount(1))
                    reqList[1].number.color = new Color(1, 0, 0);
                else
                    reqList[1].number.text = nxtData.CheeseLowCount.ToString() + "/" + nxtData.CheeseLowCount.ToString();
            }
            else
                reqList[1].gameObject.SetActive(false);

            // CheeseMed
            if (0 != nxtData.CheeseMedCount)
            {
                reqList[2].gameObject.SetActive(true);
                reqList[2].number.text = SceneStarter.Instance.userElements.GetItemCount(2).ToString() + "/" + nxtData.CheeseMedCount.ToString();
                reqList[2].number.color = new Color(1, 1, 1);

                if (nxtData.CheeseMedCount > SceneStarter.Instance.userElements.GetItemCount(2))
                    reqList[2].number.color = new Color(1, 0, 0);
                else
                    reqList[2].number.text = nxtData.CheeseMedCount.ToString() + "/" + nxtData.CheeseMedCount.ToString();
            }
            else        
                reqList[2].gameObject.SetActive(false);

            // CheeseHigh
            if (0 != nxtData.CheeseHighCount)
            {
                reqList[3].gameObject.SetActive(true);
                reqList[3].number.text = SceneStarter.Instance.userElements.GetItemCount(3).ToString() + "/" + nxtData.CheeseHighCount.ToString();
                reqList[3].number.color = new Color(1, 1, 1);

                if (nxtData.CheeseHighCount > SceneStarter.Instance.userElements.GetItemCount(3))
                    reqList[3].number.color = new Color(1, 0, 0);
                else
                    reqList[3].number.text = nxtData.CheeseHighCount.ToString() + "/" + nxtData.CheeseHighCount.ToString();
            }
            else        
                reqList[3].gameObject.SetActive(false);

            // WineLow
            if (0 != nxtData.WineLowCount)
            {
                reqList[4].gameObject.SetActive(true);
                reqList[4].number.text = SceneStarter.Instance.userElements.GetItemCount(4).ToString() + "/" + nxtData.WineLowCount.ToString();
                reqList[4].number.color = new Color(1, 1, 1);

                if (nxtData.WineLowCount > SceneStarter.Instance.userElements.GetItemCount(4))
                    reqList[4].number.color = new Color(1, 0, 0);
                else
                    reqList[4].number.text = nxtData.WineLowCount.ToString() + "/" + nxtData.WineLowCount.ToString();
            }
            else        
                reqList[4].gameObject.SetActive(false);

            // WineMed
            if (0 != nxtData.WineMedCount)
            {
                reqList[5].gameObject.SetActive(true);
                reqList[5].number.text = SceneStarter.Instance.userElements.GetItemCount(5).ToString() + "/" + nxtData.WineMedCount.ToString();
                reqList[5].number.color = new Color(1, 1, 1);

                if (nxtData.WineMedCount > SceneStarter.Instance.userElements.GetItemCount(5))
                    reqList[5].number.color = new Color(1, 0, 0);
                else
                    reqList[5].number.text = nxtData.WineMedCount.ToString() + "/" + nxtData.WineMedCount.ToString();
            }
            else      
                reqList[5].gameObject.SetActive(false);

            // WineHigh
            if (0 != nxtData.WineHighCount)
            {
                reqList[6].gameObject.SetActive(true);
                reqList[6].number.text = SceneStarter.Instance.userElements.GetItemCount(6).ToString() + "/" + nxtData.WineHighCount.ToString();
                reqList[6].number.color = new Color(1, 1, 1);

                if (nxtData.WineHighCount > SceneStarter.Instance.userElements.GetItemCount(6))
                    reqList[6].number.color = new Color(1, 0, 0);
                else
                    reqList[6].number.text = nxtData.WineHighCount.ToString() + "/" + nxtData.WineHighCount.ToString();
            }
            else
                reqList[6].gameObject.SetActive(false);

            // MeatLow
            if (0 != nxtData.MeatLowCount)
            {
                reqList[7].gameObject.SetActive(true);
                reqList[7].number.text = SceneStarter.Instance.userElements.GetItemCount(7).ToString() + "/" + nxtData.MeatLowCount.ToString();
                reqList[7].number.color = new Color(1, 1, 1);

                if (nxtData.MeatLowCount > SceneStarter.Instance.userElements.GetItemCount(7))
                    reqList[7].number.color = new Color(1, 0, 0);
                else
                    reqList[7].number.text = nxtData.MeatLowCount.ToString() + "/" + nxtData.MeatLowCount.ToString();
            }
            else       
                reqList[7].gameObject.SetActive(false);

            // MeatMed
            if (0 != nxtData.MeatMedCount)
            {
                reqList[8].gameObject.SetActive(true);
                reqList[8].number.text = SceneStarter.Instance.userElements.GetItemCount(8).ToString() + "/" + nxtData.MeatMedCount.ToString();
                reqList[8].number.color = new Color(1, 1, 1);

                if (nxtData.MeatMedCount > SceneStarter.Instance.userElements.GetItemCount(8))
                    reqList[8].number.color = new Color(1, 0, 0);
                else
                    reqList[8].number.text = nxtData.MeatMedCount.ToString() + "/" + nxtData.MeatMedCount.ToString();
            }
            else       
                reqList[8].gameObject.SetActive(false);

            // MeatHigh
            if (0 != nxtData.MeatHighCount)
            {
                reqList[9].gameObject.SetActive(true);
                reqList[9].number.text = SceneStarter.Instance.userElements.GetItemCount(9).ToString() + "/" + nxtData.MeatHighCount.ToString();
                reqList[9].number.color = new Color(1, 1, 1);

                if (nxtData.MeatHighCount > SceneStarter.Instance.userElements.GetItemCount(9))
                    reqList[9].number.color = new Color(1, 0, 0);
                else
                    reqList[9].number.text = nxtData.MeatHighCount.ToString() + "/" + nxtData.MeatHighCount.ToString();
            }
            else      
                reqList[9].gameObject.SetActive(false);


            // UnitCore
            if (0 != nxtData.CoreCount)
            {
                reqList[10].gameObject.SetActive(true);
                reqList[10].icon.sprite = SceneStarter.Instance.userElements.ItemDataList[10 + curType].ItemImg;
                reqList[10].number.text = SceneStarter.Instance.userElements.GetItemCount(10 + curType).ToString() + "/" + nxtData.CoreCount.ToString();
                reqList[10].number.color = new Color(1, 1, 1);

                if (nxtData.CoreCount > SceneStarter.Instance.userElements.GetItemCount(10 + curType))
                    reqList[10].number.color = new Color(1, 0, 0);
                else
                    reqList[10].number.text = nxtData.CoreCount.ToString() + "/" + nxtData.CoreCount.ToString();
            }
            else
                reqList[10].gameObject.SetActive(false);
        }
    }


    // 강화 실행
    public void OnClickTrainBtn()
    {
        if (SceneStarter.Instance.reinforceElements.LevelUpCheck(curLevel, curType))
        {
            SceneStarter.Instance.reinforceElements.LevelUp(curLevel, curType);
            ChangeDesc(curType);

            if (10 == curLevel)
                msgText.text = "Max Level!";
            else
                msgText.text = "Level Up!";

            // LevelUp Message On
            bMsgOn = true;
            LevelUpMsg.gameObject.SetActive(true);

            // 강화 소리
            //SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.Rankup, 0, false);

            //  실행한 소리 변수
            Sound sound = null;
            //  오디오 클립 받아오기
            var audios = SceneStarter.Instance.soundElements.LobbySoundDic[LobbySoundType.Rankup];
            int index = 0;
            //  소리 실행
            SoundManager.Instance.Play(Sound_Channel.UI, Camera.main.gameObject, audios[index], out sound, false);
            //  소리 볼륨 조절
            sound.Set_Volume(0.4f);


            msgAlpha = 1f;
            LevelUpMsg.color = new Color(1f, 1f, 1f, msgAlpha);
            msgText.color = new Color(0.934f, 0.689f, 0.0749f, msgAlpha);
            LevelUpMsg.GetComponent<RectTransform>().localPosition = new Vector2(0, -430);
        }
    }

    // 유닛타입에 따라 애니메이터를 가져오고 위치값을 변경하는 함수
    public void UpdateAnimator(int _type)
    {
        // 유닛타입에 따라 애니메이터 가져옴
        if (_type >= 15)
            SpriteAni.runtimeAnimatorController = SceneStarter.Instance.animatorElements.BuildAniDic[(CommonType)_type];
        else
            SpriteAni.runtimeAnimatorController = SceneStarter.Instance.animatorElements.UnitAniDic[(CommonType)_type];

        CommonType type = (CommonType)_type;
        // 유닛 타입에 따라 위치값 조정
        switch (type)
        {
            case CommonType.Pigeon:
            case CommonType.Falcon:
            case CommonType.Mouse:
            case CommonType.Squirrel:
            case CommonType.Lizard:
            case CommonType.Toad:
            case CommonType.Mole:
            case CommonType.Ferret:
            case CommonType.Skunk:
            case CommonType.Snake:
            case CommonType.Boar:
            case CommonType.Badger:
            case CommonType.Wolf:
            case CommonType.Chameleon:
                SpriteOffset = new Vector2(-15, 160);
                break;
            case CommonType.Owl:
                SpriteOffset = new Vector2(-15, 150);
                break;
            case CommonType.Fox:
                SpriteOffset = new Vector2(-15, 210);
                break;
        }
        SpriteRect.anchoredPosition = SpriteOffset;

        switch (type)
        {
            case CommonType.Pigeon:
            case CommonType.Falcon:
            case CommonType.Mouse:
            case CommonType.Squirrel:
            case CommonType.Lizard:
            case CommonType.Toad:
            case CommonType.Mole:
            case CommonType.Ferret:
            case CommonType.Skunk:
            case CommonType.Snake:
            case CommonType.Chameleon:
                SpriteRect.localScale = new Vector2(500, 500);
                break;
            case CommonType.Owl:
            case CommonType.Fox:
            case CommonType.Boar:
            case CommonType.Badger:
            case CommonType.Wolf:
                SpriteRect.localScale = new Vector2(400, 400);
                break;
        }
        
        switch (curType)
        {
            case 6: // Falcon
                break;
            case 0: // Squirrel
            case 1: // Lizard
            case 2: // Toad
            case 3: // Pigeon
            case 4: // Mole
            case 5: // Ferret
            case 7: // Skunk
            case 9: // Snake
            case 10: // Boar
            case 11: // Badger
            case 13: // Wolf
            case 14: // Fox
                SpriteAni.SetBool("Idle", true);
                SpriteAni.SetBool("Run", false);
                SpriteAni.SetBool("Cast", false);
                break;
            case 8: // Chameleon
                SpriteAni.SetBool("Run", false);
                SpriteAni.SetBool("Cast", false);
                break;
            case 12: // Owl
                SpriteAni.SetBool("Cast", false);
                break;
            case 15: // Wire
            case 16: // Mine
                SpriteAni.Play("Idle");
                break;
            case 17: // Turret
            case 18: // Balloon
            case 19: // Cannon
                SpriteAni.Play("Idle_RD");
                break;
        }

    }
}
