using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnitCast : FSM<NormalUnitFSM>
{
    private NormalUnitFSM ownerFSM;

    private bool isFired = false;

    private int preStateHash = 0;
    public NormalUnitCast(NormalUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = NormalUnitFSM.STATE.IDLE;
        ownerFSM.TimeCount = 0f;
        ownerFSM.Animator.SetBool("Cast", true);
        //Debug.Log("NormalUnitCast begin");
        preStateHash = ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;
        isFired = false;

        if (CommonType.Toad == ownerFSM.Base.Type)
        {
            ownerFSM.Play_Unit_Sound(UnitSoundType.Attack, 0);
        }
    }

    public override void Run()
    {
        ////  이동 해야하는 상황인 경우.
        //if (ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(NormalUnitFSM.STATE.RUN);
        //    return;
        //}

        //  공속 재기위한 시간 카운트
        ownerFSM.TimeCount += Time.deltaTime;

        //  공격 애니메이션 종료
        if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f)
        {
            if (CommonType.Toad == ownerFSM.Data.CommonType)
            {
                ToadBoom();
                ownerFSM.ChangeFSM(NormalUnitFSM.STATE.DEATH);
            }
            else
                ownerFSM.ChangeFSM(NormalUnitFSM.STATE.ATTACK_IDLE);
        }

        if (!isFired)
        {
            Fire();
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = NormalUnitFSM.STATE.IDLE;
        ownerFSM.Animator.SetBool("Cast", false);
    }

    private void ToadBoom()
    {
        if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
        {
            Character character = ownerFSM.AttackTarget.GetComponent<Character>();

            if (null != character)
            {
                if (character.IsBuilding())
                    character.Hit(ownerFSM.Base.Damage * 2f, ownerFSM.Unit);
                else
                    character.Hit(ownerFSM.Base.Damage, ownerFSM.Unit);

                Vector3Int tilePos = TilemapSystem.Instance.WorldToCellPos(ownerFSM.Pos);

                ownerFSM.Play_Unit_Sound(UnitSoundType.Attack, 1, 4);
                Fire_ToadBoom(tilePos);

                for (int i = 0; i < 8; ++i)
                {
                    Fire_ToadBoom(tilePos + new Vector3Int(Global.DirX[i], Global.DirY[i], 0));
                }
            }
        }
    }

    private void Fire()
    {
        switch (ownerFSM.Base.Type)
        {
            case CommonType.Mole:
            case CommonType.Mouse:
                {
                    if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f)
                    {
                        isFired = true;
                        if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
                        {
                            //  소리
                            ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);

                            //  데미지
                            Character character = ownerFSM.AttackTarget.GetComponent<Character>();


                            Vector3 delta = ownerFSM.AttackTarget.transform.position - ownerFSM.Pos;

                            if (ownerFSM.Unit.facingRight != delta.x >= 0)
                                ownerFSM.Unit.Flip();

                            if (null != character)
                            {
                                character.Hit(ownerFSM.Base.Damage, ownerFSM.Unit);
                                return;
                            }
                        }
                    }
                }
                break;
            case CommonType.Toad:
                {

                }
                break;
            default:
                isFired = true;
                if (null != ownerFSM.AttackTarget && ownerFSM.AttackTarget.gameObject.activeSelf)
                {
                    //  소리
                    ownerFSM.Play_Unit_Sound(UnitSoundType.Attack);

                    //  총알 발사
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
                }
                break;
        }
    }

    private void Fire_ToadBoom(Vector3Int tilePos)
    {
        var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));
        if (null == node)
            return;

        GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_HitObject);

        if (null == pullObject)
        {
            Debug.Log("NormalUnitCast : PullObject is null");
            return;
        }
        TileHitObject tileHitObject = pullObject.GetComponent<TileHitObject>();

        if (null == tileHitObject)
        {
            Debug.Log("NormalUnitCast : tileHitObject is null");
            return;
        }
        tileHitObject.Ready_Toad(ownerFSM.Base.MyCamp, 3f, node.worldPosition, ownerFSM.Unit);

        EffectManager.Instance.EffectEnable(node.worldPosition, ParticleObject.PARTICLETYPE.EXPLOSION);
    }
}
