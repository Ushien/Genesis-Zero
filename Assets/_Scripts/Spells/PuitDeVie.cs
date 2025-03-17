using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuitDeVie : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _PuitDeVie);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _PuitDeVie_H, hyper: true);
    }

    private void _PuitDeVie(Tile targetTile, List<Properties> properties = null){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), properties, spellType : SpellType.Heal);
    }

    private void _PuitDeVie_H(Tile targetTile, List<Properties> properties = null){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio(hyper : true)[0]), targetTile.GetUnit(), properties, spellType : SpellType.Heal);
    }
}
