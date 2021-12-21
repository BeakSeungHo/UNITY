using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// 작성자 : 김영현 (20-09-02)
/// 
/// 컨트롤러
/// Singleton 으로 구성하였음.
/// DontDestroyOnLoad 사용함.
/// Ready_Squad 함수로 초기화 하고 사용하여야 함.
/// 
/// Ready_Squad(진영, 유닛 타입 리스트) : 선택한 진영에 유닛 타입 리스트를 기준으로 준비한다.
/// Add_Unit(진영, 유닛)                : 선택한 진영에 유닛을 넣는다. 유닛의 타입에 따라 분대를 찾아서 넣음.
/// Add_Unit(진영, 분대 번호, 유닛)     : 선택한 진영 안에 선택한 분대 번호 안헤 유닛을 넣는다.
/// 
/// Command_Move_All(진영, 위치)        : 선택한 진영의 모든 유닛에게 해당 위치로 이동하는 명령을 내린다.
/// Command_Move_All(진영, 타일)        : 선택한 진영의 모든 유닛에게 해당 타일로 이동하는 명령을 내린다.
/// Command_Attack_All(진영, 타겟)      : 선택한 진영의 모든 유닛에게 해당 타겟을 공격하는 명령을 내린다.
/// 
/// Command_Move(진영, 분대번호, 위치)  : 선택한 진영의 특정 분대 유닛들에게 해당 위치로 이동하는 명령을 내린다.
/// Command_Move(진영, 분대번호, 타일)  : 선택한 진영의 특정 분대 유닛들에게 해당 타일로 이동하는 명령을 내린다.
/// Command_Attack(진영, 분대번호, 타겟): 선택한 진영의 특정 분대 유닛들에게 해당 타겟을 공격하는 명령을 내린다.
/// 
/// Update() or LateUpdate()            : 모든 진영의 유닛을 순회하며 죽었는지 확인한다. 
///                                     : 죽었을 경우 유닛 리스트에서 제거하고, 풀 매니저에 유닛을 반환한다.
/// 
/// 
/// 
/// 
/// </summary>

public struct Squad
{
    public CommonType Type;
    public LinkedList<CommonUnit> UnitList;

    public Squad(CommonType type)
    {
        Type = type;
        UnitList = new LinkedList<CommonUnit>();
    }

}

public class SquadTile
{
    public CommonUnit GroundUnit = null;
    public CommonUnit AirUnit = null;
}

public delegate bool ConditionToFindUnit(GameObject obj1, GameObject obj2);
public delegate float DistBetweenObjects(GameObject obj1, GameObject obj2);

public class SquadController : MonoBehaviour
{
    public static SquadController Instance = null;

    public Dictionary<Camp, List<Squad>> Squads = null;

    public Dictionary<Camp, List<CommonType>> SquadNumberToType = null;
    public Dictionary<Camp, Dictionary<CommonType, int>> TypeToSquadNumber = null;

    public Dictionary<Camp, Dictionary<Vector3Int, SquadTile>> UnitTile = null;

    readonly int[] dirX = { 1, -1, 0, 0, 1, 1, -1, -1 };
    readonly int[] dirY = { 0, 0, 1, -1, 1, -1, 1, -1 };

    Queue<Vector3Int> movingPosGround = new Queue<Vector3Int>();
    Queue<Vector3Int> movingPosAir = new Queue<Vector3Int>();
    HashSet<Vector3Int> checkVisitGround = new HashSet<Vector3Int>(new Vector3IntComparer());
    HashSet<Vector3Int> checkVisitAir = new HashSet<Vector3Int>(new Vector3IntComparer());

    private Dictionary<CommonType, Dictionary<UnitSoundType, List<AudioClip>>> unitSoundDic = null;
    private Sound CommandSound = null;

    public void Play_CommandUnitSound(Camp camp, CommonType type)
    {
        //if (!ReferenceEquals(CommandSound, null))
        if (!ReferenceEquals(null, CommandSound))
        {
            if (CommandSound.gameObject.activeSelf)
                return;
            else
                CommandSound = null;
        }

        var voiceSquadNumber = TypeToSquadNumber[camp][type];

        if (0 == Squads[camp][voiceSquadNumber].UnitList.Count)
        {
            return;
        }

        var voiceUnit = Squads[camp][voiceSquadNumber].UnitList.First.Value;

        if (null == unitSoundDic)
            unitSoundDic = SceneStarter.Instance.soundElements.UnitSoundDic;

        if (!unitSoundDic.ContainsKey(type))
        {
            Debug.Log("unitSoundDic has not " + type + " key");
            return;
        }
        if (!unitSoundDic[type].ContainsKey(UnitSoundType.Cry))
        {
            Debug.Log("unitSoundDic[" + type + "] has not " + UnitSoundType.Cry + " key");
            return;
        }

        var audioArray = SceneStarter.Instance.soundElements.UnitSoundDic[type][UnitSoundType.Cry];
        var audioIndex = Random.Range(0, audioArray.Count);

        SoundManager.Instance.Play(Sound_Channel.Voice, voiceUnit.gameObject, audioArray[audioIndex], out CommandSound);

    }

    public void Play_CommandUnitSound(Camp camp)
    {
        if (!ReferenceEquals(null, CommandSound))
        {
            if (CommandSound.gameObject.activeSelf)
                return;
            else
                CommandSound = null;
        }

        List<CommonType> types = new List<CommonType>();

        for (int i = 0; i < Squads[camp].Count; ++i)
        {
            if (0 == Squads[camp][i].UnitList.Count)
                continue;

            types.Add(Squads[camp][i].Type);
        }

        if (0 == types.Count)
            return;

        int typeIndex = Random.Range(0, types.Count);

        Play_CommandUnitSound(camp, types[typeIndex]);
    }

