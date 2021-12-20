using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffDebuff : MonoBehaviour
{
    public Character character = null;

    private float stimTimeRemaining = 0f;

    private float healDPS = 1f;

    private float poisonDPS = 1f;

    public int HealStack = 0;
    public int PoisonStack = 0;

    public bool Stim = false;

    public void Add_Poison(Character shotCharacter)
    {
        //  Poison Effect
        if (PoisonStack == 0)
            EffectManager.Instance.EffectEnable(this.gameObject, ParticleObject.PARTICLETYPE.POSION);

        GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_TickHit);

        TickHitObject tickHitObject = pullObject?.GetComponent<TickHitObject>();
        tickHitObject?.Ready(shotCharacter, 5, poisonDPS, character);
    }

    public void Add_Heal(Character shotCharacter)
    {
        //  Heal Effect
        if (HealStack == 0)
            EffectManager.Instance.EffectEnable(this.gameObject, ParticleObject.PARTICLETYPE.HEAL);

        GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_TickHit);

        TickHitObject tickHitObject = pullObject?.GetComponent<TickHitObject>();
        tickHitObject?.Ready(shotCharacter, 5, -healDPS, character);
    }

    public void Add_Stim()
    {
        if (!Stim)
            EffectManager.Instance.EffectEnable(this.gameObject, ParticleObject.PARTICLETYPE.STIM);

        Stim = true;
        stimTimeRemaining = 12f;
    }

    public void StackClear()
    {
        HealStack = 0;
        PoisonStack = 0;
        Stim = false;
    }

    private void OnEnable()
    {
        HealStack = 0;
        PoisonStack = 0;
    }

    // Start is called before the first frame update
    void Start()
    {
        CommonBase commonBase = gameObject.GetComponent<CommonBase>();
        if (null == commonBase)
        {
            Debug.Log("Poison : commonBase is null");
            return;
        }

        poisonDPS = commonBase.GetData(CommonType.Snake).Damage;
        healDPS = commonBase.GetData(CommonType.Pigeon).Damage;
    }

    // Update is called once per frame
    void Update()
    {

        if (Stim)
        {

            stimTimeRemaining -= Time.deltaTime;
            if (stimTimeRemaining <= 0f)
            {
                stimTimeRemaining = 0f;
                Stim = false;
            }

        }
    }

}
