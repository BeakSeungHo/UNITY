using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 
///  Name : 백승호
///  Date : 2020-08-31
///  
///  Desc.
///     인게임 플레이어 조작을 하기위한 조이스틱 코드
/// 
/// </summary>
public class JoyStickCtrl : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private static JoyStickCtrl instance = null;

    public RectTransform Stick;
    public Camera camera;
    Vector2 Center;
    Vector2 Dir;
    float radius;
    int TouchIndex;

    Commander Commander;

    void Awake()
    {
        if (null != Instance)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        radius = GetComponent<RectTransform>().sizeDelta.y / 2;
        Center = Stick.position;
    }
    
    public void SetCommander(Commander Player)
    {
        Commander = Player;
    }

    // 조이스틱 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 TouchPos = eventData.position;
        TouchPos = camera.ScreenToWorldPoint(TouchPos);
        Dir = (TouchPos - Center).normalized;

        float Distance = Vector2.Distance(TouchPos, Center);
        if (Distance > radius)
            Stick.position = Center + Dir * radius;
        else
            Stick.position = Center + Dir * Distance;

        Stick.localPosition = new Vector3(Stick.localPosition.x, Stick.localPosition.y, 0);

        if (Commander != null)
            Commander.Move_Commander(Dir);
        else
            Debug.Log("Failed Find Commander");

        if (GameManager.Instance.CurGameMode == GameMode.Tutorial)
        {
            if(CampaignManager.Instance.curWave() == -1)
            {
                SceneStarter.Instance.userElements.AddMissionCount(MissionType.None, 0, 1);
                if (SceneStarter.Instance.userElements.CompleteMission(MissionType.None, 0))
                {
                    CampaignManager.Instance.GenNextObjectsAfterSecond();
                    CampaignManager.Instance.DestroyObstacle();
                }
            }
        }
    }

    // 조이스틱 이동
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 TouchPos = eventData.position;
        TouchPos = camera.ScreenToWorldPoint(TouchPos);
        Dir = (TouchPos - Center).normalized;

        float Distance = Vector2.Distance(TouchPos, Center);
        if (Distance > radius)
            Stick.position = Center + Dir * radius;
        else
            Stick.position = Center + Dir * Distance;

        Stick.localPosition = new Vector3(Stick.localPosition.x, Stick.localPosition.y, 0);

        if (Commander != null)
            Commander.Move_Commander(Dir);
        else
            Debug.Log("Failed Find Commander");
    }

    // 조이스틱 이동 끝
    public void OnEndDrag(PointerEventData eventData)
    {
        Dir = Vector2.zero;
        Stick.position = Center;
        
        Stick.localPosition = new Vector3(Stick.localPosition.x, Stick.localPosition.y, 0);

        if (Commander != null)
            Commander.Move_Commander(Dir);
        else
            Debug.Log("Failed Find Commander");
    }

    

    public static JoyStickCtrl Instance
    {
        get
        {
            if (null == instance)
                return null;
            else
                return instance;
        }
    }

}