using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedUnitCastRun : FSM<AdvancedUnitFSM>
{
    private AdvancedUnitFSM ownerFSM;

    private WildBoarFlame testWeapon;

    public AdvancedUnitCastRun(AdvancedUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
        ownerFSM.AttackEffect = false;
    }

    public override void Begin()
    {
        ownerFSM.curState = AdvancedUnitFSM.STATE.CAST_RUN;

        //switch (ownerFSM.Base.Type)
        //{
        //    case CommonType.Boar:
        //        ownerFSM.Animator.SetBool("Cast", true);
        //        ownerFSM.Animator.SetBool("Run", true);
        //        break;
        //    case CommonType.Badger:
        //    case CommonType.Wolf:
        //        break;
        //}

        ownerFSM.Animator.SetBool("CastRun", true);
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
        ownerFSM.preState = AdvancedUnitFSM.STATE.CAST_RUN;

        //switch (ownerFSM.Base.Type)
        //{
        //    case CommonType.Boar:
        //        ownerFSM.Animator.SetBool("Cast", false);
        //        ownerFSM.Animator.SetBool("Run", false);
        //        break;
        //    case CommonType.Badger:
        //    case CommonType.Wolf:
        //        ownerFSM.Animator.SetBool("CastRun", false);
        //        break;
        //}
        ownerFSM.Animator.SetBool("CastRun", false);


        if (CommonType.Boar == ownerFSM.Base.Type)
            ownerFSM.End_BoarFlameSound();
    }

    public void Action_Boar()
    {
        //  이동을 멈출 경우

        //if (!ownerFSM.IsMove)
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
            return;
        }

        ////  도착
        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //    return;
        //}

        //  이동
        //ownerFSM.PathMove(0.5f);
        ownerFSM.CommonFSM.VFAgent.Move(0.5f);

        if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;

            if (!ownerFSM.CheckTargetInRange())
            {
                ownerFSM.AttackTarget = null;
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);

                var toWorldPos = ownerFSM.CommandedTarget.transform.position;
                var tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Unit, ownerFSM.Base.MyCamp, toWorldPos);

                if (Global.InvalidTilePos != tilePos)
                {
                    //ownerFSM.CommonFSM.VFAgent.Move(tilePos);

                    ownerFSM.Unit.Command_Move(tilePos);
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
                    return;
                }
                else
                {
                    Debug.Log(" Invalid TilePos!");
                }
            }
            ownerFSM.CommandedTarget = null;
        }
        //  타겟이 사라졌을 경우 체크
        if (null == ownerFSM.AttackTarget)
        {
            if (!ownerFSM.Scout_Enemy())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
                return;
            }
        }
        else
        {
            if (!ownerFSM.CheckTargetInRange())
            {
                ownerFSM.AttackTarget = null;
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
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

        //  발사!
        if (!ownerFSM.isFired)
        {
            ownerFSM.isFired = true;

            Vector3Int tilePos = TilemapSystem.Instance.WorldToCellPos(ownerFSM.AttackTarget.transform.position);
            ownerFSM.Boar_FlameThrow(tilePos);

            //  소리
            ownerFSM.Loop_BoarFlameSound();
        }

        //GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_HitObject);

        //if (null == pullObject)
        //{
        //    Debug.Log("Boar Cast : pullObject is null");
        //    return;
        //}

        //HitObject hitObject = pullObject.GetComponent<HitObject>();

        //if (null == hitObject)
        //{
        //    Debug.Log("Boar Cast : hitObject is null");
        //    return;
        //}

        //hitObject.Ready(HitType.Immediate, ownerFSM.Base.MyCamp, false, ownerFSM.AttackTarget.transform.position, 1f, ownerFSM.Base.Damage * Time.deltaTime, 0f, 9);

        //switch (ownerFSM.Data.CommonType)
        //{
        //    case CommonType.Boar:
        //        {
        //            //if (testWeapon == null)
        //            //    testWeapon = ownerFSM.weaponPos.GetChild(0).GetComponent<WildBoarFlame>();
        //            //testWeapon.Testflag1 = true;
        //            //testWeapon.Flame1StartTime = Mathf.Abs(Vector3.Magnitude(ownerFSM.AttackTarget.transform.position - ownerFSM.weaponPos.position)) / testWeapon.Flame1Speed;
        //            //testWeapon.Flame2.transform.position = new Vector3(ownerFSM.AttackTarget.transform.position.x, ownerFSM.AttackTarget.transform.position.y, -0.1f);
        //            //testWeapon.transform.LookAt(ownerFSM.AttackTarget.transform.position);
        //        }
        //        break;
        //}
    }

    public void Action_Badger()
    {
        //  공속 업
        ownerFSM.Badger_FireSpeedUp();

        //  이동을 멈출 경우
        //if (!ownerFSM.IsMove)
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
            return;
        }

        ////  도착
        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //    return;
        //}

        //  이동
        //ownerFSM.PathMove();
        ownerFSM.CommonFSM.VFAgent.Move(0.5f);

        if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;

            if (!ownerFSM.CheckTargetInRange())
            {
                ownerFSM.AttackTarget = null;
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);

                var toWorldPos = ownerFSM.CommandedTarget.transform.position;
                var tilePos = SquadController.Instance.Find_NearestTilePos(ownerFSM.Unit, ownerFSM.Base.MyCamp, toWorldPos);

                if (Global.InvalidTilePos != tilePos)
                {
                    //ownerFSM.CommonFSM.VFAgent.Move(tilePos);
                    ownerFSM.Unit.Command_Move(tilePos);
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
                    return;
                }
                else
                {
                    Debug.Log(" Invalid TilePos!");
                }
            }
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
                ownerFSM.AttackTarget = null;
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
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

        // 공격 쿨타임
        ownerFSM.TimeCount += Time.deltaTime;
        if (ownerFSM.TimeCount >= 1 / ownerFSM.AttackSpeed)
        {
            ownerFSM.TimeCount = 0f;
            ownerFSM.isFired = false;
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

                projectile.Ready(ownerFSM.Unit, ownerFSM.Base.MyCamp, ownerFSM.Unit.FirePosition, ownerFSM.Base.Damage, ownerFSM.Base.ProjectileSpeed, ownerFSM.AttackTarget.gameObject);

                ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);
            }
        }

        ////  이동
        //ownerFSM.PathMove();

        ////  타겟이 공격 범위에 있는지 체크.
        //if (null != ownerFSM.AttackTarget)
        //{
        //    if (!ownerFSM.AttackTarget.gameObject.activeSelf)
        //    {
        //        ownerFSM.AttackTarget = null;
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
        //        return;
        //    }
        //    else
        //    {
        //        if (!ownerFSM.CheckTargetInRange())
        //        {
        //            ownerFSM.AttackTarget = null;

        //            if (!ownerFSM.Scout_Enemy())
        //                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
        //            return;
        //        }
        //    }
        //}
        //else
        //{
        //    ownerFSM.Scout_Enemy();
        //}

        ////  이동하지 않게 된 경우 체크.
        //if (!ownerFSM.IsMove)
        //{
        //    if (null == ownerFSM.AttackTarget || !ownerFSM.AttackTarget.gameObject.activeSelf)
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        //    else
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //    return;
        //}

        //// 공격 쿨타임
        //ownerFSM.TimeCount += Time.deltaTime;
        //if (ownerFSM.TimeCount >= 1 / ownerFSM.AttackSpeed)
        //{
        //    ownerFSM.TimeCount = 0f;
        //    ownerFSM.isFired = false;
        //}

        ////  보는 방향 설정
        //if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
        //{
        //    Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

        //    if (ownerFSM.Unit.facingRight != delta.x >= 0)
        //        ownerFSM.Unit.Flip();
        //}

        ////  발사!
        //if (!ownerFSM.isFired)
        //{
        //    ownerFSM.isFired = true;

        //    if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
        //    {
        //        GameObject projectileObj = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_Normal);

        //        Projectile projectile = projectileObj.GetComponent<Projectile>();
        //        if (null == projectile)
        //        {
        //            Debug.Log("NormalUnitCast : projectile is null");
        //            return;
        //        }

        //        projectile.Ready(ownerFSM.Base.Type, ownerFSM.Base.MyCamp, ownerFSM.Unit.FirePosition, ownerFSM.Base.Damage, ownerFSM.Base.ProjectileSpeed, ownerFSM.AttackTarget.gameObject);

        //    }
        //}

        ////  이동 목표 지점 도착 여부 체크
        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    if (null == ownerFSM.AttackTarget || !ownerFSM.AttackTarget.gameObject.activeSelf)
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        //    else
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //    return;
        //}
    }

    public void Action_Wolf()
    {
        //  이동
        ownerFSM.CommonFSM.VFAgent.Move();
        //  이동을 멈춤.
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            //if (null == ownerFSM.AttackTarget || !ownerFSM.AttackTarget.gameObject.activeSelf)
            //    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
            //else
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
            return;
        }

        //  쿨 타임 계산
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

        //  버프 발사
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
            }
        }

        if (null == ownerFSM.AttackTarget)
        {
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.RUN);
        }

        //ownerFSM.PathMove();
    }
}
