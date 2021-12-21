using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonProjectile : FerretProjectile
{
    public override void Ready(Vector3 startPos, float damage, float speed, GameObject target)
    {
        BaseReady(startPos, damage, speed, target);

        accTime = 0f;

        Vector2Int destGridPos = TilemapSystem.Instance.WorldToTilePos(DestPos);

        DestPos = TilemapSystem.Instance.GetTile(destGridPos).worldPosition;

        // Calculate Upforce
        Vector3 delta = DestPos - startPos;
        flyingTime = 1 / Speed;

        Speed = delta.magnitude / flyingTime;

        upforce = gravity * 0.5f * flyingTime;

        EffectManager.Instance.SpriteEffectEnable(this.gameObject, SpriteEffect.SPRITETYPE.EXPLOSIONPOINT);
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

            if (null != tileHitObject)
                tileHitObject.Ready_Ferret(projectile.Camp, Damage, 1, DestPos, projectile.ShotCharacter);

            PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_Normal);

            return;
        }
    }
}
