using System.Collections.Generic;
using System.IO;
using UnityEngine;
public enum UnitState
{
    // State 패턴까지 확장해볼 것
    Idle,
    Moving,
    Attacking,
    Dying
}

public enum Faction
{
    Player,
    Enemy,
    Ally,
    Neutral
}

public class Unit : MonoBehaviour
{
    [SerializeField] private bool aiControl = false;
    [SerializeField] private Faction faction;
    [SerializeField] private Gender gender;
    [SerializeField] private string unitType;
    [SerializeField] private string unitName;
    [SerializeField] private float health = 100f;
    [SerializeField] private bool isCorpse = false;
  
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveRange = 6f;
    [SerializeField] private UnitState currentState;

    [SerializeField] private GameObject ragdollRoot;
    public GameObject RagdollRoot => ragdollRoot;

    [SerializeField] private GameObject highlightRing;
    private PathVisualizer pathVisualizer;
    

    public Vector2Int currentPos;
    public Tile currentTile;
    public int MaxActionPoints = 2;
    public int currentActionPoints;
    public bool IsSkippingTurn = false;

    private UnitStateController stateController;
    private UnitController actController;
    private Pathfinder pathfinder;

    private void Awake()
    {
        ResetActionPoints();
        stateController = GetComponent<UnitStateController>();
        actController = GetComponent<UnitController>();
        pathfinder = new Pathfinder(FindObjectOfType<GridManager>());
        pathVisualizer = GetComponentInChildren<PathVisualizer>();
        ResetHighlight();
    }

    // 게터 세터
    public UnitState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }
    public bool IsCorpse 
    {
        get => isCorpse;
        set => isCorpse = value; 
    }
    public bool AIControl
    {
        get => aiControl;
        set => aiControl = value;
    }
    public Faction Faction
    {
        get => faction;
        set => faction = value;
    }
    public Gender Gender
    {
        get => gender;
        set => gender = value;
    }
    public string UnitType
    {
        get => unitType;
        set => unitType = value;
    }
    public string UnitName
    {
        get => unitName;
        set => unitName = value;
    }

    public float Health
    {
        get => health;
        set => health = value;
    }

    public float AttackPower
    {
        get => attackPower;
        set => attackPower = value;
    }

    public float AttackRange
    {
        get => attackRange;
        set => attackRange = value;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public float MoveRange
    {
        get => moveRange;
        set => moveRange = value;
    }

    // 행동 포인트 관리
    public void ResetActionPoints() => currentActionPoints = MaxActionPoints;

    public bool UseActionPoint(int amount)
    {
        if (currentActionPoints >= amount)
        {
            currentActionPoints -= amount;
            return true;
        }
        return false;
    }

    public bool HasEnoughActionPoints(int amount) => currentActionPoints >= amount;

    // 데미지 적용 (피격 시)
    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log($"{UnitName} took {damage} damage. Remaining health: {health}");

        if (health <= 0)
        {
            TriggerDeath();
        }
    }

    private void TriggerDeath()
    {
        var stateController = GetComponent<UnitStateController>();
        if (stateController != null)
        {
            stateController.SetState("Death");
        }
        var controller = GetComponent<UnitController>();
        if (controller != null)
        {
            controller.Die();
        }
        CurrentState = UnitState.Dying;
    }

    public void SetCurrentTile(Tile tile)
    {
        currentTile = tile;
        currentPos = tile.gridPos;
        tile.isOccupied = true;
    }

    public Tile GetCurrentTile()
    {
        if (currentTile == null)
        {
            Debug.Log("Error: Unit's currentTile is set to Null");
        }
        return currentTile;
    }

    public List<Tile> GetMovableTiles(Tile origin = null)
    {
        Tile start = origin ?? currentTile;
        if (start == null)
        {
            Debug.Log("Error: GetMovableTiles NullReference occured");
            return null;
        }
        else
        {
            Pathfinder pathfinder = new Pathfinder(FindObjectOfType<GridManager>());
            return pathfinder.FindReachableTiles(start, moveRange);
        }
    }

    public void SetFaction(Faction faction)
    {
        this.faction = faction;
        var facitonData = FactionManager.GetData(this.faction);
        if(facitonData != null)
        {
            //팩션 관련 모디파이어 돌아갈 함수 자리
        }
    }

    public void Highlight(Color color, bool isOn)
    {
        if (highlightRing != null)
        {
            highlightRing.GetComponent<Renderer>().material.color = color;
            highlightRing.SetActive(isOn);
        }
    }

    public void ResetHighlight()
    {
        if (highlightRing != null)
        {
            highlightRing.SetActive(false);
        }
    }

    public void ShowPath(List<Tile> path)
    {
        if (pathVisualizer != null)
            pathVisualizer.DrawPath(path);
    }

    public void ClearPath()
    {
        if (pathVisualizer != null)
            pathVisualizer.ClearPath();
    }
}

