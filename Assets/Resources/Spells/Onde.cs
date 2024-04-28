using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onde : BaseSpell
{
    float ratio1 = 0.7f;
    float ratio2 = 0.005f;
    override public void Cast(Tile targetTile = null){
        BaseUnit targetUnit = null;
        if (targetTile != null){
            targetUnit = targetTile.GetUnit();
        }

        float finalAmount1 = ratio1 * owner.finalPower;
        float finalAmount2 = ratio2 * owner.finalPower;

        Debug.Log(GetOwner().GetName() + " lance " + GetName() + " sur " + targetTile.GetUnit().GetName());

        SpellManager.Instance.HealDamage(finalAmount1, targetUnit);
        SpellManager.Instance.ModifyPower(finalAmount2, targetUnit);

        EventManager.Instance.CastSpell(this);
    }
}
