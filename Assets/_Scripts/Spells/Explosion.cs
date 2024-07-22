using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BaseSpell
{
    void Awake(){
        ratio1 = 1.2f;
        ratio2 = 0.2f;
        ratio3 = 1f;
    }
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Explosion);
    }

    private void _Explosion(Tile targetTile){
        SpellManager.Instance.InflictDamage(ratio1 * owner.finalPower, targetTile.GetUnit());
        SpellManager.Instance.InflictDamage(ratio2 * owner.finalPower, UnitManager.Instance.GetUnitsExcept(targetTile.GetUnit()));
    }
}