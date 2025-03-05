using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// Contient les informations sur la prochaine action de l'unité.
/// </summary>
public class Instruction : MonoBehaviour
{
    public BaseUnit source;
    public BaseSpell spell;
    public Tile target;
    public bool overloaded;
    
    public void Setup(BaseUnit source_unit, BaseSpell spell_to_cast, Tile target_tile, bool hyper = false){
        source = source_unit;
        spell = spell_to_cast;
        target = target_tile;
        overloaded = hyper;
    }

    public BaseUnit GetSourceUnit(){
        return source;
    }
    public BaseSpell GetSpell(){
        return spell;
    }
    public Tile GetTargetTile(){
        return target;
    }

    public bool IsOverloaded(){
        return overloaded;
    }

    public string GetLog(){
        string log = "";

        log += ConvertTeam(GetSourceUnit().GetTeam());
        log += GetSourceUnit().GetTile().x_position;
        log += GetSourceUnit().GetTile().y_position;
        log += "-";
        log += GetSourceUnit().GetSpellIndex(GetSpell());
        log += "-";
        log += ConvertTeam(GetSourceUnit().GetTeam());
        log += GetTargetTile().x_position;
        log += GetTargetTile().y_position;
        log += "-";
        log += IsOverloaded() ? "H" : "_";
        
        return log;
    }

    private string ConvertTeam(Team team){
        if(team == Team.Ally){
            return "A";
        }
        if(team == Team.Enemy){
            return "E";
        }
        else{
            return "_";
        }
    }

    public string GetSummary()
    {
        string _sourceName = GetSourceUnit().GetName();
        string _spellName = GetSpell().GetName();
        string _targetName = "Dead unit";
        if(GetTargetTile().GetUnit() != null){
            _targetName = GetTargetTile().GetUnit().GetName();
        }

        string summary = "Source Unit : " + _sourceName + " - Spell name : " + _spellName + " - Target Unit : " + _targetName;
        if(overloaded){
            summary = summary + " - Overloaded";
        }
        return summary;
    }
}
