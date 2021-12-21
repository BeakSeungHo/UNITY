using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ProjectileType { Normal, Lizard, Ferret, Skunk, Cannon, End };

public class Projectile : MonoBehaviour
{
    public GameElements gameElements { get { return SceneStarter.Instance.gameElements; } }
    public Camp Camp = Camp.Hopper;

    public CommonType ShooterType = CommonType.End;
    public ProjectileType ProjectileType = ProjectileType.End;

    public Character ShotCharacter = null;

    private Dictionary<ProjectileType, ProjectileBase> projectiles = null;
    //public Vector3 DestPos { get { projectiles[ProjectileType].DestPos; } }
    public Vector3 DestPos
    {
        get
        { return projectiles[ProjectileType].DestPos; }
    }
    public void Ready(Character shotCharacter, Camp camp, Vector3 startPos, float damage, float speed, GameObject target)
    {
        ShooterType = shotCharacter.Base.Type;

        Camp = camp;

        switch (ShooterType)
        {
            case CommonType.Squirrel:
            case CommonType.Pigeon:
            case CommonType.Falcon:
            case CommonType.Snake:
            case CommonType.Turret:
            case CommonType.Balloon:
            case CommonType.Fox:
            case CommonType.Badger:
            case CommonType.Cabin:
            case CommonType.Pig:
            case CommonType.Farm:
                ProjectileType = ProjectileType.Normal;
                break;
            case CommonType.Lizard:
                ProjectileType = ProjectileType.Lizard;
                break;
            case CommonType.Ferret:
                ProjectileType = ProjectileType.Ferret;
                break;
            case CommonType.Cannon:
                ProjectileType = ProjectileType.Cannon;
                break;
            case CommonType.Skunk:
                ProjectileType = ProjectileType.Skunk;
                break;
        }

        if (null == projectiles || projectiles.Count == 0)
        {
            ListInitialize();
        }

        ShotCharacter = shotCharacter;

        projectiles[ProjectileType].Ready(startPos, damage, speed, target);

        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        switch (ShooterType)
        {
            case CommonType.Squirrel:
            case CommonType.Falcon:
            case CommonType.Turret:
            case CommonType.Balloon:
            case CommonType.Cabin:
            case CommonType.Fox:
            case CommonType.Pig:
            case CommonType.Farm:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Pistol];
                break;
            case CommonType.Pigeon:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Medkit];
                break;
            case CommonType.Snake:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Venom];
                break;
            case CommonType.Lizard:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Spear];
                break;
            case CommonType.Ferret:
            case CommonType.Cannon:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Artillery];
                break;
            case CommonType.Skunk:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Grenade];
                break;

            case CommonType.Badger:
                renderer.sprite = gameElements.BullletSpriteDic[eBullet.Minigun3];
                break;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        projectiles[ProjectileType].Update();
    }

    void ListInitialize()
    {
        if (null != projectiles)
        {
            projectiles.Clear();
        }

        projectiles = new Dictionary<ProjectileType, ProjectileBase>();

        projectiles.Add(ProjectileType.Normal, new NormalProjectile());
        projectiles.Add(ProjectileType.Lizard, new LizardProjectile());
        projectiles.Add(ProjectileType.Ferret, new FerretProjectile());
        projectiles.Add(ProjectileType.Skunk,  new SkunkProjectile());
        projectiles.Add(ProjectileType.Cannon, new CannonProjectile());

        foreach (var pair in projectiles)
            pair.Value.Initialize(gameObject);

    }
}
