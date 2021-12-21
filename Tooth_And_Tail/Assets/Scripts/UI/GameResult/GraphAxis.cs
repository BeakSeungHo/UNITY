using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GraphAxis : MonoBehaviour
{
    //
    public StatisticsWindow MaterStat;

    // Vertical
    public TextMeshProUGUI verMin;
    public TextMeshProUGUI verMid;
    public TextMeshProUGUI verMax;


    [SerializeField]
    private RectTransform timeTemplate;
    [SerializeField]
    private RectTransform lineTemplate;
    private RectTransform thisRect;
    private float thisWidth;     // 영역 너비

    private float gameTime;
    private int graphTimeInterval;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {
        thisRect = this.GetComponent<RectTransform>();
        thisWidth = thisRect.sizeDelta.x - 50;
    }

    // 축 초기화
    public void SetUpAxis()
    {
        thisRect = this.GetComponent<RectTransform>();
        thisWidth = thisRect.sizeDelta.x - 50;


        gameTime = Mathf.RoundToInt(SceneStarter.Instance.statisticElements.gameTime);

        // Vertical
        verMin.text = "0";


        // GraphTimeInterval
        if (gameTime <= 180)
            graphTimeInterval = 30;
        else if (gameTime <= 360)
            graphTimeInterval = 60;
        else if (gameTime <= 600)
            graphTimeInterval = 120;
        else if (gameTime > 1200)
            graphTimeInterval = 240;


        // Horizontal
        for (int i = 1; i <= gameTime / graphTimeInterval; i++)
        {
            // 축 라인 생성
            RectTransform labelLine = Instantiate(lineTemplate);
            labelLine.SetParent(this.transform.GetChild(2).GetComponent<RectTransform>());
            labelLine.localScale = new Vector3(1, 1, 1);
            labelLine.localPosition = new Vector3(0, 0, 0);
            labelLine.offsetMax = new Vector2(labelLine.offsetMax.x, 0);
            labelLine.offsetMin = new Vector2(labelLine.offsetMin.x, 0);
            labelLine.gameObject.SetActive(true);

            float xPosition = thisWidth * i * graphTimeInterval / gameTime + 30;
            labelLine.anchoredPosition = new Vector2(xPosition, 0);


            // 시간 텍스트 생성
            RectTransform labelX = Instantiate(timeTemplate);
            labelX.SetParent(this.transform.GetChild(1).GetComponent<RectTransform>());
            labelX.localScale = new Vector3(1, 1, 1);
            labelX.localPosition = new Vector3(0, 0, 0);
            labelX.gameObject.SetActive(true);

            xPosition = thisWidth * i * graphTimeInterval / gameTime + 30;
            labelX.anchoredPosition = new Vector2(xPosition, 0);

            // 시간 표시
            int min = (i * graphTimeInterval) / 60;
            int sec = (i * graphTimeInterval) % 60;

            if (sec < 10)
                labelX.GetComponent<TextMeshProUGUI>().text = min.ToString() + ":0" + sec.ToString();
            else
                labelX.GetComponent<TextMeshProUGUI>().text = min.ToString() + ":" + sec.ToString();
        }

        if (gameTime < graphTimeInterval)
        {
            // 축 라인 생성
            RectTransform labelLine = Instantiate(lineTemplate);
            labelLine.SetParent(this.transform.GetChild(2).GetComponent<RectTransform>());
            labelLine.localScale = new Vector3(1, 1, 1);
            labelLine.localPosition = new Vector3(0, 0, 0);
            labelLine.offsetMax = new Vector2(labelLine.offsetMax.x, 0);
            labelLine.offsetMin = new Vector2(labelLine.offsetMin.x, 0);
            labelLine.gameObject.SetActive(true);

            float xPosition = thisWidth + 30;
            labelLine.anchoredPosition = new Vector2(xPosition, 0);


            // 시간 텍스트 생성
            RectTransform labelX = Instantiate(timeTemplate);
            labelX.SetParent(this.transform.GetChild(1).GetComponent<RectTransform>());
            labelX.localScale = new Vector3(1, 1, 1);
            labelX.localPosition = new Vector3(0, 0, 0);
            labelX.gameObject.SetActive(true);

            xPosition = thisWidth + 30;
            labelX.anchoredPosition = new Vector2(xPosition, 0);

            // 시간 표시
            int min = (int)gameTime / 60;
            int sec = (int)gameTime % 60;

            if (sec < 10)
                labelX.GetComponent<TextMeshProUGUI>().text = min.ToString() + ":0" + sec.ToString();
            else
                labelX.GetComponent<TextMeshProUGUI>().text = min.ToString() + ":" + sec.ToString();
        }
    }

    // 세로축 갱신
    public void ChangeVerAxis()
    {
        // 부대 가치
        if (MaterStat.armyValueTab.isOn)
        {
            verMax.text = SceneStarter.Instance.statisticElements.maxValue.ToString();
            verMid.text = (SceneStarter.Instance.statisticElements.maxValue / 2).ToString();
        }
        // 식량
        else if (MaterStat.foodIncomeTab.isOn)
        {
            verMax.text = SceneStarter.Instance.statisticElements.maxFood.ToString();
            verMid.text = (SceneStarter.Instance.statisticElements.maxFood / 2).ToString();
        }
    }
}
