using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseUnit : MonoBehaviour
{
    public ScriptableUnit scriptableUnit;
    public ScriptableJob scriptableJob;
    public Tile OccupiedTile;
    public Passive emptyPassive;
    public Passive passive;
    public List<BaseSpell> availableSpells;
    public Team Team = Team.Enemy;
    public int level = 1;

    public BaseSpell heroSpell1;
    public BaseSpell heroSpell2;

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

        if(scriptableUnit.passive != null){
            passive = Instantiate(scriptableUnit.passive);

        }
        else{
            passive = Instantiate(emptyPassive);
        }
        passive.transform.parent = this.transform;
        passive.name = "Passif";
        passive.AttachToUnit(this);

        foreach (var scriptableSpell in scriptableUnit.spells)
        {
            availableSpells.Add(SpellManager.Instance.SetupSpell(scriptableSpell, this));
        }
        if (scriptableUnit.scriptableJob != null){
            LoadJob(scriptableUnit.scriptableJob);
        }
    }

    public void LoadJob(ScriptableJob scriptableJob){
        foreach (var scriptableSpell in scriptableJob.spells)
        {
            availableSpells.Add(SpellManager.Instance.SetupSpell(scriptableSpell, this));
        }
        if (scriptableJob.passive != null){
            Destroy(passive.gameObject);
            passive = Instantiate(scriptableJob.passive);
            passive.transform.parent = this.transform;
            passive.name = "Passif";
            passive.AttachToUnit(this);
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

    public Team GetTeam(){
        return Team;
    }

    public Passive GetPassive(){
        return passive;
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