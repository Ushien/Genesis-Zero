using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onde : BaseSpell
{
    float ratio1 = 0.7f;
    float ratio2 = 0.005f;
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Onde);
    }

    private void _Onde(Tile targetTile){
        float finalAmount1 = ratio1 * owner.finalPower;
        float finalAmount2 = ratio2 * owner.finalPower;
       
        SpellManager.Instance.HealDamage(finalAmount1, targetTile.GetUnit());
        SpellManager.Instance.ModifyPower(finalAmount2, targetTile.GetUnit());
    }
}
