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
            base.Cast(targetTile);

            _Bouledefeu(targetTile);

            SpellCasted(targetTile);
        }
    }

    private void _Bouledefeu(Tile targetTile){
        float finalDamages = ratio * owner.finalPower;

        SpellManager.Instance.InflictDamage(finalDamages, targetTile.GetUnit(), Properties.Pyro);
    }
}