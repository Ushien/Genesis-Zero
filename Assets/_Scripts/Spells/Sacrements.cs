using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sacrements : BaseSpell
{
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Sacrements);
    }

    private void _Sacrements(Tile targetTile){
        float finalAmount1 = GetRatio()[0] * GetOwner().GetFinalPower();
        float finalAmount2 = GetRatio()[1] * GetOwner().GetFinalPower();
       
        if(targetTile.GetUnit().isArmored()){
            SpellManager.Instance.UseSpell(GetOwner(), finalAmount2, targetTile.GetUnit());
        }
        else{
            SpellManager.Instance.UseSpell(GetOwner(), finalAmount1, targetTile.GetUnit());
        }
    }
}