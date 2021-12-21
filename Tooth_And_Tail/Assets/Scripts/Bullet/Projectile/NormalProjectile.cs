using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalProjectile : ProjectileBase
{
    public override void Ready(Vector3 startPos, float damage, float speed, GameObject target)
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
        float angle = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), moveDir).eulerAngles.z;
        gameObject.transform.eulerAngles = new Vector3(0f, 0f, angle);

        gameObject.transform.position += moveDir * Time.deltaTime * Speed;
        Vector3 delta = DestPos - gameObject.transform.position;
        if (moveDir.x * delta.x < 0 || moveDir.y * delta.y < 0 || moveDir.z * delta.z < 0)
        {
            if (target.activeSelf)
                Hit();

            PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_Normal);
            return;
        }

    } 
}
