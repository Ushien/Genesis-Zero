using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseUnit : MonoBehaviour
{
    public ScriptableUnit scriptableUnit;

    public Tile OccupiedTile;
    public List<BaseSpell> availableSpells;
    public Team Team = Team.Enemy;
    public int level = 1;

    public string unit_name = "Name";
    public int finalPower = 1;
    public int finalHealth = 1;
    [TextArea(5,10)]
    public string lore_description = "Lore Description";
    [TextArea(5,10)]
    public string fight_description = "Fight Description";

    public int armor = 0;

    public void Setup(ScriptableUnit originUnit, int setup_level, Team team){
        scriptableUnit = originUnit;
        this.name = scriptableUnit.unit_name;
        
        this.GetComponent<SpriteRenderer>().sprite = scriptableUnit.sprite;
        unit_name = scriptableUnit.unit_name;

        Team = team;
        level = setup_level;

        finalPower = GetStatFromLevel(scriptableUnit.original_power, level);
        finalHealth = GetStatFromLevel(scriptableUnit.original_health, level);

        lore_description = scriptableUnit.lore_description;
        fight_description = scriptableUnit.fight_description;

        foreach (var scriptableSpell in scriptableUnit.spells)
        {
            var new_spell = SpellManager.Instance.SetupSpell(scriptableSpell, this);
            availableSpells.Add(new_spell);
        }
    }

    public int GetStatFromLevel(int level_100_stat, int real_level){

        var level_1_stat = (float)level_100_stat/10;
        var growth_by_level = ((float)level_100_stat-level_1_stat)/99;
        var real_level_stat = (int)Math.Ceiling(level_1_stat+growth_by_level*(real_level-1));

        return real_level_stat;
    }

    public void CastSpell(int index){
        availableSpells[index].Cast();
    }

    public String GetName(){
        return unit_name;
    }

    public String GetFightDescription(){
        return fight_description;
    }

    public int GetFinalPower(){
        return finalPower;
    }

    public int GetFinalHealth(){
        return finalHealth;
    }

    public int GetLevel(){
        return level;
    }

    public List<string> GetInfo(){
        List<string> infos = new List<string>();
        /*
        infos.Add(unit_name);
        infos.Add(Team);
        infos.Add(level);
        infos.Add(finalPower);
        infos.Add(finalLife);
        infos.Add(fight_description);
        infos.Add(armor);
        infos.Add();
        infos.Add();
        infos.Add();
        */
        return infos;
    }
}