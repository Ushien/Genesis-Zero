using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEvent : BattleEvent
{
    private BaseUnit originUnit;
    private BaseUnit targetUnit;
    private int amount;

    public HealEvent(BaseUnit _originUnit, BaseUnit _targetUnit, int _amount){
        originUnit = _originUnit;
        targetUnit = _targetUnit;
        amount = _amount;
    }
    public BaseUnit GetOriginUnit(){
        return originUnit;
    }

    public void SetOriginUnit(BaseUnit _originUnit){
        originUnit = _originUnit;
    }

    public BaseUnit GetTargetUnit(){
        return targetUnit;
    }

    public int GetAmount(){
        return amount;
    }
}