using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedUnitDeath : FSM<AdvancedUnitFSM>
{
    private AdvancedUnitFSM ownerFSM;

    private int preStateHash = 0;

    public AdvancedUnitDeath(AdvancedUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = AdvancedUnitFSM.STATE.DEATH;
        ownerFSM.Animator.SetBool("Death", true);
        //Debug.Log("Death Begin");
        ownerFSM.AttackEffect = false;
        preStateHash = ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        ownerFSM.Play_Unit_PositionSound(UnitSoundType.Death);

        if (CommonType.Boar == ownerFSM.Base.Type)
        {
            var curTile = TilemapSystem.Instance.WorldToCellPos(ownerFSM.Pos);

            ownerFSM.Play_Unit_Sound(UnitSoundType.Explosion);
            Boar_Death_Boom(curTile);

            for (int i = 0; i < 8; ++i)
            {
                var tilePos = curTile + new Vector3Int(Global.DirX[i], Global.DirY[i], 0);

                Boar_Death_Boom(tilePos);
            }
        }
    }
    
    public override void Run()
    {
        //if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).length > ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime)
        //if (ownerFSM.Animator.IsInTransition(0))

        var stateInfo = ownerFSM.Animator.GetCurrentAnimatorStateInfo(0);

        //Debug.Log("normalizeTime : " + stateInfo.normalizedTime.ToString());

        if (stateInfo.fullPathHash == preStateHash)
            return;

        if (stateInfo.normalizedTime >= 0.8f)
        {
            ownerFSM.Unit.isDead = true;
            ownerFSM.Death();
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = AdvancedUnitFSM.STATE.DEATH;
        ownerFSM.Animator.SetBool("Death", false);
    }

    public void Action_Boar()
    {

    }

    public void Action_Badger()
    {

    }

    public void Action_Wolf()
    {

    }

    public void Boar_Death_Boom(Vector3Int tilePos)
    {
        GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_HitObject);

        if (null == pullObject)
        {
            Debug.Log("Boar Death Boom : pullObject is null");
            return;
        }

        TileHitObject tileHitObject = pullObject.GetComponent<TileHitObject>();

        if (null == tileHitObject)
        {
            Debug.Log("Boar Death Boom : tileHitObject is null");
            return;
        }

        var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));

        tileHitObject.Ready_Toad(ownerFSM.Base.MyCamp, 20, node.worldPosition, ownerFSM.Unit);
        EffectManager.Instance.EffectEnable(node.worldPosition, ParticleObject.PARTICLETYPE.EXPLOSION);
    }
}
