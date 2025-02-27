using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Un AfterCastEvent est créé après qu'une technique est lancée, peu importe le contexte.
/// </summary>

public class AfterCastEvent : BattleEvent
{
    private BaseUnit sourceUnit;
    private BaseSpell castedSpell;
    private Tile targetTile;

    public AfterCastEvent(BaseUnit _sourceUnit, BaseSpell _castedSpell, Tile _targetTile){
        sourceUnit = _sourceUnit;
        castedSpell = _castedSpell;
        targetTile = _targetTile;
    }

    public BaseUnit GetSourceUnit(){
        return sourceUnit;
    }

    public BaseSpell GetCastedSpell(){
        return castedSpell;
    }

    public Tile GetTargetTile(){
        return targetTile;
    }

    public override string GetSummary()
    {
        return "After Cast Event: " + GetSourceUnit().GetName() + " - " + GetCastedSpell().GetName() + " - " + targetTile.name;
    }
}
