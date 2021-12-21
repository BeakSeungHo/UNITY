using UnityEngine;

/// <summary>
/// 
/// 작성자 : 김영현 (20-09-20)
/// 
/// 모든 유닛으로 사용할 수 있는 클래스.
/// 
/// 풀 매니저에서 뽑은 뒤에, 어느 진영의 어떤 유닛으로 사용할 지 Ready 함수를 호출해주어야 한다.
/// 
/// 
/// 
/// 
/// 
/// </summary>

public class CommonUnit : Unit
{
    public GameObject Sprite = null;
    public GameObject FSM = null;
    public GameObject Sound = null;
    public HPCanvas HpCanvas = null;
    public SpriteRenderer SpriteRenderer = null;
    public SpriteRenderer minimapSprite = null;

    private CommonUnitFSM unitFSM = null;

    public bool isDead = false;
    public bool MoveLocationSetting = false;

    //public Collider2D Collider = null;
    public GameObject FirePos = null;

    public Vector3Int UnitTileDest { get { return unitFSM.UnitTileDest; } }

    public Sound Voice = null;

    /// <summary>
    /// 이 오브젝트가 공격의 타겟이 될 수 있는지 알려준다.
    /// </summary>
    /// <returns>FSM의 CanBeTarget을 반환한다.</returns>
    public override bool CanBeTarget()
    {
        return unitFSM.CanBeTarget;
    }

    /// <summary>
    /// 현재 유닛 상태가 Idle인지 체크
    /// </summary>
    /// <returns></returns>
    public bool IsIdle()
    {
        return unitFSM.IsIdle;
    }

    public override void Command_Move(Vector3 position)
    {
        if (PlaceType.Ground == Base.PlaceType)
        {
            if (!TilemapSystem.Instance.IsWalkableTile(position))
            {
                var tilePos = SquadController.Instance.Find_NearestTilePos(this, Base.MyCamp, position);
                position = TilemapSystem.Instance.CellToWorldPos(tilePos);
            }
        }


        unitFSM.Command_Move(position);
        //Debug.Log("CommonUnit : Command_Move");
    }
    
    public override void Command_Move(Vector3Int cellPos)
    {
        var position = TilemapSystem.Instance.CellToWorldPos(cellPos);
        var node = TilemapSystem.Instance.GetTile(position);

        Command_Move(node.worldPosition);
    }

    public override void Command_Attack(GameObject target)
    {
        unitFSM.Command_Attack(target);
    }

    public override void Command_Attack(Character target)
    {
        if (PlaceType.Air == target.Base.PlaceType)
        {
            if (AttackType.Ground == Base.AttackType)
            {
                Vector3 position = target.transform.position;

                var tilePos = SquadController.Instance.Find_NearestTilePos(this, Base.MyCamp, position);

                position = TilemapSystem.Instance.CellToWorldPos(tilePos);
                var node = TilemapSystem.Instance.GetTile(position);
                if (null == node)
                {
                    Debug.Log("Command_Attack : node is null");
                    return;
                }
                position = node.worldPosition;

                Command_Move(position);
                return;
            }
        }

        unitFSM.Command_Attack(target);
    }

    //  활성화 할 때 마다 호출해줘야 함.
    public bool Ready(Camp camp, CommonType Type)
    {
        Base.MyCamp = camp;
        Base.Type = Type;

        //  애니메이터 컨트롤러 설정.
        Sprite.GetComponent<Animator>().runtimeAnimatorController = SceneStarter.Instance.animatorElements.UnitAniDic[Type];

        //  스프라이트 위치 설정
        PositionInfo.Ready(Type);

        //  돌릴 FSM 설정.
        unitFSM.Ready(Type);

        // HP 색깔,위치 설정
        HpCanvas.Ready();
        SetMiniSpriteColor(camp);
        Base.Reinforcement = GameManager.Instance.CampUnitReinforcement[camp][Type];

        HP = Base.MaxHp;

        isDead = false;

        return true;
    }

    public bool Ready(Camp camp, CommonType Type, Vector3 worldPosition)
    {
        Base.MyCamp = camp;
        Base.Type = Type;

        transform.position = worldPosition;

        //  애니메이터 컨트롤러 설정.
        if (!SceneStarter.Instance.animatorElements.UnitAniDic.ContainsKey(Type))
        {
            Debug.Log("CommonUnit Animation Setting failed, there is no key : " + Type);
        }
        Sprite.GetComponent<Animator>().runtimeAnimatorController = SceneStarter.Instance.animatorElements.UnitAniDic[Type];

        //  스프라이트 위치 설정
        PositionInfo.Ready(Type);

        //  돌릴 FSM 설정.
        unitFSM.Ready(Type);

        // HP 색깔,위치 설정
        HpCanvas.Ready();
        SetMiniSpriteColor(camp);

        if (CommonType.Mouse != Base.Type)
            Base.Reinforcement = GameManager.Instance.CampUnitReinforcement[camp][Type];

        HP = Base.MaxHp;

        isDead = false;

        var cellPos = TilemapSystem.Instance.WorldToCellPos(worldPosition);

        if (Global.InvalidTilePos == cellPos)
            Debug.Log("CommonUnit : Ready failed, cellPos is Invalid");
        else
        {
            unitFSM.CurCell = cellPos;
            unitFSM.CommandedTilePos = cellPos;
        }

        SquadController.Instance.Unit_Move(this, camp, worldPosition);

        if (GameManager.Instance.CurGameMode != GameMode.Tutorial)
        {
            //  등장 사운드
            unitFSM.Play_Unit_Voice(UnitSoundType.Idle);
        }
        
        return true;
    }

    public bool Ready(Camp camp, CommonType type, Vector3 worldPosition, GameObject Owl)
    {
        Ready(camp, type, worldPosition);

        unitFSM.Set_OwlOfMouse(Owl);

        return true;
    }

    public override void Update()
    {
        base.Update();
        FogOfWar.Instance.CheckSprite(transform.position, HpCanvas, minimapSprite.gameObject, SpriteRenderer);
    }


    public override void LateUpdate()
    {
        base.LateUpdate();
        MoveLocationSetting = false;
    }

    public override void Awake()
    {
        base.Awake();

        unitFSM = FSM.GetComponent<CommonUnitFSM>();
        //Ready(Base.MyCamp, Base.Data.CommonType);
    }

    // 미니맵 스프라이트 색 정하는 함수
    public void SetMiniSpriteColor(Camp camp)
    {
        switch (camp)
        {
            case Camp.Bellafide:
                minimapSprite.color = Global.MinimapColorCommanderBellafide;
                break;
            case Camp.Hopper:
                minimapSprite.color = Global.MinimapColorCommanderHopper;
                break;
            case Camp.Quartermaster:
                minimapSprite.color = Global.MinimapColorCommanderQuartermaster;
                break;
            case Camp.Archimedes:
                minimapSprite.color = Global.MinimapColorCommanderArchimedes;
                break;
        }
    }

}
