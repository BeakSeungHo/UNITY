using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Text;
using TMPro;

public class BattleUICtrl : MonoBehaviour
{
    //싱글톤
    private static BattleUICtrl instance = null;

    //유닛바 버튼리스트
    public List<BattleUnitButton> UnitbuttonList = new List<BattleUnitButton>();
    public Image RallyAllimg;
    public Image RallyGroupimg;

    //명령 버튼리스트
    public List<Button> CommandButtonList = new List<Button>();

    //자식오브젝트들
    public List<GameObject> ChildObjList = new List<GameObject>();

    public TextMeshProUGUI FoodText;                 // 소유 음식
    public TextMeshProUGUI TimeText;                 // 게임 시간
    public TextMeshProUGUI BuildCostText;            // 건물 짓는 비용
    public TextMeshProUGUI ProductTimeText;          // 생산 시간
    public TextMeshProUGUI NameText;                 // 이름
    public TextMeshProUGUI CostText;                 // 생산 비용
    public TextMeshProUGUI SupplyText;               // 생산 개체수
    
    private int            minTemp;
    private int            secTemp;

    public SpriteRenderer UnitImg = null;            // 유닛 정보 이미지
    public BuildingManager buildingManager = null;   // 빌딩관리하는 매니저

    public float LastHitTime = -1;                   // 떨어진 유닛이나 건물이 마지막에 맞은 시간
    public Image WarningImg = null;                  // 미니맵 경고표시 이미지
    float ChangeAlphaValue = 0.05f;

    public Button MoveLeft_GristmillButton;          // 죽거나 리스폰시 제분소 왼쪽 이동 버튼
    public Button MoveRight_GristmillButton;         // 죽거나 리스폰시 제분소 왼쪽 이동 버튼
    public Button ReturnButton;                      // 땅굴 이후 누를 귀환버튼 

    public TextMeshProUGUI CastingText;              // 귀환 또는 리스폰 표시 텍스트
    public Image CastingImage;                       // 귀환 또는 리스폰 표시 이미지 

    public Image WinnerImage;                        // 끝날때 승리자 이미지
    public Image LoserImage;                         // 끝날때 패배자 이미지
    public RectTransform EndTextParentRect;          // 텍스트 부모의 RectTransForm
    public TextMeshProUGUI EndText;                  // 끝날때 텍스트

    public RectTransform StarvingUIRect;             // 굶주림 UI RectTransform
    public TextMeshProUGUI StarveTimeText;           // 굶주림 시간
    public bool bIsStarveCoroutine = false;          // 코루틴이 실행중인지 판별

    public Image JostickHandleImg;                   // 조이스틱 핸들 이미지

    public AudioSource audioSource;

    // 생산 유닛별 생산 시간 최대값 저장하는 딕셔너리
    public Dictionary<CommonType, float> MaxProductTimeDIc = new Dictionary<CommonType, float>();

    // 플레이어 커맨더
    Commander PlayerCommander = null;
    // 플레이어 유닛 리스트
    List<CommonType> PlayerUnitTypeList = null;

    // 현재 선택된 버튼
    public int CurUnitButtonIndex = 0;

    // 변경되는 사이즈
    Vector2 size1 = new Vector2(140, 140);
    Vector2 size2 = new Vector2(180, 180);

    // 게임 시간 및 통계
    public float PlayTime = 0f;

    // EndUI 이동속도
    int EndUIMoveSpeed = Global.EndMoveUISpeed;
    int EndUIMoveAmount = 90;

    int EndTextMoveSpeed = Global.EndMoveUISpeed;
    int EndTextMoveAmount = 40;

    int EndStarvingUIMoveSpeed = Global.EndMoveUISpeed;
    int EndStarvingUIMoveAmount = 40;

    public Coroutine SnapshotCoroutine = null;

