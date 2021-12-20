using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class UnitFSM
{
    public Animator Animator = null;
    public CommonUnit Unit = null;
    public CommonUnitFSM CommonFSM = null;

    protected Transform transform = null;

    public bool IsMove { get { return CommonFSM.IsMove; } set { CommonFSM.IsMove = value; } }

    public Vector3Int MoveStart { get { return CommonFSM.MoveStart; } set { CommonFSM.MoveStart = value; } }
    public Vector3Int MoveDest { get { return CommonFSM.MoveDest; } set { CommonFSM.MoveDest = value; } }
    public Vector3 MoveDir { get { return CommonFSM.MoveDir; } set { CommonFSM.MoveDir = value; } }

    /// <summary>
    /// 공격 속도 조절을 위한 시간 재는 변수
    /// </summary>
    public float TimeCount { get { return CommonFSM.TimeCount; } set { CommonFSM.TimeCount = value; } }
    //public GameObject AttackTarget  { get { return CommonFSM.AttackTarget; }set { CommonFSM.AttackTarget = value; } }

    public Character CommandedTarget { get { return CommonFSM.CommandedTarget; } set { CommonFSM.CommandedTarget = value; } }
    public Character AttackTarget { get { return CommonFSM.AttackTarget; } set { CommonFSM.AttackTarget = value; } }

    public bool IsCommandMove { get { return CommonFSM.IsCommandMove; } set { CommonFSM.IsCommandMove = value; } }

    public TileNode curTile { get { return CommonFSM.curTile; } set { CommonFSM.curTile = value; } }
    public List<TileNode> path { get { return CommonFSM.path; } set { CommonFSM.path = value; } }

    public CommonData Data { get { return Unit.Base.Data; } }
    public CommonBase Base { get { return Unit.Base; } }

    public Vector3 Pos { get { return Unit.transform.position; } }
    public Vector3Int CurCell { get { return CommonFSM.CurCell; } set { CommonFSM.CurCell = value; } }

    public virtual bool CanBeTarget()
    {

        return true;
    }

    public virtual bool IsIdle()
    {
        return true;
    }

    public virtual void Command_Move(Vector3 position)
    {
        IsCommandMove = true;

        //Vector3Int tilePos = TilemapSystem.Instance.WorldToCellPos(position);
        
        //    CommonFSM.UnitTileDest = tilePos;
    }

    public virtual void Command_Move(Vector3Int cellPos)
    {
        IsCommandMove = true;
    }

    public virtual void Command_Attack(GameObject target)
    {

    }

    public virtual void Command_Attack(Character target)
    {
        CommandedTarget = target;
    }

    public virtual void Ready()
    {

    }

    public virtual bool IsArrive()
    {
        var node = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(MoveDest));



        return Vector3.Distance(node.worldPosition, Unit.transform.position) < 0.1f;
    }

    public bool CheckTargetInRange()
    {
        if (null == AttackTarget)
            return false;

        if (!AttackTarget.gameObject.activeSelf || !AttackTarget.CanBeTarget())
        {
            AttackTarget = null;
            return false;
        }

        //return TilemapSystem.Instance.RangeInObject(Pos, AttackTarget.transform.position, Base.Range) != TilemapSystem.Invalid_Range;

        return Global.Calculate_TileDistance(transform.position, AttackTarget.transform.position) <= Base.Range;
    }

    /// <summary>
    /// 주변에 적이 있는지 확인한다.
    /// </summary>
    /// <returns>범위 안에 적 유닛이 있으면 true</returns>
    public virtual bool Scout_Enemy()
    {
        if (null != AttackTarget)
            return false;

        //GameObject gameObject = null;

        //if (InGameManager.Instance.Find_TargetInRange_ExceptCamp(Pos, Base.Range, Base.MyCamp, Base.AttackType, out gameObject) >= 0)
        //{
        //    AttackTarget = gameObject;
        //    return true;
        //}

        Character character = null;

        if (InGameManager.Instance.Find_TargetInRange_ExceptCamp(Pos, Base.Range, Base.MyCamp, Base.AttackType, out character) >= 0f)
        {
            AttackTarget = character;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 범위 안에 적이 있는지 확인
    /// </summary>
    /// <param name="range">확인하는 범위</param>
    /// <returns>범위 안에 적이 있으면 true</returns>
    public bool Scout_Enemy(int range)
    {
        Character character = null;

        bool found = InGameManager.Instance.Find_TargetInRange_ExceptCamp(Pos, range, Base.MyCamp, Base.AttackType, out character) >= 0;

        AttackTarget = character;
        return found;
        

    }

    public void Death()
    {
        switch (Data.PlaceType)
        {
            case PlaceType.Ground:
                if (SquadController.Instance.UnitTile[Base.MyCamp][CommonFSM.UnitTileDest].GroundUnit == Unit)
                    SquadController.Instance.UnitTile[Base.MyCamp][CommonFSM.UnitTileDest].GroundUnit = null;
                break;
            case PlaceType.Air:
                if (SquadController.Instance.UnitTile[Base.MyCamp][CommonFSM.UnitTileDest].AirUnit == Unit)
                    SquadController.Instance.UnitTile[Base.MyCamp][CommonFSM.UnitTileDest].AirUnit = null;
                break;
        }

        if (StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Contains(Unit))
            StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Remove(Unit);
    }

    /// <summary>
    /// 쥐의 대모 세팅을 위한 순수 가상 함수.
    /// </summary>
    /// <param name="owl">쥐가 따라다닐 대모</param>
    public virtual void Set_OwlOfMouse(GameObject owl)
    {

    }

    public virtual void Ready(GameObject parent, GameObject sprite, CommonUnitFSM commonFSM)
    {
        Animator = sprite.GetComponent<Animator>();
        Unit = parent.GetComponent<CommonUnit>();
        CommonFSM = commonFSM;
        transform = Unit.transform;
    }

    public virtual void Start()
    {

    }

    public virtual void Update()
    {
        if (null != CommandedTarget && (!CommandedTarget.gameObject.activeSelf || !CommandedTarget.CanBeTarget()))
            CommandedTarget = null;

        if (null != AttackTarget    && (!AttackTarget.gameObject.activeSelf || !AttackTarget.CanBeTarget()))
            AttackTarget = null;
    }

    public virtual void LateUpdate()
    {
        IsCommandMove = false;
    }

    /// <summary>
    /// 공격 타겟으로 향하는 경로를 찾는 함수.
    /// 만약에 경로를 찾을 필요 없이 바로 공격 가능하면 true 반환.
    /// </summary>
    public bool Check_InRangeAndFindPath(Character target = null)
    {
        if (null == target)
            target = AttackTarget;

        var fromTilePos = TilemapSystem.Instance.WorldToCellPos(transform.position);
        var toTilePos = TilemapSystem.Instance.WorldToCellPos(target.transform.position);

        //var tilePos = SquadController.Instance.Find_NearestTilePos(Base.MyCamp, fromTilePos, toTilePos, Base.Data.Range, Unit);
        //  공격하기 위해 자리잡을 위치를 찾음.
        var tilePos = SquadController.Instance.Find_PositionForAttack(Base.MyCamp, fromTilePos, toTilePos, Base.Range, Unit);

        //  자리를 잡을 수 없음.
        if (Global.InvalidTilePos == tilePos)
        {
            //AttackTarget = null;

            var nearTilePos = SquadController.Instance.Find_NearestTilePos(Base.MyCamp, toTilePos, Base.PlaceType);

            return false;
        }
        //  찾은 위치가 지금 있는 위치와 같음.
        else if (tilePos == fromTilePos)
        {
            CommonFSM.UnitTileDest = tilePos;

            //  이동할 필요가 없으므로 이동하기 위한 경로 초기화.
            if (null != path)
                path.Clear();
            curTile = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(tilePos));

            //  타일 위치는 같으나 타일 중앙에 위치하지 않을 수 있으므로, 중앙으로 이동하기 위해 이동 방향과 목적지를 설정헌다.
            IsMove = !(Vector3.Distance(curTile.worldPosition, transform.position) < 0.1f);

            MoveDir = (curTile.worldPosition - transform.position).normalized;
            MoveDest = tilePos;

            return !IsMove;
        }

        CommonFSM.UnitTileDest = tilePos;
        var worldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);

        //  목적지로 가기 위한 경로를 찾음.
        path = TilemapSystem.Instance.GetPath(transform.position, worldPos);

        //  목적지로 가는 경로를 찾을 수 없음.
        if (null == path)
        {
            Debug.Log("Check_InRangeAndFindPath() : path is null");
            return false;
        }

        path.RemoveAt(path.Count() - 1);
        curTile = path.Last();

        IsMove = true;

        // 현재 경로 노드로 설정된 노드로 향하는 방향벡터 구하기
        MoveDir = (curTile.worldPosition - transform.position).normalized;

        return false;
    }
    
    public bool Check_InRange_UsingVectorField(Character target = null)
    {
        if (null == target)
            target = AttackTarget;

        var fromTilePos = TilemapSystem.Instance.WorldToCellPos(transform.position);
        var toTilePos = TilemapSystem.Instance.WorldToCellPos(target.transform.position);

        var tilePos = SquadController.Instance.Find_PositionForAttack(Base.MyCamp, fromTilePos, toTilePos, Base.Range, Unit);

        //  자리를 잡을 수 없음.
        if (Global.InvalidTilePos == tilePos)
        {
            //Debug.Log("Can't not find location - toTilePos : " + toTilePos);
            tilePos = SquadController.Instance.Find_NearestTilePos(Unit, Base.MyCamp, toTilePos);
            if (Global.InvalidTilePos == tilePos)
            {
                Debug.Log("tilePos is Invalid");
                return false;
            }
        }
        //  찾은 위치가 지금 있는 위치와 같음.
        else if (tilePos == fromTilePos)
        {
            //Debug.Log("tilePos found for the attack is the same as where it is now.");

            CommonFSM.UnitTileDest = tilePos;

            var toWorldPos = TilemapSystem.Instance.CellToWorldPos(tilePos);
            var node = TilemapSystem.Instance.GetTile(toWorldPos);

            //  목표 지점과 충분히 가까우면 멈추고 true 반환.
            float dist = Vector3.Distance(node.worldPosition, transform.position);
            if (dist < 0.1)
            {
                CommonFSM.VFAgent.IsMove = false;
                //Debug.Log("목표 지점과 충분히 가까움");
                return true;
            }
            //else
            //{
            //    Debug.Log("목표 지점과 충분히 가깝지 않음. dist : " + dist);
            //    Debug.Log("curDest : " + CommonFSM.VFAgent.curDest + " nodePos : " + node.worldPosition + " position : " + transform.position);
            //}
           
        }
        CommonFSM.UnitTileDest = tilePos;

        var fieldKey = TilemapSystem.Instance.CellPosToGridPos(tilePos);
        CommonFSM.VFAgent.Move(fieldKey);

        return false;
    }

    public void UpdatePath(Vector3 destPos)
    {
        Vector3Int curPos = TilemapSystem.Instance.WorldToCellPos(transform.position);
        Vector3Int curDestPos = TilemapSystem.Instance.WorldToCellPos(destPos);

        // 현재 목적지와 현재 위치가 현재 마지막으로 길찾기를 시도한 목적지와 시작위치가 같다면 불필요햔 연산을 줄이기 위해 실행하지 않는다.
        if (MoveDest == curDestPos && curPos == MoveStart)
        {
            //Debug.Log("Same Path");
            return;
        }

        if (curPos == curDestPos)
        {
            Debug.Log("StartCell == DestCell");
            curTile = TilemapSystem.Instance.GetTile(destPos);

            IsMove = true;
            MoveStart = curPos;
            MoveDest = curDestPos;
            MoveDir = curTile.worldPosition - transform.position;
            MoveDir.Normalize();

            return;
            //TilemapSystem.Instance.tilemap.
        }

        path = TilemapSystem.Instance.GetPath(transform.position, destPos);

        if (path == null)
        {
            IsMove = false;
            Debug.Log("Invalid Path : path is null");
            return;
        }

        //Debug.Log("path.first - x : " + path[0].worldPosition.x.ToString() + " y : " + path[0].worldPosition.y.ToString());
        //Debug.Log(Data.CommonType.ToString() + " - path.first - x : " + path[0].X.ToString() + " y : " + path[0].Y.ToString());

        if (IsMove)
        {
            if (path.Count == 1)
            {
                Debug.Log("moveStart && Count == 1");
                return;
            }
        }

        // 현재 리스트가 경로의 역순으로 되어 있으므로 뒤에서부터 하나씩 노드를 빼 온다.

        path.RemoveAt(path.Count() - 1);
        curTile = path.Last();

        if (path.Count() == 0)
        {
            IsMove = false;
            curTile = null;
            return;
        }

        //  타일맵
        MoveStart = TilemapSystem.Instance.WorldToCellPos(transform.position);
        MoveDest = TilemapSystem.Instance.WorldToCellPos(destPos);

        IsMove = true;

        // 현재 경로 노드로 설정된 노드로 향하는 방향벡터 구하기
        MoveDir = curTile.worldPosition - transform.position;
        MoveDir.Normalize();

        //StartCoroutine(TilemapSystem.Instance.DrawPath(path));
    }

    public void PathMove(float speedScaling)
    {
        Unit.Move(MoveDir, speedScaling);

        if (Vector3.Distance(transform.position, curTile.worldPosition) < 0.1f)
        {
            if (null == path || path.Count == 0)
            {
                IsMove = false;
                curTile = null;
                return;
            }
            curTile = path.Last();
            path.RemoveAt(path.Count - 1);

            MoveDir = curTile.worldPosition - transform.position;
            MoveDir.Normalize();
        }
    }

    public void PathMove()
    {
        Unit.Move(MoveDir);

        if (Vector3.Distance(transform.position, curTile.worldPosition) < 0.1f)
        {
            if (null == path || path.Count == 0)
            {
                IsMove = false;
                curTile = null;
                return;
            }
            curTile = path.Last();
            path.RemoveAt(path.Count - 1);

            MoveDir = curTile.worldPosition - transform.position;
            MoveDir.Normalize();
        }
    }

    public virtual void OnDisable()
    {


    }

    public void Play_Unit_Voice(UnitSoundType soundType)
    {
        if (null != Unit.Voice)
        {
            if (!Unit.Voice.gameObject.activeSelf)
                Unit.Voice = null;
            else
                return;
        }

        var audios = SceneStarter.Instance.soundElements.UnitSoundDic[Base.Type][soundType];
        int index = Random.Range(0, audios.Count);

        SoundManager.Instance.Play(Sound_Channel.Voice, Unit.gameObject, audios[index]);
    }

    public void Play_Unit_Sound(UnitSoundType soundType, int startIndex, int endIndex)
    {
        var soundDic = SceneStarter.Instance.soundElements.UnitSoundDic;

        if (!soundDic.ContainsKey(Base.Type))
        {
            Debug.Log("UnitSoundDic don't contain " + Base.Type + " key.");
            return;
        }

        if (!soundDic[Base.Type].ContainsKey(soundType))
        {
            Debug.Log("UnitSoundDic[" + Base.Type + "] don't contain " + soundType + " key.");
            return;
        }

        var audioArray = SceneStarter.Instance.soundElements.UnitSoundDic[Base.Type][soundType];
        if (endIndex > audioArray.Count)
            endIndex = audioArray.Count;

        int random = Random.Range(startIndex, endIndex);

        SoundManager.Instance.Play(Sound_Channel.Effect, Unit.gameObject, audioArray[random]);
    }

    public void Play_Unit_Sound(UnitSoundType soundType, int index)
    {
        var soundDic = SceneStarter.Instance.soundElements.UnitSoundDic;

        if (!soundDic.ContainsKey(Base.Type))
        {
            Debug.Log("UnitSoundDic don't contain " + Base.Type + " key.");
            return;
        }

        if (!soundDic[Base.Type].ContainsKey(soundType))
        {
            Debug.Log("UnitSoundDic[" + Base.Type + "] don't contain " + soundType + " key.");
            return;
        }

        var audioArray = SceneStarter.Instance.soundElements.UnitSoundDic[Base.Type][soundType];

        SoundManager.Instance.Play(Sound_Channel.Effect, Unit.gameObject, audioArray[index]);
    }

    public void Play_Unit_Sound(UnitSoundType soundType)
    {
        var soundDic = SceneStarter.Instance.soundElements.UnitSoundDic;

        if (!soundDic.ContainsKey(Base.Type))
        {
            Debug.Log("UnitSoundDic don't contain " + Base.Type + " key.");
            return;
        }

        if (!soundDic[Base.Type].ContainsKey(soundType))
        {
            Debug.Log("UnitSoundDic["+ Base.Type +"] don't contain " + soundType + " key.");
            return;
        }

        var audioArray = SceneStarter.Instance.soundElements.UnitSoundDic[Base.Type][soundType];
        int random = Random.Range(0, audioArray.Count);

        SoundManager.Instance.Play(Sound_Channel.Effect, Unit.gameObject, audioArray[random]);
    }

    public void Play_Unit_PositionSound(UnitSoundType soundType)
    {
        var soundDic = SceneStarter.Instance.soundElements.UnitSoundDic;

        if (!soundDic.ContainsKey(Base.Type))
        {
            Debug.Log("UnitSoundDic don't contain " + Base.Type + " key.");
            return;
        }

        if (!soundDic[Base.Type].ContainsKey(soundType))
        {
            Debug.Log("UnitSoundDic[" + Base.Type + "] don't contain " + soundType + " key.");
            return;
        }

        var audioArray = SceneStarter.Instance.soundElements.UnitSoundDic[Base.Type][soundType];
        int random = Random.Range(0, audioArray.Count);

        SoundManager.Instance.Play(Sound_Channel.Effect, Unit.Pos, audioArray[random]);
    }
}
