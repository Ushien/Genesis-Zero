using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sacrements : BaseSpell
{
    void Awake(){
        SetRatio(1, 1.1f);
        SetRatio(2, 1.4f);
    }
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Sacrements);
    }

    private void _Sacrements(Tile targetTile){
        float finalAmount1 = GetRatio()[0] * GetOwner().GetFinalPower();
        float finalAmount2 = GetRatio()[1] * GetOwner().GetFinalPower();
       
        if(targetTile.GetUnit().isArmored()){
            SpellManager.Instance.InflictDamage(GetOwner(), finalAmount2, targetTile.GetUnit());
        }
        else{
            SpellManager.Instance.InflictDamage(GetOwner(), finalAmount1, targetTile.GetUnit());
        }
    }
}