    void Awake()
    {
        if (null != Instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void Ready()
    {
        if (GameManager.Instance.CurGameMode == GameMode.Tutorial)
        {
            SetTutorialUI();
        }
        else if (GameManager.Instance.CurGameMode >= GameMode.Campaign)
        {
            switch (GameManager.Instance.CurGameMode)
            {
                case GameMode.Campaign:
                    SetCampaignUI();
                    break;
                default:
                    SetMultiUI();
                    break;
            }

            buildingManager = GameObject.Find("BuildingManager").GetComponent<BuildingManager>();

            //플레이어 첫번째 유닛으로 설정
            PlayerCommander.Change_ControllSquad(0);
            FoodText.text = GameManager.Instance.GetFoodPlayer().ToString();
            //초기 1번째 유닛의 정보 텍스트에 저장
            CommonType FirstUnitType = GameManager.Instance.PlayerUnitType(0);
            ChangeUnitInfo(0);
            SetSpritePosition(FirstUnitType);
            //초기 1번째 유닛버튼의 크기를 키워둠
            UnitbuttonList[CurUnitButtonIndex].rectTransform.sizeDelta = size2;

            //유닛타입에 따른 유닛버튼에 스프라이트 연결
            for (int i = 0; i < GameManager.Instance.UnitCount; ++i)
            {
                UnitbuttonList[i].icon.sprite = SceneStarter.Instance.uIElements.UIIconDic[GameManager.Instance.PlayerUnitType(i)];
                UnitbuttonList[i].UnitIndex = i;
            }

            //유닛타입에 따른 유닛버튼 틴트에 스프라이트 연결
            for (int i = 0; i < GameManager.Instance.UnitCount; ++i)
            {
                UnitbuttonList[i].tint.sprite = SceneStarter.Instance.tintElements.TintDic[UnitbuttonList[i].icon.sprite.name][0];

                //캠프에 따라 틴트 색 변경
                switch (GameManager.Instance.CommanderList[0])
                {
                    case Camp.Bellafide:
                        UnitbuttonList[i].tint.color = Global.CommanderInGameColorBellafide;
                        break;
                    case Camp.Hopper:
                        UnitbuttonList[i].tint.color = Global.CommanderInGameColorHopper;
                        break;
                    case Camp.Quartermaster:
                        UnitbuttonList[i].tint.color = Global.CommanderInGameColorQuartermaster;
                        break;
                    case Camp.Archimedes:
                        UnitbuttonList[i].tint.color = Global.CommanderInGameColorArchimedes;
                        break;
                }
            }

            // 타입별 생산시간 딕셔너리 초기화
            for (int i = 0; i < PlayerUnitTypeList.Count; ++i)
            {
                MaxProductTimeDIc[PlayerUnitTypeList[i]] = 0;
            }
        }

        switch (GameManager.Instance.CommanderList[0])
        {
            case Camp.Hopper:
            case Camp.Bellafide:
                RallyAllimg.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Rallyall_BR];
                RallyGroupimg.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Rallygroup_BR];
                break;
            case Camp.Archimedes:
            case Camp.Quartermaster:
                RallyAllimg.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Rallyall_GY];
                RallyGroupimg.sprite = SceneStarter.Instance.uIElements.UITypeIconDic[UIType.Rallygroup_GY];
                break;
        }

        JostickHandleImg.sprite = SceneStarter.Instance.uIElements.UIComIconDic[GameManager.Instance.CommanderList[0]];
        LastHitTime = -1;

        if (GameManager.Instance.CurGameMode == GameMode.Campaign)
        {
            StartCoroutine(CheckCampaignTimeUp());
        }
        else if (GameManager.Instance.CurGameMode == GameMode.TimeAttack)
        {
            StartCoroutine(CheckTimeAttack());
        }

