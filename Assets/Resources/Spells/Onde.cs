using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onde : BaseSpell
{
    void Awake(){
        ratio1 = 0.7f;
        ratio2 = 0.005f;
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Onde);
    }

    private void _Onde(Tile targetTile){   
        SpellManager.Instance.HealDamage(GetFinalDamages(ratio1), targetTile.GetUnit());
        SpellManager.Instance.ModifyPower(GetFinalDamages(ratio2), targetTile.GetUnit());
    }
}
