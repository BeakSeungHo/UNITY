using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public enum TUTORIAL { MOVE, SPAWN_FIRST, SPAWN_BUILD, SPAWN_ENEMY_TERRET, SPAWN_FALCON, SPAWN_ENEMY_TERRET2, DESTROY, END };

public class Speech : MonoBehaviour
{
    public Canvas               Canvas;
    public RectTransform        CanvasRect;
    public GameObject           SpeechBubble;
    public Image                BubbleL;
    public Image                BubbleR;
    public Image                BubbleCenter;
    public Image                BubbleTail;

    // text
    public TextMeshProUGUI      text;
    public RectTransform        textTransform;
    public Rect                 textSize;
    private float               widthRatio;
    private float               heightRatio;


    TUTORIAL eventType;
    public GameObject           Parent;
    public Commander            Commander;

    // endTime
    float   endTime = 0f;
    bool    endflag = false;

    bool    startflag = true;
    bool    eventflag = false;
    int     wave = -1;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.worldCamera = InGameManager.Instance.MainCamera.Cam;
        CanvasRect.localScale = new Vector2(10f / Screen.width, 10f / Screen.height);
        CanvasRect.localPosition = new Vector3(0, 0.51f, 0);

        if (GameMode.Tutorial != GameManager.Instance.CurGameMode || Commander.Base.MyCamp != GameManager.Instance.CommanderList[0])
            Parent.SetActive(false);
        else
            eventflag = true;
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void LateUpdate()
    {
        if (Commander.transform.localScale.x < 0)
            CanvasRect.localScale = new Vector2(-10f / Screen.height, 10f / Screen.height);
        else
            CanvasRect.localScale = new Vector2(10f / Screen.height, 10f / Screen.height);


        if (endflag)
        {
            endTime += Time.deltaTime;
            if (endTime > 3f)
            {
                EndEvnet();
            }
        }

        if (eventflag)
        {
            if (startflag)
            {
                endTime = 0f;
                startflag = false;
            }
            Speech_Event(wave);

            endTime += Time.deltaTime;
            if (endTime > 0.3f)
            {
                eventflag = false;
                endflag = true;
                endTime = 0f;
            }
        }

        //  말풍선 크기 조절
        textSize = text.rectTransform.rect;
        /// 중앙
        widthRatio = textSize.x / BubbleCenter.rectTransform.rect.x + 0.2f;
        heightRatio = textSize.y / BubbleCenter.rectTransform.rect.y + 0.2f;
        BubbleCenter.rectTransform.localScale = new Vector3(widthRatio, heightRatio, 1f);
        /// 양옆
        BubbleL.rectTransform.localScale = new Vector3(heightRatio, heightRatio, 1f);
        BubbleL.rectTransform.localPosition = new Vector3(text.rectTransform.localPosition.x - textSize.width * 0.5f, text.rectTransform.localPosition.y, 0f);
        BubbleR.rectTransform.localScale = new Vector3(heightRatio, heightRatio, 1f);
        BubbleR.rectTransform.localPosition = new Vector3(text.rectTransform.localPosition.x + textSize.width * 0.5f, text.rectTransform.localPosition.y, 0f);
        /// 말풍선
        BubbleTail.rectTransform.localScale = new Vector3(heightRatio + 0.1f, heightRatio + 0.2f, 1f);
        BubbleTail.rectTransform.localPosition = new Vector3(text.rectTransform.localPosition.x, text.rectTransform.localPosition.y - textSize.height * 0.62f);

    }

    public void EventStart(int Wave)
    {
        wave = Wave;
        eventflag = true;
        startflag = true;
    }

    public void Speech_Event(int type)
    {
        eventType = (TUTORIAL)(type + 1);
        Debug.Log("Speech_Event_" + eventType + this);
        SpeechBubble.SetActive(true);
        text.gameObject.SetActive(true);

        switch (eventType)
        {
            case TUTORIAL.MOVE:
                text.text = "왼쪽 아래 스틱으로 나를 움직일 수 있다네";
                break;
            case TUTORIAL.SPAWN_FIRST:
                startflag = false;
                text.text = "소집 명령으로 용병들을 모을 수 있네";
                break;
            case TUTORIAL.SPAWN_BUILD:
                text.text = "땅굴을 지으면 용병들을 생성할 수 있네";
                break;
            case TUTORIAL.SPAWN_ENEMY_TERRET:
                text.text = "방어 건물은 용병을 생성하지는 않지만 공격이나 방어가 가능하네";
                break;
            case TUTORIAL.SPAWN_FALCON:
                text.text = "비행 용병들은 자유롭게 이동하며 지상 공격을 받지 않는다네";
                break;
            case TUTORIAL.SPAWN_ENEMY_TERRET2:
                text.text = "적군 가까이에서 소집 명령을 길게 누르면 대상을 집중 공격 할 수 있네";
                break;
            case TUTORIAL.DESTROY:
                text.text = "이제 적군의 제분소를 파괴해 전투를 끝내도록 하지. 진격!";
                break;
        }
    }
    public void EndEvnet()
    {
        Debug.Log("EndEvent");
        switch (eventType)
        {
            case TUTORIAL.MOVE:
                SpeechBubble.SetActive(false);
                break;
            case TUTORIAL.SPAWN_FIRST:
                SpeechBubble.SetActive(false);
                break;
            case TUTORIAL.SPAWN_BUILD:
                SpeechBubble.SetActive(false);
                break;
            case TUTORIAL.SPAWN_ENEMY_TERRET:
                SpeechBubble.SetActive(false);
                break;
            case TUTORIAL.SPAWN_FALCON:
                SpeechBubble.SetActive(false);
                break;
            case TUTORIAL.SPAWN_ENEMY_TERRET2:
                SpeechBubble.SetActive(false);
                break;
            case TUTORIAL.DESTROY:
                SpeechBubble.SetActive(false);
                break;
        }
        endTime = 0f;
    }
}
