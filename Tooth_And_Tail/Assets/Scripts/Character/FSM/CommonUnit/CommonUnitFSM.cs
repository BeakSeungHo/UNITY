using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// 작성자 : 김영현 (20-09-02)
/// 
/// 유닛의 상태패턴을 일괄적으로 관리하는 클래스.
/// 유닛을 풀 매니저에서 뽑아 쓸 수 있도록 이 클래스가 모든 유닛의 FSM 을 가지고 있다.
/// 
/// 
/// </summary>


public class CommonUnitFSM : MonoBehaviour
{
    public enum FSMType { Normal, Flying, Advanced, Mouse, Chameloen, End };

    private Dictionary<FSMType, UnitFSM> fSMDictionary = null;
    //private List<UnitFSM> fSMList = null;

    private FSMType fSMType = FSMType.Normal;

    public GameObject Parent = null;
    public GameObject Sprite = null;

    public CommonBase CommonBase = null;
    public CommonUnit CommonUnit = null;
    public Animator   Animator = null;
    public VectorFieldAgent VFAgent = null;

    public bool ViewPath = false;

    [HideInInspector] public bool IsMove = false;

    [HideInInspector] public Vector3Int MoveStart = Vector3Int.zero;
    /*[HideInInspector]*/
    public Vector3Int MoveDest = Vector3Int.zero;
    /*[HideInInspector]*/
    public Vector3 MoveDir = Vector3.zero;

    [HideInInspector] public float TimeCount = 0f;
    /// <summary>
    /// 공격 명령 당한 타겟을 저장하는 변수
    /// </summary>
    public Character CommandedTarget = null;
    /// <summary>
    /// 공격하려고 하는 타겟을 저장하는 변수
    /// </summary>
    public Character AttackTarget = null;

    public bool IsCommandMove = false;

    [HideInInspector] public TileNode curTile = null;
    [HideInInspector] public List<TileNode> path = null;

    [HideInInspector] public Vector3Int CurCell = Vector3Int.zero;

