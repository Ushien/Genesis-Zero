using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : BaseSpell
{
    float ratio = 1;
    override public void Cast(Tile targetTile = null){
        BaseUnit targetUnit = null;
        if (targetTile != null){
            targetUnit = targetTile.GetUnit();
        }

        if(targetUnit != null){

            float finalDamages = ratio * owner.finalPower;

            Debug.Log(GetOwner().GetName() + " lance " + GetName() + " sur " + targetTile.GetUnit().GetName());

            SpellManager.Instance.InflictDamage(finalDamages, targetUnit, Properties.Pyro);

            EventManager.Instance.CastSpell(this);
        }

    }
}