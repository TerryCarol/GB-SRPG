using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Unit/UnitTemplate")]
public class UnitTemplate : ScriptableObject
{
    public string templateName;
    public string unitName;
    public Gender gender;
    public Faction faction;
    public int health;
    public float moveSpeed;
    public int moveRange;
    public int attackPower;
    public int attackRange;
    public GameObject unitPrefab;
}
