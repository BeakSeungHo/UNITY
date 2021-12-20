using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutLiner : MonoBehaviour
{
    public static MaterialPropertyBlock materialPropertyBlock = null;
    public OutLineDir Direction = OutLineDir.End;
    
    public void Start()
    {
        //if (materialPropertyBlock == null)
        //{
        //    materialPropertyBlock = new MaterialPropertyBlock();
        //    GetComponent<SpriteRenderer>().GetPropertyBlock(materialPropertyBlock);
        //}

        //GetComponent<SpriteRenderer>().SetPropertyBlock(materialPropertyBlock);
    }

    public void ReturnPool()
    {
        PoolManager.Instance.PushOutLine(gameObject, Direction);
    }
}
