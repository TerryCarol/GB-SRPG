using System.Collections.Generic;
using UnityEngine;

public class BFS
{
    private GridManager gridManager;

    public BFS(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }

    // BFS 길찾기 메서드
    public List<Tile> FindPath(Vector2Int start, Vector2Int goal)
    {
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();

        queue.Enqueue(start);
        cameFrom[start] = start;

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == goal)
            {
                break; // 목표 지점에 도달하면 종료
            }

            foreach (Vector2Int next in GetNeighbors(current))
            {
                Tile nextTile = gridManager.GetTileAtPosition(next);
                if (!cameFrom.ContainsKey(next) &&          // 이전 타일과 동일 X
                    nextTile.isWalkable &&                  // 통행 가능 O
                    !nextTile.isOccupied)                 // 이미 점유 X
                {
                    queue.Enqueue(next);
                    cameFrom[next] = current;
                }
            }
        }

        return ReconstructPath(cameFrom, start, goal);
    }

    // 주어진 지점에서 갈 수 있는 인접 타일들을 반환
    // 코너커팅문제 예방 위한 IsDiagonalMoveValid 확인 추가
    private List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // 8방향 (상하좌우 + 대각선 방향)
        Vector2Int[] directions = new Vector2Int[]
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

        foreach (Vector2Int dir in directions)
        {
            Vector2Int neighbor = current + dir;
            
            if (gridManager.GetTileAtPosition(neighbor) != null)
            {
                if (!IsDiagonalMoveValid(current, neighbor)) continue;

                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    // 경로 재구성 (cameFrom을 바탕으로)
    private List<Tile> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int goal)
    {
        List<Tile> path = new List<Tile>();
        Vector2Int current = goal;

        while (current != start)
        {
            path.Add(gridManager.GetTileAtPosition(current));
            current = cameFrom[current];
        }

        path.Add(gridManager.GetTileAtPosition(start));
        path.Reverse();
        return path;
    }

    private bool IsDiagonalMoveValid(Vector2Int from, Vector2Int to)
    {
        Vector2Int diff = to - from;
        if (Mathf.Abs(diff.x) == 1 && Mathf.Abs(diff.y) == 1) // 대각선 타일 이동이면
        {
            Vector2Int neighbor1 = new Vector2Int(from.x + diff.x, from.y);    // 대각선 X+1 방향 이웃
            Vector2Int neighbor2 = new Vector2Int(from.x, from.y + diff.y);    // 대각선 Y+1 방향 이웃

            Tile tile1 = gridManager.GetTileAtPosition(neighbor1);
            Tile tile2 = gridManager.GetTileAtPosition(neighbor2);

            if ((tile1 != null && !tile1.isWalkable) || (tile2 != null && !tile2.isWalkable))
            {
                return false; // 둘 중 하나라도 못 지나가면 대각선 컷
            }
        }

        return true;
    }

    public List<Tile> FindReachableTiles(Vector2Int start, float maxRange)
    {
        List<Tile> reachable = new List<Tile>();
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Dictionary<Vector2Int, float> costSoFar = new Dictionary<Vector2Int, float>();

        frontier.Enqueue(start);
        costSoFar[start] = 0;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            float currentCost = costSoFar[current];

            foreach (Vector2Int nextPos in GetNeighbors(current))
            {
                Tile neighbor = gridManager.GetTileAtPosition(nextPos);
                if (neighbor == null || !neighbor.isWalkable || neighbor.isOccupied)
                    continue;

                if (costSoFar.ContainsKey(nextPos)) continue;

                float newCost = currentCost + 1;
                if (newCost <= maxRange)
                {
                    costSoFar[nextPos] = newCost;
                    frontier.Enqueue(nextPos);
                    reachable.Add(neighbor);
                }
            }
        }

        return reachable;
    }
}
