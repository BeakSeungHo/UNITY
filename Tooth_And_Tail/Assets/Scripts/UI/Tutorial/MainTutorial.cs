using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainTutorial : MonoBehaviour
{
    public GameObject       NameEnter;
    public GameObject       FinalConfirm;
    public GameObject       TutorialMsg;
    public GameObject       MainLobby;

    // Nickname Input
    public TMP_InputField   nameInput;
    public TextMeshProUGUI  warningBlank;
    public TextMeshProUGUI  warningForbid;
    public TextMeshProUGUI  warningLong;
    private float           msgAlpha;
    public float            fadeSpeed;

    // Final Confirm
    public TextMeshProUGUI  finalName;

    // Error Status
    private enum            NameError { None, Blank, Forbid, Long };
    private int             warnCur = 0;


    [SerializeField]
    private string          tempName;
    private float           msgTime = 0f;
    private bool            tutorialStart = false;


    // Start is called before the first frame update
    void Start()
    {
        warnCur = 0;
        fadeSpeed = 0.75f;
        msgTime = 0f;
    }

    private void OnEnable()
    {
        // 튜토리얼 종료
        if (SceneStarter.Instance.userElements.GetCurChapter(0))
        {
            this.gameObject.SetActive(false);
            MainLobby.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 터치하지 않아도 1.5초 이후에 튜토리얼 시작
        if (tutorialStart)
        {
            msgTime += Time.deltaTime;
            if (msgTime > 1.5f)
                OnClickTutorial();
        }

        if (0 == warnCur)
            return;

        switch (warnCur)
        {
            case (int) NameError.Blank:
                warningBlank.alpha = msgAlpha;
                msgAlpha -= Time.deltaTime * fadeSpeed;

                if (msgAlpha < 0f)
                {
                    warnCur = 0;
                    msgAlpha = 1.0f;
                    warningBlank.gameObject.SetActive(false);
                }
                break;
            case (int)NameError.Forbid:
                warningForbid.alpha = msgAlpha;
                msgAlpha -= Time.deltaTime * fadeSpeed;

                if (msgAlpha < 0f)
                {
                    warnCur = 0;
                    msgAlpha = 1.0f;
                    warningForbid.gameObject.SetActive(false);
                }
                break;
            case (int)NameError.Long:
                warningLong.alpha = msgAlpha;
                msgAlpha -= Time.deltaTime * fadeSpeed;

                if (msgAlpha < 0f)
                {
                    warnCur = 0;
                    msgAlpha = 1.0f;
                    warningLong.gameObject.SetActive(false);
                }
                break;
        }
    }

    // 닉네임 확인 
    public void OnClickCheckBtn()
    {
        tempName = nameInput.text;

        switch (NameCheck(tempName))
        {
            case NameError.None:
                warnCur = 0;
                NameEnter.gameObject.SetActive(false);
                FinalConfirm.gameObject.SetActive(true);
                finalName.text = tempName;
                break;
            case NameError.Blank:
                warnCur = (int)NameError.Blank;
                warningBlank.gameObject.SetActive(true);
                msgAlpha = 1f;
                break;
            case NameError.Forbid:
                warnCur = (int)NameError.Forbid;
                warningForbid.gameObject.SetActive(true);
                msgAlpha = 1f;
                break;
            case NameError.Long:
                warnCur = (int)NameError.Long;
                warningLong.gameObject.SetActive(true);
                msgAlpha = 1f;
                break;
        }
    }
    // 닉네임 검사
    private NameError NameCheck(string _name)
    {
        // 길이 검사
        if (0 == _name.Length)
            return NameError.Blank;
        else if (_name.Length > 8)
            return NameError.Long;

        // 문자 검사
        foreach (var data in _name)
        {
            if (!CheckCharacter(data))
                return NameError.Forbid;
        }

        return NameError.None;
    }
    // 글자 검사
    private bool CheckCharacter(char ch)
    {
        if ('_' == ch)
            return true;
        // 숫자
        else if (0x30 <= ch && ch <= 0x39)
            return true;
        // 한글
        else if ((0xAC00 <= ch && ch <= 0xD7A3) || (0x3131 <= ch && ch <= 0x318E))
            return true;
        // 영어
        else if ((0x61 <= ch && ch <= 0x7A) || (0x41 <= ch && ch <= 0x5A))
            return true;

        return false;
    }

    // 닉네임 확정
    public void OnClickConfirmBtn()
    {
        finalName.text = tempName;

        SceneStarter.Instance.userElements.UserData.UserName = tempName;

        FinalConfirm.gameObject.SetActive(false);
        TutorialMsg.gameObject.SetActive(true);
        tutorialStart = true;
    }

    // 닉네임 입력으로 돌아가기
    public void OnClickCancelBtn()
    {
        finalName.text = "";
        tempName = "";
        nameInput.text = "";

        warnCur = 0;
        NameEnter.gameObject.SetActive(true);
        FinalConfirm.gameObject.SetActive(false);
    }

    // 튜토리얼 시작
    public void OnClickTutorial()
    {
        GameManager.Instance.CurGameMode = GameMode.Tutorial;
        GameManager.Instance.SetTutorial();

        SceneStarter.Instance.statisticElements.curGameMode = GameMode.Tutorial;

        SceneManager.LoadScene("BattleScene");
        SceneManager.LoadScene("BattleScene_UI", LoadSceneMode.Additive);
    }
}
