using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoeurMécanique : BaseSpell
{
    override public void Cast(Tile targetTile = null){
        base.CastSpell(targetTile, _CoeurMecanique);
    }

    private void _CoeurMecanique(Tile targetTile){   
        //
    }
}
