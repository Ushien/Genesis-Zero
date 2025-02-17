using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoeurLumineux : BaseSpell
{
    private Modifier modifier;

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _CoeurLumineux);
    }

    private void _CoeurLumineux(Tile targetTile){
        BaseUnit targetUnit = targetTile.GetUnit();
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), spellType : SpellType.Heal);
        GetOwner().ModifyCuratifCount(1);
    }
}