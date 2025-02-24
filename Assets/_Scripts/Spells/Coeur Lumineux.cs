using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoeurLumineux : BaseSpell
{
    private Modifier modifier;

    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _CoeurLumineux);
    }

    private void _CoeurLumineux(Tile targetTile, List<Properties> properties = null){
        BaseUnit targetUnit = targetTile.GetUnit();
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), properties, spellType : SpellType.Heal);
        GetOwner().ModifyCuratifCount(1);
    }
}