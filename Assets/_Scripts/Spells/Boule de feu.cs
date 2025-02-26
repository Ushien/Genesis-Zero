using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Bouledefeu : BaseSpell
{

    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Bouledefeu);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Bouledefeu_H, hyper: true);
    }

    private void _Bouledefeu(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), Tools.CombineProperties(properties, Properties.Pyro), SpellType.Damage);
    }

    private void _Bouledefeu_H(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio(hyper:true)[0]), targetTile.GetUnit(), Tools.CombineProperties(properties, Properties.Pyro), SpellType.Damage);
    }
}