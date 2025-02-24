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
        SpellManager.Instance.ModifyPower(GetRatio()[1], targetTile.GetUnit());
    }
}