    /// <summary>
    /// 특정 진영을 제외한 진영에서 범위 안의 유닛을 찾는 함수
    /// </summary>
    /// <param name="from">범위의 중심 위치</param>
    /// <param name="range">타일 범위</param>
    /// <param name="exceptCamp">제외할 진영</param>
    /// <param name="placeType"></param>
    /// <param name="foundObject">찾은 오브젝트를 반환받을 변수</param>
    /// <returns>찾으면 true, 못 찾으면 false </returns>
    public bool Find_UnitInRange_ExceptCamp(Vector3 from, int range, Camp exceptCamp, AttackType attackType, out GameObject foundObject)
    {
        foreach (var squadCamp in Squads)
        {
            if (squadCamp.Key == exceptCamp)
                continue;
            if (Find_CampUnitInRange(from, range, squadCamp.Key, attackType, out foundObject))
                return true;
        }

        foundObject = null;
        return false;
    }

    /// <summary>
    /// 특정 진영 안에 있는 유닛을 찾는 함수
    /// </summary>
    /// <param name="from">범위의 중심 위치</param>
    /// <param name="range">타일 범위</param>
    /// <param name="campToFind">찾을 진영</param>
    /// <param name="foundObject">찾은 오브젝트를 반환받을 변수</param>
    /// <returns>찾으면 true, 못 찾으면 false</returns>
    public bool Find_CampUnitInRange(Vector3 from, int range, Camp campToFind, AttackType attackType, out GameObject foundObject)
    {
        foreach (var squad in Squads[campToFind])
        {
            foreach (var unit in squad.UnitList)
            {
                if (!unit.CanBeTarget())
                    continue;

                Vector3 to = unit.Pos;

                switch (attackType)
                {
                    case AttackType.Ground:
                        if (unit.Base.Data.PlaceType != PlaceType.Ground)
                            continue;
                        break;
                    case AttackType.Air:
                        if (unit.Base.Data.PlaceType != PlaceType.Air)
                            continue;
                        break;
                }

                //if (TilemapSystem.Instance.RangeInObject(from, to, range) != TilemapSystem.Invalid_Range)

                float dist = Global.Calculate_TileDistance(from, to);
                if (dist <= range)
                {
                    foundObject = unit.gameObject;
                    return true;
                }
            }
        }
        foundObject = null;
        return false;
    }

    /// <summary>
    /// 특정 진영에서 특정 조건을 만족하는 오브젝트를 반환한다.
    /// </summary>
    /// <param name="fromObject">중심이 되는 오브젝트</param>
    /// <param name="campToFind">찾을 진영</param>
    /// <param name="foundObject">찾은 오브젝트</param>
    /// <param name="condition">조건</param>
    /// <returns>찾으면 true, 못 찾으면 false</returns>
    public bool Find_CampUnitInRange(GameObject fromObject, Camp campToFind, out GameObject foundObject, ConditionToFindUnit condition)
    {
        foreach (var squad in Squads[campToFind])
        {
            foreach (var unit in squad.UnitList)
            {
                if (condition(fromObject, unit.gameObject))
                {
                    foundObject = unit.gameObject;
                    return true;
                }
            }
        }

        foundObject = null;
        return false;
    }

    /// <summary>
    /// 스쿼드 전체를 돌면서 조건에 맞는 유닛중 가장 가까운 유닛을 찾는다.
    /// </summary>
    /// <param name="fromObject"></param>
    /// <param name="campToFind"></param>
    /// <param name="foundObject"></param>
    /// <param name="findDistance"></param>
    /// <returns></returns>
    public float Find_NearestCampUnitInRange(GameObject fromObject, Camp campToFind, out GameObject foundObject, DistBetweenObjects findDistance)
    {
        foundObject = null;
        float minDist = -1;
        foreach (var squad in Squads[campToFind])
        {
            foreach (var unit in squad.UnitList)
            {
                float dist = findDistance(fromObject, unit.gameObject);

                if (dist < 0)
                    continue;

                if (minDist > dist || minDist < 0)
                {
                    foundObject = unit.gameObject;
                    minDist = dist;
                }
            }
        }

        return minDist;
    }

    public float Find_NearestCampUnitInRange(GameObject fromObject, Camp campToFind, out Character foundUnit, DistBetweenObjects findDistance)
    {
        foundUnit = null;
        float minDist = -1;
        foreach (var squad in Squads[campToFind])
        {
            foreach (var unit in squad.UnitList)
            {
                float dist = findDistance(fromObject, unit.gameObject);

                if (dist < 0)
                    continue;

                if (minDist > dist || minDist < 0)
                {
                    foundUnit = unit;
                    minDist = dist;
                }
            }
        }

        return minDist;
    }

