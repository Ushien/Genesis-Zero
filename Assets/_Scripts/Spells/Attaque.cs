using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AAttack : BaseSpell
{
    void Awake(){
        SetIsATechnique(false);
    }

    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Attaque);
    }

    private void _Attaque(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.UseSpell(GetOwner(), GetFinalDamages(GetRatio()[0]), targetTile.GetUnit());
    }
}