// 유닛 자체에 명령 수행 함수가 있던 구버전
/*
public class Unit : MonoBehaviour
{
    public Tile currentTile;
    public Renderer meshRenderer;
    public Vector2Int currentPos;

    private Animator animator;

    [SerializeField] private GameObject ragdollRoot;
    public GameObject RagdollRoot => ragdollRoot;
    public bool IsCorpse { get; set; }
    public UnitState CurrentState
    {
        get => currentState;
        set => currentState = value;
    }


    [SerializeField] private bool aiControl = false;
    [SerializeField] private Faction faction;
    [SerializeField] private Gender gender;
    [SerializeField] private string unitType;
    [SerializeField] private string unitName;
    [SerializeField] private float health = 100f;
    [SerializeField] private float attackPower = 10f;
    [SerializeField] private float attackRange = 1f;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveRange = 6f;

    // 게터 세터
    public bool AIControl
    {
        get => aiControl;
        set => aiControl = value;
    }
    public Faction Faction
    {
        get => faction;
        set => faction = value;
    }
    public Gender Gender
    {
        get => gender;
        set => gender = value;
    }
    public string UnitType
    {
        get => unitType;
        set => unitType = value;
    }
    public string UnitName
    {
        get => unitName;
        set => unitName = value;
    }

    public float Health
    {
        get => health;
        set => health = value;
    }

    public float AttackPower
    {
        get => attackPower;
        set => attackPower = value;
    }

    public float AttackRange
    {
        get => attackRange;
        set => attackRange = value;
    }

    public float MoveSpeed
    {
        get => moveSpeed;
        set => moveSpeed = value;
    }

    public float MoveRange
    {
        get => moveRange;
        set => moveRange = value;
    }

    private UnitStateController stateController;
    private UnitState currentState = UnitState.Idle;    // 유닛 STATE

    public PathVisualizer pathVisualizer;
    private Pathfinder pathfinder;                      // 길찾기 스크립트
    private List<Tile> path = new List<Tile>();         // 길찾기 리스트
    private int currentPathIndex = 0;                   // 길찾기 인덱스
    private Vector3 lastPosition;
    private Color originalColor;

    void Awake()
    {
        stateController = GetComponent<UnitStateController>();
        pathVisualizer = GetComponentInChildren<PathVisualizer>();
        ResetActionPoints();

        // 시작할 때 원래 머티리얼 색상 저장
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<Renderer>();
        }
        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
        }
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        Debug.Log($"애니메이터: {animator}, 컨트롤러: {animator.runtimeAnimatorController}");

        if (pathfinder == null)
        {
            GridManager gm = FindObjectOfType<GridManager>();
            pathfinder = new Pathfinder(gm);
            if (pathfinder == null)
            {
                Debug.LogError("Error : Pathfinder Not found");
            }
        }

        lastPosition = transform.position;

        if (pathVisualizer != null)
        {
            var factionData = FactionManager.GetData(this.faction);
            if (factionData != null)
            {
                pathVisualizer.SetColors(factionData.FactionColor);
            }
        }
        //highlightRing.GetComponent<Renderer>().material.color = this.Faction.color;

        IsCorpse = false;
    }

    [SerializeField] private float smoothArrivalThreshold = 0.01f;  // 도달 판정 거리
    // 매 프레임 경로따라 유닛 실제 이동
    void Update()
    {
        if (path.Count > 0 && currentPathIndex < path.Count)
        {
            //ShowPath(path);
            Tile targetTile = path[currentPathIndex];

            // 이동 중 경로 타일이 막히면 즉시 중단
            if (targetTile != null && (!targetTile.isWalkable || (targetTile.isOccupied && targetTile.GetOnTileUnit() != this)))
            {
                Debug.LogWarning($"{UnitName} 이동 중 경로 소실. 이동 중단.");
                EndMovement();
                return;
            }

            Vector3 targetPosition = targetTile.transform.position + Vector3.up * 0.5f;  // 타일의 위치로 이동

            // 타일로 서서히 이동
            currentState = UnitState.Moving;
            Vector3 direction = (targetPosition - transform.position).normalized;
            if (direction != Vector3.zero)
            {
                transform.forward = Vector3.Lerp(transform.forward, direction, 10f * Time.deltaTime);  // 부드러운 회전
            }

            // 구 이동 방법 (MoveToward)
            //transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

            // 현재 이동 방법 (Lerp 적용)
            float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
            bool isFinalTile = currentPathIndex == path.Count - 1;
            float slowDownThreshold = 0.5f; // 감속 거리 임계값 설정
            float speedFactor = isFinalTile ? Mathf.Clamp01(distanceToTarget / slowDownThreshold) : 1f; // 마지막 타일이면 감속
            float adjustedSpeed = moveSpeed * speedFactor;

            transform.position = Vector3.MoveTowards(transform.position, targetPosition, adjustedSpeed * Time.deltaTime);

            // 타일에 도달, 해당 타일을 현재 타일로 설정
            if (distanceToTarget < smoothArrivalThreshold)
            {
                Place(targetTile);
                currentPathIndex++;

                if (currentPathIndex >= path.Count)
                {
                    EndMovement();
                }
            }
        }
        else if (path.Count <= 0 && currentState == UnitState.Moving)
        {
            EndMovement();
        }

        // 오브젝트 실제 속도 계산 후 애니메이터에 전송
        float actualSpeed = ((transform.position - lastPosition).magnitude) / Time.deltaTime;
        animator.SetFloat("Speed", actualSpeed, 0.2f, Time.deltaTime);

        lastPosition = transform.position;

        if (CurrentState == UnitState.Moving)
        {
            ShowPathDuringMove();
        }
    }

    private void EndMovement()
    {
        currentState = UnitState.Idle;
        this.pathVisualizer.ClearPath();
        path.Clear();
        currentPathIndex = 0;

        if (stateController != null)
        {
            stateController.IsBusy = false;
            stateController.ChangeState(new UnitIdleState());
        }
    }

    // 유닛의 타일(위치)정보 업데이트
    public void Place(Tile tile)
    {
        if (currentTile != null)
        {
            currentTile.ResetOnTileUnit();
            currentTile.isOccupied = false;
        }

        currentTile = tile;
        currentPos = tile.gridPos; // 현재 타일 위치를 설정
        transform.position = tile.transform.position + Vector3.up * 0.5f;   // 타일 위에 살짝 띄우기
        tile.isOccupied = true;
        tile.SetOnTileUnit(this);

        if (pathfinder == null)
        {
            GridManager gm = FindObjectOfType<GridManager>();
            pathfinder = new Pathfinder(gm);
        }
    }

    // 유닛 이동할 경로 계산 요청
    public void MoveTo(Tile tile)
    {
        if (currentState != UnitState.Idle)
        {
            Debug.LogWarning("Error: 이미 이동중입니다. 이동완료 후 다시 명령하세요.");
            return;                                                         // 완료 전 또 다른 이동명령 무시
        }

        if (!HasEnoughActionPoints(1))
        {
            Debug.Log($"{unitName}은 행동력이 부족해서 이동할 수 없음.");
            return;
        }

        if (!tile.isOccupied && tile.isWalkable)
        {
            List<Tile> testPath = pathfinder.FindPath(currentTile, tile);
            int testPathLength = testPath.Count - 1;

            if (testPath == null || testPath.Count == 0)                            
            {
                Debug.LogWarning("Error: 경로를 찾을 수 없습니다.");        // 길찾기 오류 예외처리
                return;
            }
            else if (testPathLength > moveRange)                            // 유닛스탯 이동력 확인
            {
                Debug.LogWarning($"Error: 유닛 이동 거리 초과. ({testPathLength} > {moveRange})");
                return;
            }

            if (!UseActionPoint(1))
            {
                Debug.Log($"{unitName}은 행동력을 소모하지 못했음.");
                return;
            }

            path = testPath;                                                // 유닛 이동 Update에 입력
            currentPathIndex = 0;
            currentState = UnitState.Moving;                                // 이동 상태로 전환
        }
    }

    public GameObject highlightRing;
    public void Highlight(Color color, bool isOn)
    {
        if (highlightRing != null)
        {
            highlightRing.GetComponent<Renderer>().material.color = color;
            highlightRing.SetActive(isOn);
        }
        else
        {
            return;
        }
    }

    public void ResetHighlight()
    {
        if (highlightRing != null)
        {
            highlightRing.SetActive(false);
        }
        else
        {
            return;
        }

    }

    public Tile GetUnitTile()
    {
        if (currentTile == null)
        {
            Debug.Log("Error: Unit's currentTile is set to Null");
        }
        return currentTile;
    }

    public List<Tile> GetMovableTiles(Tile origin = null)
    {
        Tile start = origin ?? currentTile;
        if (start == null)
        {
            Debug.Log("Error: GetMovableTiles NullReference occured");
            return null;
        }
        else
        {
            return pathfinder.FindReachableTiles(start, moveRange);
        }
    }

    [SerializeField] private int maxActionPoints = 2;
    [SerializeField] private int currentActionPoints;

    public int MaxActionPoints
    {
        get => maxActionPoints;
        set => maxActionPoints = value;
    }
    public int ActionPoints => currentActionPoints;

    public void ResetActionPoints()
    {
        currentActionPoints = maxActionPoints;
    }

    public bool UseActionPoint(int amount)
    {
        if (currentActionPoints >= amount)
        {
            currentActionPoints -= amount;
            return true;
        }
        return false;
    }

    public bool HasEnoughActionPoints(int amount)
    {
        return currentActionPoints >= amount;
    }

    public void ShowPath(List<Tile> fullPath)
    {
        if (pathVisualizer != null && fullPath != null && fullPath.Count > currentPathIndex)
        {
            List<Tile> remainingPath = fullPath.GetRange(currentPathIndex, fullPath.Count - currentPathIndex);
            pathVisualizer.DrawPath(remainingPath);
        }
    }

    public void ClearPath()
    {
        if (pathVisualizer != null)
            pathVisualizer.ClearPath();
    }

    private void ShowPathDuringMove()
    {
        if (pathVisualizer == null || currentPathIndex >= path.Count)
            return;

        // 1. 현재 유닛 <=> 다음 타일
        Tile nextTile = path[currentPathIndex];
        Vector3 from = transform.position + Vector3.up * 0.01f;
        Vector3 to = nextTile.transform.position + Vector3.up * 0.51f;
        pathVisualizer.DrawSegment(from, to);

        // 2. 남은 전체 경로
        List<Tile> remaining = path.GetRange(currentPathIndex, path.Count - currentPathIndex);
        pathVisualizer.DrawPath(remaining);
    }
}
*/
