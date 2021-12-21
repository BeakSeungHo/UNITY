using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Snapshot
{
    public int      food;
    public int      value;

    public Snapshot(int _food, int _armyValue)
    {
        food = _food;
        value = _armyValue;
    }
};

public class StatisticElements : ScriptableObject
{
    public bool         isGameEnded;
    public GameMode     curGameMode;

    public Camp         campPlayer;                 // 진영
    public Camp         campAI;
    public bool         winPlayer;                  // 플레이어 승리 여부
    public Camp         winCamp;

    // Snapshot List
    public List<Snapshot>   playerSnapshotList;
    public List<Snapshot>   AISnapshotList;

    // GameData
    public float    gameTime;                   // 게임 시간
    public int      foodPlayer;                 // 총 식량
    public int      foodAI;
    public int      armyPlayer;                 // 최종 부대 가치
    public int      armyAI;
    public float    damagePlayer;               // 가한 피해량
    public float    damageAI;
    public int      gristmillPlayer;            // 제분소 건설 수
    public int      gristmillAI;
    public int      gristmillDestroyPlayer;     // 제분소 파괴 수
    public int      gristmillDestroyAI;

    // Graph Max
    public int      maxFood;
    public int      maxValue;

    // 초기화
    public void InitializeElement()
    {
        isGameEnded             = false;

        playerSnapshotList      = new List<Snapshot>();
        AISnapshotList          = new List<Snapshot>();

        /// 총 데이터 초기화
        foodPlayer              = 0;
        foodAI                  = 0;
        armyPlayer              = 0;
        armyAI                  = 0;
        damagePlayer            = 0.0f;
        damageAI                = 0.0f;
        gristmillPlayer         = 0;
        gristmillAI             = 0;
        gristmillDestroyPlayer  = 0;
        gristmillDestroyAI      = 0;

        maxFood = 0;
        maxValue = 0;
    }

    // 전투 시작 시 초기화
    public void GameStartStatistic()
    {
        /// 게임 모드
        curGameMode = GameManager.Instance.CurGameMode; 
        
        /// 진영 저장
        campPlayer = GameManager.Instance.CommanderList[0];
        if (GameMode.Tutorial == GameManager.Instance.CurGameMode)
            campAI = Camp.Archimedes;
        else
            campAI = GameManager.Instance.CommanderList[1];

        /// 스냅샷 리스트 초기화
        playerSnapshotList.Clear();
        AISnapshotList.Clear();

        /// 총 데이터 초기화
        gameTime                = 0.0f;
        foodPlayer              = 0;
        foodAI                  = 0;
        armyPlayer              = 0;
        armyAI                  = 0;
        damagePlayer            = 0.0f;
        damageAI                = 0.0f;
        gristmillPlayer         = 0;
        gristmillAI             = 0;
        gristmillDestroyPlayer  = 0;
        gristmillDestroyAI      = 0;

        maxFood = 0;
        maxValue = 0;
    }

    // 전투 종료 시 최종 데이터 저장
    public void GameEndStatistic(Camp loserCamp)
    {
        isGameEnded = true;

        // 승리 진영
        if (loserCamp == campPlayer)
        {
            winPlayer = false;
            winCamp = campAI;
        }
        else if (loserCamp == campAI)
        {
            winPlayer = true;
            winCamp = campPlayer;
        }
        else
            Debug.Log("진영값이 잘못되었습니다.");

        // 게임 시간
        gameTime = BattleUICtrl.Instance.PlayTime;


        // 최종 부대 가치
        foreach (var squad in SquadController.Instance.Squads[campPlayer])
            armyPlayer += SceneStarter.Instance.commonElements.CommonDataList[(int)squad.Type].Cost * squad.UnitList.Count;
        foreach (var squad in SquadController.Instance.Squads[campAI])
            armyAI += SceneStarter.Instance.commonElements.CommonDataList[(int)squad.Type].Cost * squad.UnitList.Count;
    }

    // 스냅샷 추가
    public void AddSnapshot()
    {
        int tempFood = 0;
        int tempValue = 0;

        //  플레이어
        tempFood = GameManager.Instance.CampFoodDic[campPlayer];
        foreach (var squad in SquadController.Instance.Squads[campPlayer])
            tempValue += SceneStarter.Instance.commonElements.CommonDataList[(int)squad.Type].Cost * squad.UnitList.Count;
        
        // 추가
        playerSnapshotList.Add(new Snapshot(tempFood, tempValue));

        // 최대값 갱신
        if (maxFood < tempFood)
            maxFood = tempFood;
        if (maxValue < tempValue)
            maxValue = tempValue;


        // AI
        tempFood = 0;
        tempValue = 0;

        tempFood = GameManager.Instance.CampFoodDic[campAI];
        foreach (var squad in SquadController.Instance.Squads[campAI])
            tempValue += SceneStarter.Instance.commonElements.CommonDataList[(int)squad.Type].Cost * squad.UnitList.Count;

        // 추가
        AISnapshotList.Add(new Snapshot(tempFood, tempValue));

        // 최대값 갱신
        if (maxFood < tempFood)
            maxFood = tempFood;
        if (maxValue < tempValue)
            maxValue = tempValue;
    }

    // 피해량 누적
    /// <summary>
    /// Hit 함수에서 호출되므로 가한 피해량은 반대 진영으로 누적된다. 
    /// </summary>
    /// <param name="_myCamp"> 맞은 진영 </param>
    public void AccumulateDamage(Camp _myCamp, float _damage)
    {
        if (_myCamp == campPlayer)
            damageAI += _damage;
        else if (_myCamp == campAI)
            damagePlayer += _damage;
        else
            Debug.Log("진영값이 잘못되었습니다.");
    }

    // 제분소 건설
    public void AddBuildGristmill(Camp _myCamp)
    {
        if (_myCamp == campPlayer)
            gristmillPlayer++;
        else if (_myCamp == campAI)
            gristmillAI++;
        else
            Debug.Log("진영값이 잘못되었습니다.");
    }

    // 제분소 파괴
    public void AddDestroyGrismill(Camp _myCamp)
    {
        if (_myCamp == campPlayer)
            gristmillDestroyAI++;
        else if (_myCamp == campAI)
            gristmillDestroyPlayer++;
        else
            Debug.Log("진영값이 잘못되었습니다.");
    }

    // 식량 갱신
    public void UpdateFood(Camp _myCamp)
    {
        if (_myCamp == campPlayer)
            foodPlayer++;
        else if (_myCamp == campAI)
            foodAI++;
        else
            Debug.Log("진영값이 잘못되었습니다.");
    }
}
