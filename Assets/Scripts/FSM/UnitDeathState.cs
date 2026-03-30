using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Command;
using UnityEditor.Experimental.GraphView;

public class UnitDeathState : IUnitState
{
    private Transform attacker;

    public UnitDeathState(Transform attacker = null)
    {
        this.attacker = attacker;
    }

    public void Enter(Unit unit)
    {
        ICommand deathCommand = new DeathCommand(unit);
        CommandInvoker.Instance.SetCommand(deathCommand);
    }

    public void Execute(Unit unit)
    {
        // 죽어있음 (아무것도 안하죠?)
    }

    public void Exit(Unit unit)
    {
        CommandInvoker.Instance.ClearCommand();
    }
}

