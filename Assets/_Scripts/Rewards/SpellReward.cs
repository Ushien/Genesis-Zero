using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellReward : Reward
{
    private BaseSpell spell;

    public SpellReward(BaseSpell _spell){
        spell = _spell;
    }

    public BaseSpell GetSpell(){
        return spell;
    }

    public void SetSpell(BaseSpell _spell){
        spell = _spell;
    }
}
