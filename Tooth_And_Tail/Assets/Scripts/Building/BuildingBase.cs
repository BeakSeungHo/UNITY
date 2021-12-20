using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBase : Character
{
    public static int NeutralBuildCost = 60;

    private CommonData data;

    Animator animator;

    Transform Body = null;

    SpriteRenderer spriteRenderer = null;

    BuildingStateOperator stateOperator;

    SpriteRenderer fogSprite = null;

    public HPCanvas hpCanvas = null;
    public SpriteRenderer minimapSprite = null;
    public ExtendBattleUICtrl extendBuildUICtrl = null;
    public ExtendBattleUICtrl extendSellUICtrl = null;
    public Farm farm = null;
    public Transform smokePoints;

    public bool IsNeutral = false;

    bool isDestroyed = false; // 한 번이라도 파괴 된 적이 있으면 true

    [SerializeField] int constructLevel;

    public Pool_ObjType Product_PoolType;

    public CommonType Product_Type;

    public BuildingState PreState = BuildingState.Idle;

    List<Vector2Int> occupyTiles;

    Coroutine recoveryCoroutine = null;

    Sound loopSound = null;

    public bool fireFlag;
    public bool smokeFlag;
    bool BuildSmoke;

    // Start is called before the first frame update
    public override void Awake()
    {
        base.Awake();
        stateOperator = GetComponent<BuildingStateOperator>();
        Body = transform.Find("Body");
        fogSprite = transform.Find("FogSprite").GetComponent<SpriteRenderer>();
        animator = Body.GetComponent<Animator>();
        spriteRenderer = Body.GetComponent<SpriteRenderer>();
        Base = GetComponent<CommonBase>();
        if (data == null)
            data = Base.Data;
        if (Base.Type == CommonType.Cabin)
            HPCanvas.Ready();
        HP = Base.MaxHp;
        occupyTiles = new List<Vector2Int>();
    }

    void Start()
    {
        if (data.CommonType == CommonType.Gristmill || data.CommonType == CommonType.Cabin)
        {
            Vector2Int[] indices = new Vector2Int[4];
            indices[0] = TilemapSystem.Instance.WorldToTilePos(transform.position);
            indices[1] = new Vector2Int(indices[0].x + 1, indices[0].y);
            indices[2] = new Vector2Int(indices[0].x, indices[0].y + 1);
            indices[3] = new Vector2Int(indices[0].x + 1, indices[0].y + 1);

            bool check = false;

            for (int i = 0; i < 4; i++)
            {
                if (TilemapSystem.Instance.GetTile(indices[i]) == null)
                {
                    check = true;
                    break;
                }
            }

            if (check == false)
            {
                for (int i = 0; i < 4; i++)
                {
                    TilemapSystem.Instance.GetTile(indices[i]).Occupy(this, 1, true);
                    OccupyTiles.Add(indices[i]);
                }
            }
            else
            {
                Debug.Log("이 위치에 해당 건물이 존재 할 수 없음");
            }
        }
        if (Base.Type == CommonType.Cabin)
        {
            BuildingManager.Instance.AddBuilding(Camp.End, this);
        }
    }
    public override void Update()
    {
        CheckAnimation();
        if (Base.MyCamp != Camp.End)
            if (Base.Type == CommonType.WarrenT1 || Base.Type == CommonType.WarrenT2 || Base.Type == CommonType.WarrenT3)
            {
                if (!BuildSmoke)
                {
                    Vector3 pos1 = new Vector3(transform.position.x - 0.4f, transform.position.y + 0.5f, transform.position.z);
                    Vector3 pos2 = new Vector3(transform.position.x, transform.position.y + 0.7f, transform.position.z);
                    Vector3 pos3 = new Vector3(transform.position.x + 0.4f, transform.position.y + 0.5f, transform.position.z);
                    EffectManager.Instance.SmokeEffectEnable(gameObject, pos1, 3f, false, ParticleObject.PARTICLETYPE.SMOKE_BUILD);
                    EffectManager.Instance.SmokeEffectEnable(gameObject, pos2, 3f, false, ParticleObject.PARTICLETYPE.SMOKE_BUILD);
                    EffectManager.Instance.SmokeEffectEnable(gameObject, pos3, 3f, false, ParticleObject.PARTICLETYPE.SMOKE_BUILD);
                    BuildSmoke = true;
                }
            }

        //폰트 대미지 누적용도
        DamageEnd();
    }
    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    public IEnumerator HPRecovery(float recovery)
    {
        if (BuffDebuff.HealStack > 0)
            BuffDebuff.HealStack--;

        yield return new WaitForSeconds(5f);

        BuffDebuff.Add_Heal(this);
        BuffDebuff.HealStack++;

        WaitForSeconds wait = new WaitForSeconds(1f);

        while (true)
        {
            yield return wait;
            HP += 0.5f;

            if (HP >= Base.MaxHp)
            {
                if (BuffDebuff.HealStack > 0)
                    BuffDebuff.HealStack--;
                HP = Base.MaxHp;
                yield break;
            }
        }
    }



    public void Play_Building_Sound(BuildSoundType soundType, CommonType type, float volume = 0f, Sound_Channel channel = Sound_Channel.Effect)
    {
        if (!SceneStarter.Instance.soundElements.BuildSoundDic.ContainsKey(type))
        {
            Debug.Log(type + " Key is not contained");
            return;
        }
        if (SceneStarter.Instance.soundElements.BuildSoundDic[type].ContainsKey(soundType))
        {
            var audioArray = SceneStarter.Instance.soundElements.BuildSoundDic[type][soundType];
            int random = Random.Range(0, audioArray.Count);
            Sound sound;

            Vector3 soundPos = gameObject.transform.position;

            SoundManager.Instance.Play(channel, soundPos, audioArray[random], out sound);
            if (volume != 0)
            {
                sound.Set_Volume(volume);
            }
        }
        else
        {
            Debug.Log(soundType + " Key is not contained");
        }
    }

    public void Play_BuildingGeneralSound(ComSoundType soundType, int index = -1, float volume = 0f)
    {
        if (!SceneStarter.Instance.soundElements.ComSoundDic.ContainsKey(Base.MyCamp))
        {
            Debug.Log(soundType + " Key is not contained");
            return;
        }
        if (SceneStarter.Instance.soundElements.ComSoundDic[Base.MyCamp].ContainsKey(soundType))
        {
            var audioArray = SceneStarter.Instance.soundElements.ComSoundDic[Base.MyCamp][soundType];
            int random = Random.Range(0, audioArray.Count);
            if (index != -1)
                random = index;
            Sound sound;

            Vector3 soundPos = gameObject.transform.position;

            SoundManager.Instance.Play(Sound_Channel.Effect, soundPos, audioArray[random], out sound);
            if (volume != 0f)
            {
                sound.Set_Volume(volume);
            }
        }
        else
        {
            Debug.Log(soundType + " Key is not contained");
        }
    }

    public void Play_ConstructSound(float lifeTime)
    {
        var audioArray = SceneStarter.Instance.soundElements.BuildSoundDic[Base.Type][BuildSoundType.Construct];
        int index = Random.Range(0, audioArray.Count);

        SoundManager.Instance.Play(Sound_Channel.Ambient, gameObject, audioArray[index], true, lifeTime);
    }

    public void Play_Building_Sound(BuildSoundType soundType, float volume = 0f, Sound_Channel channel = Sound_Channel.Effect)
    {
        if (!SceneStarter.Instance.soundElements.BuildSoundDic.ContainsKey(Base.Type))
        {
            Debug.Log(Base.Type + " Key is not contained");
            return;
        }
        if (SceneStarter.Instance.soundElements.BuildSoundDic[Base.Type].ContainsKey(soundType))
        {
            var audioArray = SceneStarter.Instance.soundElements.BuildSoundDic[Base.Type][soundType];
            int random = Random.Range(0, audioArray.Count);
            Sound sound;

            Vector3 soundPos = gameObject.transform.position;

            SoundManager.Instance.Play(channel, soundPos, audioArray[random], out sound);
            if (volume != 0)
            {
                sound.Set_Volume(volume);
            }
        }
        else
        {
            Debug.Log(soundType + " Key is not contained");
        }
    }

    public void Play_BuildingLoopSound(BuildSoundType soundType, float volume = 0f, int index = -1)
    {
        if (!SceneStarter.Instance.soundElements.BuildSoundDic.ContainsKey(Base.Type))
        {
            Debug.Log(Base.Type + " Key is not contained");
            return;
        }
        if (SceneStarter.Instance.soundElements.BuildSoundDic[Base.Type].ContainsKey(soundType))
        {
            var audioArray = SceneStarter.Instance.soundElements.BuildSoundDic[Base.Type][soundType];
            int random = Random.Range(0, audioArray.Count);
            if (index != -1)
                random = index;

            Vector3 soundPos = gameObject.transform.position;

            if (loopSound != null)
            {
                loopSound.Stop();
                loopSound = null;
            }

            SoundManager.Instance.Play(Sound_Channel.Ambient, soundPos, audioArray[random], out loopSound, true);
            if (volume != 0)
            {
                loopSound.Set_Volume(volume);
            }
        }
        else
        {
            Debug.Log(soundType + " Key is not contained");
        }
    }

    public void TurnOffLoopSound()
    {
        if (loopSound != null)
        {
            loopSound.Stop();
            loopSound = null;
        }
    }

    public void Play_GristmillConstructSound(int index)
    {
        BuildSoundType soundType = BuildSoundType.Construct;
        if (SceneStarter.Instance.soundElements.BuildSoundDic[Base.Type].ContainsKey(soundType))
        {
            var audioArray = SceneStarter.Instance.soundElements.BuildSoundDic[Base.Type][soundType];

            SoundManager.Instance.Play(Sound_Channel.Ambient, gameObject, audioArray[index]);
        }
        else
        {
            Debug.Log(Base.Type + " - " + soundType + " Key is not contained");
        }
    }

    public BuildingStateOperator GetBuildingStateOperator()
    {
        return stateOperator;
    }
    public void Sell()
    {
        if (farm == null || !farm.queued)
            GameManager.Instance.ChangeFoodCamp(Base.MyCamp, Base.Data.Cost);

        if (Base.MyCamp == GameManager.Instance.CommanderList[0])
        {
            //숫자
            EffectManager.Instance.FontEffectEnable(InGameManager.Instance.Commanders[Base.MyCamp].gameObject, data.Cost, FontEffect.FONTTYPE.PLUSFOOD);
        }

        if (Base.Type == CommonType.CampFire)
        {
            stateOperator.DestroyCampfire();
            return;
        }

        // 판매되는 건물이 생산건물이라면 제거 후 최대 유닛 갯수보다 현재 유닛이 많으면 초과하는 유닛을 제거한다.
        if (Base.Type >= CommonType.WarrenT1 && Base.Type <= CommonType.WarrenT3)
        {
            int extraUnits = SceneStarter.Instance.reinforceElements.GetReinforceAccCurData(Product_Type, SceneStarter.Instance.userElements.GetLevel(Product_Type)).UnitPerBuliding;
            // 최대 유닛을 얼마나 줄일 지 계산
            int decrease = (int)CommonType.WarrenT3 - ((int)Base.Type - 1) + extraUnits;

            var unitList = SquadController.Instance.Squads[Base.MyCamp][SquadController.Instance.TypeToSquadNumber[Base.MyCamp][Product_Type]].UnitList;

            // 판매할 건물이 건설중일 경우나 유닛이 없을 경우 제거 할 유닛이 없으므로 스킵
            if (unitList.Count > 0 && stateOperator.CurBuildingState != BuildingState.Construct)
            {
                int maxUnit = BuildingManager.Instance.maxUnits[Base.MyCamp][Product_Type];

                var list = BuildingManager.Instance.Buildings[Base.MyCamp][Base.Type];

                int count = 0;

                // 제거할 유닛을 계산할 때 CurUnit은 생산 중인 건물도 CurUnit을 증가시키므로 생산 중인 건물 갯수만큼 뺀다.
                foreach (var warren in list)
                {
                    if (warren.Product_Type == Product_Type && warren.stateOperator.CurBuildingState == BuildingState.Production)
                    {
                        count++;
                    }
                }

                int curUnit = BuildingManager.Instance.curUnits[Base.MyCamp][Product_Type] - count;

                var node = unitList.Last;

                // 제거할 유닛의 수
                if (curUnit > (maxUnit - decrease))
                {
                    int killCount = curUnit - (maxUnit - decrease);

                    for (int i = 0; i < killCount; i++)
                    {
                        if (node != null)
                        {
                            node.Value.HP = 0;
                            node = node.Previous;
                        }
                        else
                            break;
                    }
                }
            }



            // 건설 중인 건물은 MaxUnit을 증가시키지 않으므로 건설 중이 아닌 건물을 팔 때만 MaxUnit을 조정한다.
            if (stateOperator.CurBuildingState != BuildingState.Construct)
            {
                BuildingManager.Instance.maxUnits[Base.MyCamp][Product_Type] -= decrease;
            }
        }
        Play_BuildingGeneralSound(ComSoundType.Sell, -1, 0.3f);
        DestroyBuilding(true);
    }

    public BuildingState GetCurState()
    {
        return stateOperator.CurBuildingState;
    }

    public void ChangeState(BuildingState state)
    {
        stateOperator.ChangeState(state);
    }

    public void DestroyBuilding(bool isSell = false)
    {
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }

        if (!isSell)
        {
            Play_Building_Sound(BuildSoundType.Destroy);
            isDestroyed = true;
        }

        if (Base.Type == CommonType.Cabin)
        {
            BuildingManager.Instance.DeleteBuilding(Base.MyCamp, this);

            animator.SetBool("Idle_Neutral", false);
            animator.SetBool("Attack", false);

            Base.Type = CommonType.CampFire;
            stateOperator.ChangeState(BuildingState.Idle);
            SetStateData();

            EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.BUILDINGEXPLOSION);

            HP = 0.1f;
        }
        else if (Base.Type == CommonType.CampFire)
        {
            stateOperator.DestroyCampfire();
        }
        else
        {
            if (Base.Type == CommonType.Farm)
            {
                farm.Destroy();
            }
            else if (Base.Data.CommonType != CommonType.Gristmill && Base.Data.CommonType != CommonType.CampFire)
            {
                if (isSell)
                {
                    // 판매 이펙트
                    EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.BUILDINGSELL);
                }
                else
                {
                    // 파괴 이펙트
                    EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.BUILDINGEXPLOSION);
                }
                TilemapSystem.Instance.GetTile(gameObject.transform.position).Height = 0;

                if (stateOperator.CurBuildingState == BuildingState.Production)
                    BuildingManager.Instance.curUnits[Base.MyCamp][Product_Type]--;

                Base.ReturnPool();

                Occupying(false, null);

                BuildingManager.Instance.DeleteBuilding(Base.MyCamp, this);
            }
            else
            {
                BuildingManager.Instance.DeleteBuilding(Base.MyCamp, this);
                if (Base.Type == CommonType.Gristmill)
                {
                    // 통계 데이터 갱신
                    SceneStarter.Instance.statisticElements.AddDestroyGrismill(Base.MyCamp);

                    GetComponent<Gristmill>().DestroyFarm();

                    if (Base.MyCamp == Camp.End)
                    {
                        return;
                    }
                    stateOperator.ShowTopper(false);
                    if (isSell)
                    {
                        if (isDestroyed)
                        {
                            animator.SetBool("Idle_Des", true);
                            animator.SetBool("Idle_Neutral", false);
                        }
                        else
                        {
                            animator.SetBool("Idle_Des", false);
                            animator.SetBool("Idle_Neutral", true);
                        }
                        animator.SetInteger("Construct_Level", -1);
                    }
                    else
                    {
                        GameManager.Instance.Vibrate();
                        EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.GRSTMILL_EXPLOSION);
                        InGameManager.Instance.MainCamera.EventMove(gameObject.transform.position);

                        animator.SetBool("Idle_Des", true);
                        animator.SetBool("Idle_Neutral", false);
                    }
                    if (BuildingManager.Instance.IsGameOver(Base.MyCamp))
                        GameManager.Instance.EndGame(Base.MyCamp);
                    SetBuildBoundary(Camp.End, Base.MyCamp);
                }

                stateOperator.ChangeState(BuildingState.Idle);
                animator.SetBool("Idle", false);

                Base.MyCamp = Camp.End;
            }
        }
    }

    // 건물별로 얼마나 타일을 차지하는지, 차지한 타일로 지나갈 수 있는지 등등 여부를 판별하고 세팅한다.
    public void SetTileOccupy()
    {
        Vector2Int gridPos;
        int height = 0;
        switch (data.CommonType)
        {
            case CommonType.Turret:
            case CommonType.Cannon:
                OccupyTiles.Add(TilemapSystem.Instance.WorldToTilePos(transform.position));
                height = 1;
                break;

            case CommonType.Wire:
            case CommonType.Mine:
            case CommonType.MoleeMerge:
                OccupyTiles.Add(TilemapSystem.Instance.WorldToTilePos(transform.position));
                break;

            case CommonType.Gristmill:
            case CommonType.Cabin:
                gridPos = TilemapSystem.Instance.WorldToTilePos(transform.position);
                OccupyTiles.Add(gridPos);
                OccupyTiles.Add(new Vector2Int(gridPos.x, gridPos.y + 1));
                OccupyTiles.Add(new Vector2Int(gridPos.x + 1, gridPos.y + 1));
                OccupyTiles.Add(new Vector2Int(gridPos.x + 1, gridPos.y));
                height = 1;
                break;

            case CommonType.WarrenT1:
            case CommonType.WarrenT2:
            case CommonType.WarrenT3:
            case CommonType.Farm:
                gridPos = TilemapSystem.Instance.WorldToTilePos(transform.position);
                OccupyTiles.Add(gridPos);
                OccupyTiles.Add(new Vector2Int(gridPos.x, gridPos.y + 1));
                OccupyTiles.Add(new Vector2Int(gridPos.x + 1, gridPos.y + 1));
                OccupyTiles.Add(new Vector2Int(gridPos.x + 1, gridPos.y));
                break;
        }
        for (int i = 0; i < OccupyTiles.Count; i++)
        {
            TilemapSystem.Instance.GetTile(OccupyTiles[i]).Occupy(this, height, true);
        }
    }

    public void OccupyNeutral(Camp camp)
    {
        stateOperator.Occupy(camp);
        HP = 0.01f;
        if (Base.Type == CommonType.CampFire)
        {
            ChangeState(BuildingState.Construct);
            Occupying(true, this);
        }
    }

    public void Occupying(bool occupying, Character character)
    {
        int height = occupying == false ? 0 : 1;

        for (int i = 0; i < OccupyTiles.Count; i++)
        {
            TilemapSystem.Instance.GetTile(OccupyTiles[i]).Height = height;
            TilemapSystem.Instance.GetTile(OccupyTiles[i]).occupied = occupying;
            TilemapSystem.Instance.GetTile(OccupyTiles[i]).occupier = character;
        }
    }

    public void SetStateData()
    {
        stateOperator.SetStateData();
    }


    //해당 건물 타입에 맞게 세팅을 해준다.
    public void Make_Building()
    {
        stateOperator.Initialize();
        SetStateData();
        data = Base.Data;
        animator.runtimeAnimatorController = SceneStarter.Instance.animatorElements.BuildAniDic[data.CommonType];

        stateOperator = GetComponent<BuildingStateOperator>();
        stateOperator.SetBuilding(Product_Type, Product_PoolType);

        hpCanvas.Ready();
        SetMiniSpriteColor();
        SetTileOccupy();
    }
    //전장의 안개 애니메이션 체크
    void CheckAnimation()
    {
        // 시야에 들어왔을 때
        if (FogOfWar.Instance.CheckTileAlpha(gameObject.transform.position, GameManager.Instance.CommanderList[0]))
        {
            // 이미 밝혀져잇는곳에서 End Camp가 아닌 건물의 미니맵스프라이트 갱신
            SetMiniSpriteColor();
            if (Base.Type == CommonType.Gristmill || Base.Type == CommonType.Cabin || Base.Type == CommonType.CampFire)
                minimapSprite.gameObject.SetActive(true);
            else if (Base.MyCamp != Camp.End)
                minimapSprite.gameObject.SetActive(true);
            else
                minimapSprite.gameObject.SetActive(false);

            if (fogSprite != null && fogSprite.gameObject.activeInHierarchy == true)
            {
                fogSprite.gameObject.SetActive(false);
                spriteRenderer.enabled = true;
                HPCanvas.SetActiveUI(true);
                if (Base.Type == CommonType.Gristmill)
                {
                    SpriteRenderer topperRenderer = stateOperator.GetTopper().GetComponent<SpriteRenderer>();
                    topperRenderer.enabled = true;
                    transform.Find("TopperFogSprite").gameObject.SetActive(false);
                }
            }
        }
        // 안개에 가릴 때
        else
        {
            if (fogSprite != null && fogSprite.gameObject.activeInHierarchy == false)
            {
                spriteRenderer.enabled = false;

                // 안개에 가릴 때 가려지기 직전 스프라이트를 화면에 띄운다.
                fogSprite.gameObject.SetActive(true);
                fogSprite.sprite = spriteRenderer.sprite;

                // 풍선의 위치변화나 다른 건물로 바뀔 때 스케일 등을 맞춘다.
                fogSprite.transform.position = Body.position;
                fogSprite.transform.localScale = Body.transform.localScale;

                HPCanvas.SetActiveUI(false);

                if (Base.Type == CommonType.Gristmill)
                {
                    SpriteRenderer topperRenderer = stateOperator.GetTopper().GetComponent<SpriteRenderer>();
                    topperRenderer.enabled = false;
                    GameObject topperFogSprite = transform.Find("TopperFogSprite").gameObject;
                    topperFogSprite.SetActive(true);
                    topperFogSprite.GetComponent<SpriteRenderer>().sprite = topperRenderer.sprite;
                }
            }
        }
    }

    public void SetBuildBoundary(Camp camp, Camp refreshCamp = Camp.End)
    {
        Vector2Int tempPos = TilemapSystem.Instance.WorldToTilePos(transform.position);
        TileType CommanderTileType = TilemapSystem.Instance.GetTileType(transform.position);
        int CommanderTileFloor = TilemapSystem.Instance.GetTileElevation(transform.position);

        var circularList = StorageBoxes.Instance.CircleSearchList;

        int range = 12;
        if (Base.Type == CommonType.CampFire)
            range = 6;

        // 해당 건물에서 12타일 범위의 타일들의 캠프를 바꿔준다.
        for (int dist = 0; dist < range; ++dist)
        {
            foreach (var addTile in circularList[dist])
            {
                var lightingPos = tempPos + (Vector2Int)addTile;
                TileNode tile = TilemapSystem.Instance.GetTile(lightingPos);
                // 범위 안에 다른 진영의 영역이 있으면 무시한다.
                if (tile != null && (tile.camp == Camp.End || tile.camp == camp || (tile.camp == Base.MyCamp && camp == Camp.End)))
                {
                    tile.camp = camp;
                }
            }
        }

        Gristmill gristmill = GetComponent<Gristmill>();
        if (gristmill != null)
        {
            gristmill.FarmOccupy(camp);
        }

        // 점령당한 제분소나 캠프파이어가 파괴될 경우 겹치는 부분의 타일까지 End로 만들어버리기 때문에
        // 아직 파괴되지 않은 제분소의 땅을 다시 갱신한다.
        if (refreshCamp != Camp.End)
        {
            var Gristmills = BuildingManager.Instance.Buildings[refreshCamp][CommonType.Gristmill];
            LinkedList<BuildingBase> Campfires = null;

            if (BuildingManager.Instance.Buildings[refreshCamp].ContainsKey(CommonType.CampFire))
                Campfires = BuildingManager.Instance.Buildings[refreshCamp][CommonType.CampFire];

            foreach (var element in Gristmills)
                element.SetBuildBoundary(Base.MyCamp);

            if (Campfires != null)
            {
                foreach (var element in Campfires)
                    element.SetBuildBoundary(Base.MyCamp);
            }

            TilemapSystem.Instance.SetOutLine();
        }
    }

    public override bool CanBeTarget()
    {
        bool retVal = true;

        if (stateOperator != null && stateOperator.CurBuildingState == BuildingState.Construct)
            return true;

        if (Base.Type == CommonType.Mine || Base.Type == CommonType.Wire)
            return false;

        if (!gameObject.activeSelf)
            retVal = false;

        if (HP <= 0.01f)
            retVal = false;

        return retVal;
    }

    // 판매 UI On시키는 함수
    public void OnExtendSellUI()
    {
        if (Base.MyCamp == GameManager.Instance.CommanderList[0])
        {
            OffExtendBuildUI();
            if (farm != null)
            {
                switch (farm.GetState())
                {
                    case Farm.FarmState.Idle:
                    case Farm.FarmState.Cultivation:
                        extendSellUICtrl.gameObject.SetActive(true);
                        break;
                    default:
                        extendSellUICtrl.gameObject.SetActive(false);
                        break;
                }
            }
            else
            {
                if (Base.Type == CommonType.Gristmill)
                {
                    switch (stateOperator.CurBuildingState)
                    {
                        case BuildingState.Attack:
                        case BuildingState.Construct:
                        case BuildingState.Production:
                            extendSellUICtrl.gameObject.SetActive(true);
                            break;
                        default:
                            extendSellUICtrl.gameObject.SetActive(false);
                            break;
                    }
                }
                else if (Base.Type == CommonType.CampFire)
                {
                    return;
                }
                else
                {
                    switch (stateOperator.CurBuildingState)
                    {
                        case BuildingState.Idle:
                        case BuildingState.Attack:
                        case BuildingState.Construct:
                        case BuildingState.Production:
                            extendSellUICtrl.gameObject.SetActive(true);
                            break;
                    }
                }
            }
        }
    }

    // 판매 UI Off시키는 함수
    public void OffExtendSellUI()
    {
        if (extendSellUICtrl != null)
            extendSellUICtrl.gameObject.SetActive(false);
    }
    // 건설 UI On시키는 함수
    public void OnExtendBuildUI()
    {
        if (Base.MyCamp == Camp.End)
        {
            if (farm != null && farm.GristmillBuildingBase.Base.MyCamp == GameManager.Instance.CommanderList[0] &&
                farm.GristmillBuildingBase.GetCurState() == BuildingState.Idle)
            {
                switch (farm.GetState())
                {
                    case Farm.FarmState.Idle:
                        extendBuildUICtrl.gameObject.SetActive(true);
                        extendBuildUICtrl.ChangeBuildUIText(Base.Type);
                        break;
                }
            }
            else if (Base.Type == CommonType.Gristmill || Base.Type == CommonType.CampFire)
            {
                switch (GetCurState())
                {
                    case BuildingState.Idle:
                        if (GameManager.Instance.CampFoodDic[GameManager.Instance.CommanderList[0]] > 60)
                        {
                            extendBuildUICtrl.gameObject.SetActive(true);
                            extendBuildUICtrl.ChangeBuildUIText(Base.Type);
                        }
                        break;
                }
            }
        }
        else
            extendBuildUICtrl.gameObject.SetActive(false);
    }

    // 건설 UI Off시키는 함수
    public void OffExtendBuildUI()
    {
        if (extendBuildUICtrl != null)
            extendBuildUICtrl.gameObject.SetActive(false);
    }

    // 미니맵 스프라이트 색 정하는 함수
    public void SetMiniSpriteColor()
    {
        switch (Base.Type)
        {
            case CommonType.Wire:
            case CommonType.Mine:
            case CommonType.Turret:
            case CommonType.Balloon:
            case CommonType.Cannon:
            case CommonType.WarrenT1:
            case CommonType.WarrenT2:
            case CommonType.WarrenT3:
            case CommonType.MoleeMerge:
                switch (Base.MyCamp)
                {
                    case Camp.Bellafide:
                        minimapSprite.color = Global.CommanderInGameColorBellafide;
                        break;
                    case Camp.Hopper:
                        minimapSprite.color = Global.CommanderInGameColorHopper;
                        break;
                    case Camp.Quartermaster:
                        minimapSprite.color = Global.CommanderInGameColorQuartermaster;
                        break;
                    case Camp.Archimedes:
                        minimapSprite.color = Global.CommanderInGameColorArchimedes;
                        break;
                }
                break;
            case CommonType.Gristmill:
            case CommonType.Cabin:
            case CommonType.CampFire:
                switch (Base.MyCamp)
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
                    case Camp.End:
                        minimapSprite.color = Global.MinimapColorNeutral;
                        break;
                }
                break;
            case CommonType.Farm:
                switch (Base.MyCamp)
                {
                    case Camp.Bellafide:
                        minimapSprite.color = Global.MinimapColorCampBellafideA;
                        break;
                    case Camp.Hopper:
                        minimapSprite.color = Global.MinimapColorCampHopperA;
                        break;
                    case Camp.Quartermaster:
                        minimapSprite.color = Global.MinimapColorCampQuartermasterA;
                        break;
                    case Camp.Archimedes:
                        minimapSprite.color = Global.MinimapColorCampArchimedesA;
                        break;
                    case Camp.End:
                        minimapSprite.color = Global.MinimapColorNeutralA;
                        break;
                }
                break;

        }
    }

    protected override void Hit(float damage)
    {

        // HP가 회복중인 상태에서 맞았을 경우 회복하는 코루틴 함수를 중지시킨다.
        if (recoveryCoroutine != null)
        {
            StopCoroutine(recoveryCoroutine);
            recoveryCoroutine = null;
        }

        SetHitInfo(damage);

        HP -= damage;
        if (HP < 0)
            HP = 0;

        if (HP <= 0)
            return;


        recoveryCoroutine = StartCoroutine(HPRecovery(5f));
    }

    public List<Vector2Int> OccupyTiles
    {
        get { return occupyTiles; }
    }

    public int ConstructLevel
    {
        get { return constructLevel; }
    }

    public CommonBase GetBase()
    {
        return Base;
    }

    public Animator GetAnimator()
    {
        return animator;
    }
    public BuildingStateOperator StateOperator
    {
        get { return stateOperator; }
    }
}