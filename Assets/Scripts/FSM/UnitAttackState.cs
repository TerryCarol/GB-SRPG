using UnityEngine;
using Command;

public class UnitAttackState : IUnitState
{
    private Unit target;

    public void SetTarget(Unit target)
    {
        this.target = target;
    }

    public void Enter(Unit unit)
    {
        Debug.Log($"{unit.UnitName} entered Attack State");

        var controller = unit.GetComponent<UnitStateController>();

        if (target != null)
        {
            ICommand attackCommand = new AttackCommand(unit, target);
            CommandInvoker.Instance.SetCommand(attackCommand); // 큐에 추가
        }
        else
        {
            Debug.Log($"{unit.UnitName} has no target to attack.");
            controller.SetState("Idle");
        }
    }

    public void Execute(Unit unit)
    {
        // 공격 중 지속적인 로직?
    }

    public void Exit(Unit unit)
    {
        target = null; // 상태 변경 시 타겟 해제
        CommandInvoker.Instance.ClearCommand();
    }
}

