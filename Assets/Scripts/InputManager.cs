using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GridManager gridManager;
    public TileSelector tileSelector;

    private Unit selectedUnit = null;
    private Unit previousUnit = null;
    private bool initialUnitSpawned = false;

    void Update()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        if (!TurnManager.Instance.IsPlayerTurn()) return;

        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        if (Input.GetMouseButtonDown(1))
        {
            DeselectUnit();
        }

        // 유닛 선택 시 하이라이트
        if (selectedUnit != null)
        {
            HighlightMoveableTiles(selectedUnit);
        }
        else
        {
            HighlightHoveredUnit();
        }

        // 플레이어 턴 종료
        if (TurnManager.Instance.IsPlayerTurn() && Input.GetKeyDown(KeyCode.Tab))
        {
            TurnManager.Instance.EndPlayerTurn();
        }
    }

    private void HandleLeftClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Tile clickedTile = hit.collider.GetComponent<Tile>();
            Unit clickedUnit = hit.collider.GetComponent<Unit>();

            // 클릭된 유닛이 있을 경우 (유닛 선택)
            if (clickedUnit != null)
            {
                SelectUnit(clickedUnit);
                return;
            }

            // 타일 클릭 (이동 또는 공격)
            if (clickedTile != null)
            {
                if (selectedUnit == null)
                {
                    if (!initialUnitSpawned)
                    {
                        SpawnInitialPlayerUnit(clickedTile);
                    }
                    Debug.Log("No unit selected.");
                    return;
                }

                if (clickedTile.isOccupied)
                {
                    Unit targetUnit = clickedTile.GetOnTileUnit();
                    if (targetUnit != null && targetUnit.Faction != selectedUnit.Faction)
                    {
                        SetAttackState(targetUnit);
                    }
                }
                else
                {
                    SetMoveState(clickedTile);
                }
            }
        }
    }

    private void SelectUnit(Unit unit)
    {
        if (unit.Faction == Faction.Player)
        {
            DeselectUnit();
            selectedUnit = unit;
            selectedUnit.Highlight(Color.blue, true);
            HighlightMoveableTiles(selectedUnit);
            Debug.Log($"Selected unit: {unit.UnitName}");
        }
        else
        {
            Debug.Log($"Cannot control enemy unit: {unit.UnitName}");
        }
    }

    private void DeselectUnit()
    {
        if (selectedUnit != null)
        {
            selectedUnit.ResetHighlight();
            ReleaseHighlightMoveableTiles(selectedUnit);
            selectedUnit = null;
            Debug.Log("Unit deselected.");
        }
    }

    private void SetMoveState(Tile targetTile)
    {
        if (selectedUnit == null) return;

        var stateController = selectedUnit.GetComponent<UnitStateController>();
        if (stateController == null) return;

        stateController.SetState("Move", targetTile);
        //DeselectUnit();
    }

    private void SetAttackState(Unit targetUnit)
    {
        if (selectedUnit == null) return;

        var stateController = selectedUnit.GetComponent<UnitStateController>();
        if (stateController == null) return;

        stateController.SetState("Attack", targetUnit);
        //DeselectUnit();
    }

    private void HighlightHoveredUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null)
            {
                if (unit != previousUnit)
                {
                    previousUnit?.ResetHighlight();
                    unit.Highlight(Color.white, true);
                    previousUnit = unit;
                }
            }
        }
        else
        {
            if (previousUnit != null)
            {
                previousUnit.ResetHighlight();
                previousUnit = null;
            }
        }
    }

    private void HighlightMoveableTiles(Unit unit)
    {
        if (unit == null) return;

        List<Tile> movableTiles = unit.GetMovableTiles();
        tileSelector?.ShowTiles(movableTiles, Color.cyan);
    }

    private void ReleaseHighlightMoveableTiles(Unit unit)
    {
        tileSelector?.ClearHighlights();
    }

    // 초기 유닛 생성 (테스트용)
    public void SpawnInitialPlayerUnit(Tile clickedTile)
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

        tempUnit.SetCurrentTile(clickedTile);
        clickedTile.SetOnTileUnit(tempUnit);
        tempUnit.transform.position = clickedTile.transform.position + new Vector3(0, 1.0f, 0);
        tempUnit.Faction = Faction.Player;

        Debug.Log($"Spawned player unit: {tempUnit.UnitName}");
        initialUnitSpawned = true;
    }
}