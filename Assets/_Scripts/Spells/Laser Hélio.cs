using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LaserHelio : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _LaserHelio);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _LaserHelio_H, hyper: true);
    }

    private void _LaserHelio(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), Tools.CombineProperties(properties, Properties.Vampirisme), spellType : SpellType.Damage);
    }

    private void _LaserHelio_H(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio(hyper : true)[0]), targetTile.GetUnit(), Tools.CombineProperties(properties, Properties.Vampirisme), spellType : SpellType.Damage);
    }
}