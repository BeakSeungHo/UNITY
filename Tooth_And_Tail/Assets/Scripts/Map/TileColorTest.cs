using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileColorTest : MonoBehaviour
{
    public Tilemap tilemap;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        tilemap.RefreshAllTiles();
        tilemap.SetTileFlags(new Vector3Int(-1, 0, 0), TileFlags.None);
        tilemap.SetColor(new Vector3Int(-1, 0, 0), Color.red);
        tilemap.GetComponent<TilemapRenderer>().material.color = Color.red;

    }
}
