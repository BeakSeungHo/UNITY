using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class EncyclopediaWindow : MonoBehaviour
{
    public EncyPopup            IllustPopUp;
    public List<UnitSlot>       SlotList;

    // UnitDesc
    public TextMeshProUGUI      UnitName;
    public TextMeshProUGUI      BuildCost;
    public TextMeshProUGUI      BuildTime;

    public TextMeshProUGUI      HireCost;
    public TextMeshProUGUI      UnitApply;
    public TextMeshProUGUI      Attack;
    public TextMeshProUGUI      HP;
    public TextMeshProUGUI      Sight;
    public TextMeshProUGUI      Range;

    public TextMeshProUGUI      UnitStory;
    public RectTransform        SpriteRect;
    public SpriteRenderer       SpriteRender;
    public Animator             SpriteAni;

    public int                  curType = 0;    // 현재 유닛

    private Vector2 spriteOffset = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        var slotTempList = GetComponentsInChildren<UnitSlot>(true);

        SlotList.Clear();
        int idx = 0;
        foreach (var data in slotTempList)
        {
            data.MasterEncy = this;
            data.unitType = idx;
            data.icon.sprite = SceneStarter.Instance.uIElements.UIIconDic[(CommonType)idx];
            data.tint.sprite = SceneStarter.Instance.tintElements.TintDic[data.icon.sprite.name][0];
            data.tint.color = Global.CommanderInGameColorBellafide;

            // 획득 유닛
            if (SceneStarter.Instance.userElements.GetIsPossession(idx))
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

            SlotList.Add(data);
            idx++;
        }

        curType = 0;
        ChangeDesc(0);
        SpriteAni.runtimeAnimatorController = SceneStarter.Instance.animatorElements.UnitAniDic[0];
    }

    private void OnEnable()
    {
        curType = 0;
        ChangeDesc(0);
    }

    // Update is called once per frame
    void Update()
    {
    }

    // 정보창 갱신
    public void ChangeDesc(int _type)
    {
        curType = _type;

        UpdateAnimator(_type);
        OnClickAniIdleBtn();

        // 획득 유닛
        //if (SceneStarter.Instance.userElements.GetIsPossession(_type))
        if (true)
        {
            UnitName.text = SceneStarter.Instance.commonElements.CommonDataList[curType].Name;
            BuildCost.text = SceneStarter.Instance.commonElements.CommonDataList[curType].BuildCost.ToString();
            BuildTime.text = SceneStarter.Instance.commonElements.CommonDataList[curType].GenTime.ToString();

            HireCost.text = SceneStarter.Instance.commonElements.CommonDataList[curType].Cost.ToString();
            UnitApply.text = SceneStarter.Instance.commonElements.CommonDataList[curType].UnitPerBuliding.ToString();
            Attack.text = SceneStarter.Instance.commonElements.CommonDataList[curType].Damage.ToString();
            HP.text = SceneStarter.Instance.commonElements.CommonDataList[curType].MaxHp.ToString();
            Sight.text = SceneStarter.Instance.commonElements.CommonDataList[curType].Sight.ToString();
            Range.text = SceneStarter.Instance.commonElements.CommonDataList[curType].Range.ToString();

            UnitStory.text = SceneStarter.Instance.commonElements.CommonDataList[curType].Sentence;
        }
        // 미획득 유닛
        else
        {
            UnitName.text = "???";
            BuildCost.text = "?";
            BuildTime.text = "?";

            HireCost.text = "?";
            UnitApply.text = "?";
            Attack.text = "?";
            HP.text = "?";
            Sight.text = "?";
            Range.text = "?";

            UnitStory.text = "용병 획득 후에 스토리를 볼 수 있습니다.";
        }
    }

    public void IllustPopUpOpen()
    {
        SpriteRender.color = new Color(1, 1, 1, 0.5f);
        SpriteAni.enabled = false;
        IllustPopUp.gameObject.SetActive(true);
        IllustPopUp.Open(curType);
    }

    public void OnClickAniIdleBtn()
    {
        // 유닛타입에 따라 애니메이터 bool값 조정
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

    public void OnClickAniMoveBtn()
    {
        // 유닛타입에 따라 애니메이터 bool값 조정
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
                SpriteAni.SetBool("Idle", false);
                SpriteAni.SetBool("Run", true);
                SpriteAni.SetBool("Cast", false);
                break;
            case 8: // Chameleon
                SpriteAni.SetBool("Run", true);
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

    public void OnClickAniAttackBtn()
    {
        // 유닛타입에 따라 애니메이터 bool값 조정
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
                SpriteAni.SetBool("Idle", false);
                SpriteAni.SetBool("Run", false);
                SpriteAni.SetBool("Cast", true);
                break;
            case 8: // Chameleon
                SpriteAni.SetBool("Run", false);
                SpriteAni.SetBool("Cast", true);
                break;
            case 12: // Owl
                SpriteAni.SetBool("Cast", true);
                break;
            case 15: // Wire
                SpriteAni.Play("Idle");
                break;
            case 16: // Mine
                SpriteAni.Play("Bomb");
                break;
            case 17: // Turret
            case 18: // Balloon
            case 19: // Cannon
                SpriteAni.Play("Attack_RD");
                break;
        }
    }

    // 유닛타입에 따라 애니메이터를 가져오고 위치값을 변경하는 함수
    public void UpdateAnimator(int _type)
    {
        //유닛타입에 따라 애니메이터 가져옴
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
            case CommonType.Owl:
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
                spriteOffset = new Vector2(-130, -140);
                break;
            case CommonType.Chameleon:
                spriteOffset = new Vector2(-130, -100);
                break;
            case CommonType.Fox:
                spriteOffset = new Vector2(-130, -70);
                break;

        }

        SpriteRect.localPosition = spriteOffset;
    }
}
