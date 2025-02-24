using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainImpératrice : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _MainImpératrice);
    }

    private void _MainImpératrice(Tile targetTile, List<Properties> properties = null){   
        foreach (BaseUnit ally in UnitManager.Instance.GetUnitsExcept(GetOwner()))
        {
            // ça pourrait être rigolo que leurs attaques prossèdent les mêmes propriétés que le lancer
            ally.Attack(targetTile, propertiesToApply : null);
        }
    }
}
