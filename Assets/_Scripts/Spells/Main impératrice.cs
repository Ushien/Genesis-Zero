using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainImpératrice : BaseSpell
{
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _MainImpératrice);
    }

    private void _MainImpératrice(Tile targetTile){   
        foreach (BaseUnit ally in UnitManager.Instance.GetUnitsExcept(GetOwner()))
        {
            ally.Attack(targetTile);
        }
    }
}
