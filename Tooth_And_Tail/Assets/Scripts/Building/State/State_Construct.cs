using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class BuildingState_Construct : IBuildingState
{
    float curConstructTime = 0;
    int curConstructLevel = 0;
    int constructLevel;

    float hpPerFrame;
    float constructTime = 0f;
    float totalConstructTime = 0f;
    Animator topper;

    string animName = "Construct_0";
    public override void Initialize(GameObject obj)
    {
        base.Initialize(obj);
        if(data.CommonType == CommonType.Gristmill)
        {
            topper = gameObject.transform.Find("Topper").GetComponent<Animator>();
        }
        
    }

    public void PlayCurTime()
    {
        string curAnimName = animName + animator.GetInteger("Construct_Level");
        //if(curConstructLevel != constructLevel)
        animator.Play(curAnimName, 0, curConstructTime);
    }

    void SetCurConstructTime(int num)
    {
        float buildTime = buildingBase.Base.BuildTime;
        string curAnimName = animName + num;
        
        if(buildingBase.Base.Type == CommonType.CampFire)
        {
            animator.runtimeAnimatorController = SceneStarter.Instance.animatorElements.BuildAniDic[CommonType.Cabin];
        }
        else
            animator.runtimeAnimatorController = SceneStarter.Instance.animatorElements.BuildAniDic[data.CommonType];

        float curAnimLength = 0;

        for (int i = 0; i < animator.runtimeAnimatorController.animationClips.Length; i++)
        {
            if (curAnimName == animator.runtimeAnimatorController.animationClips[i].name)
                curAnimLength = animator.runtimeAnimatorController.animationClips[i].length;
        }
        if(constructLevel > 1)
            constructTime = (buildTime / (constructLevel - 1)) - curAnimLength;
        else
            constructTime = (buildTime / constructLevel) - curAnimLength;
    }

    public override void EnterState()
    {
        float buildTime = buildingBase.Base.BuildTime;
        hpPerFrame = data.MaxHp / buildTime;

        buildingBase.HP = 0.01f;
        constructLevel = buildingBase.ConstructLevel;

        BuildingManager.Instance.AddBuilding(buildingBase.Base.MyCamp, buildingBase);

        totalConstructTime = 0f;
        curConstructTime = 0f;
        curConstructLevel = 0;
        animator.SetInteger("Construct_Level", 0);

        SetCurConstructTime(curConstructLevel + 1);

        int soundIdx = 0;
        
        if (data.CommonType == CommonType.Gristmill)
        {
            topper.SetInteger("Construct_Level", 0);
            topper.SetBool("Show", true);
            topper.ResetTrigger("Idle");
            buildingBase.GetComponent<Gristmill>().Build(buildingBase.Base.MyCamp);
            buildingBase.Play_GristmillConstructSound(curConstructLevel);
            soundIdx = 1;
        }
        else if(data.CommonType == CommonType.CampFire)
        {
            buildingBase.Play_BuildingLoopSound(BuildSoundType.Idle, 0.8f, 0);
            buildingBase.hpCanvas.Ready();
        }
        else
        {
            buildingBase.Play_ConstructSound(constructTime);
            soundIdx = 2;
        }

        buildingBase.Play_BuildingGeneralSound(ComSoundType.Build, soundIdx, 0.5f);

        buildingBase.IsNeutral = false;

        //Effect
        if (data.CommonType == CommonType.Gristmill)
        {
            EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.BUILDING);
            EffectManager.Instance.SmokeEffectEnable(buildingBase.gameObject, buildingBase.transform.position, 2f, false, ParticleObject.PARTICLETYPE.SMOKE_GRISTMILL);
        }

        if (buildingBase.Base.MyCamp == GameManager.Instance.CommanderList[0])
        {
            switch (buildingBase.Base.Type)
            {
                case CommonType.Gristmill:
                    SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 5, 1);
                    break;
                case CommonType.Wire:
                case CommonType.Mine:
                case CommonType.Turret:
                case CommonType.Balloon:
                case CommonType.Cannon:
                    SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 3, 1);
                    break;
            }
        }
    }

    public override void ExitState()
    {
        curConstructLevel = 0;
        curConstructTime = 0f;

        //Effect
        if (buildingBase.HP > 0)
        {
            EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.BUILDEND);
        }
        
        if(buildingBase.Base.PoolType == Pool_ObjType.MoleeMerge)
        {
            int cost = SceneStarter.Instance.GetData(buildingBase.Product_Type).Cost;
            if (cost <= GameManager.Instance.CampFoodDic[buildingBase.Base.MyCamp])
            {
                GameManager.Instance.CampFoodDic[buildingBase.Base.MyCamp] -= cost;
                CommonBase commonBase = buildingBase.Base;
                GameObject unit = PoolManager.Instance.PullObject(buildingBase.Product_PoolType);
                unit.GetComponent<CommonUnit>().Ready(commonBase.MyCamp, buildingBase.Product_Type, gameObject.transform.position);

                BuildingManager.Instance.curUnits[buildingBase.Base.MyCamp][buildingBase.Product_Type]++;

                SquadController.Instance.Add_Unit(commonBase.MyCamp, unit);
            }
        }

        if(buildingBase.Base.Type == CommonType.CampFire)
        {
            stateOperator.MakeCampFire();
        }

        if (GameManager.Instance.CurGameMode == GameMode.Tutorial)
        {
            if (buildingBase.Base.PoolType == Pool_ObjType.Warrens)
            {
                CommonBase commonBase = buildingBase.Base;
                GameObject unit = PoolManager.Instance.PullObject(buildingBase.Product_PoolType);
                unit.GetComponent<CommonUnit>().Ready(commonBase.MyCamp, buildingBase.Product_Type, gameObject.transform.position);

                BuildingManager.Instance.curUnits[buildingBase.Base.MyCamp][buildingBase.Product_Type]++;

                SquadController.Instance.Add_Unit(commonBase.MyCamp, unit);
            }
            if(buildingBase.Base.MyCamp == GameManager.Instance.CommanderList[0])
            {
                SceneStarter.Instance.userElements.AddMissionCount(MissionType.None, CampaignManager.Instance.curWave() + 1, 1);
                if (SceneStarter.Instance.userElements.CompleteMission(MissionType.None, CampaignManager.Instance.curWave() + 1))
                {
                    CampaignManager.Instance.GenNextObjectsAfterSecond();
                }
            }
        }
    }

    public override BuildingState OperateState()
    {
        buildingBase.HP += Time.deltaTime * hpPerFrame;
        totalConstructTime += Time.deltaTime;
        curConstructTime += Time.deltaTime;

        if (curConstructTime >= constructTime && curConstructLevel < constructLevel )
        {
            curConstructLevel++;
            animator.SetInteger("Construct_Level", curConstructLevel);
            if (data.CommonType == CommonType.Gristmill)
            {
                topper.SetInteger("Construct_Level", curConstructLevel);
                buildingBase.Play_GristmillConstructSound(curConstructLevel);
            }
            else
            {
                buildingBase.Play_Building_Sound(BuildSoundType.Complete, 1.5f, Sound_Channel.Ambient);
            }
            SetCurConstructTime(curConstructLevel+1);
            if(curConstructLevel < constructLevel)
            {
                curConstructTime = 0f;
            }
        }

        float buildTime = buildingBase.Base.BuildTime;

        if (totalConstructTime >= buildTime && curConstructTime >= (buildTime / constructLevel))
        {
            Pool_ObjType poolType = buildingBase.Base.PoolType;

            // 풍선의 경우 공중에 떠서 둥실거리는 기능을 사용하기 위해 오브젝트를 활성화 시키며
            // 같은 위치에 건물을 지을 순 없지만 유닛이 부딪히지 않는다.
            if(data.CommonType == CommonType.Balloon)
            {
                gameObject.transform.Find("Floater").gameObject.SetActive(true);
                gameObject.transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder = 6;
            }
            // 풍선은 지나갈 수 있으므로 풍선을 제외하고 타일을 점유시킨다.
            else
            {
                if (data.CommonType == CommonType.Turret || data.CommonType == CommonType.Cannon || data.CommonType == CommonType.MoleeMerge)
                {
                    TilemapSystem.Instance.OccupyTile(buildingBase, 1, false, false);
                }
                gameObject.transform.Find("Body").GetComponent<SpriteRenderer>().sortingOrder = 4;
                // buildingBase.SetTileOccupy();
            }
            
            // 건설 시 HP를 프레임당 증가시키므로 딱 맞아떨어지지 않은 경우가 많아 최대체력으로 맞춰준다.
            if (buildingBase.HP >= data.MaxHp)
            {
                buildingBase.HP = data.MaxHp;
            }

            if (poolType == Pool_ObjType.Warrens || poolType == Pool_ObjType.MoleeMerge)
            {
                bool retVal = BuildingManager.Instance.AddMaxUnits(buildingBase.Base.MyCamp, buildingBase);
                CommonType product_Type = buildingBase.Product_Type;
                if (BuildingManager.Instance.curUnits[buildingBase.Base.MyCamp][product_Type] < BuildingManager.Instance.maxUnits[buildingBase.Base.MyCamp][product_Type])
                {
                    BuildingManager.Instance.curUnits[buildingBase.Base.MyCamp][product_Type]++;
                    return BuildingState.Production;
                }
                else
                    return BuildingState.Idle;
            }
            animator.SetBool("RD", true);

            // 제분소의 경우 건설 범위를 늘려준다.
            if (buildingBase.Base.Type == CommonType.Gristmill)
            {
                buildingBase.SetBuildBoundary(buildingBase.Base.MyCamp);
                TilemapSystem.Instance.SetOutLine();
                buildingBase.Play_GristmillConstructSound(curConstructLevel + 1);
                // 통계 데이터 갱신
                SceneStarter.Instance.statisticElements.AddBuildGristmill(buildingBase.Base.MyCamp);
            }
            
            return BuildingState.Idle;
        }
        return BuildingState.End;
    }
}