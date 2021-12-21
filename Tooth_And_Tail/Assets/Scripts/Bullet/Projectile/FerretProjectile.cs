using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FerretProjectile : ProjectileBase
{
    public Vector3 gravityDir = new Vector3(0f, -1f, 0f);
    public Vector3 upforceDir = new Vector3(0f, 1f, 0f);

    protected float gravity = 9.8f * 0.5f;
    protected float upforce = 1f;
    protected float accTime = 0f;
    protected float flyingTime = 0f;

    public override void Ready(Vector3 startPos, float damage, float speed, GameObject target)
    {
        base.Ready(startPos, damage, speed, target);

        var node = TilemapSystem.Instance.GetTile(target.transform.position);
        if (null == node)
            Debug.Log("node is null");
        else
        {
            DestPos = node.worldPosition;
        }

        accTime = 0f;

        //  Calculate Upforce
        Vector3 delta = DestPos - startPos;
        flyingTime = 1 / Speed;

        Speed = delta.magnitude / flyingTime;

        upforce = gravity * 0.5f * flyingTime;

        EffectManager.Instance.SpriteEffectEnable(this.gameObject, SpriteEffect.SPRITETYPE.EXPLOSIONPOINT);
    }

    public void BaseReady(Vector3 startPos, float damage, float speed, GameObject target)
    {
        base.Ready(startPos, damage, speed, target);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        accTime += Time.deltaTime;
        Vector3 moveDelta = (upforce * upforceDir) + (gravity * accTime * gravityDir);
        Vector3 sumDir = (moveDelta + moveDir * Speed);

        float angle = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), sumDir.normalized).eulerAngles.z;
        gameObject.transform.eulerAngles = new Vector3(0f, 0f, angle);

        gameObject.transform.position += sumDir * Time.deltaTime;

        Vector3 pos = gameObject.transform.position;
        
        //gameObject.transform.position.

        if (accTime >= flyingTime)
        {
            //  이펙트 추가 부분

            EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.EXPLOSION);
            //  Hit Function
            GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_HitObject);
            //HitObject hitObject = pullObject.GetComponent<HitObject>();
            TileHitObject tileHitObject = pullObject.GetComponent<TileHitObject>();
            //HitObject hitObject = pullObject.GetComponent<HitObject>();

            //if (null != hitObject)
            if (null != tileHitObject)
            {
                //hitObject.Ready(HitType.Immediate, projectile.Camp, false, destPos, 0.5f, Damage, 0f, 2);
                //tileHitObject.Ready(projectile.Camp, Damage, 1, true, 0f, destPos);
                tileHitObject.Ready_Ferret(projectile.Camp, Damage, 1, DestPos, projectile.ShotCharacter);
            }

            Play_ExplosionSound();

            PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_Normal);
            return;
        }
    }
}
