using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Onde : BaseSpell
{
    void Awake(){
        SetRatio(1, 0.7f);
        SetRatio(2, 0.02f);
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Onde);
    }

    private void _Onde(Tile targetTile){   
        SpellManager.Instance.HealDamage(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit());
        SpellManager.Instance.ModifyPower(GetRatio()[1], targetTile.GetUnit());
    }
}
