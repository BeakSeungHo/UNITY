using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 작성자 : 김영현
/// 최종 수정일 : (20 - 10 - 21)
/// 
/// 배틀 씬 안에서 공격하고 공격받는 모든 오브젝트는 이 클래스를 상속받는다.
/// 반드시 최상위 오브젝트가 가지고 있어야 한다.
/// 
/// </summary>
public class Character : MonoBehaviour
{
    // 기본 데이터
    public CommonBase Base = null;
    
    [SerializeField]
    public BuffDebuff BuffDebuff = null;
    public PositionInfo PositionInfo = null;

    public bool facingRight = true;
    public bool IsOnWater = false;

    public float HP = 10f;
    public int HealStack    { get { return BuffDebuff.HealStack; }      set { BuffDebuff.HealStack = value; } }
    public int PoisonStack  { get { return BuffDebuff.PoisonStack; }    set { BuffDebuff.PoisonStack = value; } }
    public bool Stim        { get { return BuffDebuff.Stim; } }

    public Vector3 Pos { get { return transform.position; } }

    private bool    recovery = false;
    private float   recoveryCoolDown = 0f;

    public bool activeSelf { get { return gameObject.activeSelf; } }

    //Font Effect 용도
    public HPCanvas HPCanvas;
    public float DamageTime = 0f;
    public float DamageAccrue = 0f;
    public bool DamageAccrueFlag = false;
    //OutLine
    public OutLine OutLine;
    //toper Outline
    public OutLine ToperOutline;
    //전장의 안개 체크용
    public bool FogActive = false;

    Coroutine WaveCoroutine = null;
    /// <summary>
    /// 이 오브젝트가 공격의 타겟이 될 수 있는지 확인한다.
    /// 상속받은 자식 클래스가 override 하지 않는다면 활성화 여부를 반환한다.
    /// </summary>
    /// <returns>타겟 가능 여부</returns>
    public virtual bool CanBeTarget()
    {
        return gameObject.activeSelf;
    }

    public Vector3 HitPosition
    {
        get
        {
            if (null == PositionInfo)
                return transform.position;

            return PositionInfo.HitPosition * transform.localScale.y + transform.position;
        }
    }

    public Vector3 FirePosition
    {
        get
        {
            return transform.position 
                + new Vector3(
                    PositionInfo.FirePosition.x * transform.localScale.x, 
                    PositionInfo.FirePosition.y * transform.localScale.y, 0f);
        }
    }

    public void SetHitInfo(float damage)
    {
        if (HPCanvas != null)
        {
            if (!HPCanvas.DamageFont.ActiveFlag)
            {
                HPCanvas.DamageFont.ActiveFlag = true;
            }
            HPCanvas.DamageFont.UpFlag = true;
            HPCanvas.DamageFont.DownFlag = false;
            DamageAccrue += damage;
            DamageAccrueFlag = true;
            HPCanvas.DamageFont.Damage = DamageAccrue;
            DamageTime = 0;
        }

        // 유닛이나 건물이 맞을때 커맨더와 떨어져있는지 체크하는 함수
        CheckWarningMinimap();

        SceneStarter.Instance.statisticElements.AccumulateDamage(Base.MyCamp, damage);
    }

    protected virtual void Hit(float damage)
    {
        SetHitInfo(damage);

        HP -= damage;
        recovery = false;
        recoveryCoolDown = 5f;
        if (HP < 0)
            HP = 0;
    }

    public void Hit(float damage, Character character)
    {
        if (HP == 0)
            return;

        Hit(damage);

        CheckMission(character);
    }

    public void Heal (float heal, Character character)
    {
        HP += heal;
        if (HP > Base.MaxHp)
            HP = Base.MaxHp;
    }

    public bool IsBuilding()
    {
        return (CommonType.Wire <= Base.Type && Base.Type <= CommonType.Farm) ||
            CommonType.Cabin == Base.Type ||
            CommonType.CampFire == Base.Type ||
            CommonType.MoleeMerge == Base.Type;

    }

