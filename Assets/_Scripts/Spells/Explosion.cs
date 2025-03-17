using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Explosion);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Explosion_H, hyper: true);
    }

    private void _Explosion(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetRatio()[0] * GetOwner().GetFinalPower(), targetTile.GetUnit(), properties);
        SpellManager.Instance.UseSpell(GetOwner(), GetRatio()[1] * GetOwner().GetFinalPower(), UnitManager.Instance.GetUnitsExcept(targetTile.GetUnit()), properties);
    }

    private void _Explosion_H(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetRatio(hyper : true)[0] * GetOwner().GetFinalPower(), UnitManager.Instance.GetUnits(team : targetTile.team), properties);
    }
}