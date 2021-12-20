using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardProjectile : ProjectileBase
{
    public Vector3 gravityDir = new Vector3(0f, -1f, 0f);
    public Vector3 upforceDir = new Vector3(0f, 1f, 0f);

    private float gravity = 9.8f * 2f;
    private float upforce = 1f;
    private float accTime = 0f;
    private float flyingTime = 0f;



    public override void Ready(Vector3 startPos, float damage, float speed, GameObject target)
    {
        base.Ready(startPos, damage, speed, target);

        accTime = 0f;

        //  Calculate Upforce
        Vector3 delta = DestPos - startPos;
        flyingTime = 1 / Speed;

        Speed = delta.magnitude / flyingTime;

        upforce = gravity * 0.5f * flyingTime;
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

        float angle = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), sumDir).eulerAngles.z;
        gameObject.transform.eulerAngles = new Vector3(0f, 0f, angle);

        gameObject.transform.position += sumDir * Time.deltaTime;

        if (accTime >= flyingTime)
        {
            //  Hit Function
            if (target.activeSelf)
                Hit();

            PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_Normal);
            return;
        }
    }
}
