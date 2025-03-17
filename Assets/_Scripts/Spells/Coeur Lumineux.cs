using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoeurLumineux : BaseSpell
{
    private Modifier modifier;

    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _CoeurLumineux);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _CoeurLumineux_H, hyper: true);
    }

    private void _CoeurLumineux(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), properties, spellType : SpellType.Heal);
        GetOwner().ModifyCuratifCount(1);
    }

    private void _CoeurLumineux_H(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio(hyper:true)[0]), targetTile.GetUnit(), properties, spellType : SpellType.Heal);
        GetOwner().ModifyCuratifCount(3);
    }
}