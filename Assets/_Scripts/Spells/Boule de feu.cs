using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : BaseSpell
{

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Bouledefeu);
    }

    override public void HyperCast(Tile targetTile = null){
        base.CastSpell(targetTile, _Bouledefeu_H, hyper: true);
    }

    private void _Bouledefeu(Tile targetTile){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), new List<Properties>(){Properties.Pyro}, SpellType.Damage);
    }

    private void _Bouledefeu_H(Tile targetTile){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio(hyper:true)[0]), targetTile.GetUnit(), new List<Properties>(){Properties.Pyro}, SpellType.Damage);
    }
}