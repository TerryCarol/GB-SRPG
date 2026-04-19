using Command;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public class UnitController : MonoBehaviour
{
    private Unit unit;
    private UnitStateController stateController;
    private Animator animator;

    private Pathfinder pathfinder;
    private List<Tile> path;
    private int currentPathIndex = 0;
    private bool isMoving = false;
    [SerializeField] private float smoothArrivalThreshold = 0.01f;

    [SerializeField] private PathVisualizer pathVisualizer;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        stateController = GetComponent<UnitStateController>();
        pathfinder = new Pathfinder(FindObjectOfType<GridManager>());
        animator = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        if (isMoving)
        {
            ProcessMovement();
        }
    }

    // 명령 실행 (CommandInvoker)
    public void ExecuteCommand(ICommand command)
    {
        CommandInvoker.Instance.SetCommand(command);
    }

    // 유닛 이동 (MoveCommand)
    public void MoveTo(Tile targetTile)
    {
        if (unit.HasEnoughActionPoints(1))
        {
            // 경로 탐색
            path = pathfinder.FindPath(unit.currentTile, targetTile);
            if (path == null || path.Count == 0)
            {
                Debug.LogWarning("Error: 경로를 찾을 수 없습니다.");
                return;
            }

            // 경로 시각화
            pathVisualizer?.DrawPath(path);

            // 애니메이터 트리거
            if (animator != null)
            {
                animator.SetBool("IsMoving", true);
            }

            // 이동 시작
            isMoving = true;
            currentPathIndex = 0;
            stateController.SetState("Moving");
            Debug.Log($"{unit.UnitName} is moving to {targetTile.gridPos}");
        }
        else
        {
            Debug.Log($"{unit.UnitName} has insufficient action points.");
        }
    }

    private void ProcessMovement()
    {
        if (path == null || currentPathIndex >= path.Count)
        {
            EndMovement();
            return;
        }

        Tile targetTile = path[currentPathIndex];
        Vector3 targetPosition = targetTile.transform.position + Vector3.up * 0.5f;
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 부드러운 회전
        if (direction != Vector3.zero)
            transform.forward = Vector3.Lerp(transform.forward, direction, 10f * Time.deltaTime);

        // 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, unit.MoveSpeed * Time.deltaTime);

        // 목표 타일에 도달
        if (Vector3.Distance(transform.position, targetPosition) < smoothArrivalThreshold)
        {
            unit.GetCurrentTile().ResetOnTileUnit();
            unit.GetCurrentTile().isOccupied = false;
            unit.SetCurrentTile(targetTile);
            targetTile.SetOnTileUnit(unit);
            currentPathIndex++;

            if (currentPathIndex >= path.Count)
            {
                EndMovement();
            }
        }
    }

    // 이동 종료
    private void EndMovement()
    {
        isMoving = false;
        // 애니메이터 트리거
        if (animator != null)
        {
            animator.SetBool("IsMoving", false);
        }

        stateController.SetState("Idle");
        pathVisualizer?.ClearPath();
        path = null;
        currentPathIndex = 0;
        unit.UseActionPoint(1);
        Debug.Log($"{unit.UnitName} has reached the destination.");
    }

    private void ShowPathDuringMove()
    {
        if (pathVisualizer == null || path == null || path.Count == 0)
            return;

        // 경로의 다음 타일 위치 계산
        if (currentPathIndex < path.Count)
        {
            Tile nextTile = path[currentPathIndex];
            Vector3 from = transform.position + Vector3.up * 0.01f;
            Vector3 to = nextTile.transform.position + Vector3.up * 0.51f;

            pathVisualizer.DrawSegment(from, to);
        }

        // 남은 전체 경로를 PathVisualizer로 전달
        List<Tile> remaining = path.GetRange(currentPathIndex, path.Count - currentPathIndex);
        pathVisualizer.DrawPath(remaining);
    }

    // 유닛 공격 (AttackCommand)
    public void Attack(Unit target)
    {
        if (unit.HasEnoughActionPoints(1))
        {
            // 대상이 존재하고 체력이 0보다 클 때만 공격
            if (target == null || target.Health <= 0)
            {
                Debug.Log("Error: Invalid attack target.");
                return;
            }

            // 공격 대상 방향으로 회전
            Vector3 dir = (target.transform.position - transform.position).normalized;
            if (dir != Vector3.zero)
                transform.forward = dir;

            // 애니메이션 처리
            Animator animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                if (unit.AttackRange <= 1f)
                {
                    animator.SetTrigger("Melee");
                    StartCoroutine(ApplyDelayedDamage(target, 0.4f)); // 근접 공격: 0.4초 지연
                }
                else
                {
                    animator.SetTrigger("Shoot");
                    StartCoroutine(ApplyDelayedDamage(target, 0.2f)); // 원거리 공격: 0.2초 지연
                }
            }
            else
            {
                ApplyDamage(target); // 애니메이션이 없을 경우 즉시 공격
            }

            // 행동력 소모
            unit.UseActionPoint(1);
            Debug.Log($"{unit.UnitName} attacked {target.UnitName}.");
            stateController.SetState("Idle");
        }
        else
        {
            Debug.Log($"{unit.UnitName} has insufficient action points.");
            stateController.SetState("Idle");
        }
    }

    // 딜레이 적용 데미지 함수
    private IEnumerator ApplyDelayedDamage(Unit target, float delay)
    {
        yield return new WaitForSeconds(delay);
        ApplyDamage(target);
    }

    // 즉시 적용 데미지 함수
    private void ApplyDamage(Unit target)
    {
        if (target == null || target.Health <= 0)
            return;

        // 데미지 적용
        target.TakeDamage(unit.AttackPower);
        Debug.Log($"{unit.UnitName} attacked {target.UnitName} for {unit.AttackPower} damage.");
    }

    // 유닛 사망 처리
    public void Die()
    {
        UnitState unitState = unit.CurrentState;
        if (unitState == UnitState.Dying) return; // 이미 죽음 처리된 경우 무시

        unitState = UnitState.Dying;
        stateController.SetState("Death");

        // 랜덤하게 래그돌 또는 애니메이션 사망
        float ragdollChance = 0.9f;
        if (Random.value < ragdollChance)
        {
            TriggerRagdoll();
        }
        else
        {
            if (animator != null)
            {
                animator.applyRootMotion = true;
                animator.SetTrigger("Die");
                float tempDelay = Random.Range(1.5f, 4.383f);
                StartCoroutine(ActivateRagdollAfterDelay(tempDelay));
            }
        }

        // 사망 정보 업데이트
        DeathInfoUpdate();
    }

    private void TriggerRagdoll(Vector3 force = default)
    {
        if (animator != null)
            animator.enabled = false;

        if (unit.RagdollRoot != null)
        {
            unit.RagdollRoot.SetActive(true);
            foreach (var rb in unit.RagdollRoot.GetComponentsInChildren<Rigidbody>())
            {
                rb.isKinematic = false;
                rb.AddForce(force);
            }
        }
    }

    // 지연 후 래그돌 활성화
    private IEnumerator ActivateRagdollAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        TriggerRagdoll();
    }

    // 사망 정보 업데이트
    private void DeathInfoUpdate()
    {
        if (unit.currentTile != null)
        {
            unit.currentTile.isOccupied = false;
            unit.currentTile.ResetOnTileUnit();
        }

        if (!unit.IsCorpse)
        {
            unit.IsCorpse = true;
        }
    }
}

