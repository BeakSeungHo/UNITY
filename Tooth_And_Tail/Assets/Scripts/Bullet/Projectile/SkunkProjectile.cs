using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkunkProjectile : ProjectileBase
{
    public Vector3 gravityDir = new Vector3(0f, -1f, 0f);
    public Vector3 upforceDir = new Vector3(0f, 1f, 0f);

    private float gravity = 9.8f * 0.5f;
    private float upforce = 1f;
    private float accTime = 0f;
    private float flyingTime = 0f;

    public override void Ready(Vector3 startPos, float damage, float speed, GameObject target)
    {
        base.Ready(startPos, damage, speed, target);

        accTime = 0f;

        var node = TilemapSystem.Instance.GetTile(target.transform.position);

        if (null != node)
            DestPos = node.worldPosition;

        //  Calculate Upforce
        Vector3 delta = DestPos - startPos;
        flyingTime = 1 / Speed;

        Speed = delta.magnitude / flyingTime;

        upforce = gravity * 0.5f * flyingTime;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    public override void Update()
    {
        accTime += Time.deltaTime;
        Vector3 moveDelta = (upforce * upforceDir) + (gravity * accTime * gravityDir);
        Vector3 sumDir = (moveDelta + moveDir * Speed);

        float angle = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), sumDir.normalized).eulerAngles.z;
        gameObject.transform.eulerAngles = new Vector3(0f, 0f, angle);

        gameObject.transform.position += sumDir * Time.deltaTime;

        Vector3 pos = gameObject.transform.position;


        if (accTime >= flyingTime)
        {
            //  착탄 지점 확인
            Vector3Int landingCellPos = TilemapSystem.Instance.WorldToCellPos(DestPos);

            destCellPos.Enqueue(landingCellPos);
            checkVisit.Add(landingCellPos);

            int count = 0;
            while (!IsItEmpty(landingCellPos))
            {
                landingCellPos = NextPosition();
                ++count;
                if (count > 1000)
                {
                    Debug.Log("Error : Infinite Loop!");
                    break;
                }
            }

            destCellPos.Clear();
            checkVisit.Clear();

            Vector3 landingPoint = TilemapSystem.Instance.CellToWorldPos(landingCellPos);

            //  Hit Function
            GameObject pullObject = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_HitObject);
            //HitObject hitObject = pullObject.GetComponent<HitObject>();
            TileHitObject tileHitObject = pullObject.GetComponent<TileHitObject>();

            //if (null != hitObject)
            if (null != tileHitObject)
            {
                //hitObject.Ready(HitType.SkunkHit, projectile.Camp, false, landingPoint, 0.5f, Damage / 10f, 5f, 0, 0.1f);
                tileHitObject.Ready_Skunk(projectile.Camp, Damage, 5f, landingPoint, projectile.ShotCharacter);
                //Effect
                EffectManager.Instance.EffectEnable(gameObject, ParticleObject.PARTICLETYPE.SKUNKATTACK);
                //StorageBoxes.Instance.BoxOfSkunkHit.AddLast(hitObject);
            }

            Play_ExplosionSound();
            PoolManager.Instance.PushObject(gameObject, Pool_ObjType.Bullet_Normal);
            return;
        }
    }

    readonly int[] dirX = { 1, -1, 0, 0 };
    readonly int[] dirY = { 0, 0, 1, -1 };

    Queue<Vector3Int> destCellPos = new Queue<Vector3Int>();
    HashSet<Vector3Int> checkVisit = new HashSet<Vector3Int>();

    Vector3Int NextPosition()
    {
        if (destCellPos.Count == 0)
        {
            Debug.Log("NextPosition Error, movingPos is empty");
        }

        Vector3Int retPos = destCellPos.Dequeue();

        BoundsInt tileBounds = TilemapSystem.Instance.tileBounds;

        for (int i = 0; i < 4; ++i)
        {
            int x = retPos.x + dirX[i];
            int y = retPos.y + dirY[i];
            Vector3Int nextPos = new Vector3Int(x, y, 0);

            if (!TilemapSystem.Instance.HasTile(nextPos))
                continue;

            if (checkVisit.Contains(nextPos))
                continue;

            destCellPos.Enqueue(nextPos);
            checkVisit.Add(nextPos);
        }

        return retPos;
    }

    bool IsItEmpty(Vector3Int checkCellPos)
    {
        //LinkedList<HitObject> box = StorageBoxes.Instance.BoxOfSkunkHit;

        return null == StorageBoxes.Instance.TileObjects[checkCellPos].SkunkHitObject;
        
    }

}
