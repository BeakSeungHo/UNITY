using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MinimapCtrl : MonoBehaviour
{
    public Tilemap tilemap;
    public Tile    tile;
    public Camera  minimapCamera;

    private void Start()
    {
        FogOfWar.Instance.SetMinimapTilemap(tilemap);
        
        var Size = TilemapSystem.Instance.tileBounds.size;
        int CurMapIndex = -1;       // 0 : Desert&Water / 1 : Grass / 2: Grass&Water

        string CurMapName = TilemapSystem.Instance.Map.name;
        CurMapName = CurMapName.Substring(0, CurMapName.IndexOf("("));
        for (int i = 0; i < TilemapSystem.Instance.Maps.Count; ++i)
        {
            if (CurMapName == TilemapSystem.Instance.Maps[i].name)
                CurMapIndex = i;
        }
        
        if (CurMapIndex == 4)
        {
            minimapCamera.transform.position = new Vector3(5, 0.5f, -10);
            minimapCamera.orthographicSize = 11;
        }
        else
        {
            minimapCamera.transform.position = new Vector3(0, 0, -10);
            minimapCamera.orthographicSize = 17;
        }

        for (int i = 0; i < Size.x; ++i)
        {
            for(int j = 0; j < Size.y; ++j)
            {
                var TileWorldPos = TilemapSystem.Instance.GetTile(new Vector2Int(i, j)).worldPosition;
                
                Vector3Int v3Int = TilemapSystem.Instance.Wall.WorldToCell(TileWorldPos);
                var Tiletype = TilemapSystem.Instance.GetTileType(v3Int);
                Tile newTile = Instantiate(tile);
                
                switch (Tiletype)
                {
                    case TileType.Wall:
                        if (TilemapSystem.Instance.Decor.HasTile(v3Int))
                        {
                            newTile.color = new Color32(0, 83, 14, 0);
                        }
                        else
                        {
                            newTile.color = new Color32(0, 0, 0, 0);
                        }
                        tilemap.SetTile(v3Int, newTile);
                        break;
                    case TileType.Ground:
                    case TileType.Ramp:
                        if (TilemapSystem.Instance.GetTileElevation(TileWorldPos) == 0)
                        {
                            switch (CurMapIndex)
                            {
                                case 0: // LightBrown
                                    newTile.color = new Color32(163, 132, 52, 0);
                                    break;
                                case 1: // Green
                                case 2:
                                    newTile.color = new Color32(100, 92, 21, 0);
                                    break;
                                case 3:
                                case 4:
                                    newTile.color = new Color32(163, 132, 52, 0);
                                    break;
                            }
                        }
                        else if (TilemapSystem.Instance.GetTileElevation(TileWorldPos) == 1)
                        {
                            switch (CurMapIndex)
                            {
                                case 0: // Beige
                                    newTile.color = new Color32(179, 141, 76, 0);
                                    break;
                                case 1: // Green2
                                    newTile.color = new Color32(125, 106, 34, 0);
                                    break;
                                case 2:
                                    newTile.color = new Color32(138, 112, 39, 0);
                                    break;
                                case 3:
                                case 4:
                                    newTile.color = new Color32(179, 141, 76, 0);
                                    break;
                            }
                        }
                        else
                        {
                            switch (CurMapIndex)
                            {
                                case 0: // Beige
                                    newTile.color = new Color32(179, 141, 76, 0);
                                    break;
                                case 1: // Green2
                                    newTile.color = new Color32(125, 106, 34, 0);
                                    break;
                                case 2:
                                    newTile.color = new Color32(163, 138, 55, 0);
                                    break;
                                case 3:
                                case 4:
                                    newTile.color = new Color32(179, 141, 76, 0);
                                    break;
                            }
                        }
                        tilemap.SetTile(v3Int, newTile);
                        break;
                    case TileType.Water:
                        newTile.color = new Color32(37, 75, 116, 0);
                        tilemap.SetTile(v3Int, newTile);
                        break;
                }
            }
        }


    }
}