        // 통계 스냅샷
        if (GameMode.Tutorial != GameManager.Instance.CurGameMode
            /*&& GameMode.Campaign != GameManager.Instance.CurGameMode*/)
            SnapshotCoroutine = StartCoroutine(StatSnapshot());
    }
    // Update is called once per frame
    void Update()
    {
        TouchCtrl();
        TimeCount();
        if (GameManager.Instance.CurGameMode >= GameMode.Campaign)
        {
            UnitProductTimeUI();
            BlinkWarningMinimap();
            CheckRespawnOrReturn();
            CastingUpdate();
        }
    }

    private void OnDestroy()
    {
        if (SnapshotCoroutine != null)
            StopCoroutine(SnapshotCoroutine);
    }

    //싱글톤
    public static BattleUICtrl Instance
    {
        get
        {
            if (null == instance)
                return null;
            else
                return instance;
        }
    }

    //플레이어 셋팅함수
    public void SetCommander(Commander Player)
    {
        PlayerCommander = Player;
        PlayerUnitTypeList = GameManager.Instance.UnitTypeDic[PlayerCommander.camp];
    }

    public void TouchCtrl()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; ++i)
            {
                Input.GetTouch(i);
            }
        }
    }

    // 시간 체크(시간바에 시간 표시)하는 함수
    public void TimeCount()
    {
        PlayTime += Time.deltaTime;
        int min = (int)PlayTime / 60;
        int sec = (int)PlayTime % 60;

        FoodText.text = GameManager.Instance.GetFoodPlayer().ToString();

        switch (GameManager.Instance.CurGameMode)
        {
            case GameMode.TimeAttack :
            case GameMode.Campaign:
                minTemp = (int)(Global.TimeAttackPlayTime - PlayTime) / 60;
                secTemp = (int)(Global.TimeAttackPlayTime - PlayTime) % 60;
                TimeText.text = minTemp.ToString("00") + " : " + secTemp.ToString("00"); // StringBuilder
                break;
            default:
                minTemp = (int)PlayTime / 60;
                secTemp = (int)PlayTime % 60;
                TimeText.text = minTemp.ToString("00") + " : " + secTemp.ToString("00"); // StringBuilder
                break;
        }
    }

    // 유닛 생산시간을 표시해주는 UI 관리 함수
    public void UnitProductTimeUI()
    {
        for (int i = 0; i < GameManager.Instance.UnitTypeDic[GameManager.Instance.CommanderList[0]].Count; ++i)
        {
            MaxProductTimeDIc[GameManager.Instance.UnitTypeDic[GameManager.Instance.CommanderList[0]][i]] = 0;
        }

        foreach (var dic in buildingManager.Buildings[PlayerCommander.camp])
        {
            switch (dic.Key)
            {
                case CommonType.WarrenT1:
                case CommonType.WarrenT2:
                case CommonType.WarrenT3:
                case CommonType.MoleeMerge:
                    foreach (var BuildingBase in dic.Value)
                    {
                        CommonType Product_Type = BuildingBase.Product_Type;
                        BuildingStateOperator stateOperator = BuildingBase.GetBuildingStateOperator();
                        
                        MaxProductTimeDIc[Product_Type] = MaxProductTimeDIc[Product_Type] < stateOperator.GetCurProductTime()
                                                            ? stateOperator.GetCurProductTime() : MaxProductTimeDIc[Product_Type];

                        if (BuildingBase.PreState == BuildingState.Idle && BuildingBase.GetCurState() == BuildingState.Production)
                        {
                            BuildingBase.PreState = BuildingState.Production;
                        }
                        else if (BuildingBase.GetCurState() == BuildingState.Production && stateOperator.GetCurProductTime() == 0)
                        {
                            BuildingBase.PreState = BuildingState.Production;
                        }
                        else if (BuildingBase.PreState == BuildingState.Production && BuildingBase.GetCurState() == BuildingState.Idle)
                        {
                            MaxProductTimeDIc[Product_Type] = 0;
                            BuildingBase.PreState = BuildingState.Idle;
                        }
                    }
                    break;
            }
        }

        for (int i = 0; i < PlayerUnitTypeList.Count; ++i)
        {
            CommonType Product_Type = PlayerUnitTypeList[i];
            if (BuildingManager.Instance.maxUnits[GameManager.Instance.CommanderList[0]].ContainsKey(Product_Type))
            {
                UnitbuttonList[i].GenTimUI.fillAmount = MaxProductTimeDIc[Product_Type] /
                                               SceneStarter.Instance.commonElements.CommonDataList[(int)Product_Type].GenTime;
            }
            else
            {
                MaxProductTimeDIc[Product_Type] = 0;
                UnitbuttonList[i].GenTimUI.fillAmount = 0;
            }

            UnitbuttonList[i].UnitCountUpdate();
        }
    }

    public void ChangeUnitInfo(int UnitBarIndex)
    {
        // 이전 버튼 크기 작게 조절
        UnitbuttonList[CurUnitButtonIndex].rectTransform.sizeDelta = size1;

        // 새로운 버튼 인덱스로 조정
        CurUnitButtonIndex = UnitBarIndex;

        // 플레이어 컨트롤 갱신
        PlayerCommander.Change_ControllSquad(CurUnitButtonIndex);

        // 유닛바 인덱스에 따른 CommonType 가져오기
        CommonType CurUnitType = GameManager.Instance.PlayerUnitType(CurUnitButtonIndex);

        // 정보갱신
        BuildCostText.text = (SceneStarter.Instance.commonElements.CommonDataList[(int)CurUnitType].BuildCost +
                        SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(CurUnitType,
                         SceneStarter.Instance.userElements.GetLevel(CurUnitType)).BuildCost).ToString();
        ProductTimeText.text = (SceneStarter.Instance.commonElements.CommonDataList[(int)CurUnitType].GenTime +
                        SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(CurUnitType,
                         SceneStarter.Instance.userElements.GetLevel(CurUnitType)).GenTime).ToString();

        if (CommonType.Wire <= CurUnitType && CurUnitType <= CommonType.Cannon)
            NameText.text = "Lv.-\n" + SceneStarter.Instance.commonElements.CommonDataList[(int)CurUnitType].Name.ToString();
        else
            NameText.text = "Lv." + SceneStarter.Instance.userElements.GetLevel(CurUnitType).ToString() + "\n" + SceneStarter.Instance.commonElements.CommonDataList[(int)CurUnitType].Name.ToString();


        CostText.text = (SceneStarter.Instance.commonElements.CommonDataList[(int)CurUnitType].Cost +
                        SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(CurUnitType,
                         SceneStarter.Instance.userElements.GetLevel(CurUnitType)).Cost).ToString();
        SupplyText.text = (SceneStarter.Instance.commonElements.CommonDataList[(int)CurUnitType].UnitPerBuliding +
                        SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(CurUnitType,
                         SceneStarter.Instance.userElements.GetLevel(CurUnitType)).UnitPerBuliding).ToString();
        UnitImg.sprite = SceneStarter.Instance.uIElements.UIPortraitDic_S[CurUnitType];
        SetSpritePosition(CurUnitType);
    }

    public void SetSpritePosition(CommonType CurUnitType)
    {
        float DivisionNum = 0f;

        switch (CurUnitType)
        {
            case CommonType.Squirrel:
                UnitImg.transform.localPosition = new Vector3(-205, -200, 0);
                DivisionNum = 4;
                break;
            case CommonType.Lizard:
                UnitImg.transform.localPosition = new Vector3(-195, -220, 0);
                DivisionNum = 4;
                break;
            case CommonType.Toad:
                UnitImg.transform.localPosition = new Vector3(-205, -200, 0);
                DivisionNum = 5;
                break;
            case CommonType.Pigeon:
                UnitImg.transform.localPosition = new Vector3(-215, -210, 0);
                DivisionNum = 5;
                break;
            case CommonType.Mole:
                UnitImg.transform.localPosition = new Vector3(-205, -200, 0);
                DivisionNum = 6;
                break;
            case CommonType.Ferret:
                UnitImg.transform.localPosition = new Vector3(-175, -150, 0);
                DivisionNum = 3.5f;
                break;
            case CommonType.Falcon:
                UnitImg.transform.localPosition = new Vector3(-150, -150, 0);
                DivisionNum = 4;
                break;
            case CommonType.Skunk:
                UnitImg.transform.localPosition = new Vector3(-160, -200, 0);
                DivisionNum = 5;
                break;
            case CommonType.Chameleon:
                UnitImg.transform.localPosition = new Vector3(-260, -170, 0);
                DivisionNum = 6;
                break;
            case CommonType.Snake:
                UnitImg.transform.localPosition = new Vector3(-230, -210, 0);
                DivisionNum = 5;
                break;
            case CommonType.Boar:
                UnitImg.transform.localPosition = new Vector3(-310, -200, 0);
                DivisionNum = 6;
                break;
            case CommonType.Badger:
                UnitImg.transform.localPosition = new Vector3(-190, -220, 0);
                DivisionNum = 3.5f;
                break;
            case CommonType.Owl:
                UnitImg.transform.localPosition = new Vector3(-210, -150, 0);
                DivisionNum = 6;
                break;
            case CommonType.Wolf:
                UnitImg.transform.localPosition = new Vector3(-205, -240, 0);
                DivisionNum = 5;
                break;
            case CommonType.Fox:
                UnitImg.transform.localPosition = new Vector3(-220, -190, 0);
                DivisionNum = 6;
                break;
            case CommonType.Wire:
                UnitImg.transform.localPosition = new Vector3(-200, -230, 0);
                DivisionNum = 5;
                break;
            case CommonType.Mine:
                UnitImg.transform.localPosition = new Vector3(-180, -230, 0);
                DivisionNum = 5;
                break;
            case CommonType.Turret:
                UnitImg.transform.localPosition = new Vector3(-190, -230, 0);
                DivisionNum = 5;
                break;
            case CommonType.Balloon:
                UnitImg.transform.localPosition = new Vector3(-200, -200, 0);
                DivisionNum = 4;
                break;
            case CommonType.Cannon:
                UnitImg.transform.localPosition = new Vector3(-200, -160, 0);
                DivisionNum = 5;
                break;
            default:
                UnitImg.transform.localPosition = new Vector3(-220, -165, 0);
                DivisionNum = 5;
                break;
        }
        UnitImg.transform.localScale = new Vector3(SceneStarter.Instance.uIElements.UIPortraitDic_S[CurUnitType].rect.size.x / DivisionNum,
                                                    SceneStarter.Instance.uIElements.UIPortraitDic_S[CurUnitType].rect.size.y /
                                                    (DivisionNum * SceneStarter.Instance.uIElements.UIPortraitDic_S[CurUnitType].rect.size.y /
                                                    SceneStarter.Instance.uIElements.UIPortraitDic_S[CurUnitType].rect.size.x), 0);
    }

    // 튜토리얼 UI 셋팅
    public void SetTutorialUI()
    {
        for (int i = 0; i < 3; ++i)
            ChildObjList[i].SetActive(false);

        //유닛바
        ChildObjList[4].SetActive(false);
        //Casting UI
        ChildObjList[7].SetActive(false);

        for (int i = 1; i < 4; ++i)
            CommandButtonList[i].gameObject.SetActive(false);
    }

    // 캠페인 UI 셋팅
    public void SetCampaignUI()
    {
        for (int i = 0; i < 3; ++i)
            ChildObjList[i].SetActive(true);

        //유닛바
        ChildObjList[4].SetActive(true);
        //Casting UI
        ChildObjList[7].SetActive(false);

        UnitbuttonList[4].gameObject.SetActive(false);
        UnitbuttonList[5].gameObject.SetActive(false);

        for (int i = 1; i < 4; ++i)
            CommandButtonList[i].gameObject.SetActive(true);

        CommandButtonList[2].gameObject.SetActive(false);
    }

    // 멀티 UI 셋팅
    public void SetMultiUI()
    {
        for (int i = 0; i < 3; ++i)
            ChildObjList[i].SetActive(true);

        //유닛바
        ChildObjList[4].SetActive(true);
        //Casting UI
        ChildObjList[7].SetActive(true);

        for (int i = 1; i < 4; ++i)
            CommandButtonList[i].gameObject.SetActive(true);
    }

    // EndUI 셋팅
    public void SetEndUI()
    {
        for (int i = 0; i < ChildObjList.Count - 1; ++i)
            ChildObjList[i].SetActive(false);

        WinnerImage.transform.parent.parent.gameObject.SetActive(true);

        WinnerImage.sprite = SceneStarter.Instance.uIElements.UIWinImageDic[GameManager.Instance.WinnerCamp];
        LoserImage.sprite = SceneStarter.Instance.uIElements.UILoseImageDic[GameManager.Instance.LoserCamp];

        switch (SceneStarter.Instance.statisticElements.winCamp)
        {
            case Camp.Bellafide:
                EndText.text = "롱코트의 득세!";
                break;
            case Camp.Hopper:
                EndText.text = "민중의 득세!";
                break;
            case Camp.Quartermaster:
                EndText.text = "KSR의 득세!";
                break;
            case Camp.Archimedes:
                EndText.text = "문명교의 득세!";
                break;
        }

        ChildObjList[ChildObjList.Count - 1].SetActive(true);
        PlayerCommander.Move_Commander(Vector3.zero);
        StartCoroutine(ActiveEndUI());
    }

    IEnumerator CheckCampaignTimeUp()
    {
        while (PlayTime < Global.CampaignPhase2Time)
        {
            yield return new WaitForSeconds(1f);
        }

        UnitbuttonList[4].gameObject.SetActive(true);
        UnitbuttonList[5].gameObject.SetActive(true);

        while (PlayTime < Global.CampaignPlayTime)
        {
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.EndGame(GameManager.Instance.CommanderList[1]);
    }

    IEnumerator CheckTimeAttack()
    {
        while (PlayTime < Global.TimeAttackPlayTime)
        {
            yield return new WaitForSeconds(1f);
        }

        var CommanderList = GameManager.Instance.CommanderList;

        if (buildingManager.Buildings[CommanderList[0]][CommonType.Gristmill].Count >=
            buildingManager.Buildings[CommanderList[1]][CommonType.Gristmill].Count)
        {
            GameManager.Instance.EndGame(CommanderList[1]);
        }
        else
            GameManager.Instance.EndGame(CommanderList[0]);
    }

    IEnumerator ActiveEndUI()
    {
        while (WinnerImage.rectTransform.anchoredPosition.x != -500)
        {
            WinnerImage.rectTransform.anchoredPosition = Vector2.MoveTowards(WinnerImage.rectTransform.anchoredPosition, new Vector2(-500, WinnerImage.rectTransform.anchoredPosition.y), EndUIMoveSpeed * Time.unscaledDeltaTime);
            LoserImage.rectTransform.anchoredPosition = Vector2.MoveTowards(LoserImage.rectTransform.anchoredPosition, new Vector2(-900, LoserImage.rectTransform.anchoredPosition.y), EndUIMoveSpeed * Time.unscaledDeltaTime);

            yield return null;
        }

        EndUIMoveSpeed = Global.EndMoveUIVibeSpeed;

        StartCoroutine(MoveEndUI());
    }

    IEnumerator MoveEndUI()
    {
        if (EndUIMoveAmount == 10)
            yield break;

        if (EndUIMoveAmount == 30)
            StartCoroutine(ActiveEndTextUI());

        while (WinnerImage.rectTransform.anchoredPosition.x != -500 - EndUIMoveAmount)
        {
            WinnerImage.rectTransform.anchoredPosition = Vector2.MoveTowards(WinnerImage.rectTransform.anchoredPosition, new Vector2(-500 - EndUIMoveAmount, WinnerImage.rectTransform.anchoredPosition.y), EndUIMoveSpeed * Time.unscaledDeltaTime);
            LoserImage.rectTransform.anchoredPosition = Vector2.MoveTowards(LoserImage.rectTransform.anchoredPosition, new Vector2(-900 + EndUIMoveAmount, LoserImage.rectTransform.anchoredPosition.y), EndUIMoveSpeed * Time.unscaledDeltaTime);

            yield return null;
        }

        EndUIMoveAmount /= 3;

        StartCoroutine(ActiveEndUI());
    }

    IEnumerator ActiveEndTextUI()
    {
        while (EndTextParentRect.anchoredPosition.y != 200)
        {
            EndTextParentRect.anchoredPosition = Vector2.MoveTowards(EndTextParentRect.anchoredPosition, new Vector2(0, 200), EndTextMoveSpeed * Time.unscaledDeltaTime);

            yield return null;
        }

        EndTextMoveSpeed = Global.EndMoveUIVibeSpeed;

        StartCoroutine(MoveEndTextUI());
    }

    IEnumerator MoveEndTextUI()
    {
        if (EndTextMoveAmount == 10)
        {
            StartCoroutine(GameManager.Instance.LoadScene());
            yield break;
        }

        while (EndTextParentRect.anchoredPosition.y != 200 - EndTextMoveAmount)
        {
            EndTextParentRect.anchoredPosition = Vector2.MoveTowards(EndTextParentRect.anchoredPosition, new Vector2(0, 200 - EndTextMoveAmount), EndTextMoveSpeed * Time.unscaledDeltaTime);

            yield return null;
        }

        EndTextMoveAmount /= 2;

        StartCoroutine(ActiveEndTextUI());
    }

    public IEnumerator ActiveStarvingUI()
    {
        bIsStarveCoroutine = true;

        while (StarvingUIRect.anchoredPosition.y != -20)
        {
            StarvingUIRect.anchoredPosition = Vector2.MoveTowards(StarvingUIRect.anchoredPosition, new Vector2(0, -20), EndStarvingUIMoveSpeed * Time.unscaledDeltaTime);

            yield return null;
        }

        EndStarvingUIMoveSpeed = Global.EndMoveUIVibeSpeed;

        StartCoroutine(MoveStarvingUI());
    }

    public IEnumerator MoveStarvingUI()
    {
        if (EndStarvingUIMoveAmount == 10)
        {
            bIsStarveCoroutine = false;
            yield break;
        }

        while (StarvingUIRect.anchoredPosition.y != -20 - EndStarvingUIMoveAmount)
        {
            StarvingUIRect.anchoredPosition = Vector2.MoveTowards(StarvingUIRect.anchoredPosition, new Vector2(0, -20 - EndStarvingUIMoveAmount), EndStarvingUIMoveSpeed * Time.unscaledDeltaTime);

            yield return null;
        }

        EndStarvingUIMoveAmount /= 2;

        StartCoroutine(ActiveStarvingUI());
    }

    public IEnumerator EndMoveStarvingUI()
    {
        EndStarvingUIMoveSpeed = Global.EndMoveUISpeed;
        EndStarvingUIMoveAmount = 40;

        while (StarvingUIRect.anchoredPosition.y != 200)
        {
            StarvingUIRect.anchoredPosition = Vector2.MoveTowards(StarvingUIRect.anchoredPosition, new Vector2(0, 200), EndStarvingUIMoveSpeed * Time.unscaledDeltaTime);

            yield return null;
        }
    }

    //모든 유닛 소집명령
    public void AllGatheringCommand()
    {
        if (PlayerCommander != null)
            PlayerCommander.RallyAll_Commander(true);
        else
            Debug.Log("Failed Find Commander");
    }

    //모든 유닛 소집명령 종료
    public void AllGatheringEnd()
    {
        if (PlayerCommander != null)
            PlayerCommander.RallyAll_Commander(false);
        else
            Debug.Log("Failed Find Commander");
    }

    //대상 유닛 소집명령
    public void UnitGatheringCommand()
    {
        if (PlayerCommander != null)
            PlayerCommander.Rally_Commander(true);
        else
            Debug.Log("Failed Find Commander");
    }

    //대상 유닛 소집명령 종료
    public void UnitGatheringEnd()
    {
        if (PlayerCommander != null)
            PlayerCommander.Rally_Commander(false);
        else
            Debug.Log("Failed Find Commander");
    }

    //건물 짓는 명령 실행
    public void BuildCommand()
    {
        if (GameManager.Instance.CurGameMode == GameMode.Campaign)
        {
            if (PlayerCommander != null)
                PlayerCommander.UnitSummons(PlayerCommander.Base.MyCamp, GameManager.Instance.UnitTypeDic[PlayerCommander.Base.MyCamp][CurUnitButtonIndex]);
            else
                Debug.Log("Failed Find Commander");

        }
        else
        {
            if (PlayerCommander != null)
                PlayerCommander.BuildBuilding();
            else
                Debug.Log("Failed Find Commander");
        }
    }

    public void ReturnCommand()
    {
        if (PlayerCommander != null)
            PlayerCommander.Digging(true);
        else
            Debug.Log("Failed Find Commander");
    }

    public void ReturnCommandEnd()
    {
        if (PlayerCommander != null)
            PlayerCommander.Digging(false);
        else
            Debug.Log("Failed Find Commander");
    }

    public void MoveLeft_GristmillCommand()
    {
        if (PlayerCommander != null)
            PlayerCommander.MoveLeft_RespawnGristmill();
        else
            Debug.Log("Failed Find Commander");
    }

    public void MoveRight_GristmillCommand()
    {
        if (PlayerCommander != null)
            PlayerCommander.MoveRight_RespawnGristmill();
        else
            Debug.Log("Failed Find Commander");
    }

    public void DecisionGristmill()
    {
        if (PlayerCommander != null)
            PlayerCommander.ReturnToBase(true);
        else
            Debug.Log("Failed Find Commander");
    }

    public void DecisionGristmillEnd()
    {
        if (PlayerCommander != null)
            PlayerCommander.ReturnToBase(false);
        else
            Debug.Log("Failed Find Commander");
    }

    public void OnClickCheatButton()
    {
        for(int i = 0; i < GameManager.Instance.CommanderList.Count; ++i)
            GameManager.Instance.ChangeFoodCamp(GameManager.Instance.CommanderList[i], 60);
    }

    public void BlinkWarningMinimap()
    {
        if ((PlayTime - LastHitTime) < 1)
        {
            if (WarningImg.color.a >= 1 || WarningImg.color.a <= 0)
                ChangeAlphaValue *= -1;

            if (WarningImg.color.a == 1)
                GameManager.Instance.Vibrate();
            
            Color color = WarningImg.color;
            color.a += ChangeAlphaValue;
            WarningImg.color = color;
        }
        else
        {
            Color color = WarningImg.color;
            color.a = 0;
            WarningImg.color = color;
        }
    }

    public void CheckRespawnOrReturn()
    {
        if (CommanderFSM.STATE.RESPAWN == PlayerCommander.commanderFSM.curState ||
                CommanderFSM.STATE.RETURN == PlayerCommander.commanderFSM.curState)
        {
            MoveLeft_GristmillButton.gameObject.SetActive(true);
            MoveRight_GristmillButton.gameObject.SetActive(true);
            if (CommanderFSM.STATE.RETURN == PlayerCommander.commanderFSM.curState)
                ReturnButton.gameObject.SetActive(true);
        }
        else
        {
            MoveLeft_GristmillButton.gameObject.SetActive(false);
            MoveRight_GristmillButton.gameObject.SetActive(false);
            ReturnButton.gameObject.SetActive(false);
            DecisionGristmillEnd();
        }
    }

    public void CastingUpdate()
    {
        if (CommanderFSM.STATE.DIG == PlayerCommander.commanderFSM.curState)
        {
            CastingText.gameObject.SetActive(true);
            CastingImage.transform.parent.gameObject.SetActive(true);
            CastingText.transform.parent.transform.localPosition = new Vector3(0, 120, 0);
            CastingText.text = "집으로 귀환";
            CastingImage.fillAmount = PlayerCommander.digTimeCount / PlayerCommander.digTime;
        }
        else if (CommanderFSM.STATE.RESPAWN == PlayerCommander.commanderFSM.curState)
        {
            CastingText.gameObject.SetActive(true);
            CastingImage.transform.parent.gameObject.SetActive(true);
            CastingText.transform.parent.transform.localPosition = new Vector3(0, 0, 0);
            CastingText.text = "리스폰 중";
            CastingImage.fillAmount = PlayerCommander.RespawnTimeCount / PlayerCommander.RespawnTime;
        }
        else
        {
            CastingText.gameObject.SetActive(false);
            CastingImage.transform.parent.gameObject.SetActive(false);
        }
    }


    // 통계 스냅샷
    public IEnumerator StatSnapshot()
    {
        while (true)
        {
            yield return new WaitForSeconds(Global.SnapshotInterval);
            SceneStarter.Instance.statisticElements.AddSnapshot();
        }
    }
}
