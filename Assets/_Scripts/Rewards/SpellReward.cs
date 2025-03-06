using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellReward : Reward
{
    private ScriptableSpell spell;

    public SpellReward(ScriptableSpell _spell){
        spell = _spell;
    }

    public ScriptableSpell GetSpell(){
        return spell;
    }

    public void SetSpell(ScriptableSpell _spell){
        spell = _spell;
    }

    public override string GetTitle(){
        return spell.name;
    }

    public override void Pick(BaseUnit unit){
        SpellManager.Instance.SetupSpell(spell, unit, unit.GetAvailableSpellIndex()+1);
    }
}
