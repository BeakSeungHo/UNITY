using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Pool_ObjType
{
    Unit_Normal = 0,
    MoleeMerge, Building_Defender, Warrens,
    Bullet_Normal, Bullet_HitObject,
    ParticleEffect, FontEffect, SpriteEffect,
    End,
    Bullet_TickHit = 13
};

public enum OutLineDir
{
    LD = 9, LU = 10, RD = 11, RU = 12, End
};


public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance = null;

    //실제 풀링될 오브젝트들을 담고 있는 큐의 리스트
    List<Queue<GameObject>> objList;

    [SerializeField] List<GameObject> prefabList = null;

    // Start is called before the first frame update
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        GameObject inst;

        objList = new List<Queue<GameObject>>();

        for (int i = 0; i < (int)Pool_ObjType.End; i++)
        {
            objList.Add(new Queue<GameObject>());

            for (int k = 0; k <= 100; k++)
            {
                inst = Instantiate(prefabList[i], transform);
                inst.SetActive(false);

                objList[i].Enqueue(inst);
            }
        }

        for (int i = (int)Pool_ObjType.End; i <= (int)OutLineDir.End; i++)
        {
            objList.Add(new Queue<GameObject>());

            for (int k = 0; k <= 100; k++)
            {
                inst = Instantiate(prefabList[i], transform);
                Color color = GetCommanderColor();
                color.a = 0.5f;
                SpriteRenderer renderer = inst.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    renderer.color = color;
                }
                inst.SetActive(false);

                objList[i].Enqueue(inst);
            }
        }
    }

    Color GetCommanderColor()
    {
        Color color = Color.white;
        switch (GameManager.Instance.CommanderList[0])
        {
            case Camp.Archimedes:
                color = Color.yellow;
                break;

            case Camp.Bellafide:
                color = Color.blue;
                break;

            case Camp.Hopper:
                color = Color.red;
                break;

            case Camp.Quartermaster:
                color = Color.green;
                break;
        }
        return color;
    }

    public GameObject PullOutLine(OutLineDir dir)
    {
        int iType = (int)dir;
        if (objList[iType].Count == 0)
        {
            GameObject inst;
            for (int i = 0; i < 20; i++)
            {
                inst = Instantiate(prefabList[iType], transform);

                Color color = GetCommanderColor();

                color.a = 0.5f;

                inst.GetComponent<SpriteRenderer>().color = color;

                inst.SetActive(false);

                objList[iType].Enqueue(inst);
            }
        }

        GameObject retVal = objList[iType].Dequeue();
        retVal.SetActive(true);
        return retVal;
    }

    public void PushOutLine(GameObject obj, OutLineDir dir)
    {
        obj.SetActive(false);
        objList[(int)dir].Enqueue(obj);
    }

    public GameObject PullObject(Pool_ObjType type)
    {
        int iType = (int)type;
        if (objList[iType].Count == 0)
        {
            GameObject inst;
            for (int i = 0; i < 20; i++)
            {
                inst = Instantiate(prefabList[iType], transform);
                inst.SetActive(false);

                objList[iType].Enqueue(inst);
            }
        }

        GameObject retVal = objList[iType].Dequeue();
        retVal.SetActive(true);
        return retVal;
    }

    public void PushObject(GameObject obj, Pool_ObjType type)
    {
        obj.SetActive(false);
        objList[(int)type].Enqueue(obj);
    }
}
