using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatePool
{
    private static Dictionary<string, Queue<IUnitState>> pool = new Dictionary<string, Queue<IUnitState>>();

    // 상태 기본값 가져오기 (대상 없는 상태)
    public static IUnitState GetState(string stateType, UnitStateController controller)
    {
        return GetState(stateType, controller, null, null);
    }

    // 상태 기본값 가져오기 (타겟 유닛 또는 타일 지정)
    public static IUnitState GetState(string stateType, UnitStateController controller, Unit targetUnit = null, Tile targetTile = null)
    {
        if (!pool.ContainsKey(stateType))
            pool[stateType] = new Queue<IUnitState>();

        IUnitState state = null;

        if (pool[stateType].Count > 0)
        {
            state = pool[stateType].Dequeue();
        }
        else
        {
            // 풀에 상태가 없으면 새로 생성
            state = stateType switch
            {
                "Idle" => new UnitIdleState(),
                "Attack" => new UnitAttackState(),
                "Move" => new UnitMoveState(),
                "Death" => new UnitDeathState(),
                _ => null
            };
        }

        // 상태가 생성되거나 풀에서 가져온 상태가 설정될 경우 대상 정보 지정
        if (state is UnitAttackState attackState)
        {
            attackState.SetTarget(targetUnit);
        }
        else if (state is UnitMoveState moveState)
        {
            moveState.SetTargetTile(targetTile);
        }

        return state;
    }

    // 풀에 저장된 상태 반환
    public static void ReturnState(string stateType, IUnitState state)
    {
        if (!pool.ContainsKey(stateType))
            pool[stateType] = new Queue<IUnitState>();

        // 상태 초기화 (대상 정보 제거)
        if (state is UnitAttackState attackState)
            attackState.SetTarget(null);
        else if (state is UnitMoveState moveState)
            moveState.SetTargetTile(null);

        pool[stateType].Enqueue(state);
    }
}
