using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoeurMécanique : BaseSpell
{
    override public void Cast(Tile targetTile = null, List<Properties> properties = null){
        base.CastSpell(targetTile, properties, _CoeurMecanique);
    }

    private void _CoeurMecanique(Tile targetTile, List<Properties> properties = null){   
        //
    }
}
