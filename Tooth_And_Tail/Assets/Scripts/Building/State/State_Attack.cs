using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetDir { LD, RD, LU, RU, End };



public partial class State_Attack : IBuildingState
{
    GameObject attackPoint = null;
    string preDir;
    float curAttackDelay = 0;
    TargetDir targetDir = TargetDir.End;

    int curAttackCount = 0;
    int attackCount = 1;

    float interAttackTime = 0.2f;
    float curInterAttackTime = 0f;

    bool nowAttacking = false;

    Character targetCharacter = null;

    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);

        curInterAttackTime = interAttackTime;

        curAttackDelay = 9999;

        
    }

    public override void EnterState()
    {
        targetCharacter = target.GetComponent<Character>();
        
        attackPoint = buildingBase.transform.Find("Body").transform.Find("FirePoint").gameObject;
        if (data.CommonType == CommonType.Cannon)
            attackCount = 4;
        else
            attackCount = 1;
        animator.SetBool("RD", true);
        preDir = "RD";
    }

    public override void ExitState()
    {
        animator.SetBool("Attack", false);
        animator.SetBool("Idle", true);
    }

    void SetTargetDir()
    {
        Vector3 myPos, targetPos;

        myPos = buildingBase.transform.position;
        targetPos = target.transform.position;

        if (myPos.x >= targetPos.x)
        {
            if (myPos.y < targetPos.y)
            {
                targetDir = TargetDir.LU;
            }
            else
            {
                targetDir = TargetDir.LD;
            }
        }

        else
        {
            if (myPos.y < targetPos.y)
            {
                targetDir = TargetDir.RU;
            }
            else
            {
                targetDir = TargetDir.RD;
            }
        }
    }

    

    BuildingState Attack_Normal()
    {
        if (targetCharacter.HP <= 0 || target == null || target.activeInHierarchy == false)
        {
            if (!SearchTarget())
            {
                return BuildingState.Idle;
            }
            else
            {
                targetCharacter = target.GetComponent<Character>();
            }
        }
        if (TilemapSystem.Instance.RangeInObject(buildingBase.transform.position, target.transform.position, data.Range) == TilemapSystem.Invalid_Range)
        {
            if (!SearchTarget())
                return BuildingState.Idle;
            else
            {
                targetCharacter = target.GetComponent<Character>();
            }
        }

        curAttackDelay += Time.deltaTime;

        if (curAttackDelay >= data.AttackSpeed)
        {
            nowAttacking = true;
            SetTargetDir();
            switch (targetDir)
            {
                case TargetDir.LD:
                    animator.Play("Attack_LD");
                    animator.SetBool(preDir, false);
                    preDir = "LD";
                    animator.SetBool(preDir, true);
                    break;

                case TargetDir.RD:
                    animator.Play("Attack_RD");
                    animator.SetBool(preDir, false);
                    preDir = "RD";
                    animator.SetBool(preDir, true);
                    break;

                case TargetDir.LU:
                    animator.Play("Attack_LU");
                    animator.SetBool(preDir, false);
                    preDir = "LU";
                    animator.SetBool(preDir, true);
                    break;

                case TargetDir.RU:
                    animator.Play("Attack_RU");
                    animator.SetBool(preDir, false);
                    preDir = "RU";
                    animator.SetBool(preDir, true);
                    break;
            }
        }
        if (nowAttacking)
        {
            curInterAttackTime += Time.deltaTime;
            if (curInterAttackTime >= interAttackTime)
            {
                buildingBase.Play_Building_Sound(BuildSoundType.Attack);
                curInterAttackTime = 0f;
                GameObject projectileObj = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_Normal);

                Projectile projectile = projectileObj.GetComponent<Projectile>();
                
                if (null != projectile)
                {
                    if (data.CommonType == CommonType.Cannon)
                    {
                        
                    }
                    projectile.Ready(buildingBase, buildingBase.Base.MyCamp, attackPoint.transform.position, data.Damage, data.ProjectileSpeed, target);
                }

                curAttackCount++;

                if (curAttackCount >= attackCount)
                {
                    curInterAttackTime = interAttackTime;
                    nowAttacking = false;
                    curAttackCount = 0;
                    curAttackDelay = 0f;
                }
            }
        }
        return BuildingState.End;
    }
    public override BuildingState OperateState()
    {
        BuildingState retVal = BuildingState.End;
        switch (data.CommonType)
        {
            case CommonType.Mine:
                Attack_Mine();
                break;
            default:
                retVal = Attack_Normal();
                break;
        }
        
        return retVal;
    }
}