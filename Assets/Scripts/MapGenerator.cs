using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public static class MapGenerator
{
    public static GameObject wallPrefab;
    public static GameObject obstaclePrefab;
    private static Transform mapObjectParent;

    public static float wallDensity = 0.1f;
    public static float obstacleDensity = 0.1f;

    private static GridManager gridManager;

    public static void Init(GameObject wall, GameObject obstacle, GridManager manager)
    {
        wallPrefab = wall;
        obstaclePrefab = obstacle;
        gridManager = manager;

        var container = GameObject.Find("MapObjects");
        if (container == null)
        {
            container = new GameObject("MapObjects");
        }
        mapObjectParent = container.transform;
    }

    public static void Generate(Dictionary<Vector2Int, Tile> tiles)
    {
        ClearObstacles(tiles);

        Vector2Int[] spawnPoints = gridManager.GetSpawnPoints();
        Vector2Int start = spawnPoints[0];
        Vector2Int goal = spawnPoints[1];

        Pathfinder pathfinder = new Pathfinder(gridManager);
        List<Tile> path = pathfinder.FindPath(tiles[start], tiles[goal]);
        if (path == null || path.Count == 0)
        {
            Debug.LogError("경로를 찾을 수 없습니다. 맵 생성 실패.");
            return;
        }

        HashSet<Tile> reservedTiles = new HashSet<Tile>(path);

        // 스폰 지역을 3x3 으로 설정하고, 테두리 한 칸은 비워둠
        foreach (Vector2Int center in spawnPoints)
        {
            for (int dx = -1; dx <= 1; dx++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    Vector2Int pos = new Vector2Int(center.x + dx, center.y + dy);
                    Tile tile = gridManager.GetTileAtPosition(pos);
                    if (tile != null)
                    {
                        reservedTiles.Add(tile);
                    }
                }
            }
        }

        // 경로 주변 상하좌우대각선 타일 1칸 씩 보호
        Vector2Int[] protectedTileDir = new Vector2Int[]
        {
            new Vector2Int(0, 1),   // ↑
            new Vector2Int(0, -1),  // ↓
            new Vector2Int(1, 0),   // →
            new Vector2Int(-1, 0),  // ←
            new Vector2Int(1, 1),   // ↗
            new Vector2Int(-1, 1),  // ↖
            new Vector2Int(1, -1),  // ↘
            new Vector2Int(-1, -1)  // ↙
        };

        foreach (var tile in path)
        {
            foreach (var dir in protectedTileDir)
            {
                Vector2Int neighborPos = tile.gridPos + dir;
                Tile neighbor = gridManager.GetTileAtPosition(neighborPos);
                if (neighbor != null)
                {
                    reservedTiles.Add(neighbor);
                }
            }
        }

        // 맵오브젝트 스폰 가능, 보호받지 않는 타일 리스트
        List<Tile> candidateTiles = new List<Tile>();
        foreach (var tile in tiles.Values)
        {
            if (!reservedTiles.Contains(tile))
            {
                candidateTiles.Add(tile);
            }
        }

        int totalCandidates = candidateTiles.Count;

        int wallCount = Mathf.RoundToInt(totalCandidates * wallDensity);
        int obstacleCount = Mathf.RoundToInt(totalCandidates * obstacleDensity);

        Shuffle(candidateTiles);

        for (int i = 0; i < wallCount && i < candidateTiles.Count; i++)
        {
            PlaceWall(candidateTiles[i]);
        }
        for (int i = wallCount; i < wallCount + obstacleCount && i < candidateTiles.Count; i++)
        {
            PlaceObstacle(candidateTiles[i]);
        }
    }

    private static void ClearObstacles(Dictionary<Vector2Int, Tile> tiles)
    {
        foreach (var tile in tiles.Values)
        {
            tile.isWalkable = true;
            tile.isWall = false;
            tile.isObstacle = false;
        }

        if (mapObjectParent != null)
        {
            foreach (Transform child in mapObjectParent)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }

    private static void PlaceWall(Tile tile)
    {
        tile.isWalkable = false;
        tile.isWall = true;

        GameObject wall = Object.Instantiate(wallPrefab, tile.transform.position, Quaternion.identity, mapObjectParent);
        Renderer renderer = wall.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            float wallHeight = renderer.bounds.size.y;
            float tileTopY = tile.transform.position.y + 0.5f;
            wall.transform.position = new Vector3(
                tile.transform.position.x,
                tileTopY + (wallHeight / 2f),
                tile.transform.position.z
            );
        }
    }

    private static void PlaceObstacle(Tile tile)
    {
        tile.isWalkable = false;
        tile.isObstacle = true;

        GameObject obs = Object.Instantiate(obstaclePrefab, tile.transform.position, Quaternion.identity, mapObjectParent);
        Renderer renderer = obs.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            float obstacleHeight = renderer.bounds.size.y;
            float tileTopY = tile.transform.position.y + 0.5f;
            obs.transform.position = new Vector3(
                tile.transform.position.x,
                tileTopY + (obstacleHeight / 2f),
                tile.transform.position.z
            );
        }
    }

    // 배열 안에서 타일 순서 무작위 변경 (랜덤성 ^)
    private static void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int j = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
