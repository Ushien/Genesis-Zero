using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuitDeVie : BaseSpell
{
    void Awake(){
        SetRatio(1, 4.5f);
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _PuitDeVie);
    }

    private void _PuitDeVie(Tile targetTile){   
        SpellManager.Instance.HealDamage(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit());
    }
}
