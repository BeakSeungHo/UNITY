using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GenNodeType { GenNode, SpawningNode }

public class CampaignManager : MonoBehaviour
{
    public static CampaignManager Instance = null;

    public List<GameObject> BreakableBoneList;

    public Transform startPosition;

    public List<ObjectGenNode> genQueue;

    public AnimatorElements animatorElements;

    public GameObject genInfoObj;
    public GameObject genInfoChild;

    public Transform genInfos;

    int genIndex = 0;
    int curBreakIndex = 0;

    // 여긴 젠 정보를 추가하기 위한 변수 입력란
    [Space(15)]
    [Header("---------오브젝트 젠 정보 편집 툴---------")]
    [SerializeField] Camp genCamp = Camp.End;
    [SerializeField] CommonType genType = CommonType.End;
    [SerializeField] CommonType genProductType = CommonType.End;
    [SerializeField] GenNodeType genNodeType = GenNodeType.GenNode;
    [SerializeField] int genCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        if (Instance)
        {
            Destroy(Instance);
            return;
        }

#if UNITY_EDITOR
        animatorElements.InitializeElement();
#endif

        startPosition = transform.Find("StartPoint");
        Instance = this;

        foreach (var node in genQueue)
        {
            if (node.nodeType == GenNodeType.SpawningNode)
            {
                StartCoroutine(node.GenStart());
            }
        }

        if (GameManager.Instance.CurGameMode == GameMode.Campaign)
        {
            genQueue[0].Gen();
        }

        if (InGameManager.Instance != null)
            InGameManager.Instance.Commanders[GameManager.Instance.CommanderList[0]].transform.position = CampaignManager.Instance.startPosition.position;
    }
    
    public ObjectGenNode CreateGenInfo()
    {
        ObjectGenNode node = null;
        switch (genNodeType)
        {
            case GenNodeType.GenNode:
                node = ScriptableObject.CreateInstance<ObjectGenNode>();
                break;

            case GenNodeType.SpawningNode:
                node = ScriptableObject.CreateInstance<SpawningNode>();
                break;
        }
        node.nodeType = genNodeType;
        node.GenCamp = genCamp;
        node.Type = genType;
        node.ProductType = genProductType;
        node.GenCount = genCount;
        node.GenPosition = new List<Vector3>();
        node.GenPosition.Add(Vector3.zero);

        return node;
    }

    public void GenNextObjectsAfterSecond()
    {
        Invoke("GenNextObjects", 1f);
    }

    public void GenNextObjects()
    {
        if (genIndex < genQueue.Count)
        {
            do
            {
                genQueue[genIndex++].Gen();
            } while (genIndex < genQueue.Count && genQueue[genIndex].Wave == genQueue[genIndex - 1].Wave);
        }
    }
    public int curWave()
    {
        if (genIndex < genQueue.Count)
            return genQueue[genIndex].Wave - 1;
        return genQueue[genQueue.Count - 1].Wave - 1;
    }

    public void DestroyObstacle()
    {
        if (curBreakIndex < BreakableBoneList.Count)
        {
            BreakableBoneList[curBreakIndex++].GetComponent<BreakableBone>().DestroyObstacle();
        }
    }
}