using System;
using UnityEngine;

public class BaseStateMachine<T> where T : Enum
{
    public T states;

    public BaseStateMachine(T states)
    {
        this.states = states;
    }
}

public enum TestState
{
    None,
    Idle,
    Attack
}