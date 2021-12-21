using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableBone : MonoBehaviour
{
    void Start()
    {
        //int childCount = transform.childCount;
        //for (int i = 0; i < childCount; i++)
        //{
        //    TileNode tile = TilemapSystem.Instance.GetTile(transform.GetChild(i).position);
        //    tile.Height = 1;
        //    tile.occupier = new BuildingBase();
        //}
    }

    public void DestroyObstacle()
    {
        int childCount = transform.childCount;
        for(int i=0;i<childCount;i++)
        {
            TileNode tile = TilemapSystem.Instance.GetTile(transform.GetChild(i).position);
            tile.Height = 0;
            tile.occupier = null;
            Destroy(transform.GetChild(i).gameObject);
        }
    }
}
