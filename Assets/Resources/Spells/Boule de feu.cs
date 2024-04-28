using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bouledefeu : BaseSpell
{
    override public void Cast(Tile targetTile = null){
        BaseUnit targetUnit = null;
        if (targetTile != null){
            targetUnit = targetTile.GetUnit();
        }

        Debug.Log(GetOwner().GetName() + " lance " + GetName() + " sur " + targetTile.GetUnit().GetName());

        SpellManager.Instance.InflictDamage(100, targetUnit, Properties.Pyro);
    }
}