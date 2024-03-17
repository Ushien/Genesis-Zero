using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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

    public void Setup(ScriptableUnit originUnit, int setup_level){
        scriptableUnit = originUnit;
        this.name = scriptableUnit.unit_name;
        
        this.GetComponent<SpriteRenderer>().sprite = scriptableUnit.sprite;
        unit_name = scriptableUnit.unit_name;

        level = setup_level;

        finalPower = GetStatFromLevel(scriptableUnit.original_power, level);
        finalLife = GetStatFromLevel(scriptableUnit.original_health, level);

        lore_description = scriptableUnit.lore_description;
        fight_description = scriptableUnit.fight_description;
    }

    public int GetStatFromLevel(int level_100_stat, int real_level){

        var level_1_stat = (float)level_100_stat/10;
        var growth_by_level = ((float)level_100_stat-level_1_stat)/99;
        var real_level_stat = (int)Math.Ceiling(level_1_stat+growth_by_level*(real_level-1));
        //var real_level_stat = (int)(level_1_stat*real_level/10);

        return real_level_stat;
    }
}

public enum Team {
    Ally = 0,
    Enemy = 1
}
