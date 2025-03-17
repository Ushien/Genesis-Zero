using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onde : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Onde);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Onde_H, hyper: true);
    }

    private void _Onde(Tile targetTile, List<Properties> properties = null){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), properties, spellType : SpellType.Heal);
        
        Modifier newModifier = Instantiate(baseModifier);
        newModifier.Setup(gameObject, _powerBonus : GetRatio()[1], _duration : Modifier.Duration.Battle);
        targetTile.GetUnit().AddGlobalModifier(newModifier);
    }

    private void _Onde_H(Tile targetTile, List<Properties> properties = null){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio(hyper : true)[0]), targetTile.GetUnit(), properties, spellType : SpellType.Heal);
        
        Modifier newModifier = Instantiate(baseModifier);
        newModifier.Setup(gameObject, _powerBonus : GetRatio(hyper : true)[1], _duration : Modifier.Duration.Battle);
        targetTile.GetUnit().AddGlobalModifier(newModifier);
    }
}