    /// <summary>
    /// 출발 위치를 기준으로 원형탐색하여 도착 위치와의 거리가 range 안에 있는 위치를 찾는다.
    /// </summary>
    /// <param name="camp">진영</param>
    /// <param name="fromTilePos">출발 위치</param>
    /// <param name="toTilePos">도착 위치</param>
    /// <param name="range">범위</param>
    /// <param name="character">탐색하는 유닛</param>
    /// <returns>찾은 위치</returns>
    public Vector3Int Find_NearestTilePos(Camp camp, Vector3Int fromTilePos, Vector3Int toTilePos, int range, Character character)
    {
        PlaceType placeType = character.Base.PlaceType;

        var waitPos = PlaceType.Ground == placeType ? movingPosGround : movingPosAir;
        var checkVisit = PlaceType.Ground == placeType ? checkVisitGround : checkVisitAir;

        waitPos.Clear();
        checkVisit.Clear();

        waitPos.Enqueue(fromTilePos);
        checkVisit.Add(fromTilePos);

        //  무한 루프를 막기 위한 카운트
        int loopCount = 0;
        int MaxLoop = 100;
        //  너비 우선 탐색
        while (waitPos.Count > 0)
        {
            var tilePos = waitPos.Dequeue();

            //  유닛이 올라갈 수 있는 타일인가?
            if (TilemapSystem.Instance.IsWalkableTile(tilePos))
            {
                //  유닛의 위치가 땅인지 유닛인지에 따라 저장 위치 바꿈.
                var unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][tilePos].GroundUnit : UnitTile[camp][tilePos].AirUnit;

                //  거리 확인
                if ((tilePos - toTilePos).magnitude <= range)
                {
                    //  빈 공간이거나, 그자리에 이미 있는 경우
                    if (null == unitSpace || unitSpace == character)
                    {
                        waitPos.Clear();
                        checkVisit.Clear();
                        return tilePos;
                    }
                }
            }

            //var worldPosition = TilemapSystem.Instance.CellToWorldPos(tilePos);
            //var node = TilemapSystem.Instance.GetTile(worldPosition);

            ////  존재하는 타일인가?
            //if (null != node)
            //{
            //    //  높이가 1 이하인가?
            //    if (node.Height < 1)
            //    {
            //        //  유닛의 위치가 땅인지 유닛인지에 따라 저장 위치 바꿈.
            //        var unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][tilePos].GroundUnit : UnitTile[camp][tilePos].AirUnit;

            //        //  거리 확인
            //        if ((tilePos - toTilePos).magnitude <= range)
            //        {
            //            //  빈 공간이거나, 그자리에 이미 있는 경우
            //            if (null == unitSpace || unitSpace == character)
            //            {
            //                waitPos.Clear();
            //                checkVisit.Clear();
            //                return tilePos;
            //            }
            //        }
            //    }
            //}

            for (int i = 0; i < 8; ++i)
            {
                var nextTile = tilePos + new Vector3Int(dirX[i], dirY[i], 0);
                if (!TilemapSystem.Instance.HasTile(nextTile))
                    continue;

                if (checkVisit.Contains(nextTile))
                    continue;

                waitPos.Enqueue(nextTile);
                checkVisit.Add(nextTile);
            }

            ++loopCount;
            if (loopCount > MaxLoop)
            {
                Debug.Log("loopCount : " + loopCount.ToString());
                //  찾지 못함.
                return Global.InvalidTilePos;
            }
        }

