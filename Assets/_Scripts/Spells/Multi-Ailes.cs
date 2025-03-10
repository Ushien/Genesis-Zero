using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiAiles : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _MultiAiles);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _MultiAiles_H);
    }

    private void _MultiAiles(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), UnitManager.Instance.GetUnits(team : targetTile.team), properties, spellType : SpellType.Damage);
        
        Modifier newModifier = Instantiate(baseModifier);
        newModifier.Setup(gameObject, _powerBonus : GetRatio()[1], _duration : Modifier.Duration.Battle);
        GetOwner().AddGlobalModifier(newModifier);
    }

    private void _MultiAiles_H(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio(hyper : true)[0]), targetTile.GetUnit(), properties, spellType : SpellType.Damage);
        
        Modifier newModifier = Instantiate(baseModifier);
        newModifier.Setup(gameObject, _powerBonus : GetRatio(hyper : true)[1], _duration : Modifier.Duration.Battle);
        GetOwner().AddGlobalModifier(newModifier);
    }
}