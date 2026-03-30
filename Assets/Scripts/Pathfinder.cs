using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;
using static UnityEngine.GraphicsBuffer;

public class Pathfinder
{
    private GridManager gridManager;
    private BFS bfs;
    private Vector2Int startTile, targetTile;

    public Pathfinder(GridManager gridManager)
    {
        this.gridManager = gridManager;
        this.bfs = new BFS(gridManager);            // BFS 인스턴스 생성
    }

    public List<Tile> FindPath(Tile start, Tile target)
    {
        startTile = start.gridPos;                  // BFS는 2차원 int 배열만 받음
        targetTile = target.gridPos;                
        return bfs.FindPath(startTile, targetTile); // BFS의 길찾기 메서드 호출
    }

    public List<Tile> FindReachableTiles(Tile start, float maxRange)
    {
        return bfs.FindReachableTiles(start.gridPos, maxRange);
    }
}
