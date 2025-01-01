using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveReward : Reward
{
    private Passive passive;

    public PassiveReward(Passive _passive){
        passive = _passive;
    }

    public Passive GetPassive(){
        return passive;
    }

    public void SetPassive(Passive _passive){
        passive = _passive;
    }
}