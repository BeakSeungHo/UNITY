using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedUnitCast : FSM<AdvancedUnitFSM>
{
    private AdvancedUnitFSM ownerFSM;

    public AdvancedUnitCast(AdvancedUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = AdvancedUnitFSM.STATE.CAST;
        ownerFSM.TimeCount = 0f;
        ownerFSM.Animator.SetBool("Cast", true);
        //ownerFSM.Animator.SetBool("Run", false);
        //Effect
        if (!ownerFSM.AttackEffect)
        {
            switch (ownerFSM.Data.CommonType)
            {
                case CommonType.Boar:
                    //GameObject Weapon = ownerFSM.CommonFSM.transform.parent.GetChild(0).GetChild(0).gameObject;
                    GameObject Weapon = ownerFSM.Unit.FirePos;
                    EffectManager.Instance.BoarEffectEnable(Weapon, ownerFSM.CommonFSM.gameObject, ParticleObject.PARTICLETYPE.BOARFLAME);


                    break;
                case CommonType.Wolf:
                    EffectManager.Instance.EffectEnable(ownerFSM.AttackTarget.gameObject, ParticleObject.PARTICLETYPE.WOLFWAVE);
                    break;
            }
            ownerFSM.AttackEffect = true;
        }

        if (CommonType.Boar == ownerFSM.Base.Type)
            ownerFSM.Start_BoarFlameSound();
    }

    public override void Run()
    {
        switch (ownerFSM.Base.Type)
        {
            case CommonType.Boar:
                Action_Boar();
                break;
            case CommonType.Badger:
                Action_Badger();
                break;
            case CommonType.Wolf:
                Action_Wolf();
                break;
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = AdvancedUnitFSM.STATE.CAST;
        ownerFSM.Animator.SetBool("Cast", false);

        if (CommonType.Boar == ownerFSM.Base.Type)
            ownerFSM.End_BoarFlameSound();
    }

    public void Action_Boar()
    {
        if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
            ownerFSM.CommandedTarget = null;
        }
        //  타겟이 사라졌을 경우 체크
        if (null == ownerFSM.AttackTarget)
        {
            if (!ownerFSM.Scout_Enemy())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
                return;
            }
        }
        else
        {
            if (!ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
                return;
            }
        }

        //  보는 방향 설정
        if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
        {
            Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

            if (ownerFSM.Unit.facingRight != delta.x >= 0)
                ownerFSM.Unit.Flip();
        }

        //  움직이게 될 경우.
        //if (ownerFSM.IsMove)
        if (ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
            return;
        }
        //  발사!
        if (!ownerFSM.isFired)
        {
            ownerFSM.isFired = true;

            Vector3Int tilePos = TilemapSystem.Instance.WorldToCellPos(ownerFSM.AttackTarget.transform.position);
            ownerFSM.Boar_FlameThrow(tilePos);

            //  소리
            ownerFSM.Loop_BoarFlameSound();

            //ownerFSM.Boar_FlameThrow(tilePos);

            //int skipCount = 0;
            //for (int i = 0; i < 8; ++i)
            //{
            //    if (skipCount < 2 && Random.Range(0f, 800f) < 200f)
            //    {
            //        ++skipCount;
            //        continue;
            //    }
            //    ownerFSM.Boar_FlameThrow(tilePos + new Vector3Int(Global.DirX[i], Global.DirY[i], 0));
            //}
        }
    }

    public void Action_Badger()
    {
        //  공속 업
        ownerFSM.Badger_FireSpeedUp();

        if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
            ownerFSM.CommandedTarget = null;
        }
        //  타겟이 사라졌을 경우 체크
        if (null == ownerFSM.AttackTarget)
        {
            if (!ownerFSM.Scout_Enemy())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
                return;
            }
        }
        else
        {
            if (!ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
                return;
            }
        }

        //  보는 방향 설정
        if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
        {
            Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

            if (ownerFSM.Unit.facingRight != delta.x >= 0)
                ownerFSM.Unit.Flip();
        }

        //  움직이게 될 경우.
        //if (ownerFSM.IsMove)
        if (ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
            return;
        }

        ////  이동하게 된 경우 체크.
        //if (ownerFSM.IsMove)
        //{
        //    if (null == ownerFSM.AttackTarget || !ownerFSM.AttackTarget.gameObject.activeSelf)
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
        //    else
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
        //    return;
        //}
        //else
        //{
        //    if (null == ownerFSM.AttackTarget || !ownerFSM.AttackTarget.gameObject.activeSelf)
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        //}

        // 공격 쿨타임
        ownerFSM.TimeCount += Time.deltaTime;
        if (ownerFSM.TimeCount >= 1 / ownerFSM.AttackSpeed)
        {
            ownerFSM.TimeCount = 0f;
            ownerFSM.isFired = false;
            
            if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.8f)
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
            }
            else
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.ATTACK_IDLE);
            }

            if (!ownerFSM.Scout_Enemy())
            {
                if (ownerFSM.IsMove)
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
                else
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
            }
        }
        
        //  발사!
        if (!ownerFSM.isFired)
        {
            ownerFSM.isFired = true;

            if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
            {
                GameObject projectileObj = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_Normal);

                Projectile projectile = projectileObj.GetComponent<Projectile>();
                if (null == projectile)
                {
                    Debug.Log("NormalUnitCast : projectile is null");
                    return;
                }

                Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

                if (ownerFSM.Unit.facingRight != delta.x >= 0)
                    ownerFSM.Unit.Flip();

                projectile.Ready(ownerFSM.Unit, ownerFSM.Base.MyCamp, ownerFSM.Unit.FirePosition, ownerFSM.Base.Damage, ownerFSM.Base.ProjectileSpeed, ownerFSM.AttackTarget.gameObject);

                ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);
            }
        }
    }

    public void Action_Wolf()
    {
        if (ownerFSM.IsMove)
        {
            //if (null == ownerFSM.AttackTarget || !ownerFSM.AttackTarget.gameObject.activeSelf)
            //    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
            //else
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
            return;
        }
        else
        {
            if (null == ownerFSM.AttackTarget || !ownerFSM.AttackTarget.gameObject.activeSelf)
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        }

        //  쿨 타임
        ownerFSM.TimeCount += Time.deltaTime;
        if (ownerFSM.TimeCount >= 1 / ownerFSM.Base.AttackSpeed)
        {
            ownerFSM.TimeCount = 0f;
            ownerFSM.isFired = false;
            if (!ownerFSM.Scout_FriendUnit())
            {
                if (ownerFSM.IsMove)
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
                else
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
            }
        }

        if (!ownerFSM.isFired)
        {
            ownerFSM.isFired = true;

            if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
            {
                Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

                if (ownerFSM.Unit.facingRight != delta.x >= 0)
                    ownerFSM.Unit.Flip();

                BuffDebuff buffDebuff = ownerFSM.AttackTarget.GetComponent<BuffDebuff>();

                if (null != buffDebuff)
                    buffDebuff.Add_Stim();

                ownerFSM.Play_Unit_Sound(UnitSoundType.Cry);

                ownerFSM.AttackTarget = null;
            }
        }
    }
}
