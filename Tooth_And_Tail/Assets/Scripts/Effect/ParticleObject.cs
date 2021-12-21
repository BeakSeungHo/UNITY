using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour
{
    public enum PARTICLETYPE
    {
        HEAL, BUILDING, BUILDEND, HIT, ASSEMBLE, FLAME,
        BOARFLAME, STIM, WOLFWAVE, EXPLOSION, SKUNKATTACK, SKUNKPOSION,
        POSION, SNAKEATTACK, ASSEMBLEBOTTOM, ASSEMBLEBOTTOMEND,
        UNITDEATH, BUILDINGEXPLOSION, BUILDINGSELL, SMOKE_BUILD, SMOKE_GRISTMILL,
        SMOKE_PLAYER, WATER_BURST, WATER_WAVE, GRSTMILL_EXPLOSION
    };
    public GameObject[] ParticleObj;
    private PARTICLETYPE Type;
    private GameObject TargetPos;
    private ParticleSystem ParticleSys;
    private ParticleSystem ParticleSysSec;

    private ParticleSystemRenderer ParticleRenderer = null;
    private ParticleManager ParticleMa;
    private Character TargetCharacter;
    private float LifeTime;

    //Boar Effect
    private CommonUnitFSM BoarFSM;
    private WildBoarFlame BoarFlame;
    //Skunk Effect
    private TileHitObject SkunkHitObject;
    private EffectCampColor EffectChangeColor;
    //Building Effect
    private BuildingStateOperator BuildingTarget;
    //Wolf
    private Camp Camp;
    //Assemble
    private CommanderFSM CommanderFSMTarget;
    //BuildingBase
    private BuildingBase BuildingBase;
    bool FlameFlag = false;
    // Start is called before the first frame update
    void Start()
    {
        //Particle[(int)Type].SetActive(true);
    }
    public void Ready(GameObject Target, PARTICLETYPE Type)
    {
        TargetPos = Target;
        this.Type = Type;
        transform.position = TargetPos.transform.position;
        LifeTime = 0f;
        switch (Type)
        {
            case PARTICLETYPE.HIT:
            case PARTICLETYPE.SNAKEATTACK:
            case PARTICLETYPE.EXPLOSION:
            case PARTICLETYPE.SKUNKATTACK:
            case PARTICLETYPE.ASSEMBLEBOTTOMEND:
            case PARTICLETYPE.BUILDINGEXPLOSION:
            case PARTICLETYPE.BUILDINGSELL:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                break;
            case PARTICLETYPE.GRSTMILL_EXPLOSION:
                Vector3 tempPos = transform.position;
                tempPos.y += 0.5f;
                transform.position = tempPos;
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                break;
            case PARTICLETYPE.WOLFWAVE:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                EffectChangeColor = ParticleObj[(int)Type].GetComponent<EffectCampColor>();
                Camp = Target.GetComponent<Character>().Base.MyCamp;
                EffectChangeColor.ColorChange(Camp, 1f);
                break;
            case PARTICLETYPE.SKUNKPOSION:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                SkunkHitObject = Target.GetComponent<TileHitObject>();
                EffectChangeColor = ParticleObj[(int)Type].GetComponent<EffectCampColor>();
                ParticleMa = ParticleObj[(int)Type].GetComponent<ParticleManager>();
                EffectChangeColor.ColorChange(SkunkHitObject.Camp, 0.2f);
                break;
            case PARTICLETYPE.BUILDING:
            case PARTICLETYPE.BUILDEND:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                ParticleMa = ParticleObj[(int)Type].GetComponent<ParticleManager>();
                BuildingTarget = Target.GetComponent<BuildingStateOperator>();
                break;
            case PARTICLETYPE.HEAL:
            case PARTICLETYPE.POSION:
            case PARTICLETYPE.STIM:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                ParticleMa = ParticleObj[(int)Type].GetComponent<ParticleManager>();
                TargetCharacter = Target.GetComponent<Character>();
                break;
            case PARTICLETYPE.ASSEMBLE:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                ParticleMa = ParticleObj[(int)Type].GetComponent<ParticleManager>();
                CommanderFSMTarget = Target.GetComponent<CommanderFSM>();
                break;
            case PARTICLETYPE.ASSEMBLEBOTTOM:
                CommanderFSMTarget = Target.GetComponent<CommanderFSM>();
                break;
            case PARTICLETYPE.UNITDEATH:
                break;

        }
        ParticleObj[(int)Type].SetActive(true);
    }

    public void Ready(Vector3 Position, PARTICLETYPE Type)
    {
        this.Type = Type;
        ParticleObj[(int)Type].SetActive(true);
        transform.position = Position;
        LifeTime = 0f;
        switch (Type)
        {
            case PARTICLETYPE.HIT:
            case PARTICLETYPE.SNAKEATTACK:
            case PARTICLETYPE.EXPLOSION:
            case PARTICLETYPE.SKUNKATTACK:
            case PARTICLETYPE.ASSEMBLEBOTTOMEND:
            case PARTICLETYPE.BUILDINGEXPLOSION:
            case PARTICLETYPE.BUILDINGSELL:
            case PARTICLETYPE.GRSTMILL_EXPLOSION:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                break;
            case PARTICLETYPE.UNITDEATH:
                break;
            case PARTICLETYPE.WATER_BURST:
            case PARTICLETYPE.WATER_WAVE:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                break;
        }
    }

    public void Ready(Vector3 Position, float Scale, PARTICLETYPE Type)
    {
        this.Type = Type;
        ParticleObj[(int)Type].SetActive(true);
        ParticleObj[(int)Type].transform.localScale = new Vector3(Scale, Scale, Scale);
        transform.position = Position;
        LifeTime = 0f;
        switch (Type)
        {
            case PARTICLETYPE.WATER_BURST:
            case PARTICLETYPE.WATER_WAVE:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                break;
        }

    }


    public void Ready(GameObject Target, GameObject Target2, PARTICLETYPE Type)
    {
        TargetPos = Target;
        this.Type = Type;
        transform.position = TargetPos.transform.position;
        ParticleObj[(int)Type].SetActive(true);
        LifeTime = 0f;
        switch (Type)
        {
            case PARTICLETYPE.BOARFLAME:
                BoarFSM = Target2.GetComponent<CommonUnitFSM>();
                //ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleMa = ParticleObj[(int)Type].GetComponent<ParticleManager>();
                BoarFlame = ParticleObj[(int)Type].GetComponent<WildBoarFlame>();
                TargetCharacter = Target.GetComponent<Character>();
                break;
        }
    }
    public void Ready(GameObject Target, Vector3 Position, float Scale, bool flag, PARTICLETYPE Type)
    {
        this.Type = Type;
        transform.position = Position;
        ParticleObj[(int)Type].SetActive(true);
        ParticleObj[(int)Type].transform.localScale = new Vector3(Scale, Scale, Scale);
        LifeTime = 0f;
        switch (Type)
        {
            case PARTICLETYPE.FLAME:

                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                ParticleRenderer = ParticleObj[(int)Type].GetComponent<ParticleSystemRenderer>();
                ParticleMa = ParticleObj[(int)Type].GetComponent<ParticleManager>();
                BuildingBase = Target.GetComponent<BuildingBase>();
                FlameFlag = flag;
                break;
            case PARTICLETYPE.SMOKE_BUILD:
                ParticleMa = ParticleObj[(int)Type].GetComponent<ParticleManager>();
                for (int i = 0; i < 2; i++)
                {
                    var main = ParticleMa.particleSystem[i].main;
                    main.loop = true;
                }
                BuildingBase = Target.GetComponent<BuildingBase>();
                break;
            case PARTICLETYPE.SMOKE_GRISTMILL:
                ParticleSys = ParticleObj[(int)Type].GetComponent<ParticleSystem>();
                BuildingBase = Target.GetComponent<BuildingBase>();
                break;
            case PARTICLETYPE.SMOKE_PLAYER:
                ParticleMa = ParticleObj[(int)Type].GetComponent<ParticleManager>();
                for (int i = 0; i < 2; i++)
                {
                    var main = ParticleMa.particleSystem[i].main;
                    main.loop = true;
                }
                CommanderFSMTarget = Target.GetComponent<CommanderFSM>();
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        switch (Type)
        {
            case PARTICLETYPE.HIT:
            case PARTICLETYPE.SNAKEATTACK:
            case PARTICLETYPE.EXPLOSION:
            case PARTICLETYPE.SKUNKATTACK:
            case PARTICLETYPE.BUILDEND:
            case PARTICLETYPE.WOLFWAVE:
            case PARTICLETYPE.ASSEMBLEBOTTOMEND:
            case PARTICLETYPE.BUILDINGEXPLOSION:
            case PARTICLETYPE.BUILDINGSELL:
            case PARTICLETYPE.WATER_BURST:
            case PARTICLETYPE.WATER_WAVE:
            case PARTICLETYPE.GRSTMILL_EXPLOSION:
                if (ParticleSys.time >= ParticleSys.main.startLifetimeMultiplier)
                {
                    ParticleObj[(int)Type].SetActive(false);
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                }
                break;
            case PARTICLETYPE.HEAL:
            case PARTICLETYPE.POSION:
            case PARTICLETYPE.STIM:
                if (TargetCharacter.gameObject.activeInHierarchy != false)
                {
                    transform.position = new Vector3(TargetPos.transform.position.x, TargetPos.transform.position.y, TargetPos.transform.position.z);
                    int BuffDebuffStack = 0;

                    if (Type == PARTICLETYPE.HEAL)
                        BuffDebuffStack = TargetCharacter.BuffDebuff.HealStack;
                    else
                        BuffDebuffStack = TargetCharacter.BuffDebuff.PoisonStack;

                    if (BuffDebuffStack < 1)
                    {
                        ParticleMa.ParticleStop();
                        LifeTime += Time.deltaTime;
                        if (LifeTime >= ParticleSys.main.startLifetimeMultiplier)
                        {
                            ParticleObj[(int)Type].SetActive(false);
                            PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                        }
                    }
                }
                else
                {
                    ParticleMa.ParticleStop();
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= ParticleSys.main.startLifetimeMultiplier)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
            case PARTICLETYPE.BOARFLAME:
                if (BoarFSM.AttackTarget != null && BoarFSM.gameObject.activeInHierarchy != false)
                {
                    transform.position = TargetPos.transform.position;
                    BoarFlame.Testflag1 = true;
                    BoarFlame.Flame1StartTime = Mathf.Abs(Vector3.Magnitude(BoarFSM.AttackTarget.transform.position - transform.position)) / BoarFlame.Flame1Speed;
                    BoarFlame.Flame2.transform.position = BoarFSM.AttackTarget.transform.position;
                    BoarFlame.transform.LookAt(BoarFSM.AttackTarget.transform.position);
                }
                else
                {
                    BoarFlame.Testflag1 = false;
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= BoarFlame.Flame1StartTime)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
            case PARTICLETYPE.SKUNKPOSION:
                if (TargetPos.gameObject.activeInHierarchy == false)
                {
                    ParticleMa.ParticleStop();
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= ParticleSys.main.startLifetimeMultiplier)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
            case PARTICLETYPE.BUILDING:
                if (BuildingTarget.CurBuildingState != BuildingState.Construct)
                {
                    Debug.Log(ParticleMa);
                    ParticleMa.ParticleStop();
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= ParticleSys.main.startLifetimeMultiplier)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
            case PARTICLETYPE.ASSEMBLE:
                if (CommanderFSMTarget.InputRally == true || CommanderFSMTarget.InputRallyAll == true)
                {
                    transform.position = new Vector3(TargetPos.transform.position.x, TargetPos.transform.position.y, TargetPos.transform.position.z);
                }
                else
                {
                    ParticleMa.ParticleStop();
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= ParticleSys.main.startLifetimeMultiplier)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
            case PARTICLETYPE.ASSEMBLEBOTTOM:
                if (CommanderFSMTarget.InputRally == true || CommanderFSMTarget.InputRallyAll == true)
                {
                    transform.position = new Vector3(TargetPos.transform.position.x - 0.029f, TargetPos.transform.position.y - 0.073f, TargetPos.transform.position.z);
                }
                else
                {
                    ParticleObj[(int)Type].SetActive(false);
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                }
                break;
            case PARTICLETYPE.UNITDEATH:
                LifeTime += Time.deltaTime;
                if (LifeTime >= 1f)
                {
                    ParticleObj[(int)Type].SetActive(false);
                    PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                }
                break;
            case PARTICLETYPE.FLAME:
                bool tempFlag = false;
                if (FlameFlag)
                    tempFlag = BuildingBase.fireFlag;
                else
                    tempFlag = BuildingBase.smokeFlag;

                if (BuildingBase.HP <= 0 || !tempFlag)
                {
                    ParticleMa.ParticleStop();
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= ParticleSys.main.startLifetimeMultiplier)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
            case PARTICLETYPE.SMOKE_BUILD:
                if (BuildingBase.StateOperator.CurBuildingState != BuildingState.Construct || !BuildingBase.gameObject.activeInHierarchy)
                {
                    ParticleMa.ParticleStop();
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= ParticleMa.particleSystem[0].main.startLifetimeMultiplier)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
            case PARTICLETYPE.SMOKE_GRISTMILL:
                if (BuildingBase.StateOperator.CurBuildingState != BuildingState.Construct || !BuildingBase.gameObject.activeInHierarchy)
                {
                    ParticleSys.Stop();
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= ParticleSys.main.startLifetimeMultiplier)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
            case PARTICLETYPE.SMOKE_PLAYER:
                if (!CommanderFSMTarget.InputDigging)
                {
                    ParticleMa.ParticleStop();
                    LifeTime += Time.deltaTime;
                    if (LifeTime >= ParticleMa.particleSystem[0].main.startLifetimeMultiplier)
                    {
                        ParticleObj[(int)Type].SetActive(false);
                        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.ParticleEffect);
                    }
                }
                break;
        }


        if (ParticleRenderer != null)
        {
            if (FogOfWar.Instance.CheckTileAlpha(gameObject.transform.position, GameManager.Instance.CommanderList[0]))
                ParticleRenderer.enabled = true;
            else
                ParticleRenderer.enabled = false;
        }

    }
}
