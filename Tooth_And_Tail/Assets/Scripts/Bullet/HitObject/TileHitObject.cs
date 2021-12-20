using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileHitType { Toad, Ferret, Skunk, Boar, Cannon }

public class TileHitObject : MonoBehaviour
{
    public Character ShotCharacter = null;
    public float Damage = 0f;
    public GameObject StarForTest = null;

    private int maxUnitHit = 0;
    private int hitCount = 0;
    private bool canAttackStructure = false;

    private float lifeTime = 0f;
    private float lifeCount = 0f;

    private Vector3Int cellPos = Vector3Int.zero;

    public Camp Camp = Camp.End;
    private TileHitType type = TileHitType.Toad;

    private List<Vector3Int> boarFireTile = new List<Vector3Int>();

    public void Ready(Camp camp, float damage, int maxUnitHit, bool canAttackStructure, float lifeTime, Vector3 worldPos)
    {
        this.Camp = camp;
        Damage = damage;
        this.maxUnitHit = maxUnitHit;
        this.canAttackStructure = canAttackStructure;
        this.lifeTime = lifeTime;

        hitCount = 0;
        lifeCount = 0f;
        this.cellPos = TilemapSystem.Instance.WorldToCellPos(worldPos);
        transform.position = worldPos;
    }

    /// <summary>
    /// 두꺼비용 타일 히트 오브젝트 레디 함수
    /// </summary>
    /// <param name="camp"></param>
    /// <param name="damage"></param>
    /// <param name="worldPos"></param>
    public void Ready_Toad(Camp camp, float damage, Vector3 worldPos, Character shotCharacter)
    {
        this.Camp = camp;
        Damage = damage;
        //this.maxUnitHit = 0;
        canAttackStructure = true;
        this.lifeTime = 0;

        hitCount = 0;
        lifeCount = 0f;
        this.cellPos = TilemapSystem.Instance.WorldToCellPos(worldPos);

        type = TileHitType.Toad;

        var node = TilemapSystem.Instance.GetTile(worldPos);
        transform.position = node.worldPosition;

        ShotCharacter = shotCharacter;

        if (AIManager.Instance.TileHitObjectTestOn)
        {
            StarForTest.SetActive(true);

        }
    }

    /// <summary>
    /// 페럿용 타일 히트 오브젝트 레디 함수
    /// </summary>
    /// <param name="camp">진영</param>
    /// <param name="damage">데미지</param>
    /// <param name="maxUnitHit">최대 타격 유닛</param>
    /// <param name="worldPos">히트 위치</param>
    public void Ready_Ferret(Camp camp, float damage, int maxUnitHit, Vector3 worldPos, Character shotCharacter)
    {
        this.Camp = camp;
        Damage = damage;
        this.maxUnitHit = maxUnitHit;
        canAttackStructure = true;
        lifeTime = 0;

        hitCount = 0;
        lifeCount = 0f;
        this.cellPos = TilemapSystem.Instance.WorldToCellPos(worldPos);

        type = TileHitType.Ferret;

        var node = TilemapSystem.Instance.GetTile(worldPos);
        transform.position = node.worldPosition;

        ShotCharacter = shotCharacter;

        if (AIManager.Instance.TileHitObjectTestOn)
        {
            StarForTest.SetActive(true);
        }
    }

    /// <summary>
    /// 스컹크용 타일 히트 오브젝트 레디 함수
    /// </summary>
    /// <param name="camp">진영</param>
    /// <param name="damage">데미지</param>
    /// <param name="lifeTime">생존 주기</param>
    /// <param name="worldPos">히트 위치</param>
    public void Ready_Skunk(Camp camp, float damage, float lifeTime, Vector3 worldPos, Character shotCharacter)
    {
        this.Camp = camp;
        Damage = damage;
        //this.maxUnitHit = 0;
        canAttackStructure = false;
        this.lifeTime = lifeTime;

        hitCount = 0;
        lifeCount = 0f;
        this.cellPos = TilemapSystem.Instance.WorldToCellPos(worldPos);

        StorageBoxes.Instance.TileObjects[cellPos].SkunkHitObject = this;

        type = TileHitType.Skunk;

        var node = TilemapSystem.Instance.GetTile(worldPos);
        transform.position = node.worldPosition;

        ShotCharacter = shotCharacter;

        if (AIManager.Instance.TileHitObjectTestOn)
        {
            StarForTest.SetActive(true);
        }

        //  연기 이펙트 추가
        EffectManager.Instance.EffectEnable(this.gameObject, ParticleObject.PARTICLETYPE.SKUNKPOSION);
    }

