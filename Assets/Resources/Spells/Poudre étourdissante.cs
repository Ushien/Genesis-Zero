using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoudreEtourdissante : BaseSpell
{

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _PoudreEtourdissante);
    }

    private void _PoudreEtourdissante(Tile targetTile){
        SpellManager.Instance.Stun(1f, targetTile.GetUnit());
        SpellManager.Instance.MultiplyHP(2f, targetTile.GetUnit());
    }
}