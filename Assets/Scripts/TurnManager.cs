using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }
    private bool isProcessingAITurn = false;
    private Faction currentFaction = Faction.Player;

    public bool IsPlayerTurn() => currentFaction == Faction.Player;
    public bool IsEnemyTurn() => currentFaction == Faction.Enemy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartPlayerTurn();
    }

    public void EndPlayerTurn()
    {
        Debug.Log("플레이어 턴 종료.");
        currentFaction = Faction.Enemy;
        StartEnemyTurn();
    }

    public void EndEnemyTurn()
    {
        Debug.Log("적 턴 종료.");
        currentFaction = Faction.Player;
        isProcessingAITurn = false;
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        Debug.Log("플레이어 턴 시작.");
        currentFaction = Faction.Player;
        ResetAP(currentFaction);
    }

    public void StartEnemyTurn()
    {
        Debug.Log("적 턴 시작.");
        currentFaction = Faction.Enemy;
        ResetAP(currentFaction);

        if (AIManager.Instance != null)
        {
            isProcessingAITurn = true;
            AIManager.Instance.StartAITurn(); // AI FSM
        }
        else
        {
            isProcessingAITurn = false;
            Debug.LogError("Error: AIManager Missing");
        }
    }

    public void ResetAP(Faction faction)
    {
        foreach (var unit in FindObjectsOfType<Unit>())
        {
            if (unit.Faction == faction)
            {
                unit.ResetActionPoints();
            }
        }
    }
}