using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitStateController : MonoBehaviour
{
    [SerializeField] private IUnitState currentState;
    public IUnitState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }
    // AI와 인풋매니저 호출을 위한 게터. 세터는 디버그 이외에 사용 지양할 것.

    private Unit unit;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        if (unit == null)
        {
            Debug.LogError("UnitStateController Error: Unit component not found.");
        }

        SetState("Idle");   // 기본 상태 설정
    }

    // 기본 상태 전환 (문자열 이름)
    public void SetState(string stateType)
    {
        ChangeState(StatePool.GetState(stateType, this));
    }

    // 대상이 필요한 상태 전환 (Target)
    public void SetState(string stateType, Unit target)
    {
        IUnitState newState = StatePool.GetState(stateType, this);

        if (newState is UnitAttackState attackState)
        {
            attackState.SetTarget(target);
        }

        ChangeState(newState);
    }

    // 타일 대상이 필요한 상태 전환 (TargetTile)
    public void SetState(string stateType, Tile targetTile)
    {
        IUnitState newState = StatePool.GetState(stateType, this);

        if (newState is UnitMoveState moveState)
        {
            moveState.SetTargetTile(targetTile);
        }

        ChangeState(newState);
    }

    // 실제 상태 변경 처리
    private void ChangeState(IUnitState newState)
    {
        if (currentState != null)
        {
            currentState.Exit(unit);
            StatePool.ReturnState(currentState.GetType().Name, currentState);
        }

        currentState = newState;
        //UnitStateSync(unit);
        currentState?.Enter(unit);
    }

    private void UnitStateSync(Unit unit)
    {
        Unit currentUnit = unit;
        string stateType = currentState.GetType().Name;
        if (stateType == "Death")
        {
            currentUnit.CurrentState = UnitState.Dying;
        }
        else if (stateType == "Idle")
        {
            currentUnit.CurrentState = UnitState.Idle;
        }
        else if (stateType == "Attack")
        {
            currentUnit.CurrentState = UnitState.Attacking;
        }
        else if (stateType == "Move")
        {
            currentUnit.CurrentState = UnitState.Moving;
        }
        else
        {
            currentUnit.CurrentState = UnitState.Idle;
        }
    }

    public Unit GetUnitData()
    {
        return unit;
    }

    public void ExecuteTurn()
    {
        currentState?.Execute(unit);
    }
    // 턴제 진행인데 턴 매니저에서 해야하나?
}

