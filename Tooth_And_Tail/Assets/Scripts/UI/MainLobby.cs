using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


/// <summary>
/// 
/// 
///     Name : Semi Park
///     Date : 2020.08.19
///     
///     Desc.
///             메인 로비 화면
/// 
/// </summary>


public class MainLobby : MonoBehaviour
{
    public GameObject           MainTutorial;

    // WIndow
    public HomescreenWindow     HomeWindow;
    public EncyclopediaWindow   EncyWindow;
    public InventoryWindow      InvenWindow;
    public TrainingWindow       TrainWindow;
    public MissionWindow        MissionWindow;
    public ShopWindow           ShopWindow;

    public CampaignWindow       CampaignWindow;
    public BattleReadyWindow    BattleWindow;
    public StatisticsWindow     StatisticWindow;

    public GameObject           ButtonReturn;

    // User Info
    public TextMeshProUGUI      Name;
    public TextMeshProUGUI      Level;
    public Image                LevelFill;

    // Wealth Info
    public TextMeshProUGUI      StaminaTime;
    public TextMeshProUGUI      Stamina;
    public TextMeshProUGUI      Gold;
    public TextMeshProUGUI      Jewel;

    //
    private int                 staminaMin;
    private int                 staminaSec;


    void Start()
    {
        // 게임이 종료된 경우
        if (SceneStarter.Instance.statisticElements.isGameEnded && SceneStarter.Instance.statisticElements.curGameMode != GameMode.Tutorial)
            GameResultScreen();

        //  메인로비 BGM 실행
        var audios = SceneStarter.Instance.soundElements.LobbySoundDic[LobbySoundType.Back];
        if (null != audios)
        {
            SoundManager.Instance.Play_BGM(audios[0]);
        }
    }

