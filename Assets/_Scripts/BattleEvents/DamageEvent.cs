using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Un DamageEvent est créé lorsqu'une unité subit des dégats, peu importe le contexte.
/// </summary>
public class DamageEvent : BattleEvent
{
    private BaseUnit originUnit;
    private BaseUnit targetUnit;
    private int health_amount;
    private int armor_amount;

    public DamageEvent(BaseUnit _originUnit, BaseUnit _targetUnit, int _health_amount, int _armor_amount){
        originUnit = _originUnit;
        targetUnit = _targetUnit;
        health_amount = _health_amount;
        armor_amount = _armor_amount;
    }
    public void SetOriginUnit(BaseUnit _originUnit){
        originUnit = _originUnit;
    }

    public BaseUnit GetOriginUnit(){
        return originUnit;
    }

    public BaseUnit GetTargetUnit(){
        return targetUnit;
    }

    public int GetHealthAmount(){
        return health_amount;
    }

    public int GetArmorAmount(){
        return armor_amount;
    }

    public int GetTotalAmount(){
        return health_amount + armor_amount;
    }

    public bool IsArmorDamages(){
        if(armor_amount > 0){
            return true;
        }
        else{
            return false;
        }
    }
}
