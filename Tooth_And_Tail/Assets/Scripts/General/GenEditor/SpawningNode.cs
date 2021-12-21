using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnNode", menuName = "Data/SpawnNode")]
public class SpawningNode : ObjectGenNode
{
    public int Level = 1;
    public float Tear1Ratio = 0;
    public float Tear2Ratio = 0;
    public float Tear3Ratio = 0;

    public override IEnumerator GenStart()
    {
        float corTime = 5f;
        switch(Level)
        {
            case 2:
                yield return new WaitForSeconds(45);
                corTime = 10;
                break;

            case 3:
                yield return new WaitForSeconds(90);
                corTime = 20;
                break;

            case 4:
                yield return new WaitForSeconds(135);
                corTime = 30;
                break;
        }
        WaitForSeconds wait = new WaitForSeconds(corTime);
        while (true)
        {
            yield return wait;
            Gen();
        }
    }

    public override void Gen()
    {
        float curTear1Ratio = GenCount * Tear1Ratio;
        float curTear2Ratio = GenCount * Tear2Ratio;
        float curTear3Ratio = GenCount * Tear3Ratio;

        int randPos = Random.Range(0, 4);

        Vector2Int genPos = Vector2Int.zero;

        switch (randPos)
        {
            case 0:
                // 오른쪽 아래
                genPos.x = Random.Range(0, TilemapSystem.Instance.tileBounds.size.x - 1);
                genPos.y = Random.Range(1, 4);
                break;

            case 1:
                // 왼쪽 위
                genPos.x = Random.Range(0, TilemapSystem.Instance.tileBounds.size.x - 1);
                genPos.y = Random.Range(TilemapSystem.Instance.tileBounds.size.y - 4, TilemapSystem.Instance.tileBounds.size.y - 1);
                break;

            case 2:
                // 왼쪽 아래
                genPos.x = Random.Range(1, 4);
                genPos.y = Random.Range(0, TilemapSystem.Instance.tileBounds.size.y - 1);
                break;

            case 3:
                // 오른쪽 위
                genPos.x = Random.Range(TilemapSystem.Instance.tileBounds.size.x - 4, TilemapSystem.Instance.tileBounds.size.x - 1);
                genPos.y = Random.Range(0, TilemapSystem.Instance.tileBounds.size.y - 1);
                break;
        }

        TileNode node = TilemapSystem.Instance.GetTile(genPos);
        if (node.Height == 1)
        {
            foreach (var neightbor in node.Neighbors)
            {
                if (neightbor.Height == 0)
                {
                    genPos = TilemapSystem.Instance.WorldToTilePos(neightbor.worldPosition);
                    break;
                }
            }
        }

        for (int i = 0; i < GenCount; i++)
        {
            CommonType type = CommonType.End;
            if (i < curTear1Ratio)
            {
                type = CommonType.Toad;
            }
            else if (i < curTear1Ratio + curTear2Ratio)
            {
                type = CommonType.Falcon;
            }
            else
                type = CommonType.Badger;

            GameObject pullObj = PoolManager.Instance.PullObject(Pool_ObjType.Unit_Normal);
            CommonUnit unit = pullObj.GetComponent<CommonUnit>();

            if (null == unit)
                return;

            Vector3 worldPos = TilemapSystem.Instance.GetTile(genPos).worldPosition;

            unit.Ready(GenCamp, type, worldPos);
            SquadController.Instance.Add_Unit(GenCamp, pullObj);
        }

        SquadController.Instance.Command_Move_All(GenCamp, new Vector3(6.94f, 0.59f));
    }
}