        //  찾지 못함.
        return Global.InvalidTilePos;
    }

    /// <summary>
    /// 공격을 위한 자리잡기 함수.
    /// </summary>
    /// <param name="camp">진영</param>
    /// <param name="fromTilePos">출발 위치</param>
    /// <param name="toTilePos">도착 위치</param>
    /// <param name="range">공격 범위</param>
    /// <param name="character">공격하는 캐릭터</param>
    /// <returns>타일 위치</returns>
    public Vector3Int Find_PositionForAttack(Camp camp, Vector3Int fromTilePos, Vector3Int toTilePos, int range, Character character)
    {
        PlaceType placeType = character.Base.PlaceType;
        Vector3Int retTilePos = Global.InvalidTilePos;

        var dist = Vector3Int.Distance(fromTilePos, toTilePos);
        var searchTiles = StorageBoxes.Instance.CircleSearchList;

        int startDist = range;

        //{
        //    var unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][fromTilePos].GroundUnit : UnitTile[camp][fromTilePos].AirUnit;

        //    if (character == unitSpace)
        //        return fromTilePos;

        //    for (int i = 0; i <= range; ++i)
        //    {

        //    }
        //}


        //  사거리 안에 있을 때.
        if (dist <= range)
        {
            //  현재 위치에 유닛이 있는지 확인.
            var unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][fromTilePos].GroundUnit : UnitTile[camp][fromTilePos].AirUnit;
            if (character == unitSpace)
                //  있으면 그 위치 반환.
                return fromTilePos;

            //  출발 위치 기준으로 원형으로 빈 공간 탐색.
            for (int i = 0; i <= range; ++i)
            {
                foreach (var addTile in searchTiles[i])
                {
                    var tilePos = fromTilePos + addTile;

                    //  유닛이 갈 수 있는 곳인지 확인
                    if (!TilemapSystem.Instance.IsWalkableTile(tilePos))
                        continue;

                    //  사거리 확인
                    var distForDest = Vector3Int.Distance(tilePos, toTilePos);

                    //  사거리 밖이면 제외
                    if (distForDest > range)
                        continue;

                    unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][tilePos].GroundUnit : UnitTile[camp][tilePos].AirUnit;

                    if (character == unitSpace || null == unitSpace)
                        return tilePos;
                }
            }

            //  위 for 문에서 return 하지 않았으면, 위치를 못 찾은 것.
            //  도착점 기준으로 한번 더 탐색.
            for (int i = 0; i <= range; ++i)
            {
                foreach (var addTile in searchTiles[i])
                {
                    var tilePos = toTilePos + addTile;

                    //  유닛이 갈 수 있는 곳인지 확인
                    if (!TilemapSystem.Instance.IsWalkableTile(tilePos))
                        continue;

                    //  사거리 확인
                    var distForDest = Vector3Int.Distance(tilePos, toTilePos);

                    //  사거리 밖이면 제외
                    if (distForDest > range)
                        continue;

                    unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][tilePos].GroundUnit : UnitTile[camp][tilePos].AirUnit;

                    if (character == unitSpace || null == unitSpace)
                        return tilePos;
                }
            }
        }
        //  사거리 밖에 있을 때
        else
        {
            Vector3 delta = fromTilePos - toTilePos;

            delta.Normalize();
            delta *= range;

            //  탐색 시작 위치를 to 기준으로 from에서 가장 가까운 사거리 안 타일 부터 시작한다.
            Vector3Int startTile = new Vector3Int(Mathf.FloorToInt(delta.x), Mathf.FloorToInt(delta.y), 0) + toTilePos;

            //  원형으로 탐색을 시작한다.
            for (int i = 0; i <= StorageBoxes.Instance.CircleSearchMaxRange; ++i)
            {
                foreach (var addTile in searchTiles[i])
                {
                    var tilePos = startTile + addTile;

                    //  유닛이 갈 수 있는 곳인지 확인
                    if (!TilemapSystem.Instance.IsWalkableTile(tilePos))
                        continue;

                    var distance = Vector3Int.Distance(tilePos, toTilePos);

                    if (distance <= range)
                    {
                        var unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][tilePos].GroundUnit : UnitTile[camp][tilePos].AirUnit;

                        if (character == unitSpace || null == unitSpace)
                            return tilePos;
                    }
                }
            }
        }

        return retTilePos;
    }

    public Vector3Int Find_NearestTilePos(Camp camp, Vector3Int tilePos, PlaceType placeType)
    {
        var circularList = StorageBoxes.Instance.CircleSearchList;
        var circularListMaxRange = StorageBoxes.Instance.CircleSearchMaxRange;

        for (int i = 0; i < circularListMaxRange; ++i)
        {
            foreach (var addTile in circularList[i])
            {
                var addedTilePos = tilePos + addTile;

                if (!TilemapSystem.Instance.IsWalkableTile(tilePos))
                    continue;

                CommonUnit unitOnSpace = null;

                switch (placeType)
                {
                    case PlaceType.Ground:
                        unitOnSpace = UnitTile[camp][tilePos].GroundUnit;
                        break;
                    case PlaceType.Air:
                        unitOnSpace = UnitTile[camp][tilePos].AirUnit;
                        break;
                }

                if (!ReferenceEquals(null, unitOnSpace))
                    continue;

                return addedTilePos;
            }
        }

        return Global.InvalidTilePos;

        //var unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][tilePos].GroundUnit : UnitTile[camp][tilePos].AirUnit;

        //if (null == unitSpace)
        //    return tilePos;

        //var waitPos = PlaceType.Ground == placeType ? movingPosGround : movingPosAir;
        //var checkVisit = PlaceType.Ground == placeType ? checkVisitGround : checkVisitAir;

        //waitPos.Clear();
        //checkVisit.Clear();

        //waitPos.Enqueue(tilePos);
        //checkVisit.Add(tilePos);

        ////  무한 루프를 막기 위한 카운트
        //int loopCount = 0;
        ////  너비 우선 탐색
        //while (waitPos.Count > 0)
        //{
        //    var retTilePos = waitPos.Dequeue();

        //    //  유닛의 위치가 땅인지 유닛인지에 따라 저장 위치 바꿈.
        //    var checkSpace = PlaceType.Ground == placeType ? UnitTile[camp][tilePos].GroundUnit : UnitTile[camp][tilePos].AirUnit;

        //    if (null == checkSpace)
        //    {
        //        waitPos.Clear();
        //        checkVisit.Clear();
        //        return retTilePos;
        //    }
        //    else
        //    {
        //        for (int i = 0; i < 8; ++i)
        //        {
        //            var nextTile = tilePos + new Vector3Int(dirX[i], dirY[i], 0);
        //            if (!TilemapSystem.Instance.HasTile(nextTile))
        //                continue;

        //            if (checkVisit.Contains(nextTile))
        //                continue;

        //            waitPos.Enqueue(nextTile);
        //            checkVisit.Add(nextTile);
        //        }
        //    }

        //    ++loopCount;
        //    if (loopCount > 100)
        //    {
        //        Debug.Log("loopCount : 100");
        //        //  찾지 못함.
        //        return Global.InvalidTilePos;
        //    }
        //}

        //return Global.InvalidTilePos;
    }

    public Vector3Int Find_NearestTilePos(CommonUnit unit, Camp camp, Vector3Int tilePos)
    {
        CommonType type = unit.Base.Data.CommonType;
        PlaceType placeType = unit.Base.Data.PlaceType;


        var circularList = StorageBoxes.Instance.CircleSearchList;
        var circularListMaxRange = StorageBoxes.Instance.CircleSearchMaxRange;
        for (int i = 0; i < circularListMaxRange; ++i)
        {
            foreach (var addTile in circularList[i])
            {
                var destTilePos = tilePos + addTile;

                if (!TilemapSystem.Instance.IsWalkableTile(tilePos))
                    continue;

                CommonUnit unitOnSpace = null;

                switch (placeType)
                {
                    case PlaceType.Ground:
                        unitOnSpace = UnitTile[camp][destTilePos].GroundUnit;
                        break;
                    case PlaceType.Air:
                        unitOnSpace = UnitTile[camp][destTilePos].AirUnit;
                        break;
                }

                if (unit == unitOnSpace)
                    return destTilePos;

                if (!ReferenceEquals(null, unitOnSpace))
                    continue;

                return destTilePos;
            }
        }

        return Global.InvalidTilePos;
    }

    public Vector3Int Find_NearestTilePos(CommonUnit unit, Camp camp, Vector3 worldPos)
    {
        CommonType type = unit.Base.Data.CommonType;
        PlaceType placeType = unit.Base.Data.PlaceType;

        Vector3Int destTilePos = TilemapSystem.Instance.WorldToCellPos(worldPos);

        var circularList = StorageBoxes.Instance.CircleSearchList;
        var circularListMaxRange = StorageBoxes.Instance.CircleSearchMaxRange;
        for (int i = 0; i < circularListMaxRange; ++i)
        {
            foreach (var addTile in circularList[i])
            {
                var tilePos = destTilePos + addTile;

                if (!TilemapSystem.Instance.IsWalkableTile(tilePos))
                    continue;

                CommonUnit unitOnSpace = null;

                switch (placeType)
                {
                    case PlaceType.Ground:
                        unitOnSpace = UnitTile[camp][tilePos].GroundUnit;
                        break;
                    case PlaceType.Air:
                        unitOnSpace = UnitTile[camp][tilePos].AirUnit;
                        break;
                }

                if (unit == unitOnSpace)
                    return tilePos;

                if (!ReferenceEquals(null, unitOnSpace))
                    continue;

                return tilePos;
            }
        }

        return Global.InvalidTilePos;
    }

    //  임시 테스트용 유닛 죽이는 함수.
    public void Test_KillUnit(Camp camp, int squad)
    {
        var squadList = Squads[camp];

        if (null == squadList)
            return;

        if (squad >= squadList.Count)
            return;

        if (squadList[squad].UnitList.Count < 1)
            return;

        squadList[squad].UnitList.First.Value.HP = 0;
    }

    //  유닛 추가 함수.
    public bool Add_Unit(Camp camp, GameObject unitObject)
    {
        CommonUnit unit = unitObject.GetComponent<CommonUnit>();

        if (null == unit)
            return false;

        return Add_Unit(camp, unit);
    }

    //  유닛 배열에서 올바른 타입의 위치를 찾아 넣어준다.
    public bool Add_Unit(Camp camp, CommonUnit unit)
    {
        CommonType cType = unit.Base.Data.CommonType;

        var squadList = Squads[camp];

        for (int i = 0; i < squadList.Count; ++i)
        {
            if (squadList[i].Type == cType)
            {
                squadList[i].UnitList.AddLast(unit);
                return true;
            }
        }
        return false;
    }

    //  스커드 번호를 직접 지정해서 넣어준다.
    public bool Add_Unit(Camp camp, int squadNum, GameObject unitObject)
    {
        CommonUnit unit = unitObject.GetComponent<CommonUnit>();

        if (null == unit)
            return false;

        return Add_Unit(camp, squadNum, unit);
    }

    public bool Add_Unit(Camp camp, int squadNum, CommonUnit unit)
    {
        if (null == unit)
            return false;

        if (null == Squads)
            return false;

        var squadList = Squads[camp];

        if (null == squadList)
            return false;

        if (squadNum < 0 || squadList.Count <= squadNum)
            return false;

        CommonType cType = unit.Base.Data.CommonType;

        if (squadList[squadNum].Type == cType)
            squadList[squadNum].UnitList.AddLast(unit);

        return true;
    }

    private int SortByRange(CommonType type1, CommonType type2)
    {
        return SceneStarter.Instance.commonElements.CommonDataList[(int)type2].Range - SceneStarter.Instance.commonElements.CommonDataList[(int)type1].Range;
    }

    public void Command_Move_All(Camp camp, Vector3 position)
    {
        var squadList = Squads[camp];

        List<CommonType> order = new List<CommonType>();

        for (int i = 0; i < squadList.Count; ++i)
        {
            if (CommonType.Mouse == SquadNumberToType[camp][i])
                continue;
            order.Add(SquadNumberToType[camp][i]);
        }

        order.Sort(SortByRange);
        var CommandedTilePos = TilemapSystem.Instance.WorldToCellPos(position);

        //  원형 배치를 위한 리스트
        var CircularList = StorageBoxes.Instance.CircleSearchList;

        int groundDist = 0;
        var groundIndex = 0;

        int airDist = 0;
        var airIndex = 0;

        //  정렬한 순서대로 순회
        for (int i = 0; i < order.Count; ++i)
        {
            CommonType commonType = order[i];
            PlaceType placeType = SceneStarter.Instance.commonElements.CommonDataList[(int)commonType].PlaceType;
            int squadNumber = TypeToSquadNumber[camp][commonType];

            foreach (var unit in squadList[squadNumber].UnitList)
            {
                var tilePos = CommandedTilePos;
                switch (placeType)
                {
                    case PlaceType.Ground:
                        tilePos += CircularList[groundDist][groundIndex];
                        //  갈수 있는 땅인지 체크
                        //while (!TilemapSystem.Instance.IsWalkableTile(tilePos))
                        while (!IsArrivable_ForGroundUnit(unit, tilePos))
                        {
                            ++groundIndex;
                            if (CircularList[groundDist].Count <= groundIndex)
                            {
                                ++groundDist;

                                if (groundDist >= StorageBoxes.Instance.CircleSearchMaxRange)
                                {
                                    Debug.Log("groundDist : " + groundDist.ToString());
                                    return;
                                }

                                groundIndex = 0;
                            }
                            tilePos = CommandedTilePos + CircularList[groundDist][groundIndex];
                        }
                        unit.Command_Move(TilemapSystem.Instance.CellToWorldPos(tilePos));

                        //  다음 노드
                        ++groundIndex;
                        //  null 이면 노드 없음 dist 올림.
                        if (CircularList[groundDist].Count <= groundIndex)
                        {
                            ++groundDist;

                            if (groundDist >= StorageBoxes.Instance.CircleSearchMaxRange)
                            {
                                Debug.Log("groundDist : " + groundDist.ToString());
                                return;
                            }

                            groundIndex = 0;
                        }

                        break;
                    case PlaceType.Air:
                        tilePos += CircularList[airDist][airIndex];

                        unit.Command_Move(TilemapSystem.Instance.CellToWorldPos(tilePos));

                        ++airIndex;
                        if (CircularList[airDist].Count <= airIndex)
                        {
                            ++airDist;
                            airIndex = 0;
                        }

                        break;
                }
            }
        }
    }

    public void Command_Move_Pigeon(Camp camp, CommonUnit unit, Vector3 position)
    {
        var CircularList = StorageBoxes.Instance.CircleSearchList;

        int pigeonRange = unit.Base.Range;
        //  비둘기가 위치할 포지션은?
        var commandedTilePos = TilemapSystem.Instance.WorldToCellPos(position);
        var fromTilePos = TilemapSystem.Instance.WorldToCellPos(unit.transform.position);

        //  목표 지점과 지금 위치간의 거리
        float fromToCommandedDist = Vector3Int.Distance(fromTilePos, commandedTilePos);

        //  거리가 0보다 크다.
        if (0f < fromToCommandedDist)
        {
            //  이미 사거리 내에 위치하고 있을 경우
            if (fromToCommandedDist <= pigeonRange)
            {
                //  그 위치에 다른 유닛이 없을 경우
                if (null == UnitTile[camp][fromTilePos].AirUnit ||
                    unit == UnitTile[camp][fromTilePos].AirUnit)
                {
                    var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(fromTilePos));
                    unit.Command_Move(node.worldPosition);
                    return;
                }
            }

            float ratio = pigeonRange / fromToCommandedDist;

            var commandedToFrom = fromTilePos - commandedTilePos;
            commandedToFrom.x = Mathf.RoundToInt(commandedToFrom.x * ratio);
            commandedToFrom.y = Mathf.RoundToInt(commandedToFrom.y * ratio);

            var toTilePos = commandedTilePos + commandedToFrom;

            //  목표에서 비둘기 사거리 만큼 떨어진 자리에서 원형 탐색
            for (int i = 0; i <= pigeonRange; ++i)
            {
                foreach (var addTile in CircularList[i])
                {
                    var tilePos = toTilePos + addTile;

                    if (!TilemapSystem.Instance.IsInBoundsTile(tilePos) ||
                        !ReferenceEquals(null, UnitTile[camp][tilePos].AirUnit) ||
                        Vector3Int.Distance(tilePos, commandedTilePos) > pigeonRange)
                        continue;

                    //Debug.Log("원형 탐색에서 자리 찾음");
                    var worldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);
                    var node = TilemapSystem.Instance.GetTile(worldPos);
                    unit.Command_Move(node.worldPosition);
                    return;
                }
            }

            //Debug.Log("첫번째 순회에서 자리를 못 찾았음.");
            //  거기서 위치를 못찾았을 경우 목표 지점을 기준으로 원형 탐색 시작
            for (int i = 0; i < StorageBoxes.Instance.CircleSearchMaxRange; ++i)
            {
                foreach (var addTile in CircularList[i])
                {
                    var tilePos = commandedTilePos + addTile;

                    if (!TilemapSystem.Instance.IsInBoundsTile(tilePos) ||
                        !ReferenceEquals(null, UnitTile[camp][tilePos].AirUnit))
                        continue;

                    var worldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);
                    var node = TilemapSystem.Instance.GetTile(worldPos);
                    unit.Command_Move(node.worldPosition);
                    return;
                }
            }
        }
        else
        {
            //Debug.Log("거리가 0임");
            unit.Command_Move(position);
        }

        Debug.Log("비둘기 이동위치를 찾을 수 없음.");
    }

    //  모든 유닛을 특정 타겟을 공격시킨다.
    public void Command_Attack_All(Camp camp, GameObject target)
    {
        var squadList = Squads[camp];
        for (int squad = 0; squad < squadList.Count; ++squad)
            Command_Attack(camp, squad, target);
    }

    public void Command_Attack_All(Camp camp, Character target)
    {
        var squadList = Squads[camp];
        for (int squad = 0; squad < squadList.Count; ++squad)
            Command_Attack(camp, squad, target);
    }

    private Vector3Int NextPosition(Queue<Vector3Int> movingPos, HashSet<Vector3Int> checkVisit)
    {
        if (movingPos.Count == 0)
        {
            Debug.Log("NextPosition Error, movingPos is empty");
        }

        Vector3Int retPos = movingPos.Dequeue();

        for (int i = 0; i < 8; ++i)
        {
            int x = retPos.x + dirX[i];
            int y = retPos.y + dirY[i];
            Vector3Int nextPos = new Vector3Int(x, y, 0);

            if (!TilemapSystem.Instance.IsWalkableTile(nextPos))
                continue;

            if (checkVisit.Contains(nextPos))
                continue;

            movingPos.Enqueue(nextPos);
            checkVisit.Add(nextPos);
        }

        return retPos;
    }

    bool IsThereEmptyPlace(Vector3Int tilePos, int squad, Camp camp, PlaceType placeType)
    {
        for (int i = 0; i < squad; ++i)
        {
            CommonType frontSquadType = SquadNumberToType[camp][i];
            PlaceType frontSquadPlaceType = SceneStarter.Instance.commonElements.CommonDataList[(int)frontSquadType].PlaceType;

            if (frontSquadPlaceType != placeType)
                continue;

            foreach (var frontUnit in Squads[camp][i].UnitList)
            {
                //if (frontUnit.MoveLocationSetting)
                //    return false;

                Vector3Int frontUnitTilePos = TilemapSystem.Instance.WorldToCellPos(frontUnit.Pos);
                if (frontUnitTilePos == tilePos)
                {
                    frontUnit.MoveLocationSetting = true;
                    return false;
                }
            }
        }
        return true;
    }

    public void Unit_Move(CommonUnit unit, Camp camp, Vector3 worldPos)
    {
        CommonType type = unit.Base.Data.CommonType;
        PlaceType placeType = unit.Base.Data.PlaceType;

        Vector3Int tilePos = TilemapSystem.Instance.WorldToCellPos(worldPos);

        if (!UnitTile.ContainsKey(camp))
        {
            Debug.Log("There is no camp : " + camp);
            Debug.Log("Squads Count : " + Squads.Count);
        }

        for (int i = 0; i < StorageBoxes.Instance.CircleSearchMaxRange; ++i)
        {
            foreach (var addTile in StorageBoxes.Instance.CircleSearchList[i])
            {
                var addedTilePos = tilePos + addTile;

                //  유닛이 갈 수 있는 곳인가?
                if (!TilemapSystem.Instance.IsWalkableTile(addedTilePos))
                    continue;

                var unitSpace = PlaceType.Ground == placeType ? UnitTile[camp][addedTilePos].GroundUnit : UnitTile[camp][addedTilePos].AirUnit;

                //  이미 그곳에 유닛이 있는가?
                if (!ReferenceEquals(null, unitSpace))
                {
                    //  그곳에 있는 유닛이 인풋으로 들어온 유닛인가?
                    if (unit == unitSpace)
                        return;
                    else
                        continue;
                }

                //  위 조건에서 여기까지 내려왔으면, 빈공간이다.
                tilePos = addedTilePos;
                unit.Command_Move(TilemapSystem.Instance.CellToWorldPos(tilePos));
                return;
            }
        }



        //var movingPos = PlaceType.Ground == placeType ? movingPosGround : movingPosAir;
        //var checkVisit = PlaceType.Ground == placeType ? checkVisitGround : checkVisitAir;

        //movingPos.Enqueue(tilePos);
        //checkVisit.Add(tilePos);

        //switch (placeType)
        //{
        //    case PlaceType.Ground:
        //        while (null != UnitTile[camp][tilePos].GroundUnit)
        //        {
        //            if (unit == UnitTile[camp][tilePos].GroundUnit)
        //            {
        //                movingPos.Clear();
        //                checkVisit.Clear();
        //                return;
        //            }
        //            tilePos = NextPosition(movingPos, checkVisit);
        //        }

        //        break;
        //    case PlaceType.Air:
        //        while (null != UnitTile[camp][tilePos].AirUnit)
        //        {
        //            if (unit == UnitTile[camp][tilePos].AirUnit)
        //            {
        //                movingPos.Clear();
        //                checkVisit.Clear();
        //                return;
        //            }
        //            tilePos = NextPosition(movingPos, checkVisit);
        //        }
        //        break;
        //}

        //unit.Command_Move(TilemapSystem.Instance.CellToWorldPos(tilePos));

        //movingPos.Clear();
        //checkVisit.Clear();
    }

    private void Command_Move(Camp camp, int squad, PlaceType placeType, Queue<Vector3Int> movingPos, HashSet<Vector3Int> checkVisit)
    {
        Vector3Int tilePos = NextPosition(movingPos, checkVisit);

        Queue<CommonUnit> movingUnit = new Queue<CommonUnit>();

        foreach (var unit in Squads[camp][squad].UnitList)
            movingUnit.Enqueue(unit);

        while (0 != movingUnit.Count)
        {
            var unit = movingUnit.Dequeue();

            if (unit.MoveLocationSetting)
                continue;

            int unitRange = unit.Base.Data.Range;

            switch (placeType)
            {
                case PlaceType.Ground:
                    while (!ReferenceEquals(null, UnitTile[camp][tilePos].GroundUnit))
                    {
                        int tileUnitRange = UnitTile[camp][tilePos].GroundUnit.Base.Range;
                        if (tileUnitRange > unitRange)
                            tilePos = NextPosition(movingPos, checkVisit);
                        else
                        {
                            movingUnit.Enqueue(UnitTile[camp][tilePos].GroundUnit);
                            break;
                        }
                    }
                    break;
                case PlaceType.Air:
                    while (!ReferenceEquals(null, UnitTile[camp][tilePos].AirUnit))
                    {
                        int tileUnitRange = UnitTile[camp][tilePos].AirUnit.Base.Range;
                        if (tileUnitRange > unitRange)
                            tilePos = NextPosition(movingPos, checkVisit);
                        else
                        {
                            movingUnit.Enqueue(UnitTile[camp][tilePos].AirUnit);
                            break;
                        }
                    }
                    break;
            }

            unit.Command_Move(TilemapSystem.Instance.CellToWorldPos(tilePos));
            tilePos = NextPosition(movingPos, checkVisit);
            unit.MoveLocationSetting = true;
        }
    }

    private bool SortByRange(CommonUnit unit1, CommonUnit unit2)
    {
        if (unit1.Base.Range == unit2.Base.Range)
            return unit1.Base.Type > unit2.Base.Type;
        return unit1.Base.Range > unit2.Base.Range;
    }

    public void Command_Move(Camp camp, int squad, Vector3 position)
    {
        var squadList = Squads[camp];

        CommonType type = SquadNumberToType[camp][squad];

        PlaceType placeType = SceneStarter.Instance.commonElements.CommonDataList[(int)type].PlaceType;

        Vector3Int commandedTilePos = TilemapSystem.Instance.WorldToCellPos(position);

        //  원형 배치를 위한 리스트
        var CircularList = StorageBoxes.Instance.CircleSearchList;

        int dist = 0;
        int index = 0;

        Vector3Int tilePos = commandedTilePos + CircularList[dist][index];

        Queue<CommonUnit> movingUnit = new Queue<CommonUnit>();

        foreach (var unit in Squads[camp][squad].UnitList)
            movingUnit.Enqueue(unit);

        while (0 != movingUnit.Count)
        {
            var unit = movingUnit.Dequeue();

            if (unit.MoveLocationSetting)
                continue;

            int unitRange = unit.Base.Range;

            switch (placeType)
            {
                case PlaceType.Ground:
                    while (!ReferenceEquals(null, UnitTile[camp][tilePos].GroundUnit))
                    {
                        int tileUnitRange = UnitTile[camp][tilePos].GroundUnit.Base.Range;
                        if (tileUnitRange > unitRange)
                        {
                            do
                            {
                                ++index;
                                if (CircularList[dist].Count <= index)
                                {
                                    ++dist;
                                    index = 0;
                                }

                                tilePos = commandedTilePos + CircularList[dist][index];
                            }
                            //while (!TilemapSystem.Instance.IsWalkableTile(tilePos));
                            while (!IsArrivable_ForGroundUnit(unit, tilePos));
                        }
                        else
                        {
                            movingUnit.Enqueue(UnitTile[camp][tilePos].GroundUnit);
                            break;
                        }
                    }
                    break;
                case PlaceType.Air:
                    while (!ReferenceEquals(null, UnitTile[camp][tilePos].AirUnit))
                    {
                        int tileUnitRange = UnitTile[camp][tilePos].AirUnit.Base.Range;
                        if (tileUnitRange > unitRange)
                        {
                            do
                            {
                                ++index;
                                if (CircularList[dist].Count <= index)
                                {
                                    ++dist;
                                    index = 0;
                                }
                                tilePos = commandedTilePos + CircularList[dist][index];
                            }
                            while (!TilemapSystem.Instance.IsInBoundsTile(tilePos));
                        }
                        else
                        {
                            movingUnit.Enqueue(UnitTile[camp][tilePos].AirUnit);
                            break;
                        }
                    }
                    break;
            }

            var tileWorldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);
            var tileNode = TilemapSystem.Instance.GetTile(tileWorldPos);

            if (null == tileNode)
            {
                Debug.Log("Command_Move : tileNode is null");
                continue;
            }
            unit.Command_Move(tileNode.worldPosition);
            do
            {
                ++index;
                if (CircularList[dist].Count <= index)
                {
                    ++dist;
                    index = 0;
                }
                tilePos = commandedTilePos + CircularList[dist][index];
            }
            while (!TilemapSystem.Instance.IsWalkableTile(tilePos));
            unit.MoveLocationSetting = true;
        }
    }

    public bool IsArrivable_ForGroundUnit(CommonUnit unit, Vector3Int cellPos)
    {
        return IsArrivable_ForGroundUnit(unit, TilemapSystem.Instance.CellToWorldPos(cellPos));
    }

    public bool IsArrivable_ForGroundUnit(CommonUnit unit, Vector3 worldPos)
    {
        if (!TilemapSystem.Instance.IsWalkableTile(worldPos))
            return false;

        //var unitNode = TilemapSystem.Instance.GetTile(unit.Pos);

        //if (null == unitNode)
        //{
        //    Debug.Log("IsArrivable_ForGroundUnit : unitNode is null");
        //    return false;
        //}

        //var gridPos = TilemapSystem.Instance.WorldToTilePos(unitNode.worldPosition);
        //if (!unitNode.VectorField.ContainsKey(gridPos))
        //    return false;

        return true;
    }

    //  특정 스쿼드만 특정 타겟을 공격시킨다.
    public void Command_Attack(Camp camp, int squad, GameObject target)
    {
        var squadList = Squads[camp];
        foreach (var unit in squadList[squad].UnitList)
        {
            if (!unit.isDead)
                unit.Command_Attack(target);
        }
    }

    public void Command_Attack(Camp camp, int squad, Character target)
    {
        var squadList = Squads[camp];
        foreach (var unit in squadList[squad].UnitList)
        {
            if (!unit.isDead)
                unit.Command_Attack(target);
        }
    }
    /// <summary>
    /// 스커드 준비 함수. 진영과 선택된 타입 리스트를 전달 받는다.
    /// </summary>
    /// <param name="camp">준비할 진영</param>
    /// <param name="TypeList">유닛 타입 리스트</param>
    public void Ready_Squad(Camp camp, List<CommonType> TypeList)
    {
        Debug.Log("Ready_Squad Camp : " + camp);
        if (null == Squads[camp])
            Squads.Add(camp, new List<Squad>());

        if (null == SquadNumberToType[camp])
            SquadNumberToType.Add(camp, new List<CommonType>());

        if (null == TypeToSquadNumber[camp])
            TypeToSquadNumber.Add(camp, new Dictionary<CommonType, int>(new CommonTypeComparer()));

        if (Squads[camp].Count > 0)
            Squads[camp].Clear();

        if (TypeToSquadNumber[camp].Count > 0)
            TypeToSquadNumber[camp].Clear();

        if (SquadNumberToType[camp].Count > 0)
            SquadNumberToType[camp].Clear();

        int count = 0;
        foreach (var cType in TypeList)
        {
            Squads[camp].Add(new Squad(cType));
            SquadNumberToType[camp].Add(cType);
            TypeToSquadNumber[camp].Add(cType, count++);
        }

        if (TypeList.Contains(CommonType.Owl))
        {
            Squads[camp].Add(new Squad(CommonType.Mouse));
            SquadNumberToType[camp].Add(CommonType.Mouse);
            TypeToSquadNumber[camp].Add(CommonType.Mouse, count++);
        }

        if (null == UnitTile)
            UnitTile = new Dictionary<Camp, Dictionary<Vector3Int, SquadTile>>(new CampComparer());

        if (!UnitTile.ContainsKey(camp))
        {
            UnitTile.Add(camp, null);
        }

        if (null == UnitTile[camp])
        {
            if (!ReferenceEquals(null, TilemapSystem.Instance))
            {
                BoundsInt tileBounds = TilemapSystem.Instance.tileBounds;
                if (!ReferenceEquals(null, tileBounds))
                {
                    UnitTile[camp] = new Dictionary<Vector3Int, SquadTile>();

                    for (int x = tileBounds.xMin, i = 0; i < tileBounds.size.x; ++i, ++x)
                    {
                        for (int y = tileBounds.yMin, j = 0; j < tileBounds.size.y; ++j, ++y)
                        {
                            UnitTile[camp].Add(new Vector3Int(x, y, 0), new SquadTile());
                        }
                    }
                }
            }
        }
    }

    private void Awake()
    {
        Instance = this;

        Squads = new Dictionary<Camp, List<Squad>>(new CampComparer());
        SquadNumberToType = new Dictionary<Camp, List<CommonType>>(new CampComparer());
        TypeToSquadNumber = new Dictionary<Camp, Dictionary<CommonType, int>>(new CampComparer());

        for (int i = 0; i < (int)Camp.End; ++i)
        {
            Squads.Add((Camp)i, new List<Squad>());
            SquadNumberToType.Add((Camp)i, new List<CommonType>());
            TypeToSquadNumber.Add((Camp)i, new Dictionary<CommonType, int>(new CommonTypeComparer()));
        }

        UnitTile = new Dictionary<Camp, Dictionary<Vector3Int, SquadTile>>(new CampComparer());

        //  Sound 연결
        unitSoundDic = SceneStarter.Instance.soundElements.UnitSoundDic;
    }

    //  매 프레임 스커드 안에있는 유닛을 순회하면서 사망했는지 확인.
    //  사망하였을 때 유닛을 리스트에서 제거하고, 풀 매니저에 반환한다.
    private void Update()
    {
        if (null == Squads)
            return;

        foreach (var squads in Squads)
        {
            var squadList = squads.Value;
            for (int i = 0; i < squadList.Count; ++i)
            {
                var unitList = squadList[i].UnitList;
                for (var node = unitList.First; !ReferenceEquals(null, node);)
                {
                    CommonUnit unit = node.Value.GetComponent<CommonUnit>();

                    Camp camp = unit.Base.MyCamp;
                    CommonType type = unit.Base.Type;

                    if (unit.isDead)
                    {
                        if (CommonType.Mouse != type)
                        {
                            BuildingManager.Instance.DeathUnit(camp, type);
                            BuildingManager.Instance.CheckCanProduction(camp, type);
                        }

                        GameObject PushObject = node.Value.transform.gameObject;

                        var tempNode = node;
                        node = node.Next;

                        unitList.Remove(tempNode);
                        PoolManager.Instance.PushObject(PushObject, Pool_ObjType.Unit_Normal);
                    }
                    else
                        node = node.Next;
                }
            }
        }
    }

    private void LateUpdate()
    {

    }
}
