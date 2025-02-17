using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : BaseSpell
{
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Explosion);
    }

    private void _Explosion(Tile targetTile){
        //SpellManager.Instance.UseSpell(GetOwner(), GetRatio()[0] * GetOwner().GetFinalPower(), targetTile.GetUnit());
        //SpellManager.Instance.UseSpell(GetOwner(), GetRatio()[1] * GetOwner().GetFinalPower(), UnitManager.Instance.GetUnitsExcept(targetTile.GetUnit()));
    }
}