using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
    private Unit hoveredUnit;
    private Unit selectedUnit;

    void Update()
    {
        HandleMouseHover();
        HandleSelection();
    }

    void HandleMouseHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Unit unit = hit.collider.GetComponent<Unit>();
            if (unit != null && unit != selectedUnit)
            {
                if (unit != hoveredUnit)
                {
                    // 이전 하이라이트 제거
                    ResetHoverHighlight();
                    hoveredUnit = unit;
                    hoveredUnit.Highlight(Color.yellow, true);
                }
            }
            else
            {
                ResetHoverHighlight();
            }
        }
        else
        {
            ResetHoverHighlight();
        }
    }

    void HandleSelection()
    {
        if (Input.GetMouseButtonDown(0) && hoveredUnit != null)
        {
            // 이전 선택 해제
            if (selectedUnit != null)
                selectedUnit.ResetHighlight();

            // 새 유닛 선택
            selectedUnit = hoveredUnit;
            hoveredUnit = null;
            selectedUnit.Highlight(Color.green, true);
        }
        else
        {

        }
    }

    void ResetHoverHighlight()
    {
        if (hoveredUnit != null && hoveredUnit != selectedUnit)
        {
            hoveredUnit.ResetHighlight();
        }
        hoveredUnit = null;
    }
}

/*
public class UnitSelector : MonoBehaviour
{
    public GridManager gridManager;
    public GameObject playerPrefab;

    private Unit hoveredUnit;
    private Unit previousUnit;

    void Update()
    {
        HandleMouseHover();
    }

    void HandleMouseHover()
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
                    if (previousUnit != null)
                    {
                        previousUnit.ResetHighlight();
                    }

                    hoveredUnit = unit;
                    hoveredUnit.Highlight(Color.white, true);
                    previousUnit = hoveredUnit;
                }
            }
        }
        else
        {
            // 마우스가 아무 유닛에도 닿지 않을 경우 하이라이트 제거
            if (previousUnit != null)
            {
                previousUnit.ResetHighlight();
                previousUnit = null;
                hoveredUnit = null;
            }
        }
    }
}
*/
