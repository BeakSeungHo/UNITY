using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShadow : MonoBehaviour
{
    public SpriteRenderer SpriteRenderer;
    public SpriteRenderer TargetSpriteRenderer;
    public GameObject Parent;
    // Start is called before the first frame update
    void Start()
    {
        //Camera.transparencySortAxis.Equals(this);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        SpriteRenderer.sprite = TargetSpriteRenderer.sprite;
        float minLength = 10000f;
        float tempY = 0f;

        for(int i=0; i< ShadowManager.Instance.ShadowObjPos.Count;i++)
        {
            float length = Vector3.Distance(Parent.transform.position, ShadowManager.Instance.ShadowObjPos[i]);
            if (length < minLength)
            {
                minLength = length;
                tempY = ShadowManager.Instance.ShadowObjPos[i].y;
            }
        }

        if (tempY < Parent.transform.position.y)
            SpriteRenderer.sortingOrder = 10;
        else
            SpriteRenderer.sortingOrder = 4;

        //전장의 안개 체크용
        FogOfWar.Instance.CheckSprite(transform.position, SpriteRenderer);
    }

}
