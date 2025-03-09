using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onde : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Onde);
    }

    private void _Onde(Tile targetTile, List<Properties> properties = null){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), properties, spellType : SpellType.Heal);
        
        Modifier newModifier = Instantiate(baseModifier);
        newModifier.Setup(gameObject, _powerBonus : GetRatio()[1], _duration : Modifier.Duration.Battle);
        targetTile.GetUnit().AddGlobalModifier(newModifier);
    }
}
