using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BaseSpell
{
    void Awake(){
        SetRatio(1, 1.2f);
        SetRatio(2, 0.2f);
    }
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Explosion);
    }

    private void _Explosion(Tile targetTile){
        SpellManager.Instance.InflictDamage(GetOwner(), GetRatio()[0] * GetOwner().GetFinalPower(), targetTile.GetUnit());
        SpellManager.Instance.InflictDamage(GetOwner(), GetRatio()[1] * GetOwner().GetFinalPower(), UnitManager.Instance.GetUnitsExcept(targetTile.GetUnit()));
    }
}