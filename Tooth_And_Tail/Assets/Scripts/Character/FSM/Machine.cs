using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Machine<T>
{
    private T owner;

    private FSM<T> curState = null;
    private FSM<T> preState = null;

    public void Begin()
    {
        if (null != curState)
            curState.Begin();
    }

    public void Run()
    {
        if (null != curState)
            curState.Run();
    }

    public void Exit()
    {
        if (null != curState)
            curState.Exit();
        curState = null;
        preState = null;
    }

    public void Change(FSM<T> state)
    {
        if (state == curState)
            return;

        preState = curState;

        if (null != curState)
            curState.Exit();

        curState = state;

        if (null != curState)
            curState.Begin();
    }

    public void ResetState(FSM<T> state)
    {
        preState = curState;

        if (null != curState)
            curState.Exit();

        curState = state;

        if (null != curState)
            curState.Begin();
    }

    public void Revert()
    {
        if (null != preState)
            Change(preState);
    }

    public void SetState(FSM<T> state, T newOwner)
    {
        owner = newOwner;
        curState = state;

        if (state != curState && null != curState)
            preState = curState;
    }
}
