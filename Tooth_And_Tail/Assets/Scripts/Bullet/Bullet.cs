using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// 작성자 : 김영현 (20-09-02)
/// 
/// 풀 매니저에서 뽑아 쓸 총알 클래스
/// 
/// 총알을 쏜 캐릭터의 타입, 날아가는 방향, 타겟, 속도, 데미지 정보를 담고 있다.
/// 
/// 
/// 
/// 
/// </summary>

public enum BulletType { Straight, Parabola, End };

public class Bullet : MonoBehaviour
{
    public GameElements gameElements = SceneStarter.Instance.gameElements;

    public Camp myCamp = Camp.Hopper;

    public float    Speed = 10f;
    public float    Damage = 10f;

    public CommonType ShooterType = CommonType.End;
    public BulletType Type = BulletType.End;

    private Vector3 moveDir = Vector3.zero;
    private Vector3 destPos = Vector3.zero;

    private GameObject target = null;

    static private Vector3 gravityDir = new Vector3(0f, -1f, 0f);
    static private Vector3 upforceDir = new Vector3(0f, 1f, 0f);

    private float gravity       = 9.8f * 0.5f;
    private float upforce       = 1f;
    private float accTime       = 0f;
    private float flyingTime    = 0f;

    public void Ready(Vector3 startPos, float speed, CommonType type, GameObject target)
    {
        ShooterType = type;
        this.target = target;

        transform.position = startPos;
        destPos = target.transform.position;

        Vector3 delta = destPos - startPos;
        delta.z = 0f;
        moveDir = delta.normalized;
        Speed = speed;

        accTime = 0f;
        
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        switch (type)
        {
            case CommonType.Squirrel:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Pistol];
                Type = BulletType.Straight;
                break;
            case CommonType.Snake:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Venom];
                Type = BulletType.Straight;
                break;
            case CommonType.Ferret:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Artillery];
                Type = BulletType.Parabola;
                Calculate_Upforce(startPos);
                break;
        }

    }

    private void Calculate_Upforce(Vector3 startPos)
    {
        Vector3 delta = destPos - startPos;
        flyingTime = 1 / Speed;

        Speed = delta.magnitude / flyingTime;

        upforce = gravity * 2f / flyingTime;
    }

    private void Update()
    {
        if (null == target)
            return;

        Move();

        switch (Type)
        {
            case BulletType.Straight:
                Vector3 delta = destPos - transform.position;
                if (moveDir.x * delta.x < 0 || moveDir.y * delta.y < 0 || moveDir.z * delta.z < 0)
                {
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_Normal);
                    return;
                }
                break;
            case BulletType.Parabola:
                if (accTime >= flyingTime)
                {
                    
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_Normal);
                    return;
                }
                break;
        }
    }

    private void Move()
    {
        switch (ShooterType)
        {
            //  직선
            case CommonType.Squirrel:
            case CommonType.Lizard:
            case CommonType.Pigeon:
            case CommonType.Falcon:
            case CommonType.Badger:
            case CommonType.Fox:
            case CommonType.Snake:
                transform.position += moveDir * Time.deltaTime * Speed;
                break;
            //  포물선
            case CommonType.Ferret:
            case CommonType.Skunk:
                accTime += Time.deltaTime;
                Vector3 moveDelta = (upforce * upforceDir) + (gravity * accTime * gravityDir) ;
                Vector3 sumDir = (moveDelta + moveDir * Speed);

                float angle = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), sumDir).eulerAngles.z;
                transform.eulerAngles = new Vector3(0f, 0f, angle);
                
                transform.position += sumDir * Time.deltaTime;
                break;
        }
    }
    
}
