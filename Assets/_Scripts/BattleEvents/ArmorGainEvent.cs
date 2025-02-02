using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Un ArmorGainEvent est créé lorsqu'une unité gagne de l'armure, peu importe le contexte.
/// </summary>
public class ArmorGainEvent : BattleEvent
{
    private BaseUnit targetUnit;
    private int amount;

    public ArmorGainEvent(BaseUnit _targetUnit, int _amount){
        targetUnit = _targetUnit;
        amount = _amount;
    }

    public BaseUnit GetTargetUnit(){
        return targetUnit;
    }

    public int GetAmount(){
        return amount;
    }

    public override string GetSummary()
    {
        return "Armor Event: " + GetTargetUnit().GetName() + " - " + GetAmount();
    }
}
