using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Révélation : BaseSpell
{
    void Awake(){
        SetRatio(1, 0.5f);
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Revelation);
    }

    private void _Revelation(Tile targetTile){   
        float amount = GetFinalDamages(GetRatio()[0]);
        targetTile.GetUnit().ModifyArmor(GetFinalDamages(GetRatio()[0]));
        targetTile.GetUnit().QueueAction(targetTile.GetUnit().ConvertArmorIntoHP, GetFinalDamages(GetRatio()[0]), 2);
    }
}
