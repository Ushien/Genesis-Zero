using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onde : BaseSpell
{
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Onde);
    }

    private void _Onde(Tile targetTile){   
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), spellType : SpellType.Heal);
        SpellManager.Instance.ModifyPower(GetRatio()[1], targetTile.GetUnit());
    }
}
