using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase
{
    public GameObject gameObject = null;
    protected Projectile projectile = null;

    public float Speed = 10f;
    public float Damage = 10f;

    protected Vector3 moveDir = Vector3.zero;
    public Vector3 DestPos = Vector3.zero;
    
    protected GameObject target = null;

    public virtual void Initialize(GameObject gameObject)
    {
        this.gameObject = gameObject;
        projectile = gameObject.GetComponent<Projectile>();
    }

    public virtual void Ready(Vector3 startPos, float damage, float speed, GameObject target)
    {
        this.target = target;

        gameObject.transform.position = new Vector3 ( startPos.x ,startPos.y, 1f );

        Character character = target.GetComponent<Character>();
        if (null != character)
            DestPos = character.HitPosition;
        else
        {
            DestPos = target.transform.position;
        }

        Vector3 delta = DestPos - startPos;
        delta.z = 0f;
        moveDir = delta.normalized;
        Speed = speed;
        Damage = damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public virtual void Update()
    {
        
    }

    public virtual void Hit()
    {
        //  타겟 정보 가져오기.
        CommonBase targetBase = target.GetComponent<CommonBase>();

        //  타겟 예외 처리.
        if (null == targetBase)
        {
            Debug.Log("BulletManager.Hit Failed. targetBase is null");
            return;
        }
        CommonType targetType = targetBase.Data.CommonType;
        
        
        Character character = target.GetComponent<Character>();

        //  캐릭터 예외 처리.
        if (null == character)
        {
            Debug.Log("BuildManager.Hit failed. target is not character");
            return;
        }

        if (CommonType.Snake == projectile.ShooterType)
        {
            character.BuffDebuff.Add_Poison(projectile.ShotCharacter);
            //SnakeAttack Effect
            EffectManager.Instance.EffectEnable(this.gameObject, ParticleObject.PARTICLETYPE.SNAKEATTACK);
        }
        else if (CommonType.Pigeon == projectile.ShooterType)
            character.BuffDebuff.Add_Heal(projectile.ShotCharacter);
        else
        {
            character.Hit(Damage, projectile.ShotCharacter);
            //Hit Effect
            EffectManager.Instance.EffectEnable(this.gameObject, ParticleObject.PARTICLETYPE.HIT);
        }

        
    }

    protected void Play_ExplosionSound()
    {
        var unitSoundDic = SceneStarter.Instance.soundElements.UnitSoundDic;
        var type = projectile.ShotCharacter.Base.Type;

        if (!unitSoundDic.ContainsKey(type))
        {
            Debug.Log("there is no " + type + " key in UnitSoundDic");
            return;
        }

        if (!unitSoundDic[type].ContainsKey(UnitSoundType.Explosion))
        {
            Debug.Log("there is no " + UnitSoundType.Explosion + " key in UnitSoundDic[" + type + "]");
            return;
        }

        var audioArray = unitSoundDic[type][UnitSoundType.Explosion];
        int index = Random.Range(0, audioArray.Count);

        SoundManager.Instance.Play(Sound_Channel.Effect, DestPos, audioArray[index]);
    }

}