    /// <summary>
    /// 멧돼지용 타일 히트 오브젝트 레디 함수
    /// </summary>
    /// <param name="camp">진영</param>
    /// <param name="damage">데미지</param>
    /// <param name="lifeTime">생존 주기</param>
    /// <param name="worldPos">히트 위치</param>
    public void Ready_Boar(Camp camp, float damage, float lifeTime, Vector3 worldPos, Character shotCharacter)
    {
        this.Camp = camp;
        Damage = damage;
        canAttackStructure = true;
        this.lifeTime = lifeTime;

        hitCount = 0;
        lifeCount = 0f;
        this.cellPos = TilemapSystem.Instance.WorldToCellPos(worldPos);

        type = TileHitType.Boar;

        var node = TilemapSystem.Instance.GetTile(worldPos);
        transform.position = node.worldPosition;

        boarFireTile.Clear();

        if (boarFireTile.Capacity < 7)
            boarFireTile.Capacity = 7;

        boarFireTile.Add(Vector3Int.zero);

        int skipCount = 0;
        for (int i = 0; i < 8; ++i)
        {
            if (skipCount < 2 && Random.Range(0f, 800f) < 200f)
            {
                ++skipCount;
                continue;
            }
            boarFireTile.Add(new Vector3Int(Global.DirX[i], Global.DirY[i], 0));
        }

        ShotCharacter = shotCharacter;

        //if (AIManager.Instance.TileHitObjectTestOn)
        //{
        //    //StarForTest.SetActive(true);
        //}
    }

    /// <summary>
    /// 캐논용 타일 히트 오브젝트 레디 함수
    /// </summary>
    /// <param name="camp">진영</param>
    /// <param name="damage">공격력</param>
    /// <param name="worldPos">히트 위치</param>
    public void Ready_Cannon(Camp camp, float damage, Vector3 worldPos, Character shotCharacter)
    {
        Ready_Toad(camp, damage, worldPos, shotCharacter);
    }

    /// <summary>
    /// 캐논용 타일 히트 오브젝트 레디 함수
    /// </summary>
    /// <param name="camp">진영</param>
    /// <param name="damage">데미지</param>
    /// <param name="cellPos">타일 위치</param>
    public void Ready_Cannon(Camp camp, float damage, Vector3Int cellPos, Character shotCharacter)
    {
        this.Camp = camp;
        Damage = damage;
        //this.maxUnitHit = 0;
        canAttackStructure = true;
        this.lifeTime = 0;

        hitCount = 0;
        lifeCount = 0f;

        var worldPos = TilemapSystem.Instance.CellToWorldPos(cellPos);
        this.cellPos = cellPos;

        type = TileHitType.Toad;

        var node = TilemapSystem.Instance.GetTile(worldPos);
        transform.position = node.worldPosition;

        ShotCharacter = shotCharacter;

        if (AIManager.Instance.TileHitObjectTestOn)
        {
            StarForTest.SetActive(true);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        switch (type)
        {
            case TileHitType.Toad:
            case TileHitType.Cannon:
                Update_Toad();
                break;
            case TileHitType.Ferret:
                Update_Ferret();
                break;
            case TileHitType.Skunk:
                Update_Skunk();
                break;
            case TileHitType.Boar:
                Update_Boar();
                break;
        }
    }

    /// <summary>
    /// 두꺼비용 업데이트
    /// </summary>
    void Update_Toad()
    {
        //  유닛 충돌 검사
        var unitSet = StorageBoxes.Instance.TileObjects[cellPos].OccupiedUnitSet;

        foreach (var unit in unitSet)
        {
            if (!unit.gameObject.activeSelf)
                continue;
            if (Camp != unit.Base.MyCamp)
            {
                unit.Hit(Damage, ShotCharacter);
            }
        }

        var tileNode = TilemapSystem.Instance.GetTile(transform.position);

        if (null != tileNode.occupier)
        {
            var buildingCommonBase = tileNode.occupier;

            Camp buildingCamp = buildingCommonBase.Base.MyCamp;
            if (Camp != buildingCamp)
            {
                var character = buildingCommonBase.gameObject.GetComponent<Character>();
                if (null == character)
                {
                    Debug.Log("building has not character component!");
                }
                else
                {
                    character.Hit(Damage, ShotCharacter);
                }
            }
        }

        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_HitObject);
    }

