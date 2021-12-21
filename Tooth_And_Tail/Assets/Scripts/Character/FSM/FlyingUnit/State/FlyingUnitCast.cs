using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingUnitCast : FSM<FlyingUnitFSM>
{
    private FlyingUnitFSM ownerFSM;

    private bool isFired = false;

    private int fireCount = 0;
    private float falconFireCoolDown = 0f;
    private float falconFireCoolTime = 1.75f / 10f;
    private float fireMax = 10;

    public FlyingUnitCast(FlyingUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = FlyingUnitFSM.STATE.IDLE;
        ownerFSM.TimeCount = 0f;
        isFired = false;
        fireCount = 0;
        falconFireCoolDown = 0f;

        if (CommonType.Owl == ownerFSM.Unit.Base.Type)
        {
            ownerFSM.Animator.SetBool("Cast", true);

            //  소리 추가
            ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);
        }
    }

    public override void Run()
    {
        switch (ownerFSM.Data.CommonType)
        {
            case CommonType.Pigeon:
                Action_Pigeon();
                break;
            case CommonType.Falcon:
                Action_Falcon();
                break;
            case CommonType.Owl:
                Action_Owl();
                break;
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = FlyingUnitFSM.STATE.IDLE;

        if (CommonType.Owl == ownerFSM.Unit.Base.Type)
            ownerFSM.Animator.SetBool("Cast", false);
    }

    private void Action_Pigeon()
    {
        //if (ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);
        //    return;
        //}

        ownerFSM.TimeCount += Time.deltaTime;

        if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.ATTACK_IDLE);
        }

        if (!isFired)
        {
            isFired = true;
            if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
            {
                GameObject projectileObj = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_Normal);

                Projectile projectile = projectileObj.GetComponent<Projectile>();
                if (null == projectile)
                {
                    Debug.Log("FlyingUnitCast : projectile is null");
                    return;
                }

                Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

                if (ownerFSM.Unit.facingRight != delta.x >= 0)
                    ownerFSM.Unit.Flip();

                projectile.Ready(ownerFSM.Unit, ownerFSM.Base.MyCamp, ownerFSM.Unit.FirePosition, ownerFSM.Base.Damage, ownerFSM.Base.ProjectileSpeed, ownerFSM.AttackTarget.gameObject);

                ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);

                //BulletManager.Instance.Fire_Bullet(ownerFSM.Unit.gameObject, ownerFSM.AttackTarget);   
            }
        }
    }

    private void Action_Falcon()
    {
        //if (ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);
        //    return;
        //}

        ownerFSM.TimeCount += Time.deltaTime;

        if (fireCount < fireMax)
        {
            falconFireCoolDown += Time.deltaTime;

            if (falconFireCoolDown >= falconFireCoolTime)
            {
                falconFireCoolDown -= falconFireCoolTime;
                isFired = false;
            }
        }
        else
        {
            ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.ATTACK_IDLE);
        }

        if (!isFired)
        {
            isFired = true;
            ++fireCount;
            if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
            {
                GameObject projectileObj = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_Normal);

                Projectile projectile = projectileObj.GetComponent<Projectile>();
                if (null == projectile)
                {
                    Debug.Log("FalconUnitCast : projectile is null");
                    return;
                }

                Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

                if (ownerFSM.Unit.facingRight != delta.x >= 0)
                    ownerFSM.Unit.Flip();

                projectile.Ready(ownerFSM.Unit, ownerFSM.Base.MyCamp, ownerFSM.Unit.FirePosition, ownerFSM.Base.Damage, ownerFSM.Base.ProjectileSpeed, ownerFSM.AttackTarget.gameObject);

                ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);

                //BulletManager.Instance.Fire_Bullet(ownerFSM.Unit.gameObject, ownerFSM.AttackTarget);   
            }
        }
    }

    private void Action_Owl()
    {

        if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
            if (ownerFSM.IsMove)
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.RUN);
            else
                ownerFSM.ChangeFSM(FlyingUnitFSM.STATE.IDLE);
        }
    }
}
