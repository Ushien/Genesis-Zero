using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : BaseSpell
{
    void Awake(){

    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Bouledefeu);
    }

    private void _Bouledefeu(Tile targetTile){
        SpellManager.Instance.InflictDamage(GetFinalDamages(GetRatio()[0]), targetTile.GetUnit(), Properties.Pyro);
    }
}