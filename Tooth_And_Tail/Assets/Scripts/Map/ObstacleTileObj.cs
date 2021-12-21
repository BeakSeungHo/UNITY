using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTileObj : MonoBehaviour
{
    GameObject col = null;
    // Start is called before the first frame update
    void Start()
    {
        TileNode tile = TilemapSystem.Instance.GetTile(transform.position);
        if (tile == null)
            return;
        tile.Height = 1;
        col = GameObject.Instantiate(TilemapSystem.Instance.ColliderList[(int)TileDir.Decor], tile.worldPosition, Quaternion.identity, TilemapSystem.Instance.Colliders);
    }

    void OnDestroy()
    {
        TileNode tile = TilemapSystem.Instance.GetTile(transform.position);
        if (tile != null)
        {
            tile.Height = 0;
            Destroy(col);
        }
    }
}