using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SettingPopUp : MonoBehaviour
{
    private const bool V = true;

    // Push Alert / Vibration
    public Toggle pushOn;
    public Toggle pushOff;
    public Toggle vibeOn;
    public Toggle vibeOff;

    // Sound
    public Slider soundBG;
    public Slider soundAB;
    public Slider soundEF;
    public Slider soundVo;
    public TextMeshProUGUI numberBG;
    public TextMeshProUGUI numberAB;
    public TextMeshProUGUI numberEF;
    public TextMeshProUGUI numberVo;


    private bool bPushOn = true;
    private bool bVibeOn = true;

    private float fBG;
    private float fAB;
    private float fEF;
    private float fVo;

    [SerializeField]
    private ScrollRect scrollRect;     // 스크롤 위치 조정을 위한 스크롤렉트


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnEnable()
    {
        soundBG.value = SoundManager.Instance.Get_Volume(Sound_Channel.BGM);
        soundAB.value = SoundManager.Instance.Get_Volume(Sound_Channel.Ambient) * 10f;
        soundEF.value = SoundManager.Instance.Get_Volume(Sound_Channel.Effect);
        soundVo.value = SoundManager.Instance.Get_Volume(Sound_Channel.Voice);

        numberBG.text = ((int)(soundBG.value * 100f)).ToString();
        numberAB.text = ((int)(soundAB.value * 100f)).ToString();
        numberEF.text = ((int)(soundEF.value * 100f)).ToString();
        numberVo.text = ((int)(soundVo.value * 100f)).ToString();
    }

    private void Awake()
    {
        // 푸쉬 알림 토글 변경
        pushOn.onValueChanged.AddListener((V) =>
        {
            if (pushOff.isOn)
            {
                ToggleOn(pushOff);
                ToggleOff(pushOn);
                bPushOn = false;
            }
            else
            {
                ToggleOn(pushOn);
                ToggleOff(pushOff);
                bPushOn = true;
            }
        });

        // 진동 토글 변경
        vibeOn.onValueChanged.AddListener((V) =>
        {
            if (vibeOff.isOn)
            {
                ToggleOn(vibeOff);
                ToggleOff(vibeOn);
                bVibeOn = false;
            }
            else
            {
                ToggleOn(vibeOn);
                ToggleOff(vibeOff);
                bVibeOn = true;
            }
        });

        // 배경음 변경
        soundBG.onValueChanged.AddListener((V) =>
        {
            fBG = soundBG.value;
            numberBG.text = ((int)(soundBG.value * 100f)).ToString();
        });
        // 환경음 변경
        soundAB.onValueChanged.AddListener((V) =>
        {
            fAB = soundAB.value;
            numberAB.text = ((int)(soundAB.value * 100f)).ToString();
        });
        // 효과음 변경
        soundEF.onValueChanged.AddListener((V) =>
        {
            fEF = soundEF.value;
            numberEF.text = ((int)(soundEF.value * 100f)).ToString();
        });
        // 보이스 변경
        soundVo.onValueChanged.AddListener((V) =>
        {
            fVo = soundVo.value;
            numberVo.text = ((int)(soundVo.value * 100f)).ToString();
        });
    }

    // 설정창 열기
    public void OnClickPopUpOpen()
    {
        this.gameObject.SetActive(true);

        scrollRect.verticalNormalizedPosition = 1.0f;
    }

    // 설정창 닫기
    public void OnClickPopUpClose()
    {
        fBG = soundBG.value;
        fAB = soundAB.value;
        fEF = soundEF.value;
        fVo = soundVo.value;

        SoundManager.Instance.Set_Volume(Sound_Channel.BGM, fBG);
        SoundManager.Instance.Set_Volume(Sound_Channel.Ambient, fAB);
        SoundManager.Instance.Set_Volume(Sound_Channel.Effect, fEF);
        SoundManager.Instance.Set_Volume(Sound_Channel.Voice, fVo);

        this.gameObject.SetActive(false);
    }

    // 토글 온/오프
    private void ToggleOn(Toggle on)
    {
        on.transform.GetChild(1).gameObject.SetActive(true);
        on.transform.GetChild(2).gameObject.SetActive(false);
    }
    private void ToggleOff(Toggle off)
    {
        off.transform.GetChild(1).gameObject.SetActive(false);
        off.transform.GetChild(2).gameObject.SetActive(true);
    }
}