    /// <summary>
    /// 페럿용 업데이트
    /// </summary>
    void Update_Ferret()
    {
        //  유닛 충돌 검사
        var unitSet = StorageBoxes.Instance.TileObjects[cellPos].OccupiedUnitSet;

        if (0 == unitSet.Count)
            Debug.Log("unitset.Count : 0");

        foreach (var unit in unitSet)
        {
            if (hitCount >= maxUnitHit)
                break;

            if (!unit.gameObject.activeSelf)
                continue;

            if (Camp != unit.Base.MyCamp)
            {
                if (PlaceType.Air == unit.Base.PlaceType)
                    continue;

                unit.Hit(Damage, ShotCharacter);
                ++hitCount;
            }
        }
        hitCount = 0;

        //  빌딩 충돌 검사
        if (canAttackStructure)
        {
            var tileNode = TilemapSystem.Instance.GetTile(transform.position);

            if (null != tileNode.occupier)
            {
                var buildingCommonBase = tileNode.occupier;

                Camp buildingCamp = buildingCommonBase.Base.MyCamp;
                if (Camp != buildingCamp)
                {
                    var character = buildingCommonBase.gameObject.GetComponent<Character>();
                    if (null == character)
                    {
                        Debug.Log("building has not character component!");
                    }
                    else
                    {
                        character.Hit(Damage, ShotCharacter);
                    }
                }
            }
        }

        PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_HitObject);
    }

    /// <summary>
    /// 스컹크용 업데이트
    /// </summary>
    void Update_Skunk()
    {
        //  유닛 충돌 검사
        var unitSet = StorageBoxes.Instance.TileObjects[cellPos].OccupiedUnitSet;

        foreach (var unit in unitSet)
        {
            if (!unit.gameObject.activeSelf)
                continue;

            if (Camp != unit.Base.MyCamp)
            {
                if (PlaceType.Air == unit.Base.PlaceType)
                    continue;

                unit.Hit(Damage * Time.deltaTime, ShotCharacter);
            }
        }

        //  생존 주기 카운트
        lifeCount += Time.deltaTime;

        if (lifeCount >= lifeTime)
        {
            PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_HitObject);
            if (this == StorageBoxes.Instance.TileObjects[cellPos].SkunkHitObject)
                StorageBoxes.Instance.TileObjects[cellPos].SkunkHitObject = null;
            return;
        }
    }

    /// <summary>
    /// 멧돼지용 업데이트
    /// </summary>
    void Update_Boar()
    {
        //  유닛 충돌 검사
        for (int i = 0; i < boarFireTile.Count; ++i)
            Hit_Unit(cellPos + boarFireTile[i]);

        HashSet<Character> hitBuilding = new HashSet<Character>();
        for (int i = 0; i < boarFireTile.Count; ++i)
        {
            var tileNode = TilemapSystem.Instance.GetTile(TilemapSystem.Instance.CellToWorldPos(cellPos + boarFireTile[i]));

            if (null != tileNode.occupier)
            {
                var building = tileNode.occupier;

                if (Global.IsAttackableBuilding(Camp, building))
                    hitBuilding.Add(building);
            }
        }
        foreach (var building in hitBuilding)
            building.Hit(Damage * Time.deltaTime, ShotCharacter);

        //  생존 주기 카운트
        lifeCount += Time.deltaTime;

        if (lifeCount >= lifeTime)
        {
            PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_HitObject);
            return;
        }
    }

    private void Hit_Unit(Vector3Int tilePos)
    {
        var unitSet = StorageBoxes.Instance.TileObjects[tilePos].OccupiedUnitSet;

        foreach (var unit in unitSet)
        {
            if (!unit.gameObject.activeSelf)
                continue;

            if (Camp != unit.Base.MyCamp)
            {
                if (PlaceType.Air == unit.Base.PlaceType)
                    continue;

                unit.Hit(Damage * Time.deltaTime, ShotCharacter);
            }
        }
    }
}