    public Vector3Int UnitTilePos = Vector3Int.zero;
    public Vector3Int UnitTileDest
    {
        get
        {
            return UnitTilePos;
        }
        set
        {
            if (SquadController.Instance.UnitTile[CommonBase.MyCamp].ContainsKey(UnitTilePos))
            {
                if (PlaceType.Ground == CommonBase.Data.PlaceType)
                {
                    if (SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].GroundUnit == CommonUnit)
                        SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].GroundUnit = null;
                }
                else if (PlaceType.Air == CommonBase.Data.PlaceType)
                {
                    if (SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].AirUnit == CommonUnit)
                        SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].AirUnit = null;
                }
            }
            UnitTilePos = value;

            if (SquadController.Instance.UnitTile[CommonBase.MyCamp].ContainsKey(UnitTilePos))
            {
                if (PlaceType.Ground == CommonBase.Data.PlaceType)
                    SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].GroundUnit = CommonUnit;
                else if (PlaceType.Air == CommonBase.Data.PlaceType)
                    SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].AirUnit = CommonUnit;
            }
        }
    }
    public Vector3Int CommandedTilePos = Vector3Int.zero;

    public bool CanBeTarget { get { return fSMDictionary[fSMType].CanBeTarget(); } }

    public bool IsIdle { get { return fSMDictionary[fSMType].IsIdle(); } }

    public void Command_Move(Vector3 position)
    {
        //fSMList[(int)fSMType].Command_Move(position);
        //Debug.Log("CommonUnitFSM : Command_Move");
        fSMDictionary[fSMType].Command_Move(position);
        CommandedTilePos = TilemapSystem.Instance.WorldToCellPos(position);
        //if (ViewPath)
        //    StartCoroutine(TilemapSystem.Instance.DrawPath(path));
    }

    public void Command_Move(Vector3Int cellPos)
    {
        fSMDictionary[fSMType].Command_Move(cellPos);
        //if (ViewPath)
        //    StartCoroutine(TilemapSystem.Instance.DrawPath(path));
    }

    public void Command_Attack(GameObject target)
    {
        //fSMList[(int)fSMType].Command_Attack(target);
        fSMDictionary[fSMType].Command_Attack(target);
    }

    public void Command_Attack(Character target)
    {
        fSMDictionary[fSMType].Command_Attack(target);
    }

    public void Ready(CommonType type)
    {
        switch (type)
        {
            case CommonType.Squirrel:
            case CommonType.Lizard:
            case CommonType.Toad:
            case CommonType.Mole:
            case CommonType.Ferret:
            case CommonType.Skunk:
            case CommonType.Snake:
            case CommonType.Fox:
                fSMType = FSMType.Normal;
                break;
            case CommonType.Chameleon:
                fSMType = FSMType.Chameloen;
                break;
            case CommonType.Pigeon:
            case CommonType.Falcon:
            case CommonType.Owl:
                fSMType = FSMType.Flying;
                break;
            case CommonType.Badger:
            case CommonType.Boar:
            case CommonType.Wolf:
                fSMType = FSMType.Advanced;
                break;
            case CommonType.Mouse:
                fSMType = FSMType.Mouse;
                break;
            default:
                Debug.Log("CommonUnitFSM Error : type is not unit");
                break;
        }

        Initialize();

        IsMove = false;
        MoveStart = Vector3Int.zero;
        MoveDest = Vector3Int.zero;
        MoveDir = Vector3.zero;

        TimeCount = 0f;
        AttackTarget = null;

        IsCommandMove = false;

        curTile = null;

        Update_TileObjectPos();

        fSMDictionary[fSMType].Ready();
    }

    public void Set_OwlOfMouse(GameObject Owl)
    {
        fSMDictionary[fSMType].Set_OwlOfMouse(Owl);
    }

    private void Awake()
    {
        Initialize();
    }

    private void Initialize()
    {
        if (null == fSMDictionary)
        {
            fSMDictionary = new Dictionary<FSMType, UnitFSM>();

            fSMDictionary.Add(FSMType.Normal, new NormalUnitFSM());
            fSMDictionary.Add(FSMType.Flying, new FlyingUnitFSM());
            fSMDictionary.Add(FSMType.Advanced, new AdvancedUnitFSM());
            fSMDictionary.Add(FSMType.Mouse, new MouseUnitFSM());
            fSMDictionary.Add(FSMType.Chameloen, new ChameleonUnitFSM());

            foreach (var pair in fSMDictionary)
                pair.Value.Ready(Parent, Sprite, this);
        }
        if (null != Parent)
        {
            if (null == CommonBase)
                CommonBase = Parent.GetComponent<CommonBase>();
            if (null == CommonUnit)
                CommonUnit = Parent.GetComponent<CommonUnit>();
        }

    }

    private void Update_TileObjectPos()
    {
        if (null == TilemapSystem.Instance ||
            null == StorageBoxes.Instance ||
            null == StorageBoxes.Instance.TileObjects)
            return;
        //  현재 위치에 따른 위치 셋팅.
        Vector3Int curCellPos = TilemapSystem.Instance.WorldToCellPos(Parent.transform.position);
        if (curCellPos != CurCell)
        {
            if (StorageBoxes.Instance.TileObjects.ContainsKey(CurCell) &&
                StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Contains(CommonUnit))
                StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Remove(CommonUnit);

            CurCell = curCellPos;
            if (StorageBoxes.Instance.TileObjects.ContainsKey(CurCell))
                StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Add(CommonUnit);
            else
            {
                Debug.Log("TileObjects have not key : " + CurCell);
            }
        }
    }

    private void Start()
    {
        //fSMList[(int)fSMType].Start();
        fSMDictionary[fSMType].Start();
    }

    private void Update()
    {
        //  물 위에 올라갔을 때 에니메이션 변경.
        if (PlaceType.Ground == CommonUnit.Base.PlaceType)
            Animator.SetBool("Swim", CommonUnit.IsOnWater);

        //fSMList[(int)fSMType].Update();
        //  공격 타겟이 죽었을 경우 AttackTarget 비우기
        if (null != AttackTarget && 
            (!AttackTarget.activeSelf || 
            !AttackTarget.CanBeTarget() || 
            !FogOfWar.Instance.CheckTileAlpha(AttackTarget.transform.position, CommonUnit.Base.MyCamp)))
            AttackTarget = null;

        //  현재 위치에 따른 위치 셋팅.
        Update_TileObjectPos();
        
        fSMDictionary[fSMType].Update();
    }

    private void LateUpdate()
    {
        fSMDictionary[fSMType].LateUpdate();
    }

    private void OnDisable()
    {
        if (null == StorageBoxes.Instance?.TileObjects ||
            null == SquadController.Instance?.UnitTile ||
            null == SceneStarter.Instance?.commonElements)
            return;

        //  현재 위치에 관련된 타일 위치 초기화.
        if (StorageBoxes.Instance.TileObjects.ContainsKey(CurCell) &&
            StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Contains(CommonUnit))
            StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Remove(CommonUnit);
        CurCell = Vector3Int.zero;

        if (SquadController.Instance.UnitTile[CommonBase.MyCamp].ContainsKey(UnitTilePos))
        {
            if (PlaceType.Ground == CommonBase.PlaceType)
            {
                if (SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].GroundUnit == CommonUnit)
                    SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].GroundUnit = null;
            }
            else if (PlaceType.Air == CommonBase.PlaceType)
            {
                if (SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].AirUnit == CommonUnit)
                    SquadController.Instance.UnitTile[CommonBase.MyCamp][UnitTilePos].AirUnit = null;
            }
        }
        UnitTilePos = Vector3Int.zero;

        curTile = null;
        path = null;

        //  타겟 비우기
        AttackTarget = null;
        CommandedTarget = null;

        //  FSM 비우기
        fSMDictionary[fSMType].OnDisable();
    }

    public void Play_Unit_Voice(UnitSoundType soundType)
    {
        fSMDictionary[fSMType].Play_Unit_Voice(soundType);
    }

    public void Play_Unit_Sound(UnitSoundType soundType)
    {
        fSMDictionary[fSMType].Play_Unit_Sound(soundType);
    }
}