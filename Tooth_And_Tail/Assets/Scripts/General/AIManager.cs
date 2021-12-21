using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    static public AIManager Instance = null;

    // public Dictionary<Camp, Commander> AICommander = new Dictionary<Camp, Commander>();
    public Dictionary<Camp, CommanderAI> AIs = new Dictionary<Camp, CommanderAI>();

    public List<Vector3> GristmillPositions = new List<Vector3>();
    public bool TileHitObjectTestOn = false;
    public InGameCamera Camera = null;

    public CommanderAI aiPrefab = null;

    public void AI_Ready(Camp camp, Commander commander)
    {
        CommanderAI ai = Instantiate(aiPrefab, transform);
        ai.Ready(commander);
        AIs.Add(camp, ai);
    }
    
    // Start is called before the first frame update
    void Awake()
    {
        if (null != Instance)
        {
            Destroy(Instance);
        }

        Instance = this;
        TileHitObjectTestOn = false;
    }

    // Update is called once per frame
    void Update()
    {
        //foreach (var pair in AIs)
        //{
        //    pair.Value.Update();
        //}

        if (Input.GetKeyDown("1"))
        {
            FogOfWar.Instance.tempFogCamp = GameManager.Instance.CommanderList[0];
            Camera.targetCamp = GameManager.Instance.CommanderList[0];
            InGameManager.controllCamp = GameManager.Instance.CommanderList[0];
        }

        if (Input.GetKeyDown("2"))
        {
            FogOfWar.Instance.tempFogCamp = GameManager.Instance.CommanderList[1];
            Camera.targetCamp = GameManager.Instance.CommanderList[1];
            InGameManager.controllCamp = GameManager.Instance.CommanderList[1];
        }

        //  임시 테스트용 코드
        //{
        //    Vector3 moveDir = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);

        //    Commander AI = AICommander[0];

        //    AI.Move_Commander(moveDir);

        //    if (Input.GetKeyDown("/"))
        //    {
        //        AI.Build(AI.transform.position);
        //    }

        //    if (Input.GetKeyDown(KeyCode.LeftShift))
        //    {
        //        GameObject pulUnitObject = PoolManager.Instance.PullObject(Pool_ObjType.Unit_Normal);
        //        CommonUnit unit = pulUnitObject.GetComponent<CommonUnit>();

        //        if (null == unit)
        //            return;

        //        unit.Ready(AI.Base.MyCamp, AI.SelectedUnit, AI.transform.position);
        //        SquadController.Instance.Add_Unit(AI.Base.MyCamp, pulUnitObject);
        //    }

        //    if (Input.GetKeyDown(KeyCode.RightShift))
        //    {
        //        AI.Command_Move_All();
        //    }

        //    if (Input.GetKeyDown("1"))
        //    {
        //        AI.Change_ControllSquad(0);
        //    }
        //    if (Input.GetKeyDown("2"))
        //    {
        //        AI.Change_ControllSquad(1);
        //    }
        //    if (Input.GetKeyDown("3"))
        //    {
        //        AI.Change_ControllSquad(2);
        //    }
        //    if (Input.GetKeyDown("4"))
        //    {
        //        AI.Change_ControllSquad(3);
        //    }
        //    if (Input.GetKeyDown("5"))
        //    {
        //        AI.Change_ControllSquad(4);
        //    }
        //    if (Input.GetKeyDown("6"))
        //    {
        //        AI.Change_ControllSquad(5);
        //    }
        //}
    }

}
