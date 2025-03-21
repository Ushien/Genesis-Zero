using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Renforcement : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Renforcement);
    }

    override public void HyperCast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _Renforcement_H, hyper: true);
    }

    private void _Renforcement(Tile targetTile, List<Properties> properties = null){
        // Do something
        GetOwner().ModifyArmor(GetFinalDamages(GetRatio()[0]));
    }

    private void _Renforcement_H(Tile targetTile, List<Properties> properties = null){
        // Do something
        GetOwner().ModifyArmor(GetFinalDamages(GetRatio(hyper : true)[0]));
    }
}