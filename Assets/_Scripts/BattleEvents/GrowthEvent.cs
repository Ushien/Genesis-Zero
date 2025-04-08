using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthEvent : BattleEvent
{
    private BaseSpell spell;

    public GrowthEvent(BaseSpell _spell){
        spell = _spell;
    }

    public BaseSpell GetSpell(){
        return spell;
    }

    public override string GetSummary()
    {
        return "Growth Event: " + GetSpell().GetName();
    }
}
