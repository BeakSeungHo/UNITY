using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DamageFontObject : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float Damage;
    public bool UpFlag;
    public bool DownFlag;
    public bool ActiveFlag;
    float y;
    float Speed = 15f;
    float MinY = 0f;
    float MaxY = 0f;
    public RectTransform BackGround;
    // Start is called before the first frame update
    void Start()
    {
    }
    public void Ready(int type)
    {
        DownFlag = false;
        ActiveFlag = false;
        Damage = 0;
        Speed = 250f;
        //transform.localPosition = new Vector3(0, 25f, 0);
        if (type < 15 || type == 29 || type == 25)
        {
            MinY = transform.localPosition.y;
            MaxY = 60f + Mathf.Abs(BackGround.offsetMax.y);
        }
        else
        {
            MinY = transform.localPosition.y;
            MaxY = 60f + Mathf.Abs(BackGround.offsetMax.y) - 30f;
        }
        y = MinY;
    }
    void LateUpdate()
    {
        text.text = Damage.ToString("#.##");
        if (ActiveFlag)
        {
            if (!DownFlag && UpFlag)
            {
                y += Time.deltaTime * Speed;
                if (y >= MaxY)
                {
                    y = MaxY;
                    UpFlag = false;
                }
            }
            else if (DownFlag && !UpFlag)
            {
                y -= Time.deltaTime * Speed;
                if (y <= MinY)
                {
                    ActiveFlag = false;
                    DownFlag = false;
                    Damage = 0;
                    y = MinY;
                }
            }
            transform.localPosition = new Vector3(0, y, 0);
        }
    }
}
