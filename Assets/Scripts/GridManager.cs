using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject tilePrefab;
    private Transform tileContainer;

    public GameObject wallPrefab;
    public GameObject obstaclePrefab;
    [Range(0f, 1f)] public float wallDensity = 0.1f;
    [Range(0f, 1f)] public float obstacleDensity = 0.1f;

    public int width = 10;
    public int height = 10;
    public float gridSize = 1f; // 스냅 간격 맞추기 위한 변수

    // tiles는 딕셔너리
    private Dictionary<Vector2Int, Tile> tiles = new Dictionary<Vector2Int, Tile>();

    void Start()
    {
        GenerateGridContainer();
        GenerateGrid();

        MapGenerator.wallDensity = wallDensity;
        MapGenerator.obstacleDensity = obstacleDensity;
        // BSP 등 적용 시 맵오브젝트매니저 분리 예정
        MapGenerator.Init(wallPrefab, obstaclePrefab, this);
        MapGenerator.Generate(tiles);
    }

    void GenerateGridContainer()
    {
        tileContainer = new GameObject("Tiles").transform;
        tileContainer.parent = transform;
        // 에디터 보기 쉽게
    }

    void GenerateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 spawnPosition = new Vector3(x * gridSize, 0, y * gridSize);
                var spawnedTile = Instantiate(tilePrefab, spawnPosition, Quaternion.identity, tileContainer);
                spawnedTile.name = $"Tile ({x}, {y})";

                Tile tile = spawnedTile.GetComponent<Tile>();
                Vector2Int gridPos = new Vector2Int(x, y);
                tile.Init(gridPos); // 좌표 설정과 텍스트 갱신
                tile.isWalkable = true;

                // 벡터를 인덱스로 하는 딕셔너리에 차곡차곡 쌓는다
                tiles[new Vector2Int(x, y)] = tile;
            }
        }
    }

    public Tile GetTileAtPosition(Vector2Int pos)
    {
        // 위치벡터 인덱스의 값이 딕셔너리에 등록되어있는지
        if (tiles.ContainsKey(pos))
        {
            // 있으면 Tile 객체 자체를 반환
            return tiles[pos];
        }
        else
        {
            return null;
        }
    }

    public Vector2Int[] GetSpawnPoints()
    {
        // 스폰 포인트를 계산하여 반환 (1,1 과 반대쪽 끝점 타일)
        Vector2Int point1 = new Vector2Int(1, 1);
        Vector2Int point2 = new Vector2Int(width - 1, height - 1);

        return new Vector2Int[] { point1, point2 };
    }

    public List<Tile> GetAllFreeWalkableTiles()
    {
        List<Tile> availableTiles = new List<Tile>();

        foreach (var tile in tiles.Values)
        {
            if (tile.isWalkable && !tile.isOccupied)
            {
                availableTiles.Add(tile);
            }
        }

        return availableTiles;
    }
}
