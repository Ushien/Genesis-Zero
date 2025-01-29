using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AAttack : BaseSpell
{
    void Awake(){
        SetIsATechnique(false);
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Attaque);
    }

    private void _Attaque(Tile targetTile){
        SpellManager.Instance.InflictDamage(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit());
    }
}