    void Update()
    {
        UpdateWealth();


        /*  인벤토리 임시 테스트용 */
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("재료템 생성");
            SceneStarter.Instance.userElements.AddItem(1, 1);
            SceneStarter.Instance.userElements.AddItem(4, 1);
            SceneStarter.Instance.userElements.AddItem(7, 1);
        }
        else if (Input.GetKeyDown(KeyCode.V))
        {
            Debug.Log("다람쥐 코어 생성");
            SceneStarter.Instance.userElements.AddItem(10, 2);
        }
    }

    private void OnEnable()
    {
        // 튜토리얼 종료
        //if (SceneStarter.Instance.userElements.GetCurChapter(0))
        //{
        //    MainTutorial.SetActive(false);
        //    this.gameObject.SetActive(true);
        //    OnClickReturnToLobby();
        //}

        Name.text = SceneStarter.Instance.userElements.UserData.UserName;
        Level.text = SceneStarter.Instance.userElements.UserData.UserLevel.ToString();
        LevelFill.fillAmount = (float)SceneStarter.Instance.userElements.UserData.UserCurExp
                                / (float)SceneStarter.Instance.userElements.UserData.UserMaxExp;
    }

    // 임시로 만들었다 알아서 수정해라..
    public void OnClickPlay()
	{
        // 튜토리얼
        //GameManager.Instance.CurGameMode = GameMode.Tutorial;
        //GameManager.Instance.SetTutorial();


        // 캠페인
        GameManager.Instance.CurGameMode = GameMode.Campaign;
        GameManager.Instance.SetCampaign();

        SceneManager.LoadScene("BattleScene");
        SceneManager.LoadScene("BattleScene_UI", LoadSceneMode.Additive);
    }

	// 임시로 만들었다 알아서 수정해라..
	public void OnClickPlayEffectScene()
	{

		SceneManager.LoadScene("EffectTestScene");


	}

    // 게임 종료 후 통계 화면
    public void GameResultScreen()
    {
        StatisticWindow.gameObject.SetActive(true);
        StatisticWindow.SetUpGraph();
        SceneStarter.Instance.statisticElements.isGameEnded = false;

        HomeWindow.gameObject.SetActive(false);
        ButtonReturn.SetActive(false);
        InvenWindow.gameObject.SetActive(false);
        EncyWindow.gameObject.SetActive(false);
        TrainWindow.gameObject.SetActive(false);
        MissionWindow.gameObject.SetActive(false);
        ShopWindow.gameObject.SetActive(false);

        BattleWindow.gameObject.SetActive(false);
    }

    // 로비 화면으로 돌아가기
    public void OnClickReturnToLobby()
    {
        HomeWindow.gameObject.SetActive(true);
        ButtonReturn.SetActive(false);
        InvenWindow.gameObject.SetActive(false);
        EncyWindow.gameObject.SetActive(false);
        TrainWindow.gameObject.SetActive(false);
        MissionWindow.gameObject.SetActive(false);
        ShopWindow.gameObject.SetActive(false);

        CampaignWindow.gameObject.SetActive(false);
        BattleWindow.gameObject.SetActive(false);
        StatisticWindow.gameObject.SetActive(false);

        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.Out, 0, false);
    }

    // 인벤토리 화면 진입
    public void OnClickInvenIcon()
    {
        HomeWindow.gameObject.SetActive(false);

        ButtonReturn.SetActive(true);
        InvenWindow.gameObject.SetActive(true);

        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.In, 0, false);
    }

    // 도감 화면 진입
    public void OnClickEncyIcon()
    {
        HomeWindow.gameObject.SetActive(false);

        ButtonReturn.SetActive(true);
        EncyWindow.gameObject.SetActive(true);

        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.In, 0, false);
    }

    // 훈련 화면 진입
    public void OnClickTrainIcon()
    {
        HomeWindow.gameObject.SetActive(false);

        ButtonReturn.SetActive(true);
        TrainWindow.gameObject.SetActive(true);

        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.In, 0, false);
    }

    // 미션 화면 진입
    public void OnClickMissionIcon()
    {
        HomeWindow.gameObject.SetActive(false);

        ButtonReturn.SetActive(true);
        MissionWindow.gameObject.SetActive(true);

        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.In, 0, false);
    }

    // 상점 화면 진입
    public void OnClickShopIcon()
    {
        HomeWindow.gameObject.SetActive(false);

        ButtonReturn.SetActive(true);
        ShopWindow.gameObject.SetActive(true);
        
        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.In, 0, false);
    }

    // 대전모드 화면 진입
    public void OnClickBattleMode()
    {
        HomeWindow.gameObject.SetActive(false);

        ButtonReturn.SetActive(true);
        BattleWindow.gameObject.SetActive(true);

        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.In, 0, false);
    }

    // 캠페인 화면 진입
    public void OnClickCampaignMode()
    {
        HomeWindow.gameObject.SetActive(false);

        ButtonReturn.SetActive(true);
        CampaignWindow.gameObject.SetActive(true);

        //  클릭 소리
        SoundManager.Instance.Play_LobbySound(Sound_Channel.UI, Camera.main.gameObject, LobbySoundType.In, 0, false);
    }

    // 재화 갱신
    public void UpdateWealth()
    {
        // 스태미나가 최대치가 아닐 때
        if (SceneStarter.Instance.userElements.UserData.UserCurStamina < SceneStarter.Instance.userElements.UserData.UserMaxStamina)
        {
            StaminaTime.gameObject.SetActive(true);

            staminaMin = (int)GameManager.Instance.ChargeStaminaTimeCount / 60;
            staminaSec = (int)GameManager.Instance.ChargeStaminaTimeCount % 60;

            if (staminaSec < 10)
                StaminaTime.text = staminaMin.ToString() + ":0" + staminaSec.ToString();
            else
                StaminaTime.text = staminaMin.ToString() + ":" + staminaSec.ToString();
        }
        else
            StaminaTime.gameObject.SetActive(false);


        Stamina.text = SceneStarter.Instance.userElements.UserData.UserCurStamina.ToString()
                        + "/" + SceneStarter.Instance.userElements.UserData.UserMaxStamina.ToString();
        Gold.text = SceneStarter.Instance.userElements.UserData.UserGold.ToString();
        Jewel.text = SceneStarter.Instance.userElements.UserData.UserDia.ToString();
    }
}
