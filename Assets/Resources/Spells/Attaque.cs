using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AAttack : BaseSpell
{
    void Awake(){
        ratio1 = 1f;
        ratio2 = 1f;
        ratio3 = 1f;
        SetIsATechnique(false);
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Attaque);
    }

    private void _Attaque(Tile targetTile){
        SpellManager.Instance.InflictDamage(GetFinalDamages(ratio1), targetTile.GetUnit());
    }
}