using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainImpératrice : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _MainImpératrice);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _MainImpératrice_H, hyper: true);
    }

    private void _MainImpératrice(Tile targetTile, List<Properties> properties = null){   
        foreach (BaseUnit ally in UnitManager.Instance.GetUnitsExcept(GetOwner()))
        {
            // ça pourrait être rigolo que leurs attaques prossèdent les mêmes propriétés que le lancer
            ally.Attack(targetTile, propertiesToApply : null);
        }
    }

    private void _MainImpératrice_H(Tile targetTile, List<Properties> properties = null){   
        foreach (BaseUnit ally in UnitManager.Instance.GetUnitsExcept(GetOwner()))
        {
            // ça pourrait être rigolo que leurs attaques prossèdent les mêmes propriétés que le lancer
            ally.Attack(targetTile, propertiesToApply : properties);
            ally.Attack(targetTile, propertiesToApply : properties);
        }
    }
}
