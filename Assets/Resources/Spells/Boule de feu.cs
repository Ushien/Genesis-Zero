using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : BaseSpell
{
    float ratio = 1;
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Bouledefeu);
    }

    private void _Bouledefeu(Tile targetTile){
        float finalDamages = ratio * owner.finalPower;

        SpellManager.Instance.InflictDamage(finalDamages, targetTile.GetUnit(), Properties.Pyro);
    }
}