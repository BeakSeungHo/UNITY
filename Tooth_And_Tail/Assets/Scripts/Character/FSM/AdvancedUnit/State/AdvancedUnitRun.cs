using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedUnitRun : FSM<AdvancedUnitFSM>
{
    private AdvancedUnitFSM ownerFSM;

    public AdvancedUnitRun(AdvancedUnitFSM ownerFSM)
    {
        this.ownerFSM = ownerFSM;
    }

    public override void Begin()
    {
        ownerFSM.curState = AdvancedUnitFSM.STATE.RUN;
        ownerFSM.Animator.SetBool("Run", true);
        ownerFSM.AttackEffect = false;
        //Debug.Log("AdvancedUnit : Run Begin");
    }
    
    public override void Run()
    {
        switch (ownerFSM.Base.Type)
        {
            case CommonType.Boar:
                Action_Boar();
                break;
            case CommonType.Badger:
                Action_Badger();
                break;
            case CommonType.Wolf:
                Action_Wolf();
                break;
        }

    }

    public override void Exit()
    {
        ownerFSM.preState = AdvancedUnitFSM.STATE.RUN;
        ownerFSM.Animator.SetBool("Run", false);
        //Debug.Log("AdvancedUnit : Run Exit");
    }

    public void Action_Boar()
    {
        if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
            ownerFSM.CommandedTarget = null;
        }

        if (null != ownerFSM.AttackTarget)
        {
            //  AttackTarget 이 커맨더라면 
            if (CommonType.Commander == ownerFSM.AttackTarget.Base.Type ||
                !ownerFSM.CheckTargetInRange())
                ownerFSM.AttackTarget = null;
        }

        if (ownerFSM.Scout_Enemy())
        {
            if (ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
            }
        }

        //ownerFSM.PathMove();
        //if (!ownerFSM.IsMove)

        ownerFSM.CommonFSM.VFAgent.Move();
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.IsMove = false;
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
            return;
        }


        //if (ownerFSM.Scout_Enemy())
        //{
        //    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
        //    return;
        //}

        //if (!ownerFSM.IsMove)
        //{
        //    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        //    return;
        //}

        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        //    return;
        //}

        //ownerFSM.PathMove();
    }

    public void Action_Badger()
    {
        ownerFSM.Badger_Cooling();

        //if (ownerFSM.isFired)
        //{
        //    ownerFSM.TimeCount += Time.deltaTime;

        //    if (ownerFSM.TimeCount >= 1 / ownerFSM.Base.AttackSpeed)
        //    {
        //        ownerFSM.TimeCount = 0f;
        //        ownerFSM.isFired = false;
        //        if (null != ownerFSM.AttackTarget)
        //        {
        //            if (ownerFSM.AttackTarget.activeSelf)
        //            {
        //                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
        //                return;
        //            }
        //            else
        //                ownerFSM.AttackTarget = null;
        //        }
        //    }
        //}

        if (null != ownerFSM.CommandedTarget)
        {
            ownerFSM.AttackTarget = ownerFSM.CommandedTarget;
            ownerFSM.CommandedTarget = null;
        }

        if (null != ownerFSM.AttackTarget)
        {
            //  AttackTarget 이 커맨더라면 
            if (CommonType.Commander == ownerFSM.AttackTarget.Base.Type ||
                !ownerFSM.CheckTargetInRange())
                ownerFSM.AttackTarget = null;
        }

        if (ownerFSM.Scout_Enemy())
        {
            if (ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
            }
        }


        //ownerFSM.PathMove();
        //if (!ownerFSM.IsMove)

        ownerFSM.CommonFSM.VFAgent.Move();
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.IsMove = false;
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
            return;
        }



        //if (!ownerFSM.IsCommandMove)
        //{
        //    if (null == ownerFSM.AttackTarget)
        //    {
        //        if (ownerFSM.Scout_Enemy())
        //        {
        //            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
        //            return;
        //        }
        //    }
        //    else
        //    {
        //        if (ownerFSM.CheckTargetInRange())
        //        {
        //            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
        //            return;
        //        }
        //    }
        //}

        //if (!ownerFSM.IsMove)
        //{
        //    if (null == ownerFSM.AttackTarget)
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        //    else
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //    return;
        //}

        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    if (null == ownerFSM.AttackTarget)
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        //    else
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //    return;
        //}

        //ownerFSM.PathMove();
    }

    public void Action_Wolf()
    {
        if (ownerFSM.isFired)
        {
            ownerFSM.TimeCount += Time.deltaTime;

            if (ownerFSM.TimeCount >= 1 / ownerFSM.Base.AttackSpeed)
            {
                ownerFSM.TimeCount = 0f;
                ownerFSM.isFired = false;
                if (null != ownerFSM.AttackTarget)
                {
                    if (ownerFSM.AttackTarget.activeSelf)
                    {
                        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
                        return;
                    }
                    else
                        ownerFSM.AttackTarget = null;
                }
            }
        }

        if (!ownerFSM.IsCommandMove)
        {
            if (null == ownerFSM.AttackTarget)
            {
                if (ownerFSM.Scout_FriendUnit())
                {
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
                    return;
                }
            }
            else
            {
                if (ownerFSM.CheckTargetInRange())
                {
                    ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
                    return;
                }
            }
        }

        //  이동
        ownerFSM.CommonFSM.VFAgent.Move();

        //  목표가 있음.
        if (null != ownerFSM.AttackTarget)
        {
            //  사거리 안에 없음.
            if (!ownerFSM.CheckTargetInRange())
            {
                //  목표 제거
                ownerFSM.AttackTarget = null;
            }
        }

        //  아군 유닛 탐색
        if (ownerFSM.Scout_FriendUnit())
        {
            //  사거리 안에 있는가?
            if (ownerFSM.CheckTargetInRange())
            {
                ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST_RUN);
                return;
            }
        }

        //  이동을 마침.
        if (!ownerFSM.CommonFSM.VFAgent.IsMove)
        {
            ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
            return;
        }

        //if (ownerFSM.IsArrive())
        //{
        //    ownerFSM.IsMove = false;
        //    if (null == ownerFSM.AttackTarget)
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.IDLE);
        //    else
        //        ownerFSM.ChangeFSM(AdvancedUnitFSM.STATE.CAST);
        //    return;
        //}

        //ownerFSM.PathMove();
    }
}
