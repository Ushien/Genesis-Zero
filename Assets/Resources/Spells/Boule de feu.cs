using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : BaseSpell
{
    void Awake(){
        ratio1 = 1f;
        ratio2 = 1f;
        ratio3 = 1f;
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Bouledefeu);
    }

    private void _Bouledefeu(Tile targetTile){
        SpellManager.Instance.InflictDamage(GetFinalDamages(ratio1), targetTile.GetUnit(), Properties.Pyro);
    }
}