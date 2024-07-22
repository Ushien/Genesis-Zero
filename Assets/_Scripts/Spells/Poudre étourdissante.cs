using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoudreEtourdissante : BaseSpell
{
    void Awake(){
        SetRatio(1, 1f);
        SetRatio(2, 2f);
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _PoudreEtourdissante);
    }

    private void _PoudreEtourdissante(Tile targetTile){
        SpellManager.Instance.Stun(1, targetTile.GetUnit());
        SpellManager.Instance.MultiplyHP(2f, targetTile.GetUnit());
    }
}