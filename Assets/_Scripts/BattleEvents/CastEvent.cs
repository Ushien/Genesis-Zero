using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

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
}
