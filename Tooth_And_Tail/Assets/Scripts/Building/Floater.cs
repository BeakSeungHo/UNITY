using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floater : MonoBehaviour
{
    protected float floatingTime = 1.5f;
    protected float curFloatingTime = 0f;

    protected Vector3 floatingDir = new Vector3(0, -0.7f, 0);

    protected Transform body = null;

    protected bool isInTheAir = false;

    Vector3 groundPos;

    void Floating()
    {
        curFloatingTime += Time.deltaTime;
        if (!isInTheAir)
        {
            body.Translate(-(floatingDir * 1.2f) * Time.deltaTime);
            if (curFloatingTime >= floatingTime)
            {
                floatingTime = 0.7f;
                curFloatingTime = 0f;
                isInTheAir = true;
            }
        }
        else
        {
            body.Translate(floatingDir * Time.deltaTime);
            if (curFloatingTime >= floatingTime)
            {
                floatingDir *= -1f;
                curFloatingTime = 0f;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        body = transform.parent.Find("Body");
        groundPos = body.transform.position;
    }

    void OnDisable()
    {
        Vector3 pos = body.transform.position - groundPos;
        body.transform.position -= pos;
    }

    // Update is called once per frame
    void Update()
    {
        Floating();
    }
}
