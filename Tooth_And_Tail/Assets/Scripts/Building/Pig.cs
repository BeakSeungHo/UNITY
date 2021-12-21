using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pig : MonoBehaviour
{
    [SerializeField] GameObject target = null;
    Transform firePoint;

    [SerializeField] Farm farm = null;

    CommonBase commonBase;
    Animator animator;

    Vector3 dest = Vector3.zero;
    Vector3 moveDir;

    [SerializeField] BuildingState state;

    float curAttackDelay = 0;

    int cultivateCount = 0;

    public SpriteRenderer SpriteRenderer = null;
    public HPCanvas HpBar = null;
    public OutLine OutLine;
    // Start is called before the first frame update
    void Awake()
    {
        firePoint = transform.Find("FirePoint");
        commonBase = GetComponent<CommonBase>();
        animator = transform.Find("Body").GetComponent<Animator>();
        state = BuildingState.Idle;
    }

    void SetDest()
    {
        animator.SetBool("Move", true);
        int rand = Random.Range(0, 8);
        dest = farm.GetWheats()[rand].transform.position;
        moveDir = (dest - transform.position).normalized;
    }

    public void SetPig(Camp camp, bool playSound = true)
    {
        commonBase.MyCamp = camp;
        cultivateCount = 0;
        SetDest();
        state = BuildingState.Idle;
        if(playSound)
            farm.buildingBase.Play_Building_Sound(BuildSoundType.Idle, CommonType.Pig);
    }

    public void Die()
    {
        // 죽는 이펙트
        gameObject.SetActive(false);
        
        farm.buildingBase.Play_Building_Sound(BuildSoundType.Destroy, CommonType.Pig);
    }

    public bool SearchTarget()
    {
        Vector3 Pos = transform.position;
        int range = commonBase.Range;

        Character character = null;
        bool found = InGameManager.Instance.Find_TargetInRange_ForPig(Pos, range, commonBase.MyCamp, commonBase.AttackType, out character) != -1;

        if (character != null)
            target = character.gameObject;
        else
            target = null;

        if (found == false)
            target = null;
        
        return found;
    }

    void SetIdle()
    {
        state = BuildingState.Idle;
        animator.SetBool("Move", true);
        SetDest();
        cultivateCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(!farm.IsActiveWheats())
        {
            gameObject.SetActive(false);
        }

        if(dest == Vector3.zero && state == BuildingState.Idle)
        {
            SetDest();
        }

        switch (state)
        {
            case BuildingState.Idle:
                if (SearchTarget())
                {
                    state = BuildingState.Attack;
                    animator.SetBool("Move", false);
                    break;
                }
        
                if (Vector3.Distance(transform.position, dest) < 0.1f)
                {
                    animator.SetBool("Move", false);
                    animator.SetBool("Cultivate", true);
                    if (animator.GetCurrentAnimatorStateInfo(0).IsName("Harvest") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                    {
                        animator.SetBool("Harvest", false);
                        cultivateCount = 0;
                        SetDest();
                    }
                    else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Cultivate") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime / 5 >= 1)
                    {
                        animator.SetBool("Cultivate", false);
                        cultivateCount++;
                        if (cultivateCount > 3)
                        {
                            animator.SetBool("Harvest", true);
                            animator.SetBool("Move", false);

                        }
                        else SetDest();
                    }
                }
                else
                {
                    transform.Translate(moveDir * Time.deltaTime * 0.5f);
                }
                break;
            case BuildingState.Attack:
                if (target == null || target.activeInHierarchy == false)
                {
                    if (!SearchTarget())
                    {
                        SetIdle();
                    }
                }
                else if (TilemapSystem.Instance.RangeInObject(transform.position, target.transform.position, commonBase.Range) == TilemapSystem.Invalid_Range)
                {
                    if (!SearchTarget())
                        SetIdle();
                }

                curAttackDelay += Time.deltaTime;
                if(curAttackDelay >= commonBase.Data.AttackSpeed)
                {
                    animator.SetBool("Attack", true);
                    curAttackDelay = 0;
                    animator.Play("Attack");
                    GameObject projectileObj = PoolManager.Instance.PullObject(Pool_ObjType.Bullet_Normal);

                    Projectile projectile = projectileObj.GetComponent<Projectile>();

                    farm.buildingBase.Play_Building_Sound(BuildSoundType.Attack, CommonType.Pig);

                    if (null != projectile && null != target)
                    {
                        projectile.Ready(farm.buildingBase, commonBase.MyCamp, firePoint.transform.position, commonBase.Damage, commonBase.ProjectileSpeed, target);
                    }
                    else
                    {
                        PoolManager.Instance.PushObject(projectileObj, Pool_ObjType.Bullet_Normal);
                    }
                }
                break;
        }
        FogOfWar.Instance.CheckSprite(transform.position, HpBar, SpriteRenderer);
    }

}