    public virtual void Move(Vector3 moveDir, float speedScaling)
    {
        float speed = Base.MoveSpeed * speedScaling;
        if (PlaceType.Ground == Base.PlaceType)
            speed *= (IsOnWater ? 0.5f : 1f);

        var occupier = TilemapSystem.Instance?.GetTile(transform.position)?.occupier;
        var type = occupier?.Base?.Type;

        if (CommonType.Wire == type)
        {
            BuildingBase buildingBase = occupier as BuildingBase;
            if (buildingBase != null && buildingBase.StateOperator.CurBuildingState != BuildingState.Construct)
                speed *= 0.5f;
        }

        transform.position += speed * moveDir * Time.deltaTime;

        if ((moveDir.x < 0 && facingRight) ||
            (moveDir.x > 0 && !facingRight))
            Flip();
    }

    public virtual void Move(Vector3 moveDir)
    {
        moveDir.Normalize();

        float speed = Base.MoveSpeed;
        if (PlaceType.Ground == Base.PlaceType)
            speed *= (IsOnWater ? 0.5f : 1f);

        var occupier = TilemapSystem.Instance?.GetTile(transform.position)?.occupier;
        var type = occupier?.Base?.Type;

        if (CommonType.Wire == type)
        {
            BuildingBase buildingBase = occupier as BuildingBase;
            if (buildingBase != null && buildingBase.StateOperator.CurBuildingState != BuildingState.Construct)
                speed *= 0.5f;
        }

        transform.position += speed * moveDir * Time.deltaTime;

        if ((moveDir.x < 0 && facingRight) ||
            (moveDir.x > 0 && !facingRight))
            Flip();

    }

    public void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1f;
        transform.localScale = localScale;

