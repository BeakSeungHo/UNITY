using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meat : MonoBehaviour
{
    CommonBase commonBase = null;
    BuildingBase buildingBase = null;
    Animator animator;

    Coroutine bakeCoroutine = null;

    public int meatLeft = 60;
    int meatQuantity = 0;
    int meatLevel = 0;

    bool nowBaking = false;

    public bool NowBaking
    {
        get { return nowBaking; }
    }


    void Start()
    {
        buildingBase = transform.parent.GetComponent<BuildingBase>();
        animator = GetComponent<Animator>();
        commonBase = transform.parent.GetComponent<CommonBase>();
        meatLevel = meatLeft / commonBase.Data.UnitPerBuliding;
    }

    void OnDisable()
    {
        if (bakeCoroutine != null)
        {
            StopCoroutine(bakeCoroutine);
            bakeCoroutine = null;
            nowBaking = false;
        }
    }

    IEnumerator BakeMeat()
    {
        while (meatQuantity < commonBase.Data.UnitPerBuliding)
        {
            if (buildingBase.HP <= 0)
                yield break;
            if (meatLeft > 0)
            {
                meatLeft--;

                if (meatLeft <= 0)
                    buildingBase.fireFlag = false;
                else
                    buildingBase.fireFlag = true;


                GameManager.Instance.ChangeFoodCamp(buildingBase.Base.MyCamp, 1);
                if (buildingBase.Base.MyCamp == GameManager.Instance.CommanderList[0])
                {
                    //숫자
                    EffectManager.Instance.FontEffectEnable(gameObject, 1, FontEffect.FONTTYPE.PLUSFOOD);
                }
                if (meatLeft % meatLevel == 0)
                {
                    animator.SetInteger("Meat_Quantity", ++meatQuantity);
                }
            }
            yield return new WaitForSeconds(commonBase.Data.AttackSpeed);
        }
        Animator anim = transform.parent.Find("Body").GetComponent<Animator>();
        anim.SetBool("Exhasted", true);
        buildingBase.DestroyBuilding();
    }

    public void Baking()
    {
        nowBaking = true;
        bakeCoroutine = StartCoroutine(BakeMeat());
    }

    public void StopBaking()
    {
        if (bakeCoroutine != null)
        {
            StopCoroutine(bakeCoroutine);
            bakeCoroutine = null;
        }
    }
}
