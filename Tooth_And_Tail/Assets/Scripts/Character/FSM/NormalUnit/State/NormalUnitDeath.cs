using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalUnitDeath : FSM<NormalUnitFSM>
{
    private NormalUnitFSM ownerFSM;

    int preStateHash = 0;

    public NormalUnitDeath(NormalUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        if(GameManager.Instance.CurGameMode == GameMode.Tutorial && ownerFSM.Base.Type == CommonType.Toad)
        {
            SceneStarter.Instance.userElements.AddMissionCount(MissionType.None, CampaignManager.Instance.curWave() + 1, 1);
            if (SceneStarter.Instance.userElements.CompleteMission(MissionType.None, CampaignManager.Instance.curWave() + 1))
            {
                CampaignManager.Instance.GenNextObjectsAfterSecond();
                CampaignManager.Instance.DestroyObstacle();
            }
        }
        ownerFSM.curState = NormalUnitFSM.STATE.DEATH;
        ownerFSM.Animator.SetBool("Death", true);
        if (CommonType.Fox != ownerFSM.Base.Type )
            EffectManager.Instance.EffectEnable(ownerFSM.Unit.gameObject, ParticleObject.PARTICLETYPE.UNITDEATH);
        else
            preStateHash = ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).fullPathHash;

        //  소리
        ownerFSM.Play_Unit_PositionSound(UnitSoundType.Death);
    }

    public override void Run()
    {
        //if (ownerFSM.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        if (CommonType.Fox != ownerFSM.Base.Type)
        {
            ownerFSM.Unit.isDead = true;
            ownerFSM.Death();
        }
        else
        {
            var stateInfo = ownerFSM.Animator.GetCurrentAnimatorStateInfo(0);

            //  아직 애니메이션이 바뀌지 않음.
            if (stateInfo.fullPathHash == preStateHash)
                return;

            if (stateInfo.normalizedTime >= 0.8f)
            {
                ownerFSM.Unit.isDead = true;
                ownerFSM.Death();
            }
        }
    }

    public override void Exit()
    {
        ownerFSM.preState = NormalUnitFSM.STATE.DEATH;
        ownerFSM.Animator.SetBool("Death", false);
    }
}
