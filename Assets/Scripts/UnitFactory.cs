using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory : MonoBehaviour
{
    [SerializeField] private UnitTemplate[] templateAssets;
    [SerializeField] private Transform unitManagerParent;

    private Dictionary<string, UnitTemplate> unitTemplates;
    private Dictionary<Faction, Transform> factionParents = new Dictionary<Faction, Transform>();

    void Awake()
    {
        unitTemplates = new Dictionary<string, UnitTemplate>();
        foreach (var template in templateAssets)
        {
            if (!unitTemplates.ContainsKey(template.templateName))
            {
                unitTemplates.Add(template.templateName, template);
            }
        }
    }

    public void SetTemplatesManually(UnitTemplate[] loadedTemplates)
    {
        templateAssets = loadedTemplates;
    }

    public Unit CreateUnit(string unitType)
    {
        if (unitTemplates.ContainsKey(unitType))
        {
            UnitTemplate template = unitTemplates[unitType];
            GameObject unitObject = Object.Instantiate(template.unitPrefab);
            Unit unit = unitObject.GetComponent<Unit>();

            unitObject.name = $"{template.templateName}";

            unit.UnitType = template.name;
            unit.Gender = template.gender;
            unit.UnitName = NameGenerator.GenerateRandomName(unit.Gender);
            unit.Faction = template.faction;

            unit.Health = template.health;
            unit.MoveSpeed = template.moveSpeed;
            unit.MoveRange = template.moveRange;
            unit.AttackPower = template.attackPower;
            unit.AttackRange = template.attackRange;

            Transform parent = GetOrCreateFactionParent(unit.Faction);
            unitObject.transform.SetParent(parent);

            return unit;
        }
        else
        {
            Debug.LogError("No unit template found for type: " + unitType);
            return null;
        }
    }

    private Transform GetOrCreateFactionParent(Faction faction)
    {
        if (factionParents.ContainsKey(faction))
            return factionParents[faction];

        string groupName = faction.ToString() + "Units";

        Transform found = unitManagerParent.Find(groupName);
        if (found == null)
        {
            GameObject newGroup = new GameObject(groupName);
            newGroup.transform.SetParent(unitManagerParent);
            found = newGroup.transform;
        }

        factionParents[faction] = found;
        return found;
    }
}