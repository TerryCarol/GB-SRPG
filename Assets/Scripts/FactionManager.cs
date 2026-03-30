using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public static class FactionManager
{
    private static Dictionary<Faction, FactionData> factionDataMap;

    static FactionManager()
    {
        factionDataMap = new Dictionary<Faction, FactionData>();

        factionDataMap[Faction.Player] = new FactionData(
            "Player",
            Color.blue,
            "The main faction controlled by the player.",
            Resources.Load<Sprite>("Icons/PlayerIcon")
            //,new List<Skill>() { /* 팩션 모디파이어 */ }
        );
        factionDataMap[Faction.Ally] = new FactionData(
            "Ally",
            Color.blue,
            "The sub faction controlled by the Friendly AI.",
            Resources.Load<Sprite>("Icons/FriendlyIcon")
            //,new List<Skill>() { /* 팩션 모디파이어 */ }
        );
        factionDataMap[Faction.Enemy] = new FactionData(
            "Enemy",
            Color.red,
            "The main faction controlled by the Enemy AI.",
            Resources.Load<Sprite>("Icons/EnemyIcon")
            //,new List<Skill>() { /* 팩션 모디파이어 */ }
        );
        factionDataMap[Faction.Neutral] = new FactionData(
            "Neutural",
            Color.white,
            "Neutural Entities.",
            Resources.Load<Sprite>("Icons/NeuturalIcon")
            //,new List<Skill>() { /* 팩션 모디파이어 */ }
        );
    }

    public static FactionData GetData(Faction faction)
    {
        return factionDataMap[faction];
    }
}
