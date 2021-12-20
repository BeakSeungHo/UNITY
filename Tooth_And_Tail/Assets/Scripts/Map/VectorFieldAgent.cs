using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// MonoBehaviour를 상속받긴 하지만, 업데이트를 따로 돌지는 않는다.
/// </summary>

public class VectorFieldAgent : MonoBehaviour
{
    public Character character = null;

    public Vector3 moveDir = Vector3.zero;

    public Vector2Int fieldKey = Vector2Int.zero;
    // 현재 타일에서 다음 타일의 위치
    public Vector3 curDest = Vector3.zero;

    public GameObject DestinationIndicatorPrefab = null;
    //private GameObject Indicator = null;

    public bool DestDebug = false;

    TileNode curTile = null;
    List<TileNode> path = null;

    int curPathIndex = 0;

    public bool useAstar = false;

    public bool IsMove = false;
    
    // Start is called before the first frame update
    void Start()
    {
        //BuildingManager.Instance.AgentList.Add(this);

        //if (DestDebug && null == Indicator)
        //{
        //    Indicator = GameObject.Instantiate(DestinationIndicatorPrefab);
        //}
    }

    private void Update()
    {
        //Indicator?.SetActive(IsMove);
    }


    private void OnDisable()
    {
        //Indicator?.SetActive(false);
    }

    public void Move(Vector2Int key)
    {
        if (fieldKey == key)
        {
            //Debug.Log("VectorFieldAgent - Move : fieldKey has same key.");
            return;
        }
        else
        {
            //Debug.Log("diffrent key");
            //if (null != Indicator)
            //{
            //    var node = TilemapSystem.Instance.GetTile(key);

            //    if (null != node)
            //        Indicator.transform.position = node.worldPosition;
            //}
        }

        // 벡터필드 로딩이 완료되어 벡터필드 사용
        if (TilemapSystem.Instance.IsLoadedVectorField())
        {
            fieldKey = key;
            IsMove = true;
            useAstar = false;
            path = null;
            VisitNode();
        }
        // 벡터필드 로딩이 완료되지 않아 에이스타 사용
        else
        {
            // Debug.Log("에이스타 사용");
            TileNode goalTile = TilemapSystem.Instance.GetTile(key);
            if (goalTile == null)
                return;

            fieldKey = key;

            curPathIndex = 0;
            TileNode tempNode;

            // 에이스타 경로를 받아온다.
            path = TilemapSystem.Instance.GetPath(transform.position, goalTile.worldPosition);
            if (path == null)
            {
                Debug.Log("VisitNode : path is null");
                IsMove = false;
                useAstar = false;
                return;
            }
            if (path.Count > 1)
                path.RemoveAt(path.Count - 1);
            path.Reverse();

            tempNode = TilemapSystem.Instance.GetTile(transform.position);
            curTile = path[curPathIndex];
            curDest = path[curPathIndex].worldPosition;
            moveDir = (path[curPathIndex].worldPosition - transform.position).normalized;

            useAstar = true;
            IsMove = true;
        }
    }

    public void Move(Vector3Int cellPos)
    {
        var destWorldPos = TilemapSystem.Instance.CellToWorldPos(cellPos);
        var key = TilemapSystem.Instance.WorldToTilePos(destWorldPos);

        Move(key);
    }

