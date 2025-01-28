using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveReward : Reward
{
    private ScriptablePassive passive;

    public PassiveReward(ScriptablePassive _passive){
        passive = _passive;
    }

    public ScriptablePassive GetPassive(){
        return passive;
    }

    public void SetPassive(ScriptablePassive _passive){
        passive = _passive;
    }

    public override string GetTitle(){
        return passive.name;
    }
}