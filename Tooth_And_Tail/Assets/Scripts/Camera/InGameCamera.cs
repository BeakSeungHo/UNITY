using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCamera : MonoBehaviour
{
    public GameObject Player;
    public Vector3 TempEventPos;
    public float Speed;
    public bool EventFlag;
    public Camera Cam;
    public Camp targetCamp = Camp.End;
    private Camp preTargetCamp = Camp.End;
    float EventTime = 0f;
    // Start is called before the first frame update
    void Start()
    {
        EventFlag = false;
        Cam = GetComponent<Camera>();
        targetCamp = GameManager.Instance.CommanderList[0];
    }

    // Update is called once per frame
    void Update()
    {
        Change_Target();

        if (!EventFlag)
            TargetPlayer();
        else
        {
            CameraEvent();
            EventTime += Time.deltaTime * 2f;
            if (EventTime > 2f)
            {
                EventTime = 0f;
                EventFlag = false;
                Time.timeScale = 1f;
            }
        }
        if (FogOfWar.Instance.MainCamera == null)
            FogOfWar.Instance.MainCamera = Cam;
    }

    void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    void Change_Target()
    {
        if (preTargetCamp == targetCamp)
            return;

        preTargetCamp = targetCamp;

        Player = InGameManager.Instance.Commanders[targetCamp].gameObject;
    }

    void TargetPlayer()
    {
        transform.position = Vector3.Lerp(transform.position, Player.transform.position, Speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
        //this.transform.position =new Vector3( Player.transform.position.x, Player.transform.position.y, Player.transform.position.z - 10f);

    }
    void CameraEvent()
    {
        transform.position = Vector3.Lerp(transform.position, TempEventPos, Speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, -10f);
    }
    public void EventMove(Vector3 Pos)
    {
        TempEventPos = Pos;
        EventFlag = true;
        Time.timeScale = 0.5f;
    }
}