using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuitDeVie : BaseSpell
{
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _PuitDeVie);
    }

    private void _PuitDeVie(Tile targetTile){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), spellType : SpellType.Heal);
    }
}
