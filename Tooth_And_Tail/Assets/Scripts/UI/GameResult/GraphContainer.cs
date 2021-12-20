using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum GraphType { ArmyValue, FoodIncome };

public class GraphContainer : MonoBehaviour
{
    public GraphType graphType;

    [SerializeField]
    private Sprite cubeSprite;
    private RectTransform graphRect;
    private float graphWidth;     // 그래프 영역 너비
    private float graphHieght;    // 그래프 영역 높이

    private List<Snapshot> playerSnapshotList;
    private List<Snapshot> AISnapshotList;
    private int snapshotCount;

    private Color playerColor;
    private Color aiColor;

    private float yMaximum;
    private float xMaximum;
    private int xInterval;


    // Start is called before the first frame update
    void Start()
    {
    }

    private void Awake()
    {
        graphRect = this.GetComponent<RectTransform>();
        graphWidth = graphRect.sizeDelta.x * 0.95f;
        graphHieght = graphRect.sizeDelta.y * 0.9f;

        xInterval = 5;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetUpGraph()
    {
        graphRect = this.GetComponent<RectTransform>();
        graphWidth = graphRect.sizeDelta.x * 0.95f;
        graphHieght = graphRect.sizeDelta.y * 0.9f;

        // 데이터 로드
        playerSnapshotList = SceneStarter.Instance.statisticElements.playerSnapshotList;
        AISnapshotList = SceneStarter.Instance.statisticElements.AISnapshotList;
        snapshotCount = playerSnapshotList.Count;
        xMaximum = Mathf.RoundToInt(SceneStarter.Instance.statisticElements.gameTime);
        xInterval = 5;

        // 색상 지정
        switch (SceneStarter.Instance.statisticElements.campPlayer)
        {
            case Camp.Bellafide:
                playerColor = Global.GraphColorBellafide;
                playerColor.a = 0.75f;
                break;
            case Camp.Hopper:
                playerColor = Global.GraphColorHopper;
                playerColor.a = 0.75f;
                break;
            case Camp.Quartermaster:
                playerColor = Global.GraphColorQuartermaster;
                playerColor.a = 0.75f;
                break;
            case Camp.Archimedes:
                playerColor = Global.GraphColorArchimedes;
                playerColor.a = 0.75f;
                break;
        }
        switch (SceneStarter.Instance.statisticElements.campAI)
        {
            case Camp.Bellafide:
                aiColor = Global.GraphColorBellafide;
                aiColor.a = 0.75f;
                break;
            case Camp.Hopper:
                aiColor = Global.GraphColorHopper;
                aiColor.a = 0.75f;
                break;
            case Camp.Quartermaster:
                aiColor = Global.GraphColorQuartermaster;
                aiColor.a = 0.75f;
                break;
            case Camp.Archimedes:
                aiColor = Global.GraphColorArchimedes;
                aiColor.a = 0.75f;
                break;
        }

        // 그래프 생성
        switch (graphType)
        {
            case GraphType.ArmyValue:
                yMaximum = SceneStarter.Instance.statisticElements.maxValue;
                ArmyValueGraph();
                break;
            case GraphType.FoodIncome:
                yMaximum = SceneStarter.Instance.statisticElements.maxFood;
                FoodIncomeGraph();
                break;
        }
    }

    public void ClearGraph()
    {
        // 기존 자식 오브젝트 삭제
        Transform[] childList = GetComponentsInChildren<Transform>(true);
        if (childList != null)
        {
            for (int i = 0; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    Destroy(childList[i].gameObject);
            }
        }
    }

    private void ArmyValueGraph()
    {
        float xPosition = 0;
        float yPosition = 0;
        GameObject curPoint;

        // 플레이어 그래프
        GameObject prevPoint = CreatePoint(new Vector2(30, 20), playerColor);
        for (int i = 0; i < snapshotCount; i++)
        {
            if ((i + 1) * xInterval > xMaximum)
                break;

            /// 점 생성
            xPosition = graphWidth * ((float)((i + 1) * xInterval) / xMaximum) + 30;
            yPosition = graphHieght * playerSnapshotList[i].value / yMaximum + 20;
            if (0 == yMaximum)
                yPosition = 20;

            curPoint = CreatePoint(new Vector2(xPosition, yPosition), playerColor);
            /// 라인 생성
            CreateConnection(prevPoint.GetComponent<RectTransform>().anchoredPosition,
                             curPoint.GetComponent<RectTransform>().anchoredPosition, playerColor);
            prevPoint = curPoint;
        }

        /// 마지막 점
        xPosition = graphWidth + 30;
        yPosition = graphHieght * playerSnapshotList[snapshotCount - 1].value / yMaximum + 20;
        if (0 == yMaximum)
            yPosition = 20;
        curPoint = CreatePoint(new Vector2(xPosition, yPosition), playerColor);
        CreateConnection(prevPoint.GetComponent<RectTransform>().anchoredPosition,
                         curPoint.GetComponent<RectTransform>().anchoredPosition, playerColor);

        // AI 그래프
        prevPoint = CreatePoint(new Vector2(30, 20), aiColor);
        for (int i = 0; i < snapshotCount; i++)
        {
            if ((i + 1) * xInterval > xMaximum)
                break;

            /// 점 생성
            xPosition = graphWidth * ((float)((i + 1) * xInterval) / xMaximum) + 30;
            yPosition = graphHieght * AISnapshotList[i].value / yMaximum + 20;
            if (0 == yMaximum)
                yPosition = 20;

            curPoint = CreatePoint(new Vector2(xPosition, yPosition), aiColor);
            /// 라인 생성
            CreateConnection(prevPoint.GetComponent<RectTransform>().anchoredPosition,
                             curPoint.GetComponent<RectTransform>().anchoredPosition, aiColor);
            prevPoint = curPoint;
        }

        /// 마지막 점
        xPosition = graphWidth + 30;
        yPosition = graphHieght * AISnapshotList[snapshotCount - 1].value / yMaximum + 20;
        if (0 == yMaximum)
            yPosition = 20;
        curPoint = CreatePoint(new Vector2(xPosition, yPosition), playerColor);
        CreateConnection(prevPoint.GetComponent<RectTransform>().anchoredPosition,
                         curPoint.GetComponent<RectTransform>().anchoredPosition, aiColor);
    }

    private void FoodIncomeGraph()
    {
        float xPosition = 0;
        float yPosition = 0;
        GameObject curPoint;

        // 플레이어 그래프
        GameObject prevPoint = CreatePoint(new Vector2(30, 20), playerColor);
        for (int i = 0; i < snapshotCount; i++)
        {
            if ((i + 1) * xInterval > xMaximum)
                break;

            /// 점 생성
            xPosition = graphWidth * ((float)((i + 1) * xInterval) / xMaximum) + 30;
            yPosition = graphHieght * playerSnapshotList[i].food / yMaximum + 20;
            curPoint = CreatePoint(new Vector2(xPosition, yPosition), playerColor);
            /// 라인 생성
            CreateConnection(prevPoint.GetComponent<RectTransform>().anchoredPosition,
                             curPoint.GetComponent<RectTransform>().anchoredPosition, playerColor);
            prevPoint = curPoint;
        }

        /// 마지막 점
        xPosition = graphWidth + 30;
        yPosition = graphHieght * playerSnapshotList[snapshotCount - 1].food / yMaximum + 20;
        curPoint = CreatePoint(new Vector2(xPosition, yPosition), playerColor);
        CreateConnection(prevPoint.GetComponent<RectTransform>().anchoredPosition,
                         curPoint.GetComponent<RectTransform>().anchoredPosition, playerColor);

        // AI 그래프
        prevPoint = CreatePoint(new Vector2(30, 20), aiColor);
        for (int i = 0; i < snapshotCount; i++)
        {
            if ((i + 1) * xInterval > xMaximum)
                break;

            /// 점 생성
            xPosition = graphWidth * ((float)((i + 1) * xInterval) / xMaximum) + 30;
            yPosition = graphHieght * AISnapshotList[i].food / yMaximum + 20;
            curPoint = CreatePoint(new Vector2(xPosition, yPosition), aiColor);
            /// 라인 생성
            CreateConnection(prevPoint.GetComponent<RectTransform>().anchoredPosition,
                             curPoint.GetComponent<RectTransform>().anchoredPosition, aiColor);
            prevPoint = curPoint;
        }

        /// 마지막 점
        xPosition = graphWidth + 30;
        yPosition = graphHieght * AISnapshotList[snapshotCount - 1].food / yMaximum + 20;
        curPoint = CreatePoint(new Vector2(xPosition, yPosition), playerColor);
        CreateConnection(prevPoint.GetComponent<RectTransform>().anchoredPosition,
                         curPoint.GetComponent<RectTransform>().anchoredPosition, aiColor);
    }

    private GameObject CreatePoint(Vector2 anchoredPosition, Color color)
    {
        GameObject gameObject = new GameObject("point", typeof(Image));
        gameObject.transform.SetParent(this.GetComponent<RectTransform>(), false);
        gameObject.GetComponent<Image>().sprite = cubeSprite;
        gameObject.GetComponent<Image>().color = color;

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(3, 3);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        return gameObject;
    }

    private void CreateConnection(Vector2 PointA, Vector2 PointB, Color color)
    {
        GameObject gameObject = new GameObject("connection", typeof(Image));
        gameObject.transform.SetParent(this.GetComponent<RectTransform>(), false);
        gameObject.GetComponent<Image>().color = color;

        RectTransform connectionRect = gameObject.GetComponent<RectTransform>();

        Vector2 dir = (PointB - PointA).normalized;
        float distance = Vector2.Distance(PointA, PointB);

        connectionRect.sizeDelta = new Vector2(distance, 3f);
        connectionRect.anchorMin = new Vector2(0, 0);
        connectionRect.anchorMax = new Vector2(0, 0);
        connectionRect.anchoredPosition = PointA + dir * distance * 0.5f;

        connectionRect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }
}
