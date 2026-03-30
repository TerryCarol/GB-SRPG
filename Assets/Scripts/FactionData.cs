using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FactionData
{
    public string Name { get; private set; }
    public Color FactionColor { get; private set; }
    public string Description { get; private set; }
    public Sprite Icon { get; private set; }

    // 팩션 별 모디파이어 추가 시
    //public List<Skill> FactionSkills { get; private set; }
    //public FactionData(string name, Color color, string description, Sprite icon, List<Skill> skills)

    public FactionData(string name, Color color, string description, Sprite icon)
    {
        Name = name;
        FactionColor = color;
        Description = description;
        Icon = icon;
        //FactionSkills = skills;
    }
}