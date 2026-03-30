using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{
    public static AIManager Instance;
    private List<Unit> aiUnits = new List<Unit>();
    private int currentAIUnitIndex = 0;

    private void Awake() => Instance = this;

    // AI 턴 시작 (AIManager가 AI 유닛들을 순차적으로 처리)
    public void StartAITurn()
    {
        aiUnits.Clear();
        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            if (unit.Faction == Faction.Enemy && unit.Health > 0)
                aiUnits.Add(unit);
        }

        currentAIUnitIndex = 0;
        StartCoroutine(ProcessAITurn());
    }

    // AI 턴 처리 (유닛 하나씩 순차적으로)
    private IEnumerator ProcessAITurn()
    {
        while (currentAIUnitIndex < aiUnits.Count)
        {
            Unit currentUnit = aiUnits[currentAIUnitIndex];
            var stateController = currentUnit.GetComponent<UnitStateController>();

            if (stateController == null || currentUnit.Health <= 0)
            {
                currentAIUnitIndex++;
                continue;
            }

            while (currentUnit.HasEnoughActionPoints(1) && !currentUnit.IsSkippingTurn)
            {
                // AI 유닛 행동 결정
                DecideAndExecuteAction(currentUnit);

                // 현재 유닛의 상태가 Idle로 돌아올 때까지 대기
                yield return new WaitUntil(() => stateController.CurrentState is UnitIdleState);
            }
            currentUnit.IsSkippingTurn = false;
            currentAIUnitIndex++;
        }

        // 모든 AI 유닛이 행동을 완료하면 턴 종료
        TurnManager.Instance.EndEnemyTurn();
    }

    // AI 행동 결정 및 명령 생성
    private void DecideAndExecuteAction(Unit unit)
    {
        var stateController = unit.GetComponent<UnitStateController>();
        if (stateController == null) return;

        // 1. 가장 가까운 적 탐색
        Unit closestEnemy = FindClosestEnemy(unit);
        if (closestEnemy == null)
        {
            unit.IsSkippingTurn = true;
            stateController.SetState("Idle");
            return;
        }

        // 2. 공격 사거리 체크
        int dx = Mathf.Abs(unit.currentPos.x - closestEnemy.currentPos.x);
        int dy = Mathf.Abs(unit.currentPos.y - closestEnemy.currentPos.y);
        int distance = Mathf.Max(dx, dy);

        // 3. 사거리 내면 공격 상태로 전환
        if (distance <= unit.AttackRange && unit.HasEnoughActionPoints(1))
        {
            //unit.UseActionPoint(1);
            stateController.SetState("Attack", closestEnemy);

            //unit.IsSkippingTurn = true;
            return;
        }
        else
        {
            // 4. 이동 가능한 타일 계산
            List<Tile> movableTiles = unit.GetMovableTiles();
            Tile targetTile = FindClosestTileTowards(closestEnemy.currentTile, movableTiles);

            if (targetTile != null)
            {
                stateController.SetState("Move", targetTile);
            }
            else
            {
                Debug.Log("Can't find route to enemy unit.");
                unit.IsSkippingTurn = true;
                stateController.SetState("Idle");
            }
        }
    }

    // 가장 가까운 적 찾기
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

    // 가장 가까운 타일 찾기
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

/*
public class AIManager : MonoBehaviour
{
    private List<UnitStateController> aiUnits = new List<UnitStateController>();
    private int currentIndex = 0;
    private bool isAITurn = false;

    void Awake()
    {
        aiUnits = new List<UnitStateController>();
    }
    void Start()
    {
        //aiUnits.AddRange(FindObjectsOfType<UnitStateController>());
    }

    public void StartAITurn()
    {
        aiUnits.Clear();
        aiUnits.AddRange(FindObjectsOfType<UnitStateController>());

        // AI통제 유닛들 AP 초기화
        foreach (var unitController in aiUnits)
        {
            var unit = unitController.GetUnitData();
            if ((unit.Faction == Faction.Enemy || unit.AIControl) && unit.Health > 0f)
            {
                unit.ResetActionPoints();
            }
        }

        if (aiUnits.Count == 0)
        {
            Debug.LogWarning("AIManager: No AI units found this turn.");
            isAITurn = false;
            TurnManager.Instance.EndEnemyTurn();
            return;
        }

        currentIndex = 0;
        isAITurn = true;
    }

    void Update()
    {
        if (!isAITurn) return;

        if (currentIndex < aiUnits.Count)
        {
            UnitStateController unitController = aiUnits[currentIndex];

            if (unitController == null || unitController.GetUnitData() == null)
            {
                currentIndex++;
                return;
            }

            var unit = unitController.GetUnitData();

            // Enemy이거나 AIControl 설정된 유닛만 실행
            if ((unit.Faction == Faction.Enemy || unit.AIControl) && unit.Health > 0f)
            {
                // 아직 동작 중이면 대기
                if (!unitController.IsBusy)
                {
                    if (unit.ActionPoints > 0)
                    {
                        unitController.ExecuteTurn(); // 상태 진입 후 busy
                        return;
                    }
                    else
                    {
                        currentIndex++;               // 다음 유닛으로 넘어감
                        return;
                    }
                }
                return;
            }
            currentIndex++;
        }
        else
        {
            Debug.Log("AI 턴 종료");
            isAITurn = false;
            TurnManager.Instance.EndEnemyTurn();
        }
    }
}
*/
