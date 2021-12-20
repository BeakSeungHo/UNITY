using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteEffect : MonoBehaviour
{
    public enum SPRITETYPE
    {
        EXPLOSIONPOINT
    };
    private SPRITETYPE Type;

    public GameObject[] SpriteEffectObj;

    private GameObject TargetPos;
    private float LifeTime;

    private Projectile CannonObject;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void Ready(GameObject Target, SPRITETYPE Type)
    {
        TargetPos = Target;
        this.Type = Type;

        LifeTime = 0f;

        switch (Type)
        {
            case SPRITETYPE.EXPLOSIONPOINT:
                Debug.Log(Target);
                CannonObject = Target.GetComponent<Projectile>();
              
                transform.position = CannonObject.DestPos;
                break;
        }
        SpriteEffectObj[(int)Type].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (Type)
        {
            case SPRITETYPE.EXPLOSIONPOINT:
                if (TargetPos.gameObject.activeInHierarchy == false)
                {
                    SpriteEffectObj[(int)Type].SetActive(false);
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.SpriteEffect);
                }
                break;
        }
    }
}
