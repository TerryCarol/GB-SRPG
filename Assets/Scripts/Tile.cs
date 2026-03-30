using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public TextMeshPro text;
    public Renderer meshRenderer; // 직접 연결

    public Vector2Int gridPos { get; set; }
    public bool isWalkable = true;
    public bool isOccupied = false;

    public bool isWall = false;         // 이동X, 시야X
    public bool isObstacle = false;     // 이동X, 시야O
    public bool BlocksVision
    {
        get => isWall;
        //set => isWall = value;
    }
    public bool BlocksMovement
    {
        get => isWall || isObstacle;
    }

    public bool debugMode = true;

    private Color originalColor;
    private Color currentColor;
    private Unit onTileUnit;

    void Awake()
    {
        // 시작할 때 원래 머티리얼 색상 저장
        if (meshRenderer == null)
        {
            meshRenderer = GetComponentInChildren<Renderer>();
        }

        if (meshRenderer != null)
        {
            originalColor = meshRenderer.material.color;
            currentColor = originalColor;
        }
    }

    public void Init(Vector2Int pos)
    {
        gridPos = pos;
        // 타일 위치벡터 받아서 텍스트에 반영
        if (text != null && debugMode)
        {
            text.text = $"({pos.x}, {pos.y})";
        }
    }

    public void Highlight(Color color)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
            currentColor = color;
        }
    }

    public void MouseHoverHighlight(Color color)
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = color;
        }
    }

    public void ResetMouseHoverHighlight()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = currentColor;
        }
    }

    public void ResetHighlight()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material.color = originalColor;
            currentColor = originalColor;
        }
    }

    public void SetOnTileUnit(Unit arrivedUnit)
    {
        onTileUnit = arrivedUnit;
    }
    public Unit GetOnTileUnit()
    {
        return onTileUnit;
    }
    public void ResetOnTileUnit()
    {
        onTileUnit = null;
    }
}
