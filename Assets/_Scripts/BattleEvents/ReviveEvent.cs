using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReviveEvent : BattleEvent
{
    private BaseUnit revivedUnit;
    private int hpAmount;

    public ReviveEvent(BaseUnit _revivedUnit, int _hpAmount){
        revivedUnit = _revivedUnit;
        hpAmount = _hpAmount;
    }
    public BaseUnit GetRevivedUnit(){
        return revivedUnit;
    }

    public int GetHPAmount(){
        return hpAmount;
    }

    public override string GetSummary()
    {
        return "Revive Event: " + GetRevivedUnit().GetName() + " - New HP amount : " + GetHPAmount();
    }
}