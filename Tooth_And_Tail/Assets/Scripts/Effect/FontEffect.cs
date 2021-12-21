using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FontEffect : MonoBehaviour
{
    public enum FONTTYPE
    {
        MINUSFOOD, PLUSFOOD, DAMAGE
    };
    private FONTTYPE Type;

    public GameObject[] FontParticleObj;

    private GameObject TargetPos;
    private float LifeTime;


    private MinusFoodEffect MinusFoodEffect;
    private PlusFoodEffect PlusFoodEffect;
    private DamageNumEffect DamageNumEffect;

    // Start is called before the first frame update
    void Start()
    {

    }
    public void Ready(GameObject Target, int NumIndex, FONTTYPE Type)
    {
        TargetPos = Target;
        this.Type = Type;

        LifeTime = 0f;

        switch (Type)
        {
            case FONTTYPE.DAMAGE:
                DamageNumEffect = transform.GetChild((int)Type).GetComponent<DamageNumEffect>();
                DamageNumEffect.Damage = NumIndex;
                transform.position = new Vector3(TargetPos.transform.position.x, TargetPos.transform.position.y + 0.5f, TargetPos.transform.position.z);
                break;
            case FONTTYPE.PLUSFOOD:
                PlusFoodEffect = transform.GetChild((int)Type).GetComponent<PlusFoodEffect>();
                PlusFoodEffect.PlusFood = NumIndex;
                transform.position = TargetPos.transform.position;
                PlusFoodEffect.Ready();
                break;
            case FONTTYPE.MINUSFOOD:
                MinusFoodEffect = transform.GetChild((int)Type).GetComponent<MinusFoodEffect>();
                MinusFoodEffect.MinusFood = NumIndex;
                transform.position = TargetPos.transform.position;
                MinusFoodEffect.Ready();
                break;
        }
        FontParticleObj[(int)Type].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        switch (Type)
        {
            case FONTTYPE.DAMAGE:
                LifeTime += Time.deltaTime;
                if (DamageNumEffect.LifeTime <= LifeTime)
                {
                    FontParticleObj[(int)Type].SetActive(false);
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.FontEffect);
                }
                else
                {
                    DamageNumEffect.Acc += 0.5f * Time.deltaTime;
                    transform.Translate(0, DamageNumEffect.Speed * DamageNumEffect.ConvertGravity(DamageNumEffect.Power, DamageNumEffect.Acc), 0);
                    DamageNumEffect.text.color = new Color(1, 1, 1, 1 - ((LifeTime / DamageNumEffect.LifeTime)));
                }
                break;
            case FONTTYPE.PLUSFOOD:
                LifeTime += Time.deltaTime;
                if (PlusFoodEffect.LifeTime <= LifeTime)
                {
                    FontParticleObj[(int)Type].SetActive(false);
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.FontEffect);
                }
                else
                {
                    transform.Translate(0, PlusFoodEffect.Speed * Time.deltaTime, 0);
                    PlusFoodEffect.text.color = new Color(1, 1, 1, 1 - ((LifeTime / PlusFoodEffect.LifeTime)));
                }
                break;
            case FONTTYPE.MINUSFOOD:
                LifeTime += Time.deltaTime;
                if (MinusFoodEffect.LifeTime <= LifeTime)
                {
                    FontParticleObj[(int)Type].SetActive(false);
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.FontEffect);
                }
                else
                {
                    transform.Translate(0, MinusFoodEffect.Speed * Time.deltaTime, 0);
                    MinusFoodEffect.text.color = new Color(1, 1, 1, 1 - ((LifeTime / MinusFoodEffect.LifeTime)));
                }
                break;
        }
    }
}
