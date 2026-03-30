using System.Collections.Generic;
using UnityEngine;

public class TileSelector : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject playerPrefab;

    private Tile hoveredTile;
    private Tile previousTile;

    private List<Tile> moveRangeHighlightTiles = new List<Tile>();

    void Update()
    {
        HandleMouseHover();
    }

    void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile tile = hit.collider.GetComponent<Tile>();
            if (tile != null)
            {
                if (tile != hoveredTile)
                {
                    // 이전 타일 하이라이트 제거
                    if (previousTile != null)
                        previousTile.ResetHighlight();

                    hoveredTile = tile;
                    hoveredTile.MouseHoverHighlight(Color.red);
                    previousTile = hoveredTile;
                }
            }
        }
        else
        {
            // 마우스가 아무 타일에도 닿지 않을 경우 하이라이트 제거
            if (previousTile != null)
            {
                previousTile.ResetMouseHoverHighlight();
                previousTile = null;
                hoveredTile = null;
            }
        }
    }

    public void ShowTiles(List<Tile> tiles, Color color)
    {
        ClearHighlights();
        foreach (Tile tile in tiles)
        {
            tile.Highlight(color);
            moveRangeHighlightTiles.Add(tile);
        }
    }

    public void ClearHighlights()
    {
        foreach (Tile tile in moveRangeHighlightTiles)
        {
            tile.ResetHighlight();
        }
        moveRangeHighlightTiles.Clear();

        if (hoveredTile != null)
        {
            hoveredTile.ResetHighlight();
            hoveredTile = null;
        }

        if (previousTile != null)
        {
            previousTile.ResetHighlight();
            previousTile = null;
        }
    }
}
