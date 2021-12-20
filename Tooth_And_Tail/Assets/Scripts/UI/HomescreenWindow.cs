using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class HomescreenWindow : MonoBehaviour
{
    public MainLobby        MasterLobby;

    // 부스터
    public GameObject       Booster;
    public TextMeshProUGUI  boosterText;

    private bool            bBoosterOn;
    private TimeSpan        leftBooster;
    private TimeSpan        prevBooster;
    private int             boosterD;
    private int             boosterH;
    private int             boosterM;
    private int             boosterS;

    // 치트
    public GameObject       Cheat;
    private bool            bCheatOn;

    // 새로 클리어한 미션이 있는지 표시하는 이미지
    public Image NewCompleteAlarm;

    // Start is called before the first frame update
    void Start()
    {
        bCheatOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        leftBooster = SceneStarter.Instance.userElements.GetRestTime();

        if (bBoosterOn && prevBooster.TotalSeconds != leftBooster.TotalSeconds)
        {
            // 부스터 시간 갱신
            prevBooster = leftBooster;

            // 부스터 종료
            if (1 > leftBooster.TotalSeconds)
            {
                bBoosterOn = false;
                Booster.SetActive(false);

                return;
            }

            boosterD = leftBooster.Days;
            boosterH = leftBooster.Hours;
            boosterM = leftBooster.Minutes;
            boosterS = leftBooster.Seconds;

            if (0 < boosterD)
                boosterText.text = (boosterD * 24 + boosterH).ToString() + "시간 " + boosterM.ToString() + "분";
            else if (0 < boosterH)
                boosterText.text = boosterH.ToString() + "시간 " + boosterM.ToString() + "분";
            else
                boosterText.text = boosterM.ToString() + "분 " + boosterS.ToString() + "초";
        }
    }

    private void OnEnable()
    {
        // 유저 정보 갱신
        MasterLobby.Name.text = SceneStarter.Instance.userElements.UserData.UserName;
        MasterLobby.Level.text = SceneStarter.Instance.userElements.UserData.UserLevel.ToString();
        MasterLobby.LevelFill.fillAmount = (float)SceneStarter.Instance.userElements.UserData.UserCurExp
                                            / (float)SceneStarter.Instance.userElements.UserData.UserMaxExp;

        // 부스터 시간 갱신
        leftBooster = SceneStarter.Instance.userElements.GetRestTime();
        prevBooster = leftBooster;

        if (1 > leftBooster.TotalSeconds)
        {
            bBoosterOn = false;
            Booster.SetActive(false);
        }
        else
        {
            bBoosterOn = true;
            Booster.SetActive(true);

            boosterD = leftBooster.Days;
            boosterH = leftBooster.Hours;
            boosterM = leftBooster.Minutes;
            boosterS = leftBooster.Seconds;

            if (0 < boosterD)
                boosterText.text = (boosterD * 24 + boosterH).ToString() + "시간 " + boosterM.ToString() + "분";
            else if (0 < boosterH)
                boosterText.text = boosterH.ToString() + "시간 " + boosterM.ToString() + "분";
            else
                boosterText.text = boosterM.ToString() + "분 " + boosterS.ToString() + "초";
        }

        // 유저 소유 골드로 업적 값 갱신
        SceneStarter.Instance.userElements.SetMissionCount(MissionType.Achievements, 1, SceneStarter.Instance.userElements.UserData.UserGold);

        // 치트 온오프
        if (bCheatOn)
            Cheat.SetActive(true);
        else
            Cheat.SetActive(false);

        if (GameManager.Instance != null && GameManager.Instance.CompleteMissionCount > GameManager.Instance.ReceivedMissionCount)
            NewCompleteAlarm.gameObject.SetActive(true);
        else
            NewCompleteAlarm.gameObject.SetActive(false);
    }

    // 치트버튼 온/오프
    public void OnClickCheat()
    {
        if (bCheatOn)
        {
            bCheatOn = false;
            Cheat.SetActive(false);
        }
        else
        {
            bCheatOn = true;
            Cheat.SetActive(true);
        }
    }

    // 모든 재료 아이템 20개 추가
    public void OnClickPlusInven()
    {
        for (int i = 1; i < 25; i++)
            SceneStarter.Instance.userElements.ItemDataList[i].ItemCount += 20;
    }
    // 모든 아이템 제거
    public void OnClickMinusInven()
    {
        SceneStarter.Instance.userElements.UserData.UserCurStamina = 0;
        SceneStarter.Instance.userElements.UserData.UserGold = 0;
        SceneStarter.Instance.userElements.UserData.UserDia = 0;

        for (int i = 1; i < 25; i++)
            SceneStarter.Instance.userElements.ItemDataList[i].ItemCount = 0;
    }
    // 탄환 추가
    public void OnClickIncreaseBullet()
    {
        SceneStarter.Instance.userElements.UserData.UserCurStamina += 10;
    }
    // 골드 추가
    public void OnClickIncreaseGold()
    {
        SceneStarter.Instance.userElements.UserData.UserGold += 50000;
        SceneStarter.Instance.userElements.SetMissionCount(MissionType.Achievements, 1, SceneStarter.Instance.userElements.UserData.UserGold);
    }
    // 다이아 추가
    public void OnClickIncreaseDia()
    {
        SceneStarter.Instance.userElements.UserData.UserDia += 500;
    }
}
