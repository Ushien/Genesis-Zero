using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class HPModificationEvent : BattleEvent
{
    private BaseUnit targetUnit;
    private int newAmount;
    private int oldAmount;

    private bool total;

    public HPModificationEvent(BaseUnit _targetUnit, int _oldAmount, int _newAmount, bool _total){
        targetUnit = _targetUnit;
        oldAmount = _oldAmount;
        newAmount = _newAmount;
        total = _total;
    }
    public BaseUnit GetTargetUnit(){
        return targetUnit;
    }

    public int GetOldAmount(){
        return oldAmount;
    }

    public int GetNewAmount(){
        return newAmount;
    }

    public bool AreTotalHP(){
        return total;
    }

    public override string GetSummary()
    {
        if(total){
                return "HP Modification Event: " + GetTargetUnit().GetName() + " - From " + GetOldAmount() + " HP - To " + GetNewAmount() + " HP" + " - (Total HP modification)";
        }
        else{
                return "HP Modification Event: " + GetTargetUnit().GetName() + " - From " + GetOldAmount() + " HP - To " + GetNewAmount() + " HP" + " - (Final HP modification)";

        }
    }
}