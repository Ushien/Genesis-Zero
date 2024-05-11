using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Révélation : BaseSpell
{
    void Awake(){
        ratio1 = 0.5f;
    }

    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _Revelation);
    }

    private void _Revelation(Tile targetTile){   
        float amount = GetFinalDamages(ratio1);
        targetTile.GetUnit().ModifyArmor(GetFinalDamages(ratio1));
        targetTile.GetUnit().QueueAction(targetTile.GetUnit().ConvertArmorIntoHP, GetFinalDamages(ratio1), 2);
    }
}
