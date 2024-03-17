using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public ScriptableUnit scriptableUnit;

    public Tile OccupiedTile;
    public Team Team = Team.Enemy;
    public int level = 1;

    public string unit_name = "Name";
    public int finalPower = 1;
    public int finalLife = 1;
    [TextArea(5,10)]
    public string lore_description = "Lore Description";
    [TextArea(5,10)]
    public string fight_description = "Fight Description";

    public int armor = 0;

    public void Setup(ScriptableUnit originUnit){
        scriptableUnit = originUnit;
        this.name = scriptableUnit.unit_name;
        
        this.GetComponent<SpriteRenderer>().sprite = scriptableUnit.sprite;
        unit_name = scriptableUnit.unit_name;
        finalPower = scriptableUnit.original_power;        
        finalLife = scriptableUnit.original_health;
        lore_description = scriptableUnit.lore_description;
        fight_description = scriptableUnit.fight_description;
    }
}

public enum Team {
    Ally = 0,
    Enemy = 1
}
