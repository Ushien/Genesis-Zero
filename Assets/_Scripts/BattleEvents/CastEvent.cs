using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

/// <summary>
/// Un CastEvent est créé lorsqu'une technique est lancée, peu importe le contexte.
/// </summary>

public class CastEvent : BattleEvent
{
    private BaseUnit sourceUnit;
    private BaseSpell castedSpell;
    private Tile targetTile;

    public CastEvent(BaseUnit _sourceUnit, BaseSpell _castedSpell, Tile _targetTile){
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

    public override string GetSummary()
    {
        return "Cast Event: " + GetSourceUnit().GetName() + " - " + GetCastedSpell().GetName() + " - " + targetTile.name;
    }
}
