using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Instruction
{
    public BaseUnit source;
    public BaseSpell spell;
    public Tile target;
    
    public Instruction(BaseUnit source_unit, BaseSpell spell_to_cast, Tile target_tile){
        source = source_unit;
        spell = spell_to_cast;
        target = target_tile;
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

    public string GetSummary()
    {
        string _sourceName = GetSourceUnit().GetName();
        string _spellName = GetSpell().GetName();
        string _targetName = "  Dead unit";
        if(GetTargetTile().GetUnit() != null){
            _targetName = GetTargetTile().GetUnit().GetName();
        }


        return "Source Unit : " + _sourceName + " - Spell name : " + _spellName + " - Target Unit : " + _targetName;
    }
}
