using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sacrements : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Sacrements);
    }

    private void _Sacrements(Tile targetTile, List<Properties> properties = null){
        float finalAmount1 = GetRatio()[0] * GetOwner().GetFinalPower();
        float finalAmount2 = GetRatio()[1] * GetOwner().GetFinalPower();
       
        if(targetTile.GetUnit().isArmored()){
            SpellManager.Instance.UseSpell(GetOwner(), finalAmount2, targetTile.GetUnit(), properties);
        }
        else{
            SpellManager.Instance.UseSpell(GetOwner(), finalAmount1, targetTile.GetUnit(), properties);
        }
    }
}