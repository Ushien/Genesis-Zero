using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Un DamageEvent est créé lorsqu'une unité subit des dégats, peu importe le contexte.
/// </summary>
public class DamageEvent : BattleEvent
{
    private BaseUnit targetUnit;
    private int amount;
    private bool armorDamages;

    public DamageEvent(BaseUnit _targetUnit, int _amount, bool _armorDamages = false){
        targetUnit = _targetUnit;
        amount = _amount;
        armorDamages = _armorDamages;
    }

    public BaseUnit GetTargetUnit(){
        return targetUnit;
    }

    public int GetAmount(){
        return amount;
    }

    public bool IsArmorDamages(){
        return armorDamages;
    }
}
