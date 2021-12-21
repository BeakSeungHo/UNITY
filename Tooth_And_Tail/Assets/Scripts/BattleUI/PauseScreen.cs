using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScreen : MonoBehaviour
{
    public GameObject       PlayScreen;
    public GameObject       Menu;
    public GameObject       HowToPlay;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 게임 일시정지
    public void OnClickPause()
    {
        PlayScreen.SetActive(false);
        this.gameObject.SetActive(true);
        Menu.SetActive(true);

        /// 시간 멈추는 함수 들어감
        Time.timeScale = 0f;
        SoundManager.Instance.Pause(Sound_Channel.Ambient);
        SoundManager.Instance.Pause(Sound_Channel.Effect);
        SoundManager.Instance.Pause(Sound_Channel.Voice);
    }

    // 게임 재개
    public void OnClickResume()
    {
        /// 시간 재개 함수 들어감
        Time.timeScale = 1f;
        SoundManager.Instance.Resume(Sound_Channel.Ambient);
        SoundManager.Instance.Resume(Sound_Channel.Effect);
        SoundManager.Instance.Resume(Sound_Channel.Voice);

        PlayScreen.SetActive(true);
        Menu.SetActive(false);
    }

    // 도움말
    public void OnClickHowToPlay()
    {
        Menu.SetActive(false);
        HowToPlay.SetActive(true);
    }
    // 도움말 닫기
    public void OnClickReturnToMenu()
    {
        HowToPlay.SetActive(false);
        Menu.SetActive(true);
    }

    // 항복
    public void OnClickSurrender()
    {
        Menu.SetActive(false);
        InGameManager.Instance.Surrender();
    }
}
