using Command;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

/*
public class UnitIdleState : IUnitState
{
    private UnitStateController controller;
    private List<Tile> movableTiles;
    public void Enter(Unit unit)
    {
        // 상태 진입 시
        //Debug.Log($"{unit.UnitName} entered Idle State");

        controller = unit.GetComponent<UnitStateController>();
        controller.IsBusy = false;

        Tile originTile = unit.GetUnitTile();
        if(unit.GetMovableTiles(originTile) != null)
        {
            movableTiles = unit.GetMovableTiles(originTile);
        }
        else
        {
            // 오류
            EndTurn();
            return;
        }
    }

    public void Execute(Unit unit)
    {
        // 가장 가까운 적 찾기
        Unit closestEnemy = FindClosestEnemy(unit);
        if (closestEnemy == null)
        {
            EndTurn();
            return;
        }

        // Chebyshev 거리 (대각선도 1타일 취급)
        float distanceToEnemy = Mathf.Max(
            Mathf.Abs(unit.currentPos.x - closestEnemy.currentPos.x),
            Mathf.Abs(unit.currentPos.y - closestEnemy.currentPos.y)
        );

        // 공격 사거리 이내 적 발견 -> AttackState 전이
        if (distanceToEnemy <= unit.AttackRange)
        {
            controller.ChangeState(new UnitAttackState(closestEnemy));
            return;
        }

        // 처음 위치 기준으로 이동 가능한 타일 없으면 턴 종료
        if (movableTiles == null || movableTiles.Count == 0)
        {
            EndTurn();
            return;
        }

        // 적 근처로 이동할 최적 타일 찾기
        Tile targetTile = FindClosestTileTowards(closestEnemy.GetUnitTile(), movableTiles);

        if (targetTile != null)
        {
            if (controller != null)
            {
                controller.ChangeState(new UnitMoveState(targetTile));
            }
            else
            {
                EndTurn();
            }
        }
        else
        {
            EndTurn();
        }
        // UnitApproachState로 분리 필요?
        // UnitPatrolState 필요
    }

    public void Exit(Unit unit)
    {
        unit.GetComponent<UnitStateController>().IsBusy = false;
    }

    private void EndTurn()
    {
        if (controller != null)
            controller.IsBusy = false; // 턴 종료 신호
    }

    private Unit FindClosestEnemy(Unit self)
    {
        Unit[] allUnits = GameObject.FindObjectsOfType<Unit>();
        Unit closestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var other in allUnits)
        {
            if (other == self) continue;
            if (other.Health <= 0f) continue;
            if (other.Faction == self.Faction) continue;

            float dist = Vector2Int.Distance(self.currentPos, other.currentPos);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestEnemy = other;
            }
        }

        return closestEnemy;
    }
    private Tile FindClosestTileTowards(Tile target, List<Tile> tiles)
    {
        Tile closestTile = null;
        float closestDistance = float.MaxValue;

        foreach (var tile in tiles)
        {
            float dist = Vector2Int.Distance(tile.gridPos, target.gridPos);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closestTile = tile;
            }
        }

        return closestTile;
    }
}
*/

public class UnitIdleState : IUnitState
{
    public void Enter(Unit unit)
    {
        Debug.Log($"{unit.UnitName} : Idle STATE.");
    }

    public void Execute(Unit unit)
    {
        // AIManager나 InputManager로부터 아무것도 명령받지 않은 기본 상태
    }

    public void Exit(Unit unit)
    {
        Debug.Log($"{unit.UnitName} : Exiting Idle STATE");
    }

    public void HandleInput(Unit unit, Tile targetTile)
    {
        //공격
        if (targetTile.isOccupied)
        {
            Unit target = targetTile.GetOnTileUnit();
            if (target != null && target.Faction != unit.Faction)
            {
                unit.GetComponent<UnitStateController>().SetState("Attack", target);
                return;
            }
            else
            {
                Debug.Log("Target tile is BRUH!");
                return;
            }
        }

        //이동
        List<Tile> movableTiles = unit.GetMovableTiles();
        if (movableTiles.Contains(targetTile))
        {
            unit.GetComponent<UnitStateController>().SetState("Move", targetTile);
        }
        else
        {
            Debug.Log("Target tile is out of range!");
        }
    }
}
