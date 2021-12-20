using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TickHitObject : MonoBehaviour
{
    private Pool_ObjType poolType = Pool_ObjType.Bullet_TickHit;

    private float timeCount = 0f;
    private float maxTime = 0f;

    public float DamagePerSecond = 0f;

    [HideInInspector]   public Character ShotCharacter = null;

    public Character targetCharacter = null;

    private bool heal = false;

    public void Ready(Character shotCharacter, float maxTime, float dPS, Character targetCharacter)
    {
        ShotCharacter = shotCharacter;

        timeCount = 0;

        this.maxTime = maxTime;
        this.targetCharacter = targetCharacter;

        if (dPS < 0)
        {
            heal = true;
            DamagePerSecond = -dPS;


            ++targetCharacter.HealStack;
        }
        else
        {
            heal = false;
            DamagePerSecond = dPS;


            ++targetCharacter.PoisonStack;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //  종료 체크
        if (EndCheck())
        {
            PoolManager.Instance.PushObject(gameObject, poolType);
            if (heal)
                --targetCharacter.HealStack;
            else
                --targetCharacter.PoisonStack;
            return;
        }

        timeCount += Time.deltaTime;

        if (heal)
            targetCharacter.Heal(DamagePerSecond * Time.deltaTime, ShotCharacter);
        else
            targetCharacter.Hit(DamagePerSecond * Time.deltaTime, ShotCharacter);
    }

    /// <summary>
    /// 종료 체크
    /// </summary>
    /// <returns>true 종료, false 아직 진행 중</returns>
    private bool EndCheck()
    {
            //  시간 체크
        if (timeCount >= maxTime ||
            //  타겟이 비활성화 됨.
            !targetCharacter.activeSelf)
            return true;

        return false;
    }
    

    private void Deal()
    {
    }
}