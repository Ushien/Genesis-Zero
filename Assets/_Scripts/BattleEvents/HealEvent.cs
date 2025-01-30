using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealEvent : BattleEvent
{
    private BaseUnit targetUnit;
    private int amount;

    public HealEvent(BaseUnit _targetUnit, int _amount){
        targetUnit = _targetUnit;
        amount = _amount;
    }

    public BaseUnit GetTargetUnit(){
        return targetUnit;
    }

    public int GetAmount(){
        return amount;
    }
}