    // 길찾기를 할 때 다음 노드에 방문하여 다음 노드로의 이동이 유효한 이동인지 검사 후 방향 갱신
    void VisitNode()
    {
        curTile = TilemapSystem.Instance.GetTile(transform.position);

        if (!curTile.VectorField.ContainsKey(fieldKey))
        {
            Debug.Log("fieldKey x : " + fieldKey.x.ToString() + " y : " + fieldKey.y.ToString());
            return;
        }
        curDest = curTile.VectorField[fieldKey];

        if (curTile.VectorField[fieldKey] == Vector3.zero)
        {
            curDest = curTile.worldPosition;
            
            if (Check_Arrive())
            {
                IsMove = false;
                useAstar = false;
                return;
            }

        }

        moveDir = (curDest - transform.position).normalized;
        TileNode checkNode = TilemapSystem.Instance.GetTile(curDest);

        // 만약 현재 노드가 가리키는 노드가 이동이 불가능한 곳이라면 에이스타를 이용하여
        // 그 구역만 빠져 나간다.
        bool check = TilemapSystem.Instance.IsWalkableTile(checkNode.worldPosition);

        if (!check)
        {
            curPathIndex = 0;
            //TileNode tempNode;
            // 현재 체크노드가 가리키는 방향이 갈 수 없는 곳일 수 있으므로 갈 수 있는 타일이 나올 때 까지 갱신한다.
            while (true)
            {
                if (false == TilemapSystem.Instance.IsWalkableTile(checkNode.VectorField[fieldKey]))
                {
                    checkNode = TilemapSystem.Instance.GetTile(checkNode.VectorField[fieldKey]);
                    if (null == checkNode)
                    {
                        Debug.Log("VectorFieldAgent - VisitNode : checkNode is null.");
                        break;
                    }
                    //checkNode = TilemapSystem.Instance.GetTile(tempNode.worldPosition);
                }
                else
                    break;
            }
            path = TilemapSystem.Instance.GetPath(transform.position, checkNode.VectorField[fieldKey]);
            if (null == path)
            {
                Debug.Log("VisitNode : path is null");
                IsMove = false;
                useAstar = false;
                return;
            }

            path.RemoveAt(path.Count - 1);
            path.Reverse();

            //tempNode = TilemapSystem.Instance.GetTile(transform.position);
            curTile = path[curPathIndex];
            curDest = path[curPathIndex].worldPosition;
            moveDir = (path[curPathIndex].worldPosition - transform.position).normalized;

            useAstar = true;
        }
        else
        {
            //Debug.Log("벡터필드 사용");
        }
    }

    //  각 유닛의 Run 상태에서 직접 호출해준다.
    public void Move(float speedRatio = 1f)
    {
        if (!IsMove)
            return;

        if (useAstar)
        {
            if (null != character)
                character.Move(moveDir);
            else
                transform.position += moveDir * Time.deltaTime * speedRatio;

            if (Check_Arrive())
            {
                curPathIndex++;
                if (curPathIndex >= path.Count)
                {
                    // Debug.Log("에이스타 끝");
                    useAstar = false;

                    if (TilemapSystem.Instance.IsLoadedVectorField())
                        VisitNode();
                    else
                        IsMove = false;
                }
                else
                {
                    curDest = path[curPathIndex].worldPosition;
                    moveDir = (path[curPathIndex].worldPosition - transform.position).normalized;
                }
            }
        }
        else
        {
            if (null != character)
                character.Move(moveDir);
            else
                transform.position += moveDir * Time.deltaTime * speedRatio;

            if (Check_Arrive())
            {
                VisitNode();
            }
        }
    }

    /// <summary>
    /// 목적지를 지나쳤는지, 도착했는지 체크함.
    /// </summary>
    /// <returns>true 일 경우 다음 목절지로 변경해야함</returns>
    public bool Check_Arrive()
    {
        if (Check_CloseEnough())
            return true;

        return Check_PassDest();
    }

    public bool Check_CloseEnough()
    {
        // 목적지까지의 거리가 매우 가까움 -> 다음 목적지로 바꿔야함.
        float dist = Vector3.Distance(transform.position, curDest);
        if (Vector3.Distance(transform.position, curDest) < 0.1f)
        {
            //Debug.Log("curDest와 거리 : " + dist);
            return true;
        }

        return false;
    }

    public bool Check_PassDest()
    {        //  현재 위치에서 목적지로 향하는 벡터
        var delta = curDest - transform.position;

        //  이동하는 방향이 같다면 양수가 나오고, 다르면 음수가 나옴
        //  음수가 나왔다 -> 목적지를 지나쳤다.
        if (moveDir.x * delta.x < 0f || moveDir.y * delta.y < 0f)
        {
            //Debug.Log("목적지를 지나침. ");
            //Debug.Log("moveDir : " + moveDir + " delta : " + delta);

            return true;
        }

        return false;
    }
}
