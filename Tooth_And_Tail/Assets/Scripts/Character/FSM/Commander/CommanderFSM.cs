using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommanderFSM : MonoBehaviour
{
    public Machine<CommanderFSM> State;

    public enum STATE { IDLE, RUN, RALLY_STAND, RALLY_RUN, ATTACK_STAND, ATTACK_RUN, DIG, RETURN, DEATH, RESPAWN, END };

    public FSM<CommanderFSM>[] ArrState = new FSM<CommanderFSM>[(int)STATE.END];

    public STATE curState = STATE.END;
    public STATE preState = STATE.END;

    public GameObject   Parent = null;
    public GameObject   Sprite = null;

    public Animator    animator = null;
    public Commander   commander = null;

    [HideInInspector] public bool isAttack      = false;
    [HideInInspector] public bool isRally       = false;
    [HideInInspector] public bool isCommandAll  = false;
    [HideInInspector] public bool IsDigging     = false;
    [HideInInspector] public bool IsReturn      = false;

    [HideInInspector] public Vector3 moveDir = Vector3.zero;
    [HideInInspector] public bool InputRally = false;
    [HideInInspector] public bool InputRallyAll = false;
    [HideInInspector] public bool InputDigging = false;

    [HideInInspector] public bool InputReturn = false;
    private bool preInputReturn = false;

    [HideInInspector] public bool CanBeTarget = true;

    [HideInInspector] public Vector3Int CurCell = Vector3Int.zero;

    private bool preCommanded = false;
    private Sound comSound = null;
    private float footStepCount = 0f;
    private float footStepTime = 0.3f;

    //private GameObject AttackTarget { get { return commander.AttackTarget; } }
    private Character AttackTarget { get { return commander.AttackTarget; } }

    private bool RallyEffect = false;

    public CommanderFSM()
    {
        State = new Machine<CommanderFSM>();

        ArrState[(int)STATE.IDLE]           = new CommanderIdle(this);
        ArrState[(int)STATE.RUN]            = new CommanderRun(this);
        ArrState[(int)STATE.RALLY_STAND]    = new CommanderRallyStand(this);
        ArrState[(int)STATE.RALLY_RUN]      = new CommanderRallyRun(this);
        ArrState[(int)STATE.ATTACK_STAND]   = new CommanderAttackStand(this);
        ArrState[(int)STATE.ATTACK_RUN]     = new CommanderAttackRun(this);
        ArrState[(int)STATE.DIG]            = new CommanderDig(this);
        ArrState[(int)STATE.RETURN]         = new CommanderReturn(this);
        ArrState[(int)STATE.DEATH]          = new CommanderDeath(this);
        ArrState[(int)STATE.RESPAWN]        = new CommanderRespawn(this);

        State.SetState(ArrState[(int)STATE.IDLE], this);
    }

    public void ChangeFSM(STATE newState)
    {
        if ((int)newState < (int)STATE.END)
            State.Change(ArrState[(int)newState]);
    }

    //  움직이는 입력이 들어올 경우 true, Out은 움직일 방향을 내보낸다.
    public bool InputMove(out Vector3 Out)
    {
        //Debug.Log("x : " + moveDir.x.ToString() + "y : " + moveDir.y.ToString());

        Out = new Vector3(moveDir.x, moveDir.y, 0f);

        return Out != Vector3.zero;
    }

    public void Begin()
    {
        State.Begin();
    }

    public void Run()
    {
        if (commander.HP <= 0 && STATE.RESPAWN != curState)
            ChangeFSM(STATE.DEATH);

        if (!preCommanded)
        {
            if (InputRally)
            {
                SquadController.Instance.Play_CommandUnitSound(commander.Base.MyCamp, commander.SelectedUnit);
            }
            else if (InputRallyAll)
            {
                SquadController.Instance.Play_CommandUnitSound(commander.Base.MyCamp);
            }
        }
        else
        {
            //Debug.Log("이전프레임 누름.");
        }

        preCommanded = InputRally || InputRallyAll;

        isCommandAll    = false;
        isAttack        = false;
        isRally         = false;
        IsDigging       = false;
        IsReturn        = false;

        //  귀환 버튼 Down인지 판별.
        IsReturn = !preInputReturn && InputReturn;
        preInputReturn = InputReturn;

        if (InputDigging)
        {
            if (commander.IsOnWater) //  물 위에 있을 때 예외 처리
            {

            }
            else
                IsDigging = true;
        }
        else if (InputRally)
        {
            isCommandAll = false;
            if (null != AttackTarget && AttackTarget.activeSelf)
            {
                var TargetBase = AttackTarget.GetComponent<CommonBase>();
                if (commander.Base.MyCamp != TargetBase.MyCamp ||
                    !(Camp.End == TargetBase.MyCamp && CommonType.Farm == TargetBase.Type))
                    isAttack = true;
                else
                    isRally = true;
            }
            else
                isRally = true;

            //RallyEffect
            if (!RallyEffect)
            {
                EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.ASSEMBLE);
                int index = SquadController.Instance.TypeToSquadNumber[commander.Base.MyCamp][commander.SelectedUnit];
                if (SquadController.Instance.Squads[commander.Base.MyCamp][index].UnitList.Count != 0)
                    EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.ASSEMBLEBOTTOM);
                RallyEffect = true;
            }
        }
        else if (InputRallyAll)
        {
            isCommandAll = true;
            if (null != AttackTarget && AttackTarget.activeSelf)
            {
                var TargetBase = AttackTarget.GetComponent<CommonBase>();
                if (commander.Base.MyCamp != TargetBase.MyCamp ||
                    !(Camp.End == TargetBase.MyCamp && CommonType.Farm == TargetBase.Type))
                    isAttack = true;
                else
                    isRally = true;
            }
            else
                isRally = true;

            //RallyEffect
            if (!RallyEffect)
            {
                EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.ASSEMBLE);
                EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.ASSEMBLEBOTTOM);
                RallyEffect = true;
            }
        }
        else
        {
            if (RallyEffect)
            {
                EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.ASSEMBLEBOTTOMEND);
                RallyEffect = false;
            }
        }


        State.Run();
    }

    public void Exit()
    {
        State.Exit();
    }

    private void Awake()
    {

    }

    private void Start()
    {
        Begin();
    }

    private void Update()
    {
        animator.SetBool("Swim", commander.IsOnWater);

        Vector3Int curCellPos = TilemapSystem.Instance.WorldToCellPos(Parent.transform.position);
        if (curCellPos != CurCell)
        {
            if (StorageBoxes.Instance.TileObjects.ContainsKey(CurCell) &&
                StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Contains(commander))
                StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Remove(commander);

            CurCell = curCellPos;
            if (StorageBoxes.Instance.TileObjects.ContainsKey(CurCell))
                StorageBoxes.Instance.TileObjects[CurCell].OccupiedUnitSet.Add(commander);
        }

        Run();
    }

    public void Play_CommanderSound(Sound_Channel channel, ComSoundType soundType, int index)
    {
        var soundDic = SceneStarter.Instance.soundElements.ComSoundDic;

        if (!soundDic.ContainsKey(commander.camp))
        {
            Debug.Log("ComSoundDic don't contain " + commander.camp + " key.");
            return;
        }

        if (!soundDic[commander.camp].ContainsKey(soundType))
        {
            Debug.Log("ComSoundDic[" + commander.camp + "] don't contain " + soundType + " key.");
            return;
        }

        var audioArray = soundDic[commander.camp][soundType];

        SoundManager.Instance.Play(channel, commander.gameObject, audioArray[index]);
    }

    public void Play_CommanderSound(ComSoundType soundType, int index)
    {
        var soundDic = SceneStarter.Instance.soundElements.ComSoundDic;

        if (!soundDic.ContainsKey(commander.camp))
        {
            Debug.Log("ComSoundDic don't contain " + commander.camp + " key.");
            return;
        }

        if (!soundDic[commander.camp].ContainsKey(soundType))
        {
            Debug.Log("ComSoundDic[" + commander.camp + "] don't contain " + soundType + " key.");
            return;
        }

        var audioArray = soundDic[commander.camp][soundType];

        SoundManager.Instance.Play(Sound_Channel.Voice, commander.gameObject, audioArray[index]);
    }

    public void Play_CommanderSound(ComSoundType soundType)
    {
        if (null != comSound)
        {
            if (comSound.gameObject.activeSelf)
                return;
            else
                comSound = null;
        }

        var soundDic = SceneStarter.Instance.soundElements.ComSoundDic;

        if (!soundDic.ContainsKey(commander.camp))
        {
            Debug.Log("ComSoundDic don't contain " + commander.camp + " key.");
            return;
        }

        if (!soundDic[commander.camp].ContainsKey(soundType))
        {
            Debug.Log("ComSoundDic[" + commander.camp + "] don't contain " + soundType + " key.");
            return;
        }

        var audioArray = soundDic[commander.camp][soundType];
        int random = Random.Range(0, audioArray.Count);

        SoundManager.Instance.Play(Sound_Channel.Voice, commander.gameObject, audioArray[random], out comSound);
    }

    public void Play_FootStepSound()
    {
        footStepCount += Time.deltaTime;

        if (footStepCount >= footStepTime)
        {
            footStepCount -= footStepTime;

            List<AudioClip> audios = null;

            var tileType = TilemapSystem.Instance.GetTileType(commander.Pos);

            switch (tileType)
            {
                case TileType.Ground:
                case TileType.Ramp:
                    audios = SceneStarter.Instance.soundElements.FootSoundDic[FootSoundType.Dirt];
                    break;
                case TileType.Water:
                    audios = SceneStarter.Instance.soundElements.FootSoundDic[FootSoundType.Water];
                    break;
                default:
                    audios = SceneStarter.Instance.soundElements.FootSoundDic[FootSoundType.Dirt];
                    break;
            }

            int index = Random.Range(0, audios.Count);

            Sound sound = null;
            SoundManager.Instance.Play(Sound_Channel.Effect, commander.gameObject, audios[index], out sound);

            sound.Set_Volume(0.5f);
        }
    }
}