        facingRight = !facingRight;
    }

	public virtual void Awake()
	{
	}

    public virtual void Update()
    {
        OnWater();
        //폰트 대미지 누적용도
        DamageEnd();
    }
    public virtual void LateUpdate()
    {
        UpdateOutline();
    }
    public void Recovery(float healTick = 5f)
    {
        if (HP < Base.MaxHp)
        {
            if (recovery)
            {
                HP += Time.deltaTime * healTick;
                if (HP >= Base.MaxHp)
                {
                    HP = Base.MaxHp;
                    recovery = false;
                }
            }
            else
            {
                recoveryCoolDown -= Time.deltaTime;
                if (recoveryCoolDown <= 0)
                {
                    recovery = true;
                    recoveryCoolDown = 0f;
                }
            }
        }
    }

    void OnWater()
    {
        if (PlaceType.Air == Base.PlaceType)
            return;

        Vector3Int cellPos = TilemapSystem.Instance.WorldToCellPos(transform.position);
        ElevationTile elvTile = (ElevationTile)TilemapSystem.Instance.Ground.GetTile(cellPos);


        if (null == elvTile)
            IsOnWater = false;
        else
        {
            if (elvTile.TileType == TileType.Water)
            {
                if (!IsOnWater)
                {
                    EffectManager.Instance.EffectEnable(transform.position, ParticleObject.PARTICLETYPE.WATER_BURST);
                    Sound sound;
                    SoundManager.Instance.Play(Sound_Channel.Effect, gameObject.transform.position, SceneStarter.Instance.soundElements.ComSoundDic[Base.MyCamp][ComSoundType.Enterwater][0], out sound);
                    sound.Set_Volume(0.1f);
                    WaveCoroutine = StartCoroutine(WaterWave());
                    IsOnWater = true;
                }
            }
            else
            {
                IsOnWater = false;
                if (WaveCoroutine != null)
                    StopCoroutine(WaveCoroutine);
            }
        }

    }


    public IEnumerator WaterWave()
    {
        while (true)
        {
            float tempTime = 0f;
            tempTime = 0.2f;
            yield return new WaitForSeconds(tempTime);
            float tempScale = 0f;
            switch (Base.Type)
            {
                case CommonType.Squirrel:
                case CommonType.Lizard:
                case CommonType.Toad:
                case CommonType.Mole:
                    tempScale = 0.3f;
                    break; 
                case CommonType.Ferret:
                case CommonType.Skunk:
                case CommonType.Chameleon:
                case CommonType.Snake:
                    tempScale = 0.5f;
                    break;
                case CommonType.Boar:
                case CommonType.Badger:
                case CommonType.Owl:
                case CommonType.Wolf:
                case CommonType.Fox:
                    tempScale = 0.7f;
                    break;
                case CommonType.Mouse:
                    tempScale = 0.2f;
                    break;
                case CommonType.Commander:
                    tempScale = 0.7f;
                    break;
                case CommonType.MoleeMerge:
                    break;
                case CommonType.End:
                    break;
            }
            EffectManager.Instance.EffectScaleEnable(transform.position, tempScale, ParticleObject.PARTICLETYPE.WATER_WAVE);
        }
    }

    // 유닛이나 건물이 맞을때 체크하는 함수
    public void CheckWarningMinimap()
    {
        // 캠프가 플레이어일때 맞은대상과 플레이어 거리를 계산하여 미니맵 경고표시 여부결정
        if (Base.MyCamp == GameManager.Instance.CommanderList[0] &&
            Vector3.Distance(transform.position, InGameManager.Instance.Commanders[Base.MyCamp].transform.position) > 5)
        {
            BattleUICtrl.Instance.LastHitTime = BattleUICtrl.Instance.PlayTime;
        }
    }

    void CheckMission(Character character)
    {
        if (HP == 0 && character.Base.MyCamp == GameManager.Instance.CommanderList[0])
        {
            switch (GameManager.Instance.CurGameMode)
            {
                case GameMode.Tutorial:
                    if(Base.Type != CommonType.Toad)
                    {
                        SceneStarter.Instance.userElements.AddMissionCount(MissionType.None, CampaignManager.Instance.curWave() + 1, 1);
                        if (SceneStarter.Instance.userElements.CompleteMission(MissionType.None, CampaignManager.Instance.curWave() + 1))
                        {
                            CampaignManager.Instance.GenNextObjectsAfterSecond();
                            CampaignManager.Instance.DestroyObstacle();
                        }
                        break;
                    }
                    break;
                case GameMode.Campaign:
                    if (Base.MyCamp != GameManager.Instance.CommanderList[0])
                    {
                        GameManager.Instance.CampFoodDic[character.Base.MyCamp] += Mathf.RoundToInt(Base.Cost * 0.5f);

                        switch (Base.Type)
                        {
                            case CommonType.Squirrel:
                                SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 2, 1);
                                break;
                        }
                        switch (character.Base.Type)
                        {
                            case CommonType.Falcon:
                                if (Base.PlaceType == PlaceType.Air)
                                    SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 6, 1);
                                break;
                            case CommonType.Boar:
                                SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 8, 1);
                                break;
                        }
                        
                        GameManager.Instance.MissionKillCount++;
                    }
                    break;
                default:
                    if (Base.MyCamp != GameManager.Instance.CommanderList[0])
                    {
                        switch (Base.Type)
                        {
                            case CommonType.Squirrel:
                                SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 2, 1);
                                break;
                        }
                        switch (character.Base.Type)
                        {
                            case CommonType.Falcon:
                                if (Base.PlaceType == PlaceType.Air)
                                    SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 6, 1);
                                break;
                            case CommonType.Boar:
                                SceneStarter.Instance.userElements.AddMissionCount(MissionType.Achievements, 8, 1);
                                break;
                        }
                        
                        GameManager.Instance.MissionKillCount++;
                    }
                    break;
            }
        }
    }
    void UpdateOutline()
    {
        if (Base.MyCamp != GameManager.Instance.CommanderList[0])
        {
            if (InGameManager.Instance.Commanders[GameManager.Instance.CommanderList[0]].AttackTarget == this)
            {
                if (OutLine != null)
                    OutLine.outlineSize = 1;
                if (ToperOutline != null)
                    ToperOutline.outlineSize = 1;
            }
            else
            {
                if (OutLine != null)
                    OutLine.outlineSize = 0;
                if (ToperOutline != null)
                    ToperOutline.outlineSize = 0;
            }
        }
    }
    protected void DamageEnd()
    {
        if (DamageAccrueFlag)
        {
            DamageTime += Time.deltaTime;
            if (DamageTime >= 0.7f)
            {
                HPCanvas.DamageFont.DownFlag = true;
                DamageAccrueFlag = false;
                DamageTime = 0f;
                DamageAccrue = 0f;
            }
        }
    }
    private void OnEnable()
    {
        if (OutLine != null)
            OutLine.outlineSize = 0;
    }
}
