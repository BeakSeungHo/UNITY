using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System;

[CreateAssetMenu(fileName = "New Elevation Tile", menuName = "Tiles/Elevation Tile")]
public class ElevationTile : Tile
{
    // 타일의 층수
    [SerializeField] int elevation = 0;

    [SerializeField] TileType tileType;

    // 타일의 방향(벽 등의 지형의 형태에 맞는 콜라이더를 까는데 사용한다.)
    [SerializeField] TileDir tileDir;

    public int Elevation
    {
        get { return elevation; }
    }

    public TileType TileType
    {
        get { return tileType; }
    }

    public TileDir TileDir
    {
        get { return tileDir; }
    }
}

public enum TileType { Error = -1, Ground, Wall, Water, Ramp };
public enum TileDir { LD, RD, LU, RU, L_IN, L_OUT, R_IN, R_OUT, U_IN, U_OUT, D_IN, D_OUT, Decor };