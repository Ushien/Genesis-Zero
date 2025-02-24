using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoudreEtourdissante : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _PoudreEtourdissante);
    }

    private void _PoudreEtourdissante(Tile targetTile, List<Properties> properties = null){
        SpellManager.Instance.Stun(1, targetTile.GetUnit());
        SpellManager.Instance.MultiplyHP(2f, targetTile.GetUnit());
    }
}