// 구버전
/*
public class UnitController : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject unitPrefab;

    private Unit selectedUnit = null;
    private Unit previousUnit = null;
    private bool initialUnitSpawned = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 플레이어 턴인지 확인
            if (!TurnManager.Instance.IsPlayerTurn())
            {
                Debug.Log("This is not player's turn");
                return;
            }
            // 타일 기반 유닛 선택
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Tile clickedTile = hit.collider.GetComponent<Tile>();
                Unit clickedUnit = hit.collider.GetComponent<Unit>();

                // 캐릭터 매쉬콜라이더 클릭 시 위치타일로 변환
                if (clickedUnit != null)
                {
                    clickedTile = clickedUnit.GetUnitTile();
                }

                // 타일 클릭 시
                if (clickedTile != null)
                {
                    Vector2Int pos = clickedTile.gridPos;
                    //Debug.Log($"Clicked Tile at {pos}");

                    if (clickedTile.isOccupied)
                    {
                        // 타일이 비어있지 않은데 유닛이 선택되어있지 않을 때 (유닛선택)
                        if (selectedUnit == null)
                        {
                            Unit unitOnTile = clickedTile.GetOnTileUnit();
                            if (unitOnTile != null && unitOnTile.Faction == Faction.Player)
                            {
                                selectedUnit = unitOnTile;
                                //Debug.Log($"Selected unit at {pos}");
                            }
                            else
                            {
                                // 적 선택 가능 + 제한된 정보 표시 기능 구현할 것
                                Debug.Log($"Selected unit at {pos} Is {unitOnTile.Faction} Faction.");
                            }
                        }
                        // 타일이 비어있지 않은데 유닛이 선택되어있을 때 (상호작용, 공격 등)
                        else
                        {
                            Unit targetUnit = clickedTile.GetOnTileUnit();

                            if (targetUnit != null && selectedUnit != null)
                            {
                                // 같은 팩션은 공격하지 않게 처리
                                if (targetUnit.Faction != selectedUnit.Faction)
                                {
                                    // Chebyshev 거리로 사거리 내인지 확인
                                    int dx = Mathf.Abs(selectedUnit.currentPos.x - targetUnit.currentPos.x);
                                    int dy = Mathf.Abs(selectedUnit.currentPos.y - targetUnit.currentPos.y);
                                    int dist = Mathf.Max(dx, dy);

                                    if (dist <= selectedUnit.AttackRange && selectedUnit.HasEnoughActionPoints(1))
                                    {
                                        // 커맨드 패턴으로 공격 실행
                                        var attackCommand = new Command.AttackCommand(selectedUnit, targetUnit);
                                        CommandInvoker.Instance.SetCommand(attackCommand);
                                        Debug.Log($"{selectedUnit.UnitName} attacks {targetUnit.UnitName}!");
                                    }
                                    else
                                    {
                                        Debug.Log("공격 불가: 사거리 밖이거나 행동력 부족");
                                    }
                                }
                                else
                                {
                                    Debug.Log("같은 진영 유닛은 공격할 수 없습니다.");
                                }
                            }
                        }
                    }
                    else
                    {
                        //타일이 비어있고 유닛이 선택되있을 때 (이동)
                        if (selectedUnit != null)
                        {
                            selectedUnit.MoveTo(clickedTile);
                            //Debug.Log($"Moved selected unit to {pos}");
                        }
                        //타일이 비어있고 유닛이 선택되있지 않을 때
                        else
                        {
                            if (!initialUnitSpawned)
                            {
                                SpawnInitialPlayerUnit(clickedTile);
                            }
                            else
                            {
                                Debug.Log($"No unit selected.");
                            }
                        }
                    }
                }
                
                // 기타 등등 타일이 아닌 곳 클릭 시
                else
                {
                    DeselectUnit();
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectUnit();
        }

        // 유닛 선택시 초록색 하이라이트
        if (selectedUnit != null)
        {
            selectedUnit.Highlight(Color.blue, true);
            if(previousUnit != selectedUnit)
            {
                previousUnit = selectedUnit;
            }

            if(selectedUnit.MoveRange > 0f && selectedUnit.HasEnoughActionPoints(1) && selectedUnit.CurrentState == UnitState.Idle)
            {
                HighlightMoveableTiles(selectedUnit);
            }
            else
            {
                ReleaseHighlightMoveableTiles(selectedUnit);
            }
        }

        // 유닛 위로 마우스 호버 시 하이라이트
        if (selectedUnit == null)
        {
            HighlightHoveredUnit();
        }

        // 플레이어 턴일 때만 Tab 키로 턴 종료 가능
        if (TurnManager.Instance.IsPlayerTurn() && Input.GetKeyDown(KeyCode.Tab))
        {
            Debug.Log("Tab 키 입력: 플레이어 턴 종료");
            TurnManager.Instance.EndPlayerTurn();
        }
    }

    void SpawnInitialPlayerUnit(Tile clickedTile)
    {
        UnitFactory factory = FindObjectOfType<UnitFactory>();
        if (factory == null)
        {
            Debug.LogError("No UnitFactory found in the scene.");
            return;
        }

        Unit tempUnit = factory.CreateUnit("Player");
        if (tempUnit == null)
        {
            Debug.LogError("Factory returned null. Check unitType.");
            return;
        }

        tempUnit.Place(clickedTile);
        clickedTile.SetOnTileUnit(tempUnit);

        if (tempUnit.Faction != Faction.Player)
        {
            tempUnit.Faction = Faction.Player;
        }
        if(tempUnit.MaxActionPoints >= 0)
        {
            tempUnit.MaxActionPoints = 100;
        }

        Debug.Log($"Spawned player unit via factory: {tempUnit.UnitName}");
        initialUnitSpawned = true;
    }

    void SpawnPlayerUnit(Tile designatedTile, string unitType)
    {
        UnitFactory factory = FindObjectOfType<UnitFactory>();
        if (factory == null)
        {
            Debug.LogError("No UnitFactory found in the scene.");
            return;
        }

        Unit tempUnit = factory.CreateUnit(unitType);
        if (tempUnit == null)
        {
            Debug.LogError("Factory returned null. Check unitType.");
            return;
        }

        tempUnit.Place(designatedTile);
        designatedTile.SetOnTileUnit(tempUnit);

        if (tempUnit.Faction != Faction.Player)
        {
            tempUnit.Faction = Faction.Player;
        }

        Debug.Log($"Spawned player unit via factory: {tempUnit.UnitName}");
        initialUnitSpawned = true;
    }

    private Unit hoveredUnit;
    private Unit previoushoveredUnit;
    void HighlightHoveredUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null)
            {
                if (unit != hoveredUnit)
                {
                    // 이전 유닛 하이라이트 제거
                    if (previoushoveredUnit != null)
                    {
                        previoushoveredUnit.ResetHighlight();
                    }

                    hoveredUnit = unit;
                    hoveredUnit.Highlight(Color.white, true);
                    previoushoveredUnit = hoveredUnit;
                }
            }
            else
            {
                // 마우스가 아무 유닛에도 닿지 않을 경우 하이라이트 제거
                if (previoushoveredUnit != null)
                {
                    previoushoveredUnit.ResetHighlight();
                    previoushoveredUnit = null;
                    hoveredUnit = null;
                }
            }
        }
        else
        {
            // 마우스가 아무것에도 닿지 않을 경우 하이라이트 제거
            if (previoushoveredUnit != null)
            {
                previoushoveredUnit.ResetHighlight();
                previoushoveredUnit = null;
                hoveredUnit = null;
            }
        }
    }

    void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.ResetHighlight();
            ReleaseHighlightMoveableTiles(selectedUnit);
            selectedUnit = null;
            Debug.Log("유닛 선택 해제됨.");
        }
    }

    public TileSelector tileSelector;
    void HighlightMoveableTiles(Unit unit)
    {
        if (unit == null) return;

        List<Tile> movableTiles = unit.GetMovableTiles();
        tileSelector.ShowTiles(movableTiles, Color.cyan);
    }

    void ReleaseHighlightMoveableTiles(Unit unit)
    {
        tileSelector.ClearHighlights();
    }
}
*/

