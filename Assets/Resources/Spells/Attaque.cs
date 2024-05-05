using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AAttack : BaseSpell
{
    float ratio = 1;
    void Start()
    {
        SetIsATechnique(false);
    }
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Attaque);
    }

    private void _Attaque(Tile targetTile){
        float finalDamages = ratio * owner.finalPower;

        SpellManager.Instance.InflictDamage(finalDamages, targetTile.GetUnit());
    }
}