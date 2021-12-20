using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gristmill : MonoBehaviour
{
    public Farm[] Farms = null;

    CommonBase commonBase;
    public BuildingBase buildingBase;
    
    Animator animator;

    bool checkDefault = false;

    public Transform firePoint;

    // Start is called before the first frame update
    void Start()
    {
        commonBase = GetComponent<CommonBase>();
        buildingBase = GetComponent<BuildingBase>();
        animator = transform.Find("Body").GetComponent<Animator>();
        ShadowManager.Instance.ShadowObjPos.Add(transform.position);
        AIManager.Instance.GristmillPositions.Add(transform.position);
    }

    public void FarmOccupy(Camp camp)
    {
        foreach (var tilePos in buildingBase.OccupyTiles)
        {
            TilemapSystem.Instance.GetTile(tilePos).camp = camp;
        }
        for (int i = 0; i < Farms.Length; i++)
        {
            foreach (var tilePos in Farms[i].buildingBase.OccupyTiles)
            {
                TilemapSystem.Instance.GetTile(tilePos).camp = camp;
            }
        }
    }

    public void SetDefaultGristmill(Camp camp, bool farmless = false)
    {
        animator.SetBool("Idle", true);
        animator.SetBool("Idle_Neutral", false);
        commonBase.MyCamp = camp;
        if (farmless == false)
        {
            List<int> randomList = new List<int>();

            for (int i = 0; i < 8; i++)
            {
                randomList.Add(i);
            }

            int randNum;
            for (int i = 0; i < 4; i++)
            {
                while (true)
                {
                    randNum = Random.Range(0, randomList.Count);
                    if (randomList[randNum] != -1)
                    {
                        break;
                    }
                }
                Farms[randomList[randNum]].Production(true, camp);
                //GameManager.Instance.ChangeFoodCamp(camp, 500);
                //Farm[randomList[randNum]].GetComponent<Farm>().Cultivation(camp);
                randomList[randNum] = -1;
            }
        }
        else
        {
            foreach(var farm in Farms)
            {
                farm.buildingBase.Occupying(false, null);
                //TilemapSystem.Instance.OccupyTile(null, 0, true, false);
                farm.gameObject.SetActive(false);
            }
        }
        buildingBase.HP = commonBase.Data.MaxHp;
        buildingBase.IsNeutral = false;
        buildingBase.hpCanvas.Ready();
        buildingBase.SetMiniSpriteColor();
        buildingBase.SetBuildBoundary(camp);
        BuildingManager.Instance.AddBuilding(camp, buildingBase);
        TilemapSystem.Instance.SetOutLine();

        if (GameManager.Instance.CurGameMode != GameMode.Tutorial)
        {
            // 커맨더 시작 위치 지정
            var commanderTilePos = InGameManager.Instance.Find_NearestEmptyTile(transform.position);
            var tileNode = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(commanderTilePos));

            InGameManager.Instance.Commanders[camp].transform.position = tileNode.worldPosition;
        }
    }

    public void DestroyFarm()
    {
        for(int i=0;i<Farms.Length;i++)
        {
            if(Farms[i].buildingBase.Base.MyCamp != Camp.End)
                Farms[i].buildingBase.DestroyBuilding();
            Farms[i].warningCanvas.gameObject.SetActive(false);
        }
    }
    
    public bool Build(Camp camp)
    {
        int curFood = GameManager.Instance.GetFood(camp);
        if (curFood < commonBase.Data.Cost)
        {
            Debug.Log("식량이 부족합니다");
            return false;
        }
        GameManager.Instance.ChangeFoodCamp(camp, -commonBase.Data.Cost);

        if (commonBase.MyCamp == GameManager.Instance.CommanderList[0])
        {
            //숫자
            EffectManager.Instance.FontEffectEnable(InGameManager.Instance.Commanders[commonBase.MyCamp].gameObject, commonBase.Data.Cost, FontEffect.FONTTYPE.MINUSFOOD);
        }

        commonBase.MyCamp = camp;
        animator.SetBool("Idle", false);
        animator.SetBool("Idle_Des", false);
        animator.SetBool("Idle_Neutral", false);

        buildingBase.hpCanvas.Ready();
        buildingBase.SetMiniSpriteColor();
        return true;
    }

    // Update is called once per frame
    void Update()
    {
        if (!checkDefault)
        {
            if (GameManager.Instance.CurGameMode >= GameMode.Multi)
            {
                Camp buildinglessCamp = BuildingManager.Instance.GetBuildinglessCamp();
                if (buildinglessCamp != Camp.Error)
                {
                    SetDefaultGristmill(buildinglessCamp);
                }
            }
            else if (GameManager.Instance.CurGameMode == GameMode.Tutorial)
            {
                SetDefaultGristmill(Camp.Archimedes, true);
            }
            else if (GameManager.Instance.CurGameMode == GameMode.Campaign)
            {
                SetDefaultGristmill(Camp.Bellafide, true);
            }
            checkDefault = true;
        }
        if (buildingBase.Base.MyCamp != Camp.End)
        {
            if (buildingBase.StateOperator.CurBuildingState == BuildingState.Idle)
            {

                if (buildingBase.HP <= 100)
                {
                    if (!buildingBase.smokeFlag)
                    {
                        buildingBase.smokeFlag = true;
                        EffectManager.Instance.SmokeEffectEnable(buildingBase.gameObject, buildingBase.smokePoints.position, 1f, false, ParticleObject.PARTICLETYPE.FLAME);

                    }
                }
                if (buildingBase.HP <= 50)
                {
                    if (!buildingBase.fireFlag)
                    {
                        buildingBase.fireFlag = true;
                        EffectManager.Instance.SmokeEffectEnable(buildingBase.gameObject, firePoint.position, 1f, true, ParticleObject.PARTICLETYPE.FLAME);
                    }
                }


                if (buildingBase.HP > 50)
                    buildingBase.fireFlag = false;
                if (buildingBase.HP > 100)
                    buildingBase.smokeFlag = false;
            }
        }
    }
}