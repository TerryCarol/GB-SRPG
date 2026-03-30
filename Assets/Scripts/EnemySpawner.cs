using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public UnitFactory unitFactory;
    public float spawnInterval = 0.1f;
    public bool spawnerIsOn = true;
    public float nextSpawnTime = 1f;

    public int spawnerUnitLimit = 10;
    private int spawnedUnitCount = 0;
    private GridManager gridManager;

    void Start()
    {
        gridManager = FindObjectOfType<GridManager>();  // GridManager를 찾음
        nextSpawnTime = Time.time + spawnInterval;
    }

    private void Update()
    {
        if (spawnerIsOn && Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            nextSpawnTime = Time.time + spawnInterval;
        }
    }

    void SpawnEnemy()
    {
        while(spawnedUnitCount < spawnerUnitLimit)
        {
            // 랜덤하게 유닛을 생성
            string unitType = GetRandomUnitType();
            Debug.Log($"Trying to spawn {unitType}");

            Unit unit = unitFactory.CreateUnit(unitType);
            unit.AIControl = true;

            if (unit != null)
            {
                // 1. 양끝단 스폰 포인트 중 하나를 선택
                //Vector2Int[] spawnPoints = gridManager.GetSpawnPoints();
                //Vector2Int spawnPosition2D = spawnPoints[Random.Range(0, spawnPoints.Length)];

                // 2. 그리드맵 전역 완전 랜덤 선택
                List<Tile> validTiles = gridManager.GetAllFreeWalkableTiles();

                // 2. 방식 스폰
                if (validTiles.Count > 0)
                {
                    Tile tile = validTiles[Random.Range(0, validTiles.Count)];
                    Vector3 spawnPosition = tile.transform.position + Vector3.up * 1.1f;

                    unit.transform.position = spawnPosition;
                    unit.SetCurrentTile(tile);

                    Debug.Log($"{unitType} spawned at {tile.gridPos}");

                    var controller = unit.GetComponent<UnitStateController>();
                    if (controller != null)
                    {
                        controller.SetState("Idle");
                    }

                    spawnedUnitCount++;
                }
                else
                {
                    Destroy(unit.gameObject);
                    Debug.LogWarning("No valid tiles to spawn the unit.");
                    spawnerIsOn = false;
                }

                // 1. 방식 스폰 (비활성화)
                /*
                Tile tile = gridManager.GetTileAtPosition(spawnPosition2D);

                if (tile != null && tile.isWalkable && !tile.isOccupied && spawnPoints.Length > 0)
                {
                    // 스폰 포인트를 Vector2Int에서 Vector3로 변환
                    Vector3 spawnPosition = new Vector3(spawnPosition2D.x * gridManager.gridSize, 1.1f, spawnPosition2D.y * gridManager.gridSize);

                    // 유닛을 해당 위치로 스폰
                    unit.transform.position = spawnPosition;
                    unit.Place(tile);
                    Debug.Log($"{unitType} spawned at {spawnPosition2D}");

                    var controller = unit.GetComponent<UnitStateController>();
                    if (controller != null)
                    {
                        controller.ChangeState(new UnitIdleState());
                    }
                    spawnedUnitCount++;

                    return;
                }
                else
                {
                    Destroy(unit.gameObject);

                    if (tile.isOccupied)
                    {
                        Debug.Log($"{unitType} cannot be spawned. Spawnpoint is already occupied.");
                    }
                    else if (!tile.isWalkable)
                    {
                        Debug.Log($"{unitType} cannot be spawned. Spawnpoint is not passable terrain.");
                    }

                    // 스폰포인트 개수의 횟수동안 스폰 실패 시 스포너 작동 정지
                    if(spawnedUnitCount >= 2)
                    {
                        spawnerIsOn = false;
                        Debug.Log("Turning off spawner.");
                    }
                }
                */
            }
            else
            {
                Debug.LogError("Error: Failed to spawn unit.");
            }
        }
    }

    string GetRandomUnitType()
    {
        // 스테이지마다 다르게 유닛을 선택하도록 설정
        // 예시로 랜덤 유닛 선택
        string[] unitTypes = { "Gunner", "Bruiser" };
        return unitTypes[Random.Range(0, unitTypes.Length)];
    }
}
