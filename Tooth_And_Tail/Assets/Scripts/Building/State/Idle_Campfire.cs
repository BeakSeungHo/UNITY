using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class State_Idle : IBuildingState
{
    bool isOccupyingCampfire = false;

    public void MakeCampfire()
    {
        gameObject.transform.Find("Meat").GetComponent<Meat>().Baking();
        buildingBase.SetBuildBoundary(buildingBase.Base.MyCamp);
        buildingBase.fireFlag = true;
        buildingBase.Play_Building_Sound(BuildSoundType.Complete, 1f, Sound_Channel.Ambient);
        buildingBase.Play_BuildingLoopSound(BuildSoundType.Idle, 1f, 0);
        EffectManager.Instance.SmokeEffectEnable(buildingBase.gameObject, buildingBase.smokePoints.position, 1.25f, true, ParticleObject.PARTICLETYPE.FLAME);
        TilemapSystem.Instance.SetOutLine();
    }

    BuildingState Idle_Campfire()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Destroyed"))
        {
            animator.SetBool("CampFire", true);
            buildingBase.transform.Find("Meat").gameObject.SetActive(true);
        }

        return BuildingState.End;
    }

    public void DestroyCampfire()
    {
        isOccupyingCampfire = false;

        curTime = 0;
        
        animator.SetInteger("Construct_Level", -1);

        buildingBase.ChangeState(BuildingState.Idle);

        BuildingManager.Instance.DeleteBuilding(buildingBase.Base.MyCamp, buildingBase);
        if (BuildingManager.Instance.IsGameOver(buildingBase.Base.MyCamp))
            GameManager.Instance.EndGame(buildingBase.Base.MyCamp);
        buildingBase.Occupying(false, buildingBase);
        buildingBase.SetBuildBoundary(Camp.End, buildingBase.Base.MyCamp);

        buildingBase.Play_Building_Sound(BuildSoundType.Destroy, 1f, Sound_Channel.Ambient);

        buildingBase.Base.MyCamp = Camp.End;

        buildingBase.TurnOffLoopSound();
    }

    public bool NowOccupying()
    {
        return isOccupyingCampfire;
    }
}
