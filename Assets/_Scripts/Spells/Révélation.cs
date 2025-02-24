using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Révélation : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Revelation);
    }

    private void _Revelation(Tile targetTile, List<Properties> properties = null){   
        float amount = GetFinalDamages(GetRatio()[0]);
        targetTile.GetUnit().ModifyArmor(GetFinalDamages(GetRatio()[0]));
        targetTile.GetUnit().QueueAction(targetTile.GetUnit().ConvertArmorIntoHP, GetFinalDamages(GetRatio()[0]), 2);
    }
}
