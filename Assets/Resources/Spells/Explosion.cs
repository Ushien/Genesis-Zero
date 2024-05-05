using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BaseSpell
{
    float ratio1 = 1.2f;
    float ratio2 = 0.2f;
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Explosion);
    }

    private void _Explosion(Tile targetTile){
        SpellManager.Instance.InflictDamage(ratio1 * owner.finalPower, targetTile.GetUnit());
        SpellManager.Instance.InflictDamage(ratio2 * owner.finalPower, UnitManager.Instance.GetUnitsExcept(targetTile.GetUnit